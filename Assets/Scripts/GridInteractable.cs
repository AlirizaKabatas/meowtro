using UnityEngine;

public class GridInteractable : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;
    
    [Header("Colors")]
    [Tooltip("Mouse üzerine gelince alacağı renk")]
    public Color hoverColor = Color.green;
    
    [Tooltip("Tıklanınca (Panel açıkken) alacağı renk")]
    public Color pressedColor = Color.green;
    
    [Header("UI")]
    [Tooltip("Tıklanınca açılacak UI Panel Objesi (Hiyerarşiden sürükleyin)")]
    public GameObject targetPanel;
    
    public bool isOccupied = false;
    private bool isSelected = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null && rend.material != null)
        {
            originalColor = rend.material.color;
        }

        // Oyun başında panelin kapalı olduğundan emin ol
        if (targetPanel != null)
        {
            targetPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Panel açıksa ve ESC'ye basılırsa kapat ve rengi düzelt
        if (isSelected && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    void OnMouseEnter()
    {
        if (!isOccupied && !isSelected && rend != null && rend.material != null)
        {
            rend.material.color = hoverColor;
        }
    }

    void OnMouseExit()
    {
        if (!isOccupied && !isSelected && rend != null && rend.material != null)
        {
            rend.material.color = originalColor;
        }
    }

    void OnMouseDown()
    {
        if (isOccupied)
        {
            Debug.Log("Bu grid hücresi zaten dolu!");
            return;
        }
        if (isSelected) return; // Zaten açıksa bir şey yapma
        
        isSelected = true;
        if (rend != null && rend.material != null)
        {
            rend.material.color = pressedColor;
        }

        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Grid hücresinde açılacak Panel atanmamış!");
        }
    }

    public void ClosePanel()
    {
        Deselect();

        if (targetPanel != null)
        {
            targetPanel.SetActive(false);
        }
    }

    public void Deselect()
    {
        isSelected = false;
        if (!isOccupied && rend != null && rend.material != null)
        {
            rend.material.color = originalColor;
        }
    }

    public void MarkAsOccupied()
    {
        isOccupied = true;
        isSelected = false;
        if (rend != null && rend.material != null)
        {
            // Orijinal rengini biraz koyu yap
            originalColor = new Color(0.3f, 0.3f, 0.3f); 
            rend.material.color = originalColor;
        }
    }
}
