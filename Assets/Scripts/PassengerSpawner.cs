using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class OptionalWaypoint
{
    [Tooltip("Uğranılabilecek opsiyonel nokta (Örn: Bilet Gişesi, Kahveci)")]
    public Transform waypoint;
    
    [Tooltip("Yolcunun buraya uğrama ihtimali (% olarak)")]
    [Range(0f, 100f)]
    public float probability = 5f;
}

[System.Serializable]
public class RouteNode
{
    [Tooltip("Gidilecek ana durak.")]
    public Transform mainWaypoint;

    [Tooltip("BU DURAKTAN SONRA gidilebilecek opsiyonel noktalar.")]
    public List<OptionalWaypoint> optionalDetours;
}

public class PassengerSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [Tooltip("Doğacak AI Prefabı")]
    public GameObject passengerPrefab;
    
    [Tooltip("Kaç saniyede bir yeni yolcu doğacak?")]
    public float spawnInterval = 3f;
    
    [Header("Route Settings")]
    [Tooltip("Yolcunun sırayla gitmek zorunda olduğu ANA duraklar ve sonrasındaki opsiyonel uğrak yerleri.")]
    public List<RouteNode> commonRouteNodes;

    [Tooltip("Ana duraklar bitince, bu listeden RASTGELE BİRİNİ seçip son olarak oraya gidecek ve kaybolacak.")]
    public List<Transform> exitPoints;

    private float timer;
    
    // Tüm spawnerlar arasında ortak ziyaret sayısını tutmak için static dictionary kullanıyoruz
    private static Dictionary<Transform, int> visitCounts = new Dictionary<Transform, int>();

    private void Start()
    {
        timer = spawnInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnPassenger();
            timer = spawnInterval;
        }
    }

    private void SpawnPassenger()
    {
        if (passengerPrefab == null) return;

        GameObject passengerObj = Instantiate(passengerPrefab, transform.position, transform.rotation);
        PassengerAI ai = passengerObj.GetComponent<PassengerAI>();

        if (ai != null)
        {
            List<Transform> myRoute = new List<Transform>();
            List<bool> isOptionalList = new List<bool>();

            // 1. Ana duraklar ve durak sonrası opsiyonelleri sırayla ekle
            if (commonRouteNodes != null)
            {
                foreach (var node in commonRouteNodes)
                {
                    // Önce ana durağı ekle
                    if (node.mainWaypoint != null)
                    {
                        myRoute.Add(node.mainWaypoint);
                        isOptionalList.Add(false);
                    }

                    // Sonra bu duraktan sonraki opsiyonel noktaları hesapla
                    if (node.optionalDetours != null)
                    {
                        foreach (var opt in node.optionalDetours)
                        {
                            if (opt.waypoint != null && Random.Range(0f, 100f) <= opt.probability)
                            {
                                myRoute.Add(opt.waypoint);
                                isOptionalList.Add(true);
                                
                                // Ziyaretçi sayacını artır ve Console'a yazdır
                                if (!visitCounts.ContainsKey(opt.waypoint)) 
                                    visitCounts[opt.waypoint] = 0;
                                    
                                visitCounts[opt.waypoint]++;
                                Debug.Log($"[Metro Analiz] Bir yolcu '{opt.waypoint.name}' noktasına uğramaya karar verdi! Bu noktanın toplam ziyaretçisi: {visitCounts[opt.waypoint]}");
                            }
                        }
                    }
                }
            }

            // 2. Rastgele bir çıkış noktası ekle
            if (exitPoints != null && exitPoints.Count > 0)
            {
                Transform randomExit = exitPoints[Random.Range(0, exitPoints.Count)];
                myRoute.Add(randomExit);
                isOptionalList.Add(false);
            }

            // Oluşturulan rotayı AI'a ver
            ai.InitializeRoute(myRoute, isOptionalList);
        }
    }
}
