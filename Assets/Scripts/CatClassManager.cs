using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CatClassManager : MonoBehaviour
{
    public static CatClassManager Instance { get; private set; }

    [SerializeField]
    private List<CatClass> unlockedClasses = new List<CatClass>();

    private const string PREFS_KEY = "UnlockedCatClasses";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        LoadUnlockedClasses();
    }

    private void LoadUnlockedClasses()
    {
        unlockedClasses.Clear();

        // Her zaman açık olması gereken başlangıç sınıfları
        UnlockClass(CatClass.ClassicBeggar, false);
        UnlockClass(CatClass.BeautifulCat, false);

        string savedData = PlayerPrefs.GetString(PREFS_KEY, "");
        if (!string.IsNullOrEmpty(savedData))
        {
            string[] classNames = savedData.Split(',');
            foreach (string className in classNames)
            {
                if (System.Enum.TryParse(className, out CatClass parsedClass))
                {
                    UnlockClass(parsedClass, false);
                }
            }
        }
    }

    private void SaveUnlockedClasses()
    {
        List<string> classNames = unlockedClasses.Select(c => c.ToString()).ToList();
        PlayerPrefs.SetString(PREFS_KEY, string.Join(",", classNames));
        PlayerPrefs.Save();
    }

    public void UnlockClass(CatClass newClass, bool save = true)
    {
        if (newClass != CatClass.None && !unlockedClasses.Contains(newClass))
        {
            unlockedClasses.Add(newClass);
            Debug.Log($"[CatClassManager] Yeni Sınıf Açıldı: {newClass}!");
            
            if (save)
            {
                SaveUnlockedClasses();
            }
        }
    }

    public CatClass GetRandomUnlockedClass(List<CatClass> excludeClasses)
    {
        List<CatClass> validClasses = unlockedClasses.Where(c => !excludeClasses.Contains(c)).ToList();

        if (validClasses.Count > 0)
        {
            return validClasses[Random.Range(0, validClasses.Count)];
        }
        
        return CatClass.None;
    }

    [ContextMenu("Sıfırla (Kayıtları Sil)")]
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(PREFS_KEY);
        LoadUnlockedClasses();
        Debug.Log("[CatClassManager] Tüm sınıf ilerlemeleri sıfırlandı!");
    }
}
