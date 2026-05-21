using UnityEngine;
using UnityEditor;

public class SetupSystemsEditor : EditorWindow
{
    [MenuItem("MEOWTRO/Sistemleri Kur ve Prefab Oluştur")]
    public static void SetupSystems()
    {
        // 1. InteractionManager (GameManager) Kurulumu
        InteractionManager interactionManager = FindObjectOfType<InteractionManager>();
        if (interactionManager == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<InteractionManager>();
            Debug.Log("GameManager ve InteractionManager sahneye eklendi.");
        }

        // 2. CameraController'ı Main Camera'ya ekleme
        if (Camera.main != null)
        {
            CameraController camController = Camera.main.GetComponent<CameraController>();
            if (camController == null)
            {
                Camera.main.gameObject.AddComponent<CameraController>();
                Debug.Log("CameraController ana kameraya eklendi.");
            }
        }
        else
        {
            Debug.LogWarning("Sahnede 'MainCamera' etiketli bir kamera bulunamadı.");
        }

        // 3. Kamera Prefab'ı Oluşturma
        string prefabPath = "Assets/Prefabs/SecurityCameraNode.prefab";
        
        // Eğer Prefabs klasörü yoksa oluştur
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // Geçici obje oluştur
        GameObject cameraNodeGO = new GameObject("SecurityCameraNode");
        
        // Ana gövde (Küp)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(cameraNodeGO.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(0.5f, 0.5f, 0.8f);

        // Lens (Silindir)
        GameObject lens = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        lens.name = "Lens";
        lens.transform.SetParent(cameraNodeGO.transform);
        // Lensi gövdenin önüne, ileriye bakacak şekilde (Z ekseninde) yerleştir
        lens.transform.localPosition = new Vector3(0, 0, 0.45f);
        lens.transform.localRotation = Quaternion.Euler(90, 0, 0); // Z eksenine bakması için döndür
        lens.transform.localScale = new Vector3(0.3f, 0.2f, 0.3f);

        // Etkileşim için Collider ana objeye eklenmeli, çocuklardaki colliderları temizleyelim
        DestroyImmediate(body.GetComponent<Collider>());
        DestroyImmediate(lens.GetComponent<Collider>());

        BoxCollider mainCollider = cameraNodeGO.AddComponent<BoxCollider>();
        mainCollider.size = new Vector3(0.6f, 0.6f, 1f);
        mainCollider.center = new Vector3(0, 0, 0.1f);

        // Componentleri ekle
        cameraNodeGO.AddComponent<CameraNode>(); // OutlineController otomatik gelir
        
        // Prefab olarak kaydet
        PrefabUtility.SaveAsPrefabAsset(cameraNodeGO, prefabPath);
        
        // Sahnede kalan geçici objeyi sil
        DestroyImmediate(cameraNodeGO);

        Debug.Log("SecurityCameraNode prefab'ı 'Assets/Prefabs' klasörüne oluşturuldu!");
        
        // Değişiklikleri kaydetmek için Editor'ü uyar
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
