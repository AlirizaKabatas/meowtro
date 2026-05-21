using UnityEngine;

public class Cat3DStudio : MonoBehaviour
{
    public static Cat3DStudio Instance { get; private set; }

    public Camera studioCamera;
    public RenderTexture renderTexture;
    
    private GameObject _currentModel;
    private float _rotationSpeed = 30f; // Yavaşça döner

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SetupStudio();
    }

    private void SetupStudio()
    {
        // 1. Studio objesini oyuncunun göremeyeceği çok uzak bir yere taşı
        transform.position = new Vector3(0, -5000, 0);

        // 2. Stüdyo Kamerası oluştur
        GameObject camObj = new GameObject("StudioCamera");
        camObj.transform.SetParent(transform);
        camObj.transform.localPosition = new Vector3(0, 1.5f, -3f); // Kamerayı biraz yukarı ve geriye al
        camObj.transform.localRotation = Quaternion.Euler(15f, 0, 0); // Hafif aşağı baksın
        
        studioCamera = camObj.AddComponent<Camera>();
        studioCamera.clearFlags = CameraClearFlags.SolidColor;
        studioCamera.backgroundColor = new Color(0.12f, 0.12f, 0.12f, 1f); // Koyu gri stüdyo arkaplanı
        studioCamera.cullingMask = 1 << 31; // Sadece "31" katmanını görsün

        // 3. Render Texture Oluştur (URP Hatalarını önlemek için ARGB32 formatını zorluyoruz)
        renderTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
        renderTexture.name = "CatStudioRT";
        renderTexture.Create();
        studioCamera.targetTexture = renderTexture;
        
        // 4. Stüdyo Işıklandırması
        GameObject lightObj = new GameObject("StudioLight");
        lightObj.transform.SetParent(transform);
        lightObj.transform.localPosition = new Vector3(-2, 3, -2);
        lightObj.transform.localRotation = Quaternion.Euler(45, 45, 0);
        Light dirLight = lightObj.AddComponent<Light>();
        dirLight.type = LightType.Directional;
        dirLight.intensity = 1.2f;
        dirLight.cullingMask = 1 << 31;
    }

    public void ShowCat(GameObject catInstance)
    {
        ClearCat();

        if (catInstance != null)
        {
            // Orijinal kedinin tamamen görsel (işlevsiz) bir kopyasını alıyoruz
            _currentModel = Instantiate(catInstance, transform);
            _currentModel.transform.localPosition = Vector3.zero;
            
            // Prefab'ın kendi orijinal rotasyonunu koru (Ters durmasını engeller)
            _currentModel.transform.localRotation = catInstance.transform.localRotation;
            
            // Eğer DefaultCat objesi büyükse ufaltılabilir, şimdilik birebir kopyala
            _currentModel.transform.localScale = Vector3.one; 
            _currentModel.SetActive(true);

            // Tüm objeleri 31. katmana taşı ki ana kamera görmesin, sadece stüdyo görsün
            SetLayerRecursively(_currentModel, 31);

            // Üzerindeki scriptleri (çalışmaması için) kapat (Silmek RequireComponent hatasına yol açar)
            MonoBehaviour[] scripts = _currentModel.GetComponentsInChildren<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script != null) script.enabled = false; 
            }
        }
    }

    public Sprite TakeSnapshot(GameObject catPrefab)
    {
        if (catPrefab == null) return null;

        // Kamera ve Render Texture kurulu olduğundan emin ol
        if (studioCamera == null || renderTexture == null)
        {
            SetupStudio();
        }

        // Görsel kopyayı stüdyoya yerleştir
        GameObject tempModel = Instantiate(catPrefab, transform);
        tempModel.transform.localPosition = Vector3.zero;
        
        // Prefab'ın orijinal rotasyonuyla orantılı hafif bir açı veriyoruz (Ters durmaz)
        tempModel.transform.localRotation = catPrefab.transform.localRotation * Quaternion.Euler(15f, 30f, 0f);
        tempModel.transform.localScale = Vector3.one;
        tempModel.SetActive(true);

        SetLayerRecursively(tempModel, 31);

        MonoBehaviour[] scripts = tempModel.GetComponentsInChildren<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != null) script.enabled = false;
        }

        // Sahnedeki diğer objeleri etkilememesi için hemen render alıyoruz
        RenderTexture currentActive = RenderTexture.active;
        RenderTexture.active = renderTexture;
        
        studioCamera.Render();

        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();

        RenderTexture.active = currentActive;

        // Hemen temizle
        DestroyImmediate(tempModel);

        // Texture'ı Sprite'a dönüştür
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    public void ClearCat()
    {
        if (_currentModel != null)
        {
            Destroy(_currentModel);
            _currentModel = null;
        }
    }

    private void Update()
    {
        if (_currentModel != null)
        {
            // Kendi yerel ekseninde (Space.Self) yavaşça döndür, böylece eğik modeller düzgün döner
            _currentModel.transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
