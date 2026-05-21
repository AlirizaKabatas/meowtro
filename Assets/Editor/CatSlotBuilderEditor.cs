using UnityEngine;
using UnityEditor;

public class CatSlotBuilderEditor : EditorWindow
{
    [MenuItem("MEOWTRO/Kedi Slotu Prefab'ı Oluştur")]
    public static void BuildCatSlot()
    {
        // Klasör kontrolü
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // Kedi slotu ana objesi (Düz bir zemin Quad)
        GameObject slotObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        slotObj.name = "CatSlotNode";
        
        // Zemin gibi durması için X ekseninde 90 derece yatır
        slotObj.transform.rotation = Quaternion.Euler(90, 0, 0);
        // Biraz büyütelim (Kedi rahat sığsın)
        slotObj.transform.localScale = new Vector3(2f, 2f, 1f);

        // Quad'da varsayılan olarak MeshCollider vardır. Etkileşim (Raycast) için daha verimli olması adına BoxCollider ile değiştirelim
        DestroyImmediate(slotObj.GetComponent<MeshCollider>());
        BoxCollider boxCol = slotObj.AddComponent<BoxCollider>();
        boxCol.size = new Vector3(1f, 1f, 0.1f);

        // Materyal oluşturup hafif yeşilimsi/fark edilebilir bir renk yapalım
        Material slotMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (slotMat.shader == null) // URP kurulu değilse Standard kullan
        {
            slotMat = new Material(Shader.Find("Standard"));
        }
        slotMat.color = new Color(0.1f, 0.6f, 0.1f, 0.8f); // Hafif koyu yeşil
        
        // Materyali assets klasörüne kaydedelim
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
        AssetDatabase.CreateAsset(slotMat, "Assets/Materials/CatSlotMaterial.mat");
        
        // Quad'a materyali ver
        slotObj.GetComponent<MeshRenderer>().sharedMaterial = slotMat;

        // Yazdığımız özel etkileşim kodunu ekle (Outline otomatik eklenecek)
        slotObj.AddComponent<SlotInteractable>();

        // OutlineController'ın parlama rengini belirgin bir sarı veya beyaza çekelim
        OutlineController outline = slotObj.GetComponent<OutlineController>();
        if (outline != null)
        {
            outline.highlightColor = new Color(1.5f, 1.5f, 0.5f, 1f); // Parlak sarı
        }

        // Prefab olarak kaydet
        string prefabPath = "Assets/Prefabs/CatSlotNode.prefab";
        PrefabUtility.SaveAsPrefabAsset(slotObj, prefabPath);
        
        // Sahnedeki kalıntıyı sil
        DestroyImmediate(slotObj);

        Debug.Log("CatSlotNode prefab'ı başarıyla Assets/Prefabs klasörüne oluşturuldu!");
        
        // Değişiklikleri kaydetmek için Editor'ü uyar
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
