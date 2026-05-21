using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CatStats))]
public class CatBreeding : MonoBehaviour
{
    [Header("Parents")]
    public CatStats mother;
    public CatStats father;

    private CatStats babyStats;

    private void Start()
    {
        babyStats = GetComponent<CatStats>();
        
        if (mother != null && father != null)
        {
            Breed();
        }
        else
        {
            Debug.LogWarning("Anne veya baba atanmadığı için çiftleşme yapılamadı.");
        }
    }

    [ContextMenu("Tekrar Çiftleştir (Test)")]
    public void Breed()
    {
        if (babyStats == null) babyStats = GetComponent<CatStats>();

        // 1. Cinsiyet Belirleme
        babyStats.gender = (Random.value > 0.5f) ? CatGender.Male : CatGender.Female;
        babyStats.isShiny = false; // Varsayılan

        // 2. Class Belirleme
        DetermineClasses();

        // 3. Stat Belirleme
        babyStats.efficiency = CalculateStat(mother.efficiency, father.efficiency);
        babyStats.cuteness = CalculateStat(mother.cuteness, father.cuteness);
        babyStats.charisma = CalculateStat(mother.charisma, father.charisma);
        babyStats.meowing = CalculateStat(mother.meowing, father.meowing);
        babyStats.rhythm = CalculateStat(mother.rhythm, father.rhythm);
        babyStats.acting = CalculateStat(mother.acting, father.acting);
        babyStats.sleightOfPaw = CalculateStat(mother.sleightOfPaw, father.sleightOfPaw);
        babyStats.nurturing = CalculateStat(mother.nurturing, father.nurturing);
        
        if (CatClassManager.Instance != null)
        {
            CatClassManager.Instance.UnlockClass(babyStats.primaryClass);
            CatClassManager.Instance.UnlockClass(babyStats.secondaryClass);
        }

        Debug.Log($"[Breeding] Yeni yavru doğdu! Cinsiyet: {babyStats.gender}, Shiny: {babyStats.isShiny}, Class 1: {babyStats.primaryClass}, Class 2: {babyStats.secondaryClass}");
    }

    private void DetermineClasses()
    {
        List<CatClass> motherClasses = GetValidClasses(mother);
        List<CatClass> fatherClasses = GetValidClasses(father);

        // Anne ve babanın classlarının tamamen aynı olup olmadığını kontrol et
        bool areClassesIdentical = motherClasses.Count > 0 && 
                                   motherClasses.Count == fatherClasses.Count && 
                                   !motherClasses.Except(fatherClasses).Any();

        if (areClassesIdentical)
        {
            // İkisi de birebir aynı classlara sahipse yavru kesinlikle o classları alır ve Shiny (Şanslı) olur!
            babyStats.isShiny = true;
            babyStats.primaryClass = motherClasses[0];
            babyStats.secondaryClass = (motherClasses.Count > 1) ? motherClasses[1] : CatClass.None;
        }
        else
        {
            // Anne ve baba farklı classlara sahipse (Miras Havuzu Oluştur)
            HashSet<CatClass> inheritedPool = new HashSet<CatClass>(motherClasses);
            inheritedPool.UnionWith(fatherClasses);
            List<CatClass> poolList = inheritedPool.ToList();

            float randomVal = Random.value;
            // Olasılıklar:
            // %40 İhtimalle Anne veya Babanın 1 sınıfını alır
            // %30 İhtimalle Anne veya Babanın 2 sınıfını birden alır
            // %20 İhtimalle 1 Miras alınan sınıf + 1 Tamamen yeni sınıf alır (Mutasyon)
            // %10 İhtimalle Sadece 1 Tamamen yeni sınıf alır (Tam Mutasyon)

            if (randomVal < 0.40f || poolList.Count == 0) // %40 - 1 Inherited
            {
                babyStats.primaryClass = poolList.Count > 0 ? poolList[Random.Range(0, poolList.Count)] : GetRandomClass(new List<CatClass>());
                babyStats.secondaryClass = CatClass.None;
            }
            else if (randomVal < 0.70f) // %30 - 2 Inherited
            {
                if (poolList.Count >= 2)
                {
                    poolList = poolList.OrderBy(x => Random.value).ToList();
                    babyStats.primaryClass = poolList[0];
                    babyStats.secondaryClass = poolList[1];
                }
                else
                {
                    // Havuzda sadece 1 class varsa onu al, yanını None bırak
                    babyStats.primaryClass = poolList[0];
                    babyStats.secondaryClass = CatClass.None;
                }
            }
            else if (randomVal < 0.90f) // %20 - 1 Inherited + 1 Mutated
            {
                babyStats.primaryClass = poolList[Random.Range(0, poolList.Count)];
                CatClass mutated = GetRecipeMutation(poolList);
                
                if (mutated != CatClass.None && mutated != babyStats.primaryClass)
                {
                    babyStats.secondaryClass = mutated;
                }
                else
                {
                    List<CatClass> exclude = new List<CatClass> { babyStats.primaryClass };
                    babyStats.secondaryClass = GetRandomClass(exclude); // Havuzda olmayan yepyeni bir sınıf
                }
            }
            else // %10 - 1 Mutated (Ailesiyle alakası olmayan bir sınıf)
            {
                CatClass mutated = GetRecipeMutation(poolList);
                if (mutated != CatClass.None)
                {
                    babyStats.primaryClass = mutated;
                }
                else
                {
                    babyStats.primaryClass = GetRandomClass(poolList);
                }
                babyStats.secondaryClass = CatClass.None;
            }
        }
    }

