using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("Geçiş Ayarları")]
    [Tooltip("Kameranın yeni konuma geçiş süresi (saniye cinsinden)")]
    public float transitionDuration = 1.0f;

    public bool isLockedByUI = false;

    private bool _isTransitioning = false;
    private CameraNode _activeNode = null;

    private float _currentPitch;
    private float _currentYaw;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetActiveNode(CameraNode node)
    {
        _activeNode = node;
        _currentPitch = _activeNode.DefaultPitch;
        _currentYaw = _activeNode.DefaultYaw;
    }

    public void MoveToNode(Transform targetNode)
    {
        if (!_isTransitioning)
        {
            StartCoroutine(TransitionRoutine(targetNode));
        }
    }

    private IEnumerator TransitionRoutine(Transform target)
    {
        _isTransitioning = true;
        _activeNode = null; // Geçiş esnasında dönüş (look) kontrollerini kapat
        
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        
        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            
            // Yumuşak geçiş için Ease-in-out
            t = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);

            yield return null;
        }

        // Geçiş sonunda tam hedefe oturt
        transform.position = endPos;
        transform.rotation = endRot;

        _isTransitioning = false;
        
        // Hedefe vardığımızda kamerayı kontrol edebilmek için Active Node'u ayarla
        CameraNode node = target.GetComponent<CameraNode>();
        if (node != null)
        {
            SetActiveNode(node);
        }
    }

    private void Update()
    {
        // Geçiş esnasında, bir kameraya bağlı değilken veya UI açıkken dönüş yapma
        if (_isTransitioning || _activeNode == null || isLockedByUI) return;

        float yawInput = 0f;
        float pitchInput = 0f;

        // Farenin Sağ Tuşu (Mouse 1) ile hızlı dönüş
        if (Input.GetMouseButton(1))
        {
            yawInput = Input.GetAxis("Mouse X") * _activeNode.mouseSpeed * Time.deltaTime;
            pitchInput = Input.GetAxis("Mouse Y") * _activeNode.mouseSpeed * Time.deltaTime;
        }
        else // WASD ile yavaş dönüş
        {
            yawInput = Input.GetAxis("Horizontal") * _activeNode.wasdSpeed * Time.deltaTime;
            pitchInput = Input.GetAxis("Vertical") * _activeNode.wasdSpeed * Time.deltaTime;
        }

        if (yawInput != 0f || pitchInput != 0f)
        {
            _currentYaw += yawInput;
            _currentPitch -= pitchInput; // Mouse/Klavye yukarı iken pitch açısının düşmesi gerekir

            // Yukarı/Aşağı Sınırlandırma (Pitch)
            _currentPitch = Mathf.Clamp(
                _currentPitch, 
                _activeNode.DefaultPitch + _activeNode.minPitch, 
                _activeNode.DefaultPitch + _activeNode.maxPitch
            );

            // Sağa/Sola Sınırlandırma (Yaw)
            if (!_activeNode.unlimitedYaw)
            {
                _currentYaw = Mathf.Clamp(
                    _currentYaw, 
                    _activeNode.DefaultYaw + _activeNode.minYaw, 
                    _activeNode.DefaultYaw + _activeNode.maxYaw
                );
            }

            transform.rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
        }
    }
}
