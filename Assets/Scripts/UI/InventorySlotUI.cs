using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image iconImage;
    public GameObject breedingIcon;
    public GameObject workingIcon;

    private GameObject _assignedCat;

    public void Setup(GameObject cat)
    {
        _assignedCat = cat;
        if (iconImage != null && cat != null)
        {
            CatStats stats = cat.GetComponent<CatStats>();
            if (stats != null)
            {
                if (stats.catIcon != null)
                {
                    iconImage.sprite = stats.catIcon;
                    iconImage.color = Color.white;
                }
                else
                {
                    iconImage.sprite = null;
                    iconImage.color = stats.catColor; // fail-safe
                }
            }
            else
            {
                iconImage.sprite = null;
                iconImage.color = new Color(1, 1, 1, 1f); 
            }
        }

        if (CatInventoryManager.Instance != null)
        {
            CatState state = CatInventoryManager.Instance.GetCatState(cat);
            if (breedingIcon != null) breedingIcon.SetActive(state == CatState.Breeding);
            if (workingIcon != null) workingIcon.SetActive(state == CatState.Working);
        }
    }

    public void SetupEmpty()
    {
        _assignedCat = null;
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.color = new Color(1, 1, 1, 0.1f); 
        }
        if (breedingIcon != null) breedingIcon.SetActive(false);
        if (workingIcon != null) workingIcon.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_assignedCat != null && UIManager.Instance != null)
        {
            CatStats stats = _assignedCat.GetComponent<CatStats>();
            if (stats != null)
            {
                string tooltipInfo = $"<b><size=26>{stats.catName}</size></b>\n";
                tooltipInfo += "<b>Class:</b> " + stats.primaryClass.ToString();
                if (stats.secondaryClass != CatClass.None)
                {
                    tooltipInfo += "\n<b>Class 2:</b> " + stats.secondaryClass.ToString();
                }

                // Ekleme: Çalışıyor veya Çiftleşiyorsa tooltipte uyaralım
                CatState state = CatInventoryManager.Instance.GetCatState(_assignedCat);
                if (state == CatState.Breeding) tooltipInfo += "\n<color=red>[BREEDING]</color>";
                if (state == CatState.Working) tooltipInfo += "\n<color=yellow>[WORKING]</color>";

                UIManager.Instance.ShowTooltip(tooltipInfo);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideTooltip();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_assignedCat != null && UIManager.Instance != null && !UIManager.Instance.IsPopupOpen)
        {
            UIManager.Instance.HandleCatClickFromInventory(_assignedCat);
        }
    }
}
