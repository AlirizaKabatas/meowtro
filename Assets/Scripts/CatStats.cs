using UnityEngine;

public enum CatClass
{
    None,
    ClassicBeggar,
    SingerCat,
    MusicianCat,
    InjuredCat,
    BeautifulCat,
    ThiefCat,
    LivingStatue,
    MotherCat,
    DancerCat,
    ScoutCat
}

public enum CatGender
{
    Male,
    Female
}

public class CatStats : MonoBehaviour
{
    [Header("General Info")]
    public string catName = "Unnamed Cat";
    public Color catColor = Color.white;
    public CatGender gender;
    [Tooltip("Bu kedi nadir (Shiny) mi?")]
    public bool isShiny;
    [HideInInspector]
    public Sprite catIcon;

    [Header("Cat Classes (Max 2)")]
    [Tooltip("Kedinin ana sınıfı")]
    public CatClass primaryClass = CatClass.ClassicBeggar;
    
    [Tooltip("Kedinin yan sınıfı (Yoksa None seçin)")]
    public CatClass secondaryClass = CatClass.None;

    [Header("Cat Stats (0 - 100)")]
    [Range(0, 100)] public int efficiency;
    [Range(0, 100)] public int cuteness;
    [Range(0, 100)] public int charisma;
    [Range(0, 100)] public int meowing;
    [Range(0, 100)] public int rhythm;
    [Range(0, 100)] public int acting;
    [Range(0, 100)] public int sleightOfPaw;
    [Range(0, 100)] public int nurturing;

    // Inspector'da sınıfların aynı seçilmesini önlemek için küçük bir kontrol eklenebilir
    private void OnValidate()
    {
        // Eğer iki sınıf aynıysa ve None değilse uyarı verebiliriz veya ikinciyi None yapabiliriz
        if (primaryClass == secondaryClass && primaryClass != CatClass.None)
        {
            Debug.LogWarning("Bir kedinin Primary ve Secondary sınıfı aynı olamaz! Secondary sınıf None yapıldı.");
            secondaryClass = CatClass.None;
        }
    }

    [Header("Food Accumulation")]
    public int accumulatedKuruMama;
    public int accumulatedYasMama;
    public int accumulatedAcBitir;
    public int accumulatedKuruEt;

    private float _timer10s = 0f;
    private float _timer60s = 0f;

    private void Update()
    {
        if (CatInventoryManager.Instance != null && CatInventoryManager.Instance.GetCatState(gameObject) == CatState.Working)
        {
            _timer10s += Time.deltaTime;
            _timer60s += Time.deltaTime;

            if (_timer10s >= 10f)
            {
                _timer10s -= 10f;
                Generate10sFood();
            }

            if (_timer60s >= 60f)
            {
                _timer60s -= 60f;
                Generate60sFood();
            }
        }
    }

    private void Generate10sFood()
    {
        float kuruMult = 1f + ((efficiency + cuteness) / 200f);
        float yasMult = 1f + ((charisma + meowing) / 200f);
        
        accumulatedKuruMama += Mathf.RoundToInt(5f * kuruMult);
        accumulatedYasMama += Mathf.RoundToInt(2f * yasMult);
        
        // Refresh UI if this cat is currently selected in CatStats panel
        if (UIManager.Instance != null && UIManager.Instance.IsStatsOpen && UIManager.Instance.GetCurrentSelectedCat() == gameObject)
        {
            UIManager.Instance.RefreshFoodUI(this);
        }
    }

    private void Generate60sFood()
    {
        float acMult = 1f + ((rhythm + acting) / 200f);
        float etMult = 1f + ((sleightOfPaw + nurturing) / 200f);

        accumulatedAcBitir += Mathf.RoundToInt(3f * acMult);
        accumulatedKuruEt += Mathf.RoundToInt(2f * etMult);

        if (UIManager.Instance != null && UIManager.Instance.IsStatsOpen && UIManager.Instance.GetCurrentSelectedCat() == gameObject)
        {
            UIManager.Instance.RefreshFoodUI(this);
        }
    }
    
    public void CollectFood()
    {
        if (FoodManager.Instance != null)
        {
            FoodManager.Instance.AddAll(accumulatedYasMama, accumulatedAcBitir, accumulatedKuruMama, accumulatedKuruEt);
            accumulatedYasMama = 0;
            accumulatedAcBitir = 0;
            accumulatedKuruMama = 0;
            accumulatedKuruEt = 0;
            
            if (UIManager.Instance != null && UIManager.Instance.IsStatsOpen && UIManager.Instance.GetCurrentSelectedCat() == gameObject)
            {
                UIManager.Instance.RefreshFoodUI(this);
            }
        }
    }
}
