using UnityEngine;
using UnityEditor;

public class AutoSpawnBlueCube
{
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        if (!EditorPrefs.GetBool("HasSpawnedBlueCube", false))
        {
            SpawnCube();
            EditorPrefs.SetBool("HasSpawnedBlueCube", true);
        }
    }

    [MenuItem("Tools/Spawn Blue Cube")]
    public static void SpawnCube()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "Blue Cube";
        
        Renderer renderer = cube.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Create a simple blue material using URP Lit shader
            Material blueMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            blueMat.color = new Color(0f, 0.5f, 1f); // Blue color
            renderer.material = blueMat;
        }
        
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(cube, "Spawn Blue Cube");
        Selection.activeObject = cube;
        
        Debug.Log("Mavi küp başarıyla sahneye eklendi!");
    }
}
