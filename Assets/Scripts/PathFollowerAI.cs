using UnityEngine;

[System.Serializable]
public class WaypointData
{
    public Transform pointTransform;
    
    [Tooltip("Bu noktaya ulaştığında ne kadar bekleyecek? (saniye)")]
    public float waitTime = 0f;
    
    [Header("Optional Settings")]
    [Tooltip("Bu nokta atlanabilir (es geçilebilir) mi?")]
    public bool isOptional = false;
    
    [Tooltip("Atlanma ihtimali (% olarak)")]
    [Range(0, 100)] public float skipProbability = 50f;
}

public class PathFollowerAI : MonoBehaviour
{
    public WaypointData[] route;
    public float speed = 3f;
    
    private int currentIndex = 0;
    private float currentWaitTimer = 0f;
    private bool isWaiting = false;

    void Start()
    {
        DetermineNextValidWaypoint();
    }

    void Update()
    {
        if (route == null || route.Length == 0) return;

        if (currentIndex < route.Length)
        {
            if (isWaiting)
            {
                currentWaitTimer -= Time.deltaTime;
                if (currentWaitTimer <= 0f)
                {
                    isWaiting = false;
                    currentIndex++;
                    DetermineNextValidWaypoint();
                }
                return; // Beklerken hareket etme
            }

            Transform target = route[currentIndex].pointTransform;
            if (target == null) 
            {
                currentIndex++;
                DetermineNextValidWaypoint();
                return;
            }
            
            Vector3 direction = target.position - transform.position;
            direction.y = 0;

            if (direction.magnitude > 0.1f)
            {
                Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
                }
            }
            else
            {
                // Hedefe ulaşıldı
                float waitTime = route[currentIndex].waitTime;
                if (waitTime > 0f)
                {
                    isWaiting = true;
                    currentWaitTimer = waitTime;
                    Debug.Log($"AI '{target.name}' noktasına ulaştı, {waitTime} saniye bekliyor...");
                }
                else
                {
                    currentIndex++;
                    DetermineNextValidWaypoint();
                }
            }
        }
        else
        {
            Debug.Log("AI rotayı tamamladı ve kendini yok ediyor.");
            Destroy(gameObject);
        }
    }

    // Bir sonraki hedefin es geçilip geçilmeyeceğini belirler
    private void DetermineNextValidWaypoint()
    {
        while (currentIndex < route.Length)
        {
            WaypointData wp = route[currentIndex];
            if (wp.pointTransform == null)
            {
                currentIndex++;
                continue;
            }

            if (wp.isOptional)
            {
                float randomVal = Random.Range(0f, 100f);
                if (randomVal < wp.skipProbability)
                {
                    Debug.Log($"AI '{wp.pointTransform.name}' noktasını es geçti! (İhtimal: %{wp.skipProbability}, Gelen Zar: {randomVal:F1})");
                    currentIndex++; // Bu noktayı atla, sonrakine geç
                    continue;
                }
                else
                {
                    Debug.Log($"AI '{wp.pointTransform.name}' noktasına gitmeye karar verdi. (İhtimal: %{wp.skipProbability}, Gelen Zar: {randomVal:F1})");
                }
            }
            break; // Gidilecek geçerli bir nokta bulundu
        }
    }
}
