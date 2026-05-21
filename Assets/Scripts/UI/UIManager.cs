using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum InventoryMode { Normal, BreedingSelection }

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Paneller")]
    public GameObject inventoryPanel;
    public GameObject catStatsPanel;
    public GameObject breedingPanel;
    public GameObject tooltipPanel;

    [Header("Envanter UI Elemanları")]
    public Transform inventoryContentGrid;
    public GameObject inventorySlotPrefab;
    public InventoryMode currentMode = InventoryMode.Normal;

    [Header("Top Left Food UI")]
    public TextMeshProUGUI globalKuruMamaText;
    public TextMeshProUGUI globalYasMamaText;
    public TextMeshProUGUI globalAcBitirText;
    public TextMeshProUGUI globalKuruEtText;

    [Header("Stat UI Elemanları (Place/Remove)")]
    public RawImage cat3DDisplay;
    public TextMeshProUGUI catNameText;
    public TextMeshProUGUI class1Text;
    public TextMeshProUGUI class2Text;
    public TextMeshProUGUI genderText;
    public TextMeshProUGUI[] statTexts;
    public Button actionButton;
    public TextMeshProUGUI actionButtonText;
    
    [Header("Cat Food Stats UI")]
    public TextMeshProUGUI localKuruMamaText;
    public TextMeshProUGUI localYasMamaText;
    public TextMeshProUGUI localAcBitirText;
    public TextMeshProUGUI localKuruEtText;
    public Button collectFoodButton;

    [Header("Gün Sonu Paneli")]
    public GameObject endOfDayPanel;
    public TextMeshProUGUI endOfDayTotalKuruMamaText;
    public TextMeshProUGUI endOfDayTotalYasMamaText;
    public TextMeshProUGUI endOfDayTotalAcBitirText;
    public TextMeshProUGUI endOfDayTotalKuruEtText;
    public Button endOfDayCloseButton;

    [Header("Breeding Paneli")]
    public GameObject breedingIdleState;
    public GameObject breedingProgressState;
    public GameObject breedingCompletedState;
    public TextMeshProUGUI motherStatsText;
    public TextMeshProUGUI fatherStatsText;
    public Image motherIconImage;
    public Image fatherIconImage;
    public Button breedButton;
    public Slider breedingProgressBar;
    public TextMeshProUGUI newCatStatsText;
    public Button takeBabyCatButton;
    public TMP_InputField nameInputField;

    [Header("Uyarı Pop-up")]
    public GameObject warningPopup;
    public TextMeshProUGUI warningText;
    public Button warningYesButton;
    public Button warningNoButton;

    [Header("Tooltip Elemanları")]
    public TextMeshProUGUI tooltipText;

    public bool IsInventoryOpen => inventoryPanel != null && inventoryPanel.activeSelf;
    public bool IsStatsOpen => catStatsPanel != null && catStatsPanel.activeSelf;
    public bool IsBreedingOpen => breedingPanel != null && breedingPanel.activeSelf;
    public bool IsPopupOpen => warningPopup != null && warningPopup.activeSelf;
    public bool IsEndOfDayOpen => endOfDayPanel != null && endOfDayPanel.activeSelf;

    private SlotInteractable _currentSlot;
    private GameObject _selectedCatPrefab;
    private bool _isRemoveMode = false;
    private GameObject _pendingWorkingCatForBreeding;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        CloseAllPanels();

        if (warningYesButton != null) warningYesButton.onClick.AddListener(ConfirmRemoveWorkingCatForBreeding);
        if (warningNoButton != null) warningNoButton.onClick.AddListener(CancelPopup);
        if (breedButton != null) breedButton.onClick.AddListener(() => CatBreedingManager.Instance.StartBreeding());
        if (takeBabyCatButton != null) takeBabyCatButton.onClick.AddListener(TakeBabyCatClick);
        if (collectFoodButton != null) collectFoodButton.onClick.AddListener(OnCollectFoodClicked);
        if (endOfDayCloseButton != null) endOfDayCloseButton.onClick.AddListener(CloseEndOfDayPanel);
        
        // Sol üst UI kayma hatası düzeltmesi (Anchor ayarı)
        if (globalKuruMamaText != null)
        {
            SetTopLeftAnchor(globalKuruMamaText.GetComponent<RectTransform>(), 20);
            SetTopLeftAnchor(globalYasMamaText.GetComponent<RectTransform>(), 60);
            SetTopLeftAnchor(globalAcBitirText.GetComponent<RectTransform>(), 100);
            SetTopLeftAnchor(globalKuruEtText.GetComponent<RectTransform>(), 140);
        }

        if (FoodManager.Instance != null)
        {
            FoodManager.Instance.OnFoodAmountsChanged += UpdateGlobalFoodUI;
            UpdateGlobalFoodUI();
        }
    }

    private void OnDestroy()
    {
        if (FoodManager.Instance != null)
        {
            FoodManager.Instance.OnFoodAmountsChanged -= UpdateGlobalFoodUI;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPopupOpen)
            {
                CancelPopup();
            }
            else if (IsEndOfDayOpen)
            {
                CloseEndOfDayPanel();
            }
            else if (IsStatsOpen && !_isRemoveMode)
            {
                catStatsPanel.SetActive(false);
            }
            else
            {
                CloseAllPanels();
            }
        }

        if (Input.GetKeyDown(KeyCode.G) && !IsInventoryOpen && !IsBreedingOpen && !IsStatsOpen && !IsEndOfDayOpen)
        {
            OpenEndOfDayPanel();
        }

        // Global Mama Panelini diğer UI'lar açıkken gizle
        if (globalKuruMamaText != null && globalKuruMamaText.transform.parent != null)
        {
            bool anyPanelOpen = IsInventoryOpen || IsStatsOpen || IsBreedingOpen || IsEndOfDayOpen || IsPopupOpen;
            globalKuruMamaText.transform.parent.gameObject.SetActive(!anyPanelOpen);
        }

        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                tooltipPanel.transform.parent.GetComponent<RectTransform>(), 
                Input.mousePosition, 
                null, 
                out localPoint);
            
            tooltipPanel.transform.localPosition = localPoint + new Vector2(20f, -20f);
        }
    }

    // --- AÇMA KAPAMA METOTLARI ---

    public void OpenInventory(SlotInteractable slot)
    {
        _currentSlot = slot;
        currentMode = InventoryMode.Normal;
        
        inventoryPanel.SetActive(true);
        catStatsPanel.SetActive(false);
        if (breedingPanel != null) breedingPanel.SetActive(false);
        
        RefreshInventoryUI();
        LockControls(true);
    }

    public void OpenBreeding()
    {
        currentMode = InventoryMode.BreedingSelection;
        
        inventoryPanel.SetActive(true);
        if (catStatsPanel != null) catStatsPanel.SetActive(false);
        if (breedingPanel != null) breedingPanel.SetActive(true);
        
        RefreshInventoryUI();
        RefreshBreedingUI();
        LockControls(true);
    }

    public GameObject GetCurrentSelectedCat()
    {
        return _selectedCatPrefab;
    }

    private void SetTopLeftAnchor(RectTransform rect, float yOffset)
    {
        if (rect == null) return;
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(20, -yOffset);
    }

    public void OpenCatStatsForPlace(GameObject cat)
    {
        _selectedCatPrefab = cat;
        _isRemoveMode = false;
        
        catStatsPanel.SetActive(true);
        
        actionButtonText.text = "PLACE";
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(PlaceCat);
        
        // Sadece yerleştirilmiş kediler mama toplar, bu yüzden panelde gizliyoruz
        if (localKuruMamaText != null && localKuruMamaText.transform.parent != null)
        {
            localKuruMamaText.transform.parent.gameObject.SetActive(false);
        }
        if (collectFoodButton != null)
        {
            collectFoodButton.gameObject.SetActive(false);
        }

        CatStats stats = cat.GetComponent<CatStats>();
        FillCatStatsUI(stats);
        Setup3DStudio(cat);
    }

    public void OpenCatStatsForRemove(SlotInteractable slot)
    {
        _currentSlot = slot;
        _selectedCatPrefab = slot.occupiedCat;
        _isRemoveMode = true;
        
        inventoryPanel.SetActive(false);
        catStatsPanel.SetActive(true);
        
        actionButtonText.text = "REMOVE";
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(RemoveCat);
        
        // Yerleştirilmiş kedi olduğu için mama panelini gösteriyoruz
        if (localKuruMamaText != null && localKuruMamaText.transform.parent != null)
        {
            localKuruMamaText.transform.parent.gameObject.SetActive(true);
        }
        if (collectFoodButton != null)
        {
            collectFoodButton.gameObject.SetActive(true);
        }

        CatStats stats = slot.occupiedCat.GetComponent<CatStats>();
        FillCatStatsUI(stats);
        RefreshFoodUI(stats);
        Setup3DStudio(slot.occupiedCat);
        LockControls(true);
    }

    private void Setup3DStudio(GameObject catPrefab)
    {
        if (Cat3DStudio.Instance != null && cat3DDisplay != null)
        {
            Cat3DStudio.Instance.ShowCat(catPrefab);
            cat3DDisplay.texture = Cat3DStudio.Instance.renderTexture;
        }
    }

    public void CloseAllPanels()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (catStatsPanel != null) catStatsPanel.SetActive(false);
        if (breedingPanel != null) breedingPanel.SetActive(false);
        if (warningPopup != null) warningPopup.SetActive(false);
        if (endOfDayPanel != null) endOfDayPanel.SetActive(false);
        HideTooltip();
        LockControls(false);

        if (Cat3DStudio.Instance != null) Cat3DStudio.Instance.ClearCat();
        
        if (_currentSlot != null)
        {
            _currentSlot.OnDeselect();
            _currentSlot = null;
        }

        Time.timeScale = 1f;
    }

    // --- ETKİLEŞİM VE GÜNCELLEME ---

    public void HandleCatClickFromInventory(GameObject cat)
    {
        if (CatInventoryManager.Instance == null) return;
        CatState state = CatInventoryManager.Instance.GetCatState(cat);

        if (currentMode == InventoryMode.Normal)
        {
            if (state == CatState.Breeding || state == CatState.Working) return; // Uyarı verilebilir
            OpenCatStatsForPlace(cat);
        }
        else if (currentMode == InventoryMode.BreedingSelection)
        {
            if (state == CatState.Breeding) return; // Zaten ağaçta

            if (state == CatState.Working)
            {
                _pendingWorkingCatForBreeding = cat;
                if (warningText != null) warningText.text = "Bu kedi şu anda çalışıyor. Görev yerinden kaldırıp çiftleşmeye almak ister misin?";
                if (warningPopup != null) warningPopup.SetActive(true);
            }
            else // Idle
            {
                CatBreedingManager.Instance.AssignParent(cat);
            }
        }
    }

    public void ConfirmRemoveWorkingCatForBreeding()
    {
        if (warningPopup != null) warningPopup.SetActive(false);

        if (_pendingWorkingCatForBreeding != null)
        {
            SlotInteractable[] slots = FindObjectsOfType<SlotInteractable>();
            foreach (var slot in slots)
            {
                if (slot.occupiedCat == _pendingWorkingCatForBreeding)
                {
                    slot.ClearOccupation();
                    break;
                }
            }

            _pendingWorkingCatForBreeding.SetActive(false); // Idle yap
            CatBreedingManager.Instance.AssignParent(_pendingWorkingCatForBreeding);
            _pendingWorkingCatForBreeding = null;
        }
    }

    public void CancelPopup()
    {
        if (warningPopup != null) warningPopup.SetActive(false);
        _pendingWorkingCatForBreeding = null;
    }

    public void RefreshInventoryUI()
    {
        foreach (Transform child in inventoryContentGrid)
        {
            Destroy(child.gameObject);
        }

        if (CatInventoryManager.Instance != null)
        {
            int totalSlots = Mathf.Max(50, CatInventoryManager.Instance.allCats.Count + (6 - (CatInventoryManager.Instance.allCats.Count % 6))); 
            
            for (int i = 0; i < totalSlots; i++)
            {
                GameObject slotGO = Instantiate(inventorySlotPrefab, inventoryContentGrid);
                InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
                
                if (i < CatInventoryManager.Instance.allCats.Count)
                {
                    slotUI.Setup(CatInventoryManager.Instance.allCats[i]);
                }
                else
                {
                    slotUI.SetupEmpty();
                }
            }
        }
    }

    public void RefreshBreedingUI()
    {
        if (CatBreedingManager.Instance == null || breedingPanel == null) return;

        bool isProgress = CatBreedingManager.Instance.isBreedingInProgress;
        bool isDone = CatBreedingManager.Instance.newBabyCat != null;

        breedingIdleState.SetActive(!isProgress && !isDone);
        breedingProgressState.SetActive(isProgress);
        breedingCompletedState.SetActive(!isProgress && isDone);

        if (!isProgress && !isDone)
        {
            // Idle State Update
            motherStatsText.text = GetMiniStatsText(CatBreedingManager.Instance.motherCat);
            fatherStatsText.text = GetMiniStatsText(CatBreedingManager.Instance.fatherCat);
            breedButton.interactable = (CatBreedingManager.Instance.motherCat != null && CatBreedingManager.Instance.fatherCat != null);

            UpdateParentBoxUI(CatBreedingManager.Instance.motherCat, motherIconImage, new Color(0.8f, 0.3f, 0.5f, 0.6f));
            UpdateParentBoxUI(CatBreedingManager.Instance.fatherCat, fatherIconImage, new Color(0.3f, 0.5f, 0.8f, 0.6f));
        }
        else if (!isProgress && isDone)
        {
            // Completed State Update
            newCatStatsText.text = GetFullStatsText(CatBreedingManager.Instance.newBabyCat);
        }
    }

    private void UpdateParentBoxUI(GameObject cat, Image iconImg, Color defaultColor)
    {
        if (iconImg == null) return;
        if (cat != null)
        {
            CatStats stats = cat.GetComponent<CatStats>();
            if (stats != null && stats.catIcon != null)
            {
                iconImg.sprite = stats.catIcon;
                iconImg.color = Color.white;
            }
            else if (stats != null)
            {
                iconImg.sprite = null;
                iconImg.color = stats.catColor;
            }
        }
        else
        {
            iconImg.sprite = null;
            iconImg.color = defaultColor;
        }
    }

    public void UpdateBreedingProgress(float progress)
    {
        if (breedingProgressBar != null) breedingProgressBar.value = progress;
    }

    private string GetMiniStatsText(GameObject cat)
    {
        if (cat == null) return "BOŞ\n(Envanterden Seçin)";
        CatStats stats = cat.GetComponent<CatStats>();
        if (stats == null) return "Veri Yok";

        return $"<b>{stats.catName}</b>\n{stats.primaryClass}\nEff: {stats.efficiency} Cut: {stats.cuteness}";
    }

    private string GetFullStatsText(GameObject cat)
    {
        if (cat == null) return "";
        CatStats s = cat.GetComponent<CatStats>();
        if (s == null) return "";

        string sec = s.secondaryClass == CatClass.None ? "" : $"Class 2: {s.secondaryClass}";
        return $"Class 1: {s.primaryClass}     Gender: {s.gender}\n{sec}\n\n" +
               $"Efficiency: {s.efficiency}   Cuteness: {s.cuteness}\n" +
               $"Charisma: {s.charisma}     Meowing: {s.meowing}\n" +
               $"Rhythm: {s.rhythm}        Acting: {s.acting}\n" +
               $"Sleight Of Paw: {s.sleightOfPaw}  Nurturing: {s.nurturing}";
    }

    public void TakeBabyCatClick()
    {
        if (CatBreedingManager.Instance != null && CatBreedingManager.Instance.newBabyCat != null)
        {
            CatStats stats = CatBreedingManager.Instance.newBabyCat.GetComponent<CatStats>();
            if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
            {
                stats.catName = nameInputField.text;
            }
            CatBreedingManager.Instance.TakeBabyCat();
            if (nameInputField != null) nameInputField.text = ""; // Temizle
        }
    }

    private void FillCatStatsUI(CatStats stats)
    {
        if (stats == null) return;

        if (catNameText != null) catNameText.text = stats.catName;
        class1Text.text = "Class 1: " + stats.primaryClass.ToString();
        class2Text.text = stats.secondaryClass == CatClass.None ? "" : "Class 2: " + stats.secondaryClass.ToString();
        genderText.text = "Gender: " + stats.gender.ToString();

        if (statTexts.Length >= 8)
        {
            statTexts[0].text = "Efficiency: " + stats.efficiency;
            statTexts[1].text = "Cuteness: " + stats.cuteness;
            statTexts[2].text = "Charisma: " + stats.charisma;
            statTexts[3].text = "Meowing: " + stats.meowing;
            statTexts[4].text = "Rhythm: " + stats.rhythm;
            statTexts[5].text = "Acting: " + stats.acting;
            statTexts[6].text = "Sleight Of Paw: " + stats.sleightOfPaw;
            statTexts[7].text = "Nurturing: " + stats.nurturing;
        }
    }

    private void PlaceCat()
    {
        if (_currentSlot != null && _selectedCatPrefab != null)
        {
            if (!_selectedCatPrefab.scene.IsValid())
            {
                GameObject inst = Instantiate(_selectedCatPrefab);
                
                int index = CatInventoryManager.Instance.allCats.IndexOf(_selectedCatPrefab);
                if (index >= 0) CatInventoryManager.Instance.allCats[index] = inst;
                else CatInventoryManager.Instance.AddCat(inst);
                
                _selectedCatPrefab = inst;
            }

            _selectedCatPrefab.transform.SetParent(_currentSlot.transform);
            
            // Kedinin slotun altında ezilmemesi ve biraz daha büyük durması için scale ve pos ayarları
            _selectedCatPrefab.transform.localPosition = new Vector3(0, 0.4f, 0);
            _selectedCatPrefab.transform.localScale = Vector3.one * 1.3f;
            _selectedCatPrefab.transform.localRotation = Quaternion.identity;
            
            _selectedCatPrefab.SetActive(true);
            _currentSlot.MarkAsOccupied(_selectedCatPrefab);
            CatInventoryManager.Instance.RefreshUI();
        }
        CloseAllPanels();
    }

    private void RemoveCat()
    {
        if (_currentSlot != null && _currentSlot.isOccupied)
        {
            GameObject currentCatObj = _currentSlot.occupiedCat;
            currentCatObj.SetActive(false); 
            _currentSlot.ClearOccupation(); 
            CatInventoryManager.Instance.RefreshUI();
        }
        CloseAllPanels();
    }

    // --- FOOD SYSTEM METHODS ---

    public void RefreshFoodUI(CatStats stats)
    {
        if (stats == null) return;
        
        if (localKuruMamaText != null) localKuruMamaText.text = "Kuru Mama: " + stats.accumulatedKuruMama;
        if (localYasMamaText != null) localYasMamaText.text = "Yaş Mama: " + stats.accumulatedYasMama;
        if (localAcBitirText != null) localAcBitirText.text = "Aç Bitir: " + stats.accumulatedAcBitir;
        if (localKuruEtText != null) localKuruEtText.text = "Kuru Et: " + stats.accumulatedKuruEt;
        
        if (collectFoodButton != null)
        {
            bool hasFood = (stats.accumulatedKuruMama > 0 || stats.accumulatedYasMama > 0 || stats.accumulatedAcBitir > 0 || stats.accumulatedKuruEt > 0);
            collectFoodButton.interactable = hasFood;
        }
    }

    private void OnCollectFoodClicked()
    {
        if (_selectedCatPrefab != null)
        {
            CatStats stats = _selectedCatPrefab.GetComponent<CatStats>();
            if (stats != null)
            {
                stats.CollectFood();
            }
        }
    }

    private void UpdateGlobalFoodUI()
    {
        if (FoodManager.Instance == null) return;
        if (globalKuruMamaText != null) globalKuruMamaText.text = "Kuru Mama: " + FoodManager.Instance.kuruMamaCount.ToString();
        if (globalYasMamaText != null) globalYasMamaText.text = "Yaş Mama: " + FoodManager.Instance.yasMamaCount.ToString();
        if (globalAcBitirText != null) globalAcBitirText.text = "Aç Bitir: " + FoodManager.Instance.acBitirCount.ToString();
        if (globalKuruEtText != null) globalKuruEtText.text = "Kuru Et: " + FoodManager.Instance.kuruEtCount.ToString();
    }

    public void OpenEndOfDayPanel()
    {
        if (endOfDayPanel == null) return;

        int totalKuruMama = 0;
        int totalYasMama = 0;
        int totalAcBitir = 0;
        int totalKuruEt = 0;

        SlotInteractable[] slots = FindObjectsOfType<SlotInteractable>();
        foreach (var slot in slots)
        {
            if (slot.isOccupied && slot.occupiedCat != null)
            {
                CatStats stats = slot.occupiedCat.GetComponent<CatStats>();
                if (stats != null)
                {
                    totalKuruMama += stats.accumulatedKuruMama;
                    totalYasMama += stats.accumulatedYasMama;
                    totalAcBitir += stats.accumulatedAcBitir;
                    totalKuruEt += stats.accumulatedKuruEt;
                }
            }
        }

        if (endOfDayTotalKuruMamaText != null) endOfDayTotalKuruMamaText.text = "Toplanan Kuru Mama: " + totalKuruMama;
        if (endOfDayTotalYasMamaText != null) endOfDayTotalYasMamaText.text = "Toplanan Yaş Mama: " + totalYasMama;
        if (endOfDayTotalAcBitirText != null) endOfDayTotalAcBitirText.text = "Toplanan Aç Bitir: " + totalAcBitir;
        if (endOfDayTotalKuruEtText != null) endOfDayTotalKuruEtText.text = "Toplanan Kuru Et: " + totalKuruEt;

        endOfDayPanel.SetActive(true);
        LockControls(true);
        Time.timeScale = 0f; // Zamanı durdur
    }

    public void CloseEndOfDayPanel()
    {
        SlotInteractable[] slots = FindObjectsOfType<SlotInteractable>();
        foreach (var slot in slots)
        {
            if (slot.isOccupied && slot.occupiedCat != null)
            {
                CatStats stats = slot.occupiedCat.GetComponent<CatStats>();
                if (stats != null)
                {
                    stats.CollectFood();
                }
            }
        }
        
        // Sahnedeki tüm yolcuları bul ve yok et (Gün Resetlenmesi)
        PassengerAI[] activePassengers = FindObjectsOfType<PassengerAI>();
        foreach (var p in activePassengers)
        {
            if (p != null) Destroy(p.gameObject);
        }
        
        if (endOfDayPanel != null) endOfDayPanel.SetActive(false);
        LockControls(false);
        Time.timeScale = 1f; // Zamanı devam ettir
    }

    public void ShowTooltip(string text)
    {
        if (tooltipPanel != null)
        {
            tooltipText.text = text;
            tooltipPanel.SetActive(true);
        }
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    private void LockControls(bool lockState)
    {
        if (CameraController.Instance != null) CameraController.Instance.isLockedByUI = lockState;
        if (InteractionManager.Instance != null) InteractionManager.Instance.isLockedByUI = lockState;
    }
}
