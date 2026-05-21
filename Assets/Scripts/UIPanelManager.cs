using UnityEngine;

public class UIPanelManager : MonoBehaviour
{
    public GameObject panelToToggle;
    
    [Header("Cat Prefabs")]
    [Tooltip("Panelden seçildiğinde spawn edilecek kedi prefabları")]
    public GameObject cat1Prefab;
    public GameObject cat2Prefab;

    private GridInteractable currentSelectedGrid;
    private GameObject pendingCatPrefab;

    void Start()
    {
        // Oyun başladığında panelin kapalı olduğundan emin ol
        if (panelToToggle != null) 
        {
            panelToToggle.SetActive(false);
        }
    }

    void Update()
    {
        // Panel açıksa ve Enter tuşuna basılırsa işlemi onayla
        if (panelToToggle != null && panelToToggle.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (pendingCatPrefab != null)
                {
                    SpawnCatFromPrefab(pendingCatPrefab);
                }
                else
                {
                    Debug.LogWarning("Önce bir kedi seçmelisiniz!");
                }
            }
        }
    }

    public void OpenPanelForGrid(GridInteractable grid)
    {
        if (panelToToggle != null)
        {
            currentSelectedGrid = grid;
            pendingCatPrefab = null; // Yeni panel açıldığında seçimi sıfırla
            panelToToggle.SetActive(true);
            Debug.Log("Panel açıldı, Grid seçildi: " + grid.gameObject.name);
        }
    }

    public void ClosePanel()
    {
        if (panelToToggle != null)
        {
            if (currentSelectedGrid != null)
            {
                currentSelectedGrid.Deselect(); // Panele basıp çıkılırsa rengi eski haline dönsün
                currentSelectedGrid = null;
            }
            pendingCatPrefab = null; // Seçimi sıfırla
            panelToToggle.SetActive(false);
            Debug.Log("Panel kapatıldı.");
        }
    }

    // UI Butonları tarafından OnClick ile çağrılacak metotlar
    public void SelectCat1()
    {
        pendingCatPrefab = cat1Prefab;
        Debug.Log("Kedi 1 seçildi. Onaylamak için ENTER tuşuna basın.");
    }

    public void SelectCat2()
    {
        pendingCatPrefab = cat2Prefab;
        Debug.Log("Kedi 2 seçildi. Onaylamak için ENTER tuşuna basın.");
    }

    private void SpawnCatFromPrefab(GameObject prefab)
    {
        if (currentSelectedGrid == null) return;
        if (currentSelectedGrid.isOccupied)
        {
            Debug.LogWarning("Bu grid zaten dolu, başka kedi yerleştirilemez!");
            return;
        }

        if (prefab != null)
        {
            // Gridin hemen üstünde çıkması için Y pozisyonunu hafif artırıyoruz
            Vector3 spawnPos = currentSelectedGrid.transform.position;
            spawnPos.y += 0.05f; 

            GameObject spawnedCat = Instantiate(prefab, spawnPos, Quaternion.identity);

            // Grid'i dolu işaretle
            currentSelectedGrid.MarkAsOccupied();
            
            Debug.Log(spawnedCat.name + " başarıyla yerleştirildi!");
        }
        else
        {
            Debug.LogError("Spawn edilmek istenen kedi prefab'ı atanmamış!");
            currentSelectedGrid.Deselect();
        }
        
        // İşlem bittikten sonra paneli kapat
        ClosePanel();
    }
}
