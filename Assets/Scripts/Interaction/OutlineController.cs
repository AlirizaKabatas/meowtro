using UnityEngine;

public class OutlineController : MonoBehaviour
{
    private Renderer[] _renderers;
    private Color[] _originalColors;
    
    [Header("Outline / Highlight Ayarları")]
    public Color highlightColor = new Color(1.5f, 1.5f, 1.5f, 1f); // Rengi biraz parlaklaştırmak için
    
    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];
        
        for (int i = 0; i < _renderers.Length; i++)
        {
            // URP'de varsayılan renk özelliği "_BaseColor" şeklindedir. Standart pipeline'da "_Color".
            if (_renderers[i].material.HasProperty("_BaseColor"))
            {
                _originalColors[i] = _renderers[i].material.GetColor("_BaseColor");
            }
            else if (_renderers[i].material.HasProperty("_Color"))
            {
                _originalColors[i] = _renderers[i].material.GetColor("_Color");
            }
        }
    }

    public void SetOutline(bool active)
    {
        // İleride Asset Store'dan bir Outline paketi (örn: QuickOutline) indirirseniz,
        // bu metodun içini silip o paketin aç/kapat kodunu yazabilirsiniz.
        for (int i = 0; i < _renderers.Length; i++)
        {
            Color targetColor = active ? _originalColors[i] * highlightColor : _originalColors[i];
            
            if (_renderers[i].material.HasProperty("_BaseColor"))
            {
                _renderers[i].material.SetColor("_BaseColor", targetColor);
            }
            else if (_renderers[i].material.HasProperty("_Color"))
            {
                _renderers[i].material.SetColor("_Color", targetColor);
            }
        }
    }
}
