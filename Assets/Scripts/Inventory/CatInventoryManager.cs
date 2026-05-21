using System.Collections.Generic;
using UnityEngine;

public enum CatState 
{ 
    Idle, 
    Working, 
    Breeding 
}

public class CatInventoryManager : MonoBehaviour
{
    public static CatInventoryManager Instance { get; private set; }

    [Header("Oyuna Başlarken Verilecek Kedi Prefabları")]
    public List<GameObject> startingCatPrefabs = new List<GameObject>();

    [Header("Mevcut Tüm Kedi Objeleri (Otomatik Dolar)")]
    public List<GameObject> allCats = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Önce allCats içinde manuel eklenmiş prefab var mı diye kontrol et ve Instantiate et
        for (int i = 0; i < allCats.Count; i++)
        {
            if (allCats[i] != null && !allCats[i].scene.IsValid())
            {
                GameObject inst = Instantiate(allCats[i]);
                inst.SetActive(false);
                allCats[i] = inst;
            }
        }

        // Başlangıç prefablarını gerçek instance'lara dönüştürüp gizle
        foreach (var prefab in startingCatPrefabs)
        {
            if (prefab != null)
            {
                GameObject newCat = Instantiate(prefab);
                newCat.SetActive(false);
                allCats.Add(newCat);
            }
        }
        StartCoroutine(GenerateStartingThumbnails());
    }

    private System.Collections.IEnumerator GenerateStartingThumbnails()
    {
        // GPU mesh yüklemelerinin tamamlanması için 1 frame bekliyoruz (Beyaz ikon sorununu çözer)
        yield return null;

        Cat3DStudio studio = FindObjectOfType<Cat3DStudio>();
        if (studio != null)
        {
            foreach (var cat in allCats)
            {
                CatStats stats = cat.GetComponent<CatStats>();
                if (stats != null && stats.catIcon == null)
                {
                    stats.catIcon = studio.TakeSnapshot(cat);
                }
            }
        }
        RefreshUI();
    }

    public void AddCat(GameObject catObj)
    {
        if (!allCats.Contains(catObj))
        {
            CatStats stats = catObj.GetComponent<CatStats>();
            if (stats != null && stats.catIcon == null)
            {
                Cat3DStudio studio = FindObjectOfType<Cat3DStudio>();
                if (studio != null)
                {
                    stats.catIcon = studio.TakeSnapshot(catObj);
                }
            }
            allCats.Add(catObj);
            RefreshUI();
        }
    }

    public void RemoveCat(GameObject catObj)
    {
        if (allCats.Contains(catObj))
        {
            allCats.Remove(catObj);
            RefreshUI();
        }
    }

    public CatState GetCatState(GameObject cat)
    {
        if (cat == null) return CatState.Idle;

        if (CatBreedingManager.Instance != null && CatBreedingManager.Instance.IsBreeding(cat))
        {
            return CatState.Breeding;
        }
        
        if (cat.scene.IsValid() && cat.activeInHierarchy)
        {
            return CatState.Working;
        }

        return CatState.Idle;
    }

    public void RefreshUI()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsInventoryOpen)
        {
            UIManager.Instance.RefreshInventoryUI();
        }
    }
}
