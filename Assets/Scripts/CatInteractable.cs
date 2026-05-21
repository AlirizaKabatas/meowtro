using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CatStats))]
public class CatInteractable : MonoBehaviour
{
    private CatStats catStats;

    private void Awake()
    {
        catStats = GetComponent<CatStats>();
    }

    private void OnMouseDown()
    {
        // Eğer tıklama bir UI elemanının üzerindeyse (örn. panelin kendisi), algılama
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (CatPanelManager.Instance != null && catStats != null)
        {
            CatPanelManager.Instance.OpenPanel(catStats);
        }
    }
}
