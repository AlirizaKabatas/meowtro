using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class HierarchyOrganizerEditor : EditorWindow
{
    [MenuItem("MEOWTRO/Sahneyi Düzenle (MetroScene vb.)")]
    public static void OrganizeScene()
    {
        // "Sahne" isimli ana klasör objesini bul veya oluştur
        GameObject rootSahne = GameObject.Find("Sahne");
        if (rootSahne == null)
        {
            rootSahne = new GameObject("Sahne");
            Undo.RegisterCreatedObjectUndo(rootSahne, "Create Sahne Root");
        }

        // Sahnedeki tüm root (en üst seviye) objeleri al
        Scene activeScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = activeScene.GetRootGameObjects();

        Dictionary<string, GameObject> groupFolders = new Dictionary<string, GameObject>();

        foreach (GameObject obj in rootObjects)
        {
            // Önemli objeleri dışarıda bırak:
            // Ana Kamera, Işık, Yöneticiler, "Sahne" objesinin kendisi ve Kamera Nodları
            if (obj == rootSahne ||
                obj.name == "Main Camera" ||
                obj.name == "Directional Light" ||
                obj.name == "GameManager" ||
                obj.name == "InteractionManager" ||
                obj.GetComponentInChildren<CameraNode>() != null ||
                obj.GetComponent<Camera>() != null)
            {
                continue; // Bunları atla, dışarıda kalsınlar
            }

            // Objenin isminden kopyalama eklerini sil (örn: "Cube (1)" -> "Cube")
            string baseName = Regex.Replace(obj.name, @"\s*\(\d+\)", "").Trim();

            // Eğer bu isimde bir grup klasörü (boş obje) henüz yoksa, oluştur
            if (!groupFolders.ContainsKey(baseName))
            {
                // Önce "Sahne" objesinin altında bu isimde bir grup var mı diye bakalım
                Transform existingGroup = rootSahne.transform.Find(baseName + "_Group");
                GameObject groupObj;
                
                if (existingGroup != null)
                {
                    groupObj = existingGroup.gameObject;
                }
                else
                {
                    groupObj = new GameObject(baseName + "_Group");
                    groupObj.transform.SetParent(rootSahne.transform);
                    Undo.RegisterCreatedObjectUndo(groupObj, "Create Group");
                }
                
                groupFolders[baseName] = groupObj;
            }

            // Objeyi ilgili grubun içine at
            Undo.SetTransformParent(obj.transform, groupFolders[baseName].transform, "Group Object");
        }

        Debug.Log("Sahne başarıyla düzenlendi! Tüm kopya objeler gruplanıp 'Sahne' objesinin altına taşındı. Kameralar dışarıda bırakıldı.");
    }
}
