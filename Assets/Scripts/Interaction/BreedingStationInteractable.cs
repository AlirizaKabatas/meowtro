using UnityEngine;

[RequireComponent(typeof(OutlineController))]
public class BreedingStationInteractable : MonoBehaviour, IInteractable
{
    private OutlineController _outlineController;

    private void Awake()
    {
        _outlineController = GetComponent<OutlineController>();
    }

    public void OnHoverEnter()
    {
        if (UIManager.Instance != null && (UIManager.Instance.IsInventoryOpen || UIManager.Instance.IsBreedingOpen)) return;
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
            UIManager.Instance.OpenBreeding();
        }
    }
}
