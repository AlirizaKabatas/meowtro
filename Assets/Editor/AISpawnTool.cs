using UnityEngine;
using UnityEditor;

public class AISpawnTool
{
    // Auto-run kaldırıldı, artık aynı objeleri tekrar spawnlamayacak.

    [MenuItem("Tools/Spawn AI Setup")]
    public static void SetupAI()
    {
        // 1. Zemin (Plane) oluştur
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Ground";
        plane.transform.position = Vector3.zero;
        
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        if (planeRenderer != null)
        {
            Material groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            groundMat.color = new Color(0.2f, 0.3f, 0.2f); // Koyu yeşil
            planeRenderer.material = groundMat;
        }

        // 2. Rota noktalarını (Waypoints) oluştur
        GameObject waypointGroup = new GameObject("Waypoints");
        
        Transform[] waypoints = new Transform[4];
        Vector3[] positions = new Vector3[] {
            new Vector3(-4, 0, 4),  // Spawn Waypoint (Yeşil)
            new Vector3(4, 0, 4),   // Normal (Sarı)
            new Vector3(4, 0, -4),  // Normal (Sarı)
            new Vector3(-4, 0, -4)  // Disappear Waypoint (Kırmızı)
        };

        for (int i = 0; i < 4; i++)
        {
            string wpName = (i == 0) ? "Spawn Waypoint" : ((i == 3) ? "Disappear Waypoint" : "Waypoint " + i);
            GameObject wp = new GameObject(wpName);
            wp.transform.position = positions[i];
            wp.transform.SetParent(waypointGroup.transform);
            waypoints[i] = wp.transform;
            
            // Editörde görmek için küçük küreler ekle
            GameObject wpVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            wpVisual.transform.SetParent(wp.transform);
            wpVisual.transform.localPosition = Vector3.zero;
            wpVisual.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            
            Renderer wpRenderer = wpVisual.GetComponent<Renderer>();
            if (wpRenderer != null)
            {
                Material wpMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                if (i == 0) wpMat.color = Color.green;
                else if (i == 3) wpMat.color = Color.red;
                else wpMat.color = Color.yellow;
                
                wpRenderer.material = wpMat;
            }
            
            // Çarpışmaları engellemek için collider'ı sil
            Object.DestroyImmediate(wpVisual.GetComponent<Collider>());
        }

        // 3. AI (Capsule) oluştur
        GameObject aiAgent = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        aiAgent.name = "AI Agent";
        
        // Gerçek insan boyu yaklaşık 1.80m. Unity'de Capsule default 2 metredir.
        // Boyunu 1.8m yapmak için Y eksenindeki scale değerini 0.9 yapıyoruz.
        aiAgent.transform.localScale = new Vector3(1f, 0.9f, 1f);
        
        // Y ekseni scale'i 0.9 olduğunda, kapsülün tam yere basması için merkez yüksekliğini 0.9'a alıyoruz.
        aiAgent.transform.position = new Vector3(-4, 0.9f, 4);
        
        Renderer aiRenderer = aiAgent.GetComponent<Renderer>();
        if (aiRenderer != null)
        {
            Material aiMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            aiMat.color = new Color(0.8f, 0.2f, 0.2f); // Kırmızımsı
            aiRenderer.material = aiMat;
        }

        // Scripti ekle ve rotayı bağla
        PathFollowerAI follower = aiAgent.AddComponent<PathFollowerAI>();
        follower.route = new WaypointData[4];
        for (int i = 0; i < 4; i++)
        {
            follower.route[i] = new WaypointData { pointTransform = waypoints[i] };
        }

        // Geri alma (Undo) sistemi için kayıt
        Undo.RegisterCreatedObjectUndo(plane, "Spawn AI Setup");
        Undo.RegisterCreatedObjectUndo(waypointGroup, "Spawn AI Setup");
        Undo.RegisterCreatedObjectUndo(aiAgent, "Spawn AI Setup");

        // Seçimi AI'a odakla
        Selection.activeGameObject = aiAgent;
        
        Debug.Log("Zemin, Rota ve AI başarıyla eklendi! Play butonuna basarak hareketini izleyebilirsiniz.");
    }
}
