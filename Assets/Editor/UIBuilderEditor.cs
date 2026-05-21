using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class UIBuilderEditor : EditorWindow
{
    [MenuItem("MEOWTRO/Envanter UI Sistemini Kur")]
    public static void BuildUI()
    {
        GameObject canvasObj = new GameObject("CatUIManager_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        UIManager uiManager = canvasObj.AddComponent<UIManager>();

        // 2. Inventory Panel
        GameObject invPanel = new GameObject("InventoryPanel");
        invPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform invRect = invPanel.AddComponent<RectTransform>();
        invRect.anchorMin = new Vector2(0f, 0f); 
        invRect.anchorMax = new Vector2(0.6f, 1f); 
        invRect.offsetMin = Vector2.zero; invRect.offsetMax = Vector2.zero;
        Image invImg = invPanel.AddComponent<Image>();
        invImg.color = new Color(0, 0, 0, 0.85f);

        GameObject escTextObj = new GameObject("EscText");
        escTextObj.transform.SetParent(invPanel.transform, false);
        RectTransform escRect = escTextObj.AddComponent<RectTransform>();
        escRect.anchorMin = new Vector2(0.5f, 0); escRect.anchorMax = new Vector2(0.5f, 0);
        escRect.anchoredPosition = new Vector2(0, 50);
        escRect.sizeDelta = new Vector2(400, 50);
        TextMeshProUGUI escTmp = escTextObj.AddComponent<TextMeshProUGUI>();
        escTmp.text = "Kapatmak İçin ESC'ye Basın";
        escTmp.alignment = TextAlignmentOptions.Center;
        escTmp.color = Color.white;
        escTmp.fontSize = 24;

        GameObject scrollViewObj = new GameObject("Scroll View");
        scrollViewObj.transform.SetParent(invPanel.transform, false);
        RectTransform svRect = scrollViewObj.AddComponent<RectTransform>();
        svRect.anchorMin = new Vector2(0.02f, 0.1f);
        svRect.anchorMax = new Vector2(0.98f, 0.95f);
        svRect.offsetMin = Vector2.zero; svRect.offsetMax = Vector2.zero;
        ScrollRect scrollRect = scrollViewObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 30f;

        GameObject viewportObj = new GameObject("Viewport");
        viewportObj.transform.SetParent(scrollViewObj.transform, false);
        RectTransform vpRect = viewportObj.AddComponent<RectTransform>();
        vpRect.anchorMin = Vector2.zero; vpRect.anchorMax = Vector2.one;
        vpRect.offsetMin = Vector2.zero; vpRect.offsetMax = Vector2.zero;
        Image vpImg = viewportObj.AddComponent<Image>();
        vpImg.color = new Color(1, 1, 1, 0.01f);
        Mask mask = viewportObj.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        GameObject gridObj = new GameObject("SlotGrid");
        gridObj.transform.SetParent(viewportObj.transform, false);
        RectTransform gridRect = gridObj.AddComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0, 1);
        gridRect.anchorMax = new Vector2(1, 1);
        gridRect.pivot = new Vector2(0.5f, 1f);
        gridRect.anchoredPosition = Vector2.zero;
        
        GridLayoutGroup gridGroup = gridObj.AddComponent<GridLayoutGroup>();
        gridGroup.cellSize = new Vector2(150, 150);
        gridGroup.spacing = new Vector2(15, 15);
        gridGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridGroup.constraintCount = 6;
        gridGroup.childAlignment = TextAnchor.UpperCenter;
        gridGroup.padding = new RectOffset(10, 10, 10, 10);
        
        ContentSizeFitter fitter = gridObj.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; 

        scrollRect.content = gridRect;
        scrollRect.viewport = vpRect;

        // --- SLOT PREFAB ---
        GameObject slotPrefab = new GameObject("InventorySlotPrefab");
        RectTransform slotRect = slotPrefab.AddComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(150, 150);
        Image slotBg = slotPrefab.AddComponent<Image>();
        slotBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        InventorySlotUI slotUI = slotPrefab.AddComponent<InventorySlotUI>();
        
        GameObject slotIcon = new GameObject("Icon");
        slotIcon.transform.SetParent(slotPrefab.transform, false);
        RectTransform iconRect = slotIcon.AddComponent<RectTransform>();
        iconRect.anchorMin = Vector2.zero; iconRect.anchorMax = Vector2.one;
        iconRect.offsetMin = new Vector2(10, 10); iconRect.offsetMax = new Vector2(-10, -10);
        Image iconImg = slotIcon.AddComponent<Image>();
        slotUI.iconImage = iconImg;

        GameObject breedIco = new GameObject("BreedingIcon");
        breedIco.transform.SetParent(slotPrefab.transform, false);
        RectTransform biRect = breedIco.AddComponent<RectTransform>();
        biRect.anchorMin = new Vector2(1,1); biRect.anchorMax = new Vector2(1,1); biRect.pivot = new Vector2(1,1);
        biRect.anchoredPosition = new Vector2(-5, -5);
        biRect.sizeDelta = new Vector2(40, 40);
        Image biImg = breedIco.AddComponent<Image>();
        biImg.color = Color.red; // Kırmızı ikon (Kalp vs yapılabilir)
        slotUI.breedingIcon = breedIco;

        GameObject workIco = new GameObject("WorkingIcon");
        workIco.transform.SetParent(slotPrefab.transform, false);
        RectTransform wiRect = workIco.AddComponent<RectTransform>();
        wiRect.anchorMin = new Vector2(1,1); wiRect.anchorMax = new Vector2(1,1); wiRect.pivot = new Vector2(1,1);
        wiRect.anchoredPosition = new Vector2(-5, -5);
        wiRect.sizeDelta = new Vector2(40, 40);
        Image wiImg = workIco.AddComponent<Image>();
        wiImg.color = Color.yellow; // Sarı ikon (Çekiç vs yapılabilir)
        slotUI.workingIcon = workIco;

        // --- 3. Cat Stats Panel (Sağ taraf - Place/Remove modu) ---
        GameObject statsPanel = new GameObject("CatStatsPanel");
        statsPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform statsRect = statsPanel.AddComponent<RectTransform>();
        statsRect.anchorMin = new Vector2(0.62f, 0.1f);
        statsRect.anchorMax = new Vector2(0.98f, 0.9f); 
        statsRect.offsetMin = Vector2.zero; statsRect.offsetMax = Vector2.zero;
        Image statsImg = statsPanel.AddComponent<Image>();
        statsImg.color = new Color(0.18f, 0.18f, 0.18f, 1f); // Koyu Gri

        TextMeshProUGUI AddText(string name, Transform parent, Vector2 pos, Vector2 size, int fSize, TextAlignmentOptions align = TextAlignmentOptions.Left)
        {
            GameObject t = new GameObject(name);
            t.transform.SetParent(parent, false);
            RectTransform r = t.AddComponent<RectTransform>();
            r.anchoredPosition = pos;
            r.sizeDelta = size;
            TextMeshProUGUI tmp = t.AddComponent<TextMeshProUGUI>();
            tmp.color = Color.black;
            tmp.fontSize = fSize;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = align;
            return tmp;
        }

        // CatStatsPanel Layout: RawImage, Texts, Line, Grid, Button
        GameObject rawImgObj = new GameObject("Cat3DDisplay");
        rawImgObj.transform.SetParent(statsPanel.transform, false);
        RectTransform rawRect = rawImgObj.AddComponent<RectTransform>();
        rawRect.anchorMin = new Vector2(0, 1); rawRect.anchorMax = new Vector2(0, 1);
        rawRect.pivot = new Vector2(0, 1);
        rawRect.anchoredPosition = new Vector2(20, -20);
        rawRect.sizeDelta = new Vector2(250, 250);
        RawImage rawImg = rawImgObj.AddComponent<RawImage>();
        rawImg.color = Color.white;

        TextMeshProUGUI nameTxt = AddText("NameText", statsPanel.transform, new Vector2(290, -40), new Vector2(300, 50), 32, TextAlignmentOptions.Left);
        nameTxt.color = Color.white; nameTxt.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1); nameTxt.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1); nameTxt.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

        TextMeshProUGUI c1 = AddText("Class1Text", statsPanel.transform, new Vector2(290, -100), new Vector2(300, 35), 24, TextAlignmentOptions.Left);
        c1.color = new Color(0.8f, 0.8f, 0.8f); c1.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1); c1.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1); c1.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        
        TextMeshProUGUI c2 = AddText("Class2Text", statsPanel.transform, new Vector2(290, -140), new Vector2(300, 35), 24, TextAlignmentOptions.Left);
        c2.color = new Color(0.8f, 0.8f, 0.8f); c2.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1); c2.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1); c2.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

        TextMeshProUGUI gen = AddText("GenderText", statsPanel.transform, new Vector2(290, -180), new Vector2(300, 35), 24, TextAlignmentOptions.Left);
        gen.color = new Color(0.8f, 0.8f, 0.8f); gen.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1); gen.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1); gen.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

        GameObject line = new GameObject("Line");
        line.transform.SetParent(statsPanel.transform, false);
        RectTransform lineRect = line.AddComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0, 1); lineRect.anchorMax = new Vector2(1, 1);
        lineRect.anchoredPosition = new Vector2(0, -290);
        lineRect.sizeDelta = new Vector2(0, 5);
        Image lineImg = line.AddComponent<Image>();
        lineImg.color = new Color(0.1f, 0.1f, 0.1f, 1f);

        GameObject statGrid = new GameObject("StatsGrid");
        statGrid.transform.SetParent(statsPanel.transform, false);
        RectTransform sgRect = statGrid.AddComponent<RectTransform>();
        sgRect.anchorMin = new Vector2(0.5f, 0.5f); sgRect.anchorMax = new Vector2(0.5f, 0.5f);
        sgRect.anchoredPosition = new Vector2(0, -50);
        sgRect.sizeDelta = new Vector2(600, 300);
        GridLayoutGroup sgGroup = statGrid.AddComponent<GridLayoutGroup>();
        sgGroup.cellSize = new Vector2(280, 40);
        sgGroup.spacing = new Vector2(20, 20);
        sgGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        sgGroup.constraintCount = 2;

        TextMeshProUGUI[] statTexts = new TextMeshProUGUI[8];
        for (int i = 0; i < 8; i++)
        {
            statTexts[i] = AddText("Stat" + i, statGrid.transform, Vector2.zero, new Vector2(280, 40), 24);
            statTexts[i].color = Color.white;
        }

        GameObject btnObj = new GameObject("ActionButton");
        btnObj.transform.SetParent(statsPanel.transform, false);
        RectTransform btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0); btnRect.anchorMax = new Vector2(0.5f, 0); btnRect.pivot = new Vector2(0.5f, 0);
        btnRect.anchoredPosition = new Vector2(0, 40);
        btnRect.sizeDelta = new Vector2(300, 80);
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.1f, 0.6f, 0.8f, 1f); // Mavi ton
        Button btn = btnObj.AddComponent<Button>();
        TextMeshProUGUI btnText = AddText("Text", btnObj.transform, Vector2.zero, new Vector2(300, 80), 36, TextAlignmentOptions.Center);
        btnText.color = Color.white;
        btnText.GetComponent<RectTransform>().anchorMin = Vector2.zero; btnText.GetComponent<RectTransform>().anchorMax = Vector2.one;
        btnText.GetComponent<RectTransform>().offsetMin = Vector2.zero; btnText.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // --- 4. Breeding Panel (Ağaç Çiftleşme) ---
        GameObject breedingPanel = new GameObject("BreedingPanel");
        breedingPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform bpRect = breedingPanel.AddComponent<RectTransform>();
        bpRect.anchorMin = new Vector2(0.62f, 0.1f); bpRect.anchorMax = new Vector2(0.98f, 0.9f);
        bpRect.offsetMin = Vector2.zero; bpRect.offsetMax = Vector2.zero;
        Image bpImg = breedingPanel.AddComponent<Image>();
        bpImg.color = new Color(0.15f, 0.15f, 0.15f, 1f); // Koyu Gri

        // Idle State
        GameObject idleState = new GameObject("IdleState");
        idleState.transform.SetParent(breedingPanel.transform, false);
        RectTransform idleRect = idleState.AddComponent<RectTransform>();
        idleRect.anchorMin = Vector2.zero; idleRect.anchorMax = Vector2.one; idleRect.offsetMin = Vector2.zero; idleRect.offsetMax = Vector2.zero;

        GameObject mBox = new GameObject("MotherBox");
        mBox.transform.SetParent(idleState.transform, false);
        RectTransform mbRect = mBox.AddComponent<RectTransform>();
        mbRect.anchorMin = new Vector2(0.5f, 0.5f); mbRect.anchorMax = new Vector2(0.5f, 0.5f);
        mbRect.anchoredPosition = new Vector2(-150, 100); mbRect.sizeDelta = new Vector2(200, 200);
        Image mbImg = mBox.AddComponent<Image>(); mbImg.color = new Color(0.8f,0.3f,0.5f, 0.6f); // Pembe
        TextMeshProUGUI mTitle = AddText("MTitle", idleState.transform, new Vector2(-150, 230), new Vector2(250, 40), 26, TextAlignmentOptions.Center);
        mTitle.color = Color.white; mTitle.text = "♀ FEMALE SLOT"; mTitle.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f); mTitle.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        TextMeshProUGUI mStat = AddText("MotherStats", idleState.transform, new Vector2(-150, -50), new Vector2(250, 80), 22, TextAlignmentOptions.Center);
        mStat.color = Color.white; mStat.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f); mStat.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        
        GameObject plus = new GameObject("Plus");
        plus.transform.SetParent(idleState.transform, false);
        RectTransform pRect = plus.AddComponent<RectTransform>();
        pRect.anchorMin = new Vector2(0.5f, 0.5f); pRect.anchorMax = new Vector2(0.5f, 0.5f);
        pRect.anchoredPosition = new Vector2(0, 100); pRect.sizeDelta = new Vector2(50, 50);
        TextMeshProUGUI pTxt = plus.AddComponent<TextMeshProUGUI>(); pTxt.text = "+"; pTxt.fontSize = 60; pTxt.color = Color.white; pTxt.alignment = TextAlignmentOptions.Center;

        GameObject fBox = new GameObject("FatherBox");
        fBox.transform.SetParent(idleState.transform, false);
        RectTransform fbRect = fBox.AddComponent<RectTransform>();
        fbRect.anchorMin = new Vector2(0.5f, 0.5f); fbRect.anchorMax = new Vector2(0.5f, 0.5f);
        fbRect.anchoredPosition = new Vector2(150, 100); fbRect.sizeDelta = new Vector2(200, 200);
        Image fbImg = fBox.AddComponent<Image>(); fbImg.color = new Color(0.3f,0.5f,0.8f, 0.6f); // Mavi
        TextMeshProUGUI fTitle = AddText("FTitle", idleState.transform, new Vector2(150, 230), new Vector2(250, 40), 26, TextAlignmentOptions.Center);
        fTitle.color = Color.white; fTitle.text = "♂ MALE SLOT"; fTitle.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f); fTitle.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        TextMeshProUGUI fStat = AddText("FatherStats", idleState.transform, new Vector2(150, -50), new Vector2(250, 80), 22, TextAlignmentOptions.Center);
        fStat.color = Color.white; fStat.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f); fStat.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

        GameObject bBtnObj = new GameObject("BreedButton");
        bBtnObj.transform.SetParent(idleState.transform, false);
        RectTransform bbRect = bBtnObj.AddComponent<RectTransform>();
        bbRect.anchorMin = new Vector2(0.5f, 0); bbRect.anchorMax = new Vector2(0.5f, 0);
        bbRect.anchoredPosition = new Vector2(0, 100); bbRect.sizeDelta = new Vector2(300, 80);
        Image bbImg = bBtnObj.AddComponent<Image>(); bbImg.color = new Color(0.1f, 0.6f, 0.1f, 1f);
        Button bBtn = bBtnObj.AddComponent<Button>();
        TextMeshProUGUI bbTxt = AddText("Text", bBtnObj.transform, Vector2.zero, new Vector2(300, 80), 36, TextAlignmentOptions.Center);
        bbTxt.text = "BREED"; bbTxt.color = Color.white; bbTxt.GetComponent<RectTransform>().anchorMin = Vector2.zero; bbTxt.GetComponent<RectTransform>().anchorMax = Vector2.one; bbTxt.GetComponent<RectTransform>().offsetMin = Vector2.zero; bbTxt.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // Progress State
        GameObject progState = new GameObject("ProgressState");
        progState.transform.SetParent(breedingPanel.transform, false);
        RectTransform prRect = progState.AddComponent<RectTransform>();
        prRect.anchorMin = Vector2.zero; prRect.anchorMax = Vector2.one; prRect.offsetMin = Vector2.zero; prRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI brTxt = AddText("BrText", progState.transform, new Vector2(0, 100), new Vector2(400, 100), 40, TextAlignmentOptions.Center);
        brTxt.color = Color.white; brTxt.text = "Breeding..."; brTxt.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f); brTxt.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

        GameObject sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(progState.transform, false);
        RectTransform slRect = sliderObj.AddComponent<RectTransform>();
        slRect.anchorMin = new Vector2(0.5f, 0.5f); slRect.anchorMax = new Vector2(0.5f, 0.5f);
        slRect.anchoredPosition = Vector2.zero; slRect.sizeDelta = new Vector2(500, 40);
        Image slBg = sliderObj.AddComponent<Image>(); slBg.color = new Color(0,0,0,0.5f);
        
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform faRect = fillArea.AddComponent<RectTransform>();
        faRect.anchorMin = Vector2.zero; faRect.anchorMax = Vector2.one; faRect.offsetMin = Vector2.zero; faRect.offsetMax = Vector2.zero;
        
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fRect = fill.AddComponent<RectTransform>();
        fRect.anchorMin = Vector2.zero; fRect.anchorMax = Vector2.one; fRect.offsetMin = Vector2.zero; fRect.offsetMax = Vector2.zero;
        Image fImg = fill.AddComponent<Image>(); fImg.color = Color.green;
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.fillRect = fRect;

        // Completed State
        GameObject compState = new GameObject("CompletedState");
        compState.transform.SetParent(breedingPanel.transform, false);
        RectTransform csRect = compState.AddComponent<RectTransform>();
        csRect.anchorMin = Vector2.zero; csRect.anchorMax = Vector2.one; csRect.offsetMin = Vector2.zero; csRect.offsetMax = Vector2.zero;

        TextMeshProUGUI compTxt = AddText("NewCatStats", compState.transform, new Vector2(0, 50), new Vector2(600, 500), 26, TextAlignmentOptions.Center);
        compTxt.color = Color.white; compTxt.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f); compTxt.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        
        GameObject inputObj = new GameObject("NameInputField");
        inputObj.transform.SetParent(compState.transform, false);
        RectTransform inRect = inputObj.AddComponent<RectTransform>();
        inRect.anchorMin = new Vector2(0.5f, 0); inRect.anchorMax = new Vector2(0.5f, 0);
        inRect.anchoredPosition = new Vector2(0, 220); inRect.sizeDelta = new Vector2(400, 60);
        Image inImg = inputObj.AddComponent<Image>(); inImg.color = Color.white;
        TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputObj.transform, false);
        RectTransform tRect = textObj.AddComponent<RectTransform>();
        tRect.anchorMin = Vector2.zero; tRect.anchorMax = Vector2.one;
        tRect.offsetMin = new Vector2(10, 0); tRect.offsetMax = new Vector2(-10, 0);
        TextMeshProUGUI inText = textObj.AddComponent<TextMeshProUGUI>();
        inText.color = Color.black; inText.fontSize = 28; inText.alignment = TextAlignmentOptions.Left;
        inputField.textComponent = inText;

        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(inputObj.transform, false);
        RectTransform phRect = placeholderObj.AddComponent<RectTransform>();
        phRect.anchorMin = Vector2.zero; phRect.anchorMax = Vector2.one;
        phRect.offsetMin = new Vector2(10, 0); phRect.offsetMax = new Vector2(-10, 0);
        TextMeshProUGUI pText = placeholderObj.AddComponent<TextMeshProUGUI>();
        pText.color = new Color(0, 0, 0, 0.5f); pText.fontSize = 28; pText.alignment = TextAlignmentOptions.Left;
        pText.text = "Kediye isim ver...";
        inputField.placeholder = pText;

        GameObject tBtnObj = new GameObject("TakeButton");
        tBtnObj.transform.SetParent(compState.transform, false);
        RectTransform tbRect = tBtnObj.AddComponent<RectTransform>();
        tbRect.anchorMin = new Vector2(0.5f, 0); tbRect.anchorMax = new Vector2(0.5f, 0);
        tbRect.anchoredPosition = new Vector2(0, 100); tbRect.sizeDelta = new Vector2(300, 80);
        Image tbImg = tBtnObj.AddComponent<Image>(); tbImg.color = new Color(0.1f, 0.6f, 0.8f, 1f);
        Button tBtn = tBtnObj.AddComponent<Button>();
        TextMeshProUGUI tbTxt = AddText("Text", tBtnObj.transform, Vector2.zero, new Vector2(300, 80), 36, TextAlignmentOptions.Center);
        tbTxt.text = "NAME IT & TAKE"; tbTxt.color = Color.white; tbTxt.GetComponent<RectTransform>().anchorMin = Vector2.zero; tbTxt.GetComponent<RectTransform>().anchorMax = Vector2.one; tbTxt.GetComponent<RectTransform>().offsetMin = Vector2.zero; tbTxt.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        // --- 5. Warning Popup (Ortada) ---
        GameObject wpObj = new GameObject("WarningPopup");
        wpObj.transform.SetParent(canvasObj.transform, false);
        RectTransform wpRect = wpObj.AddComponent<RectTransform>();
        wpRect.anchorMin = Vector2.zero; wpRect.anchorMax = Vector2.one; wpRect.offsetMin = Vector2.zero; wpRect.offsetMax = Vector2.zero;
        Image wpBg = wpObj.AddComponent<Image>(); wpBg.color = new Color(0,0,0, 0.8f);

        GameObject wpBox = new GameObject("Box");
        wpBox.transform.SetParent(wpObj.transform, false);
        RectTransform wpbRect = wpBox.AddComponent<RectTransform>();
        wpbRect.anchorMin = new Vector2(0.5f, 0.5f); wpbRect.anchorMax = new Vector2(0.5f, 0.5f);
        wpbRect.anchoredPosition = Vector2.zero; wpbRect.sizeDelta = new Vector2(600, 300);
        Image wpbImg = wpBox.AddComponent<Image>(); wpbImg.color = new Color(0.2f,0.2f,0.2f,1f);

        TextMeshProUGUI wTxt = AddText("WarnText", wpBox.transform, new Vector2(0, 50), new Vector2(500, 150), 24, TextAlignmentOptions.Center);
        wTxt.color = Color.white; wTxt.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f); wTxt.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);

        Button CreateWPBtn(string n, string t, Vector2 pos, Color c)
        {
            GameObject o = new GameObject(n);
            o.transform.SetParent(wpBox.transform, false);
            RectTransform r = o.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.5f, 0.5f); r.anchorMax = new Vector2(0.5f, 0.5f);
            r.anchoredPosition = pos; r.sizeDelta = new Vector2(200, 60);
            Image img = o.AddComponent<Image>(); img.color = c;
            Button btn = o.AddComponent<Button>();
            TextMeshProUGUI txt = AddText("T", o.transform, Vector2.zero, new Vector2(200, 60), 24, TextAlignmentOptions.Center);
            txt.color = Color.white; txt.text = t; txt.GetComponent<RectTransform>().anchorMin = Vector2.zero; txt.GetComponent<RectTransform>().anchorMax = Vector2.one; txt.GetComponent<RectTransform>().offsetMin = Vector2.zero; txt.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            return btn;
        }

        Button yBtn = CreateWPBtn("YesBtn", "EVET", new Vector2(-120, -80), new Color(0.1f, 0.6f, 0.1f));
        Button nBtn = CreateWPBtn("NoBtn", "HAYIR", new Vector2(120, -80), new Color(0.6f, 0.1f, 0.1f));

        // --- 6. Tooltip Panel ---
        GameObject tooltipObj = new GameObject("TooltipPanel");
        tooltipObj.transform.SetParent(canvasObj.transform, false);
        RectTransform ttRect = tooltipObj.AddComponent<RectTransform>();
        ttRect.sizeDelta = new Vector2(300, 120);
        ttRect.pivot = new Vector2(0, 1);
        Image ttImg = tooltipObj.AddComponent<Image>();
        ttImg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        TextMeshProUGUI ttText = AddText("Text", tooltipObj.transform, Vector2.zero, new Vector2(280, 100), 22, TextAlignmentOptions.Center);
        ttText.color = Color.white;
        ttText.GetComponent<RectTransform>().anchorMin = Vector2.zero; ttText.GetComponent<RectTransform>().anchorMax = Vector2.one;
        ttText.GetComponent<RectTransform>().offsetMin = new Vector2(10,10); ttText.GetComponent<RectTransform>().offsetMax = new Vector2(-10,-10);

        // --- 7. UIManager Bağlantıları ---
        uiManager.inventoryPanel = invPanel;
        uiManager.catStatsPanel = statsPanel;
        uiManager.breedingPanel = breedingPanel;
        uiManager.tooltipPanel = tooltipObj;
        uiManager.inventoryContentGrid = gridObj.transform;
        
        uiManager.cat3DDisplay = rawImg;
        uiManager.catNameText = nameTxt;
        uiManager.class1Text = c1;
        uiManager.class2Text = c2;
        uiManager.genderText = gen;
        uiManager.statTexts = statTexts;
        uiManager.actionButton = btn;
        uiManager.actionButtonText = btnText;
        
        uiManager.breedingIdleState = idleState;
        uiManager.breedingProgressState = progState;
        uiManager.breedingCompletedState = compState;
        uiManager.motherStatsText = mStat;
        uiManager.fatherStatsText = fStat;
        uiManager.motherIconImage = mbImg;
        uiManager.fatherIconImage = fbImg;
        uiManager.breedButton = bBtn;
        uiManager.breedingProgressBar = slider;
        uiManager.newCatStatsText = compTxt;
        uiManager.takeBabyCatButton = tBtn;
        uiManager.nameInputField = inputField;
        
        uiManager.warningPopup = wpObj;
        uiManager.warningText = wTxt;
        uiManager.warningYesButton = yBtn;
        uiManager.warningNoButton = nBtn;
        
        uiManager.tooltipText = ttText;

        // Prefab Kayıt
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI")) AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(slotPrefab, "Assets/Prefabs/UI/InventorySlotPrefab.prefab");
        uiManager.inventorySlotPrefab = savedPrefab;
        DestroyImmediate(slotPrefab);

        // Panelleri Gizle
        invPanel.SetActive(false);
        statsPanel.SetActive(false);
        breedingPanel.SetActive(false);
        wpObj.SetActive(false);
        tooltipObj.SetActive(false);

        // Yöneticileri Kur
        if (FindObjectOfType<CatInventoryManager>() == null)
        {
            GameObject imObj = new GameObject("CatInventoryManager");
            imObj.AddComponent<CatInventoryManager>();
        }
        if (FindObjectOfType<InteractionManager>() == null)
        {
            GameObject intObj = new GameObject("InteractionManager");
            intObj.AddComponent<InteractionManager>();
        }
        if (FindObjectOfType<CatBreedingManager>() == null)
        {
            GameObject cbObj = new GameObject("CatBreedingManager");
            cbObj.AddComponent<CatBreedingManager>();
        }
        if (FindObjectOfType<Cat3DStudio>() == null)
        {
            GameObject stuObj = new GameObject("Cat3DStudio");
            stuObj.AddComponent<Cat3DStudio>();
        }
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        Debug.Log("Kapsamlı Çiftleşme ve Envanter UI Sistemi Başarıyla Kuruldu!");
    }
}
