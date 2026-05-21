using UnityEngine;

[RequireComponent(typeof(OutlineController))]
public class SlotInteractable : MonoBehaviour, IInteractable
{
    private OutlineController _outlineController;
    public bool isOccupied = false;
    public GameObject occupiedCat = null;

    private void Awake()
    {
        _outlineController = GetComponent<OutlineController>();
    }

    public void OnHoverEnter()
    {
        // Eğer UI açıksa outline tetiklenme
        if (UIManager.Instance != null && (UIManager.Instance.IsInventoryOpen || UIManager.Instance.IsStatsOpen)) return;
        
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
        if (UIManager.Instance != null)
        {
            if (isOccupied)
            {
                UIManager.Instance.OpenCatStatsForRemove(this);
            }
            else
            {
                UIManager.Instance.OpenInventory(this);
            }
        }
        else
        {
            Debug.LogError("UIManager sahne üzerinde bulunamadı!");
        }
    }

    public void MarkAsOccupied(GameObject cat)
    {
        isOccupied = true;
        occupiedCat = cat;
        if (_outlineController != null) _outlineController.SetOutline(false);
    }

    public void ClearOccupation()
    {
        isOccupied = false;
        occupiedCat = null;
    }
}
