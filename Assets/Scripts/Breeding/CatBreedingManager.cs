using UnityEngine;
using System.Collections.Generic;

public class CatBreedingManager : MonoBehaviour
{
    public static CatBreedingManager Instance { get; private set; }

    public GameObject motherCat;
    public GameObject fatherCat;
    public GameObject newBabyCat;

    public bool isBreedingInProgress = false;
    public float breedingProgress = 0f; // 0.0 ile 1.0 arası
    public float breedingDuration = 10f;

    [Header("Default Cat Prefab")]
    [Tooltip("Yeni doğan kediler bu prefabdan türetilecektir. Assets/Prefabs/DefaultCat.prefab olmalı.")]
    public GameObject defaultCatPrefab; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (isBreedingInProgress)
        {
            breedingProgress += Time.deltaTime / breedingDuration;
            
            if (UIManager.Instance != null && UIManager.Instance.IsBreedingOpen)
            {
                UIManager.Instance.UpdateBreedingProgress(breedingProgress);
            }

            if (breedingProgress >= 1f)
            {
                CompleteBreeding();
            }
        }
    }

    public bool IsBreeding(GameObject cat)
    {
        // Kedi anne veya babaysa ve çiftleşme aktifse Breeding durumundadır
        return isBreedingInProgress && (cat == motherCat || cat == fatherCat);
    }

    public void AssignParent(GameObject cat)
    {
        CatStats stats = cat.GetComponent<CatStats>();
        if (stats == null) return;

        if (stats.gender == CatGender.Male)
        {
            if (fatherCat == null) fatherCat = cat;
            else if (fatherCat == cat) fatherCat = null; // Toggle
            else fatherCat = cat;
        }
        else if (stats.gender == CatGender.Female)
        {
            if (motherCat == null) motherCat = cat;
            else if (motherCat == cat) motherCat = null; // Toggle
            else motherCat = cat;
        }
        
        if (UIManager.Instance != null) 
        {
            UIManager.Instance.RefreshBreedingUI();
            UIManager.Instance.RefreshInventoryUI();
        }
    }

    public void RemoveParent(GameObject cat)
    {
        if (motherCat == cat) motherCat = null;
        if (fatherCat == cat) fatherCat = null;

        if (UIManager.Instance != null) 
        {
            UIManager.Instance.RefreshBreedingUI();
            UIManager.Instance.RefreshInventoryUI();
        }
    }

    public void ClearParents()
    {
        motherCat = null;
        fatherCat = null;
        newBabyCat = null;
        isBreedingInProgress = false;
        breedingProgress = 0f;
    }

    public void StartBreeding()
    {
        if (motherCat != null && fatherCat != null && !isBreedingInProgress)
        {
            isBreedingInProgress = true;
            breedingProgress = 0f;
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.RefreshInventoryUI(); // Durumları Breeding'e geçtiği için ikonları yenile
                UIManager.Instance.RefreshBreedingUI();
            }
        }
    }

    private void CompleteBreeding()
    {
        isBreedingInProgress = false;
        breedingProgress = 1f;

        if (defaultCatPrefab != null)
        {
            newBabyCat = Instantiate(defaultCatPrefab);
            newBabyCat.SetActive(false); // Henüz gizli tut

            CatBreeding breeder = newBabyCat.GetComponent<CatBreeding>();
            if (breeder == null) breeder = newBabyCat.AddComponent<CatBreeding>();

            CatStats stats = newBabyCat.GetComponent<CatStats>();
            // Rastgele Renk Belirle
            stats.catColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f); 
            stats.catName = "Unnamed Cat";

            // Objeyi de boya
            MeshRenderer mr = newBabyCat.GetComponentInChildren<MeshRenderer>();
            if (mr != null) mr.material.color = stats.catColor;

            breeder.mother = motherCat.GetComponent<CatStats>();
            breeder.father = fatherCat.GetComponent<CatStats>();
            breeder.Breed(); // Statları hesapla
        }
        else
        {
            Debug.LogError("DefaultCat prefab atanmamış! Üretim yapılamıyor.");
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshInventoryUI(); // Anne baba tekrar boşa (Idle) düştü, yenile
            if (UIManager.Instance.IsBreedingOpen)
            {
                UIManager.Instance.RefreshBreedingUI();
            }
        }
    }

    public void TakeBabyCat()
    {
        if (newBabyCat != null)
        {
            CatInventoryManager.Instance.AddCat(newBabyCat);
        }
        ClearParents();
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshBreedingUI();
        }
    }
}
