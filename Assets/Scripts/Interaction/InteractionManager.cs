using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    [Header("Raycast Ayarları")]
    public float interactDistance = 100f;
    public LayerMask interactableLayerMask = ~0; // Varsayılan olarak her layer

    public bool isLockedByUI = false;

    private IInteractable _currentHovered;
    private IInteractable _currentSelected;

    private Camera _mainCamera;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_mainCamera == null || isLockedByUI) return;
        
        HandleHover();
        HandleClick();
    }

    private void HandleHover()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayerMask))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            
            if (interactable != null)
            {
                if (_currentHovered != interactable)
                {
                    // Eski hover'ı temizle (eğer seçili değilse)
                    if (_currentHovered != null && _currentHovered != _currentSelected)
                    {
                        _currentHovered.OnHoverExit();
                    }
                        
                    _currentHovered = interactable;
                    
                    // Yeni hover'a gir (eğer zaten seçili değilse)
                    if (_currentHovered != _currentSelected)
                    {
                        _currentHovered.OnHoverEnter();
                    }
                }
            }
            else
            {
                ClearHover();
            }
        }
        else
        {
            ClearHover();
        }
    }

    private void ClearHover()
    {
        if (_currentHovered != null)
        {
            if (_currentHovered != _currentSelected)
            {
                _currentHovered.OnHoverExit();
            }
            _currentHovered = null;
        }
    }

    private void HandleClick()
    {
        if (Input.GetMouseButtonDown(0)) // M1 (Sol Tık)
        {
            if (_currentHovered != null)
            {
                if (_currentHovered == _currentSelected)
                {
                    // 2. Tıklama durumu: Obje zaten seçiliyken tekrar tıklandı (Etkileşim)
                    _currentSelected.OnInteract();
                    _currentSelected.OnHoverExit(); // Etkileşim bitince outline kapanır
                    _currentSelected = null;
                    _currentHovered = null; // Bir sonraki karede raycast yeniden algılasın
                }
                else
                {
                    // 1. Tıklama durumu: Yeni bir obje seçildi
                    if (_currentSelected != null)
                    {
                        _currentSelected.OnDeselect(); // Eski seçimi kaldır
                    }
                    _currentSelected = _currentHovered;
                    _currentSelected.OnSelect(); // Yeni seçimi etkinleştir
                }
            }
            else
            {
                // Boşluğa tıklandıysa mevcut seçimi temizle
                if (_currentSelected != null)
                {
                    _currentSelected.OnDeselect();
                    _currentSelected = null;
                }
            }
        }
    }
}