    private int CalculateStat(int motherStat, int fatherStat)
    {
        int baseStat = (motherStat + fatherStat) / 2;

        if (babyStats.isShiny)
        {
            // Shiny kedi ise statı KESİNLİKLE anne ve babasının ortalamasından yüksek olmalı!
            int bonus = Random.Range(3, 16); // +3 ile +15 arası bonus stat
            return Mathf.Clamp(baseStat + bonus, 0, 100);
        }
        else
        {
            // Normal yavru kedi
            float rand = Random.value;
            int change = Random.Range(1, 11); // 1 ile 10 arası puan oynar
            
            if (rand < 0.33f)
            {
                // Biraz üstü
                return Mathf.Clamp(baseStat + change, 0, 100);
            }
            else if (rand < 0.66f)
            {
                // Biraz altı
                return Mathf.Clamp(baseStat - change, 0, 100);
            }
            else
            {
                // Ortalama (Aynı)
                return Mathf.Clamp(baseStat, 0, 100);
            }
        }
    }

    private List<CatClass> GetValidClasses(CatStats parent)
    {
        List<CatClass> classes = new List<CatClass>();
        if (parent != null)
        {
            if (parent.primaryClass != CatClass.None) classes.Add(parent.primaryClass);
            if (parent.secondaryClass != CatClass.None && parent.secondaryClass != parent.primaryClass) classes.Add(parent.secondaryClass);
        }
        return classes;
    }

    private CatClass GetRandomClass(List<CatClass> excludeClasses)
    {
        if (CatClassManager.Instance != null)
        {
            return CatClassManager.Instance.GetRandomUnlockedClass(excludeClasses);
        }
        else
        {
            Debug.LogWarning("CatClassManager sahnede bulunamadı! Fallback olarak None dönüyor.");
            return CatClass.None;
        }
    }

    private CatClass GetRecipeMutation(List<CatClass> poolList)
    {
        List<CatClass> possibleMutations = new List<CatClass>();

        // Tier 2
        if (poolList.Contains(CatClass.ClassicBeggar) && poolList.Contains(CatClass.BeautifulCat)) possibleMutations.Add(CatClass.SingerCat);
        
        // Tier 3
        if (poolList.Contains(CatClass.ClassicBeggar) && poolList.Contains(CatClass.SingerCat)) possibleMutations.Add(CatClass.MusicianCat);
        if (poolList.Contains(CatClass.BeautifulCat) && poolList.Contains(CatClass.SingerCat)) possibleMutations.Add(CatClass.DancerCat);
        
        // Tier 4
        if (poolList.Contains(CatClass.ClassicBeggar) && poolList.Contains(CatClass.DancerCat)) possibleMutations.Add(CatClass.ThiefCat);
        if (poolList.Contains(CatClass.SingerCat) && poolList.Contains(CatClass.ThiefCat)) possibleMutations.Add(CatClass.InjuredCat);
        
        // Tier 5
        if (poolList.Contains(CatClass.DancerCat) && poolList.Contains(CatClass.MusicianCat)) possibleMutations.Add(CatClass.LivingStatue);
        if (poolList.Contains(CatClass.ThiefCat) && poolList.Contains(CatClass.InjuredCat)) possibleMutations.Add(CatClass.ScoutCat);
        if (poolList.Contains(CatClass.ScoutCat) && poolList.Contains(CatClass.BeautifulCat)) possibleMutations.Add(CatClass.MotherCat);

        // Ailede zaten var olan sınıfları çıkar (mutasyon sayılması için yeni olması gerekir)
        possibleMutations.RemoveAll(c => poolList.Contains(c));

        if (possibleMutations.Count > 0)
        {
            return possibleMutations[Random.Range(0, possibleMutations.Count)];
        }

        return CatClass.None;
    }
}
