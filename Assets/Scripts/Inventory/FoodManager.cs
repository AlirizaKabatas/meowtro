using UnityEngine;
using System;

public class FoodManager : MonoBehaviour
{
    public static FoodManager Instance { get; private set; }

    [Header("Global Bakiye")]
    public int yasMamaCount = 0;
    public int acBitirCount = 0;
    public int kuruMamaCount = 0;
    public int kuruEtCount = 0;

    public event Action OnFoodAmountsChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddYasMama(int amount)
    {
        yasMamaCount += amount;
        OnFoodAmountsChanged?.Invoke();
    }

    public void AddAcBitir(int amount)
    {
        acBitirCount += amount;
        OnFoodAmountsChanged?.Invoke();
    }

    public void AddKuruMama(int amount)
    {
        kuruMamaCount += amount;
        OnFoodAmountsChanged?.Invoke();
    }

    public void AddKuruEt(int amount)
    {
        kuruEtCount += amount;
        OnFoodAmountsChanged?.Invoke();
    }

    public void AddAll(int yasMama, int acBitir, int kuruMama, int kuruEt)
    {
        yasMamaCount += yasMama;
        acBitirCount += acBitir;
        kuruMamaCount += kuruMama;
        kuruEtCount += kuruEt;
        OnFoodAmountsChanged?.Invoke();
    }
}
