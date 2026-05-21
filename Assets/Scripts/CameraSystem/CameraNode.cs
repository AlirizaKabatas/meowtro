using UnityEngine;

public class CameraNode : MonoBehaviour, IInteractable
{
    [Header("Duvar Kontrol Ayarları")]
    [Tooltip("Kamera geçişini engelleyecek katmanlar (Örn: Wall veya Environment)")]
    public LayerMask obstacleLayerMask;

    [Header("Başlangıç Ayarları")]
    [Tooltip("Oyun başladığında Ana Kamera (Main Camera) direkt olarak bu noktadan başlasın mı?")]
    public bool isStartingNode = false;

    [Header("Dönüş (Look) Hızları")]
    [Tooltip("WASD tuşlarıyla dönüş hızı")]
    public float wasdSpeed = 50f;
    [Tooltip("Farenin sağ tıkıyla dönüş hızı")]
    public float mouseSpeed = 250f;

    [Header("Dikey Dönüş Sınırları (Yukarı/Aşağı)")]
    [Tooltip("Ne kadar yukarı bakabileceği (Eksi değer girilmelidir, örn: -40)")]
    public float minPitch = -40f;
    [Tooltip("Ne kadar aşağı bakabileceği (Artı değer girilmelidir, örn: 50)")]
    public float maxPitch = 50f;

    [Header("Yatay Dönüş Sınırları (Sağa/Sola)")]
    [Tooltip("Eğer işaretlenirse sola ve sağa bir sınır olmadan 360 derece dönebilir.")]
    public bool unlimitedYaw = false;
    [Tooltip("Ne kadar sola dönebileceği (Eksi değer, örn: -90)")]
    public float minYaw = -90f;
    [Tooltip("Ne kadar sağa dönebileceği (Artı değer, örn: 90)")]
    public float maxYaw = 90f;

    // Kameranın varsayılan merkez açıları
    public float DefaultPitch { get; private set; }
    public float DefaultYaw { get; private set; }

    private OutlineController _outlineController;

    private void Awake()
    {
        _outlineController = GetComponent<OutlineController>();
        
        // Objenin sahnede konulduğu ilk açıyı merkeze al
        Vector3 euler = transform.rotation.eulerAngles;
        
        DefaultPitch = euler.x;
        if (DefaultPitch > 180f) DefaultPitch -= 360f; // -180 ile 180 arasına sıkıştır
        
        DefaultYaw = euler.y;
        if (DefaultYaw > 180f) DefaultYaw -= 360f;
    }

    private void Start()
    {
        if (isStartingNode)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.transform.position = transform.position;
                mainCam.transform.rotation = transform.rotation;
                
                if (CameraController.Instance != null)
                {
                    CameraController.Instance.SetActiveNode(this);
                }
            }
        }
    }

    public void OnHoverEnter()
    {
        if (_outlineController != null) _outlineController.SetOutline(true);
    }

    public void OnHoverExit()
    {
        if (_outlineController != null) _outlineController.SetOutline(false);
    }

    public void OnSelect()
    {
        if (_outlineController != null) _outlineController.SetOutline(true);
    }

    public void OnDeselect()
    {
        if (_outlineController != null) _outlineController.SetOutline(false);
    }

    public void OnInteract()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Vector3 startPos = mainCam.transform.position;
            Vector3 endPos = transform.position;
            Vector3 direction = endPos - startPos;
            float distance = direction.magnitude;

            if (Physics.Raycast(startPos, direction.normalized, out RaycastHit hit, distance, obstacleLayerMask))
            {
                Debug.Log($"Geçiş engellendi! Arada bir engel var: {hit.collider.name}");
                return;
            }

            if (CameraController.Instance != null)
            {
                CameraController.Instance.MoveToNode(transform);
            }
            else
            {
                Debug.LogError("CameraController bulunamadı.");
            }
        }
    }
}
