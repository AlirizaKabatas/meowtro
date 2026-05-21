using UnityEngine;

public interface IInteractable
{
    // Fare obje üzerine geldiğinde çağrılır
    void OnHoverEnter();

    // Fare obje üzerinden çekildiğinde çağrılır
    void OnHoverExit();

    // Obje 1 kez tıklandığında (seçildiğinde) çağrılır
    void OnSelect();

    // Obje seçiliyken tekrar tıklandığında (etkileşim) çağrılır
    void OnInteract();

    // Obje seçiliyken başka bir yere tıklanırsa seçimi kaldırmak için
    void OnDeselect();
}
