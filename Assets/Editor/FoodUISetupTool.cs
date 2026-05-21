using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class FoodUISetupTool : EditorWindow
{
    [MenuItem("MEOWTRO/Mama UI Sistemini Kur")]
    public static void BuildFoodUI()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("Sahnede UIManager bulunamadı! Önce ana UI sistemini kurduğunuzdan emin olun.");
            return;
        }

        Canvas canvas = uiManager.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("UIManager bir Canvas altında değil!");
            return;
        }

        // 1. FoodManager Kurulumu
        if (FindObjectOfType<FoodManager>() == null)
        {
            GameObject foodMgr = new GameObject("FoodManager");
            foodMgr.AddComponent<FoodManager>();
        }

        // Yardımcı Metot
        TextMeshProUGUI AddText(string name, Transform parent, Vector2 pos, Vector2 size, int fSize, TextAlignmentOptions align = TextAlignmentOptions.Left)
        {
            GameObject t = new GameObject(name);
            t.transform.SetParent(parent, false);
            RectTransform r = t.AddComponent<RectTransform>();
            r.anchoredPosition = pos;
            r.sizeDelta = size;
            TextMeshProUGUI tmp = t.AddComponent<TextMeshProUGUI>();
            tmp.color = Color.white;
            tmp.fontSize = fSize;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = align;
            return tmp;
        }

        // 2. Global Bakiye UI (Sol Üst)
        GameObject globalFoodPanel = new GameObject("GlobalFoodPanel");
        globalFoodPanel.transform.SetParent(canvas.transform, false);
        RectTransform gfRect = globalFoodPanel.AddComponent<RectTransform>();
        gfRect.anchorMin = new Vector2(0, 1);
        gfRect.anchorMax = new Vector2(0, 1);
        gfRect.pivot = new Vector2(0, 1);
        gfRect.anchoredPosition = new Vector2(20, -20);
        gfRect.sizeDelta = new Vector2(300, 200);
        Image gfImg = globalFoodPanel.AddComponent<Image>();
        gfImg.color = new Color(0, 0, 0, 0.7f);

        uiManager.globalKuruMamaText = AddText("KuruMamaTxt", globalFoodPanel.transform, new Vector2(10, -20), new Vector2(280, 40), 24);
        uiManager.globalYasMamaText = AddText("YasMamaTxt", globalFoodPanel.transform, new Vector2(10, -60), new Vector2(280, 40), 24);
        uiManager.globalAcBitirText = AddText("AcBitirTxt", globalFoodPanel.transform, new Vector2(10, -100), new Vector2(280, 40), 24);
        uiManager.globalKuruEtText = AddText("KuruEtTxt", globalFoodPanel.transform, new Vector2(10, -140), new Vector2(280, 40), 24);

        // 3. CatStatsPanel içine Mama Bilgisi ve Topla Butonu Ekleme
        if (uiManager.catStatsPanel != null)
        {
            GameObject localFoodPanel = new GameObject("LocalFoodPanel");
            localFoodPanel.transform.SetParent(uiManager.catStatsPanel.transform, false);
            RectTransform lfRect = localFoodPanel.AddComponent<RectTransform>();
            lfRect.anchorMin = new Vector2(0.5f, 0); lfRect.anchorMax = new Vector2(0.5f, 0);
            lfRect.pivot = new Vector2(0.5f, 0);
            lfRect.anchoredPosition = new Vector2(0, 150); // ActionButton'ın üstünde
            lfRect.sizeDelta = new Vector2(500, 100);

            GridLayoutGroup lfGroup = localFoodPanel.AddComponent<GridLayoutGroup>();
            lfGroup.cellSize = new Vector2(240, 40);
            lfGroup.spacing = new Vector2(10, 10);
            lfGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            lfGroup.constraintCount = 2;

            uiManager.localKuruMamaText = AddText("LKuru", localFoodPanel.transform, Vector2.zero, new Vector2(240, 40), 20, TextAlignmentOptions.Center);
            uiManager.localYasMamaText = AddText("LYas", localFoodPanel.transform, Vector2.zero, new Vector2(240, 40), 20, TextAlignmentOptions.Center);
            uiManager.localAcBitirText = AddText("LAc", localFoodPanel.transform, Vector2.zero, new Vector2(240, 40), 20, TextAlignmentOptions.Center);
            uiManager.localKuruEtText = AddText("LEt", localFoodPanel.transform, Vector2.zero, new Vector2(240, 40), 20, TextAlignmentOptions.Center);

            // Collect Butonu
            GameObject collectBtnObj = new GameObject("CollectButton");
            collectBtnObj.transform.SetParent(uiManager.catStatsPanel.transform, false);
            RectTransform cbRect = collectBtnObj.AddComponent<RectTransform>();
            cbRect.anchorMin = new Vector2(0.85f, 0.1f); cbRect.anchorMax = new Vector2(0.85f, 0.1f);
            cbRect.pivot = new Vector2(0.5f, 0.5f);
            cbRect.anchoredPosition = new Vector2(-20, 80);
            cbRect.sizeDelta = new Vector2(150, 60);
            Image cbImg = collectBtnObj.AddComponent<Image>();
            cbImg.color = new Color(0.8f, 0.5f, 0.1f, 1f); // Turuncu
            uiManager.collectFoodButton = collectBtnObj.AddComponent<Button>();

            TextMeshProUGUI cbTxt = AddText("Text", collectBtnObj.transform, Vector2.zero, new Vector2(150, 60), 24, TextAlignmentOptions.Center);
            cbTxt.text = "TOPLA";
            cbTxt.GetComponent<RectTransform>().anchorMin = Vector2.zero; cbTxt.GetComponent<RectTransform>().anchorMax = Vector2.one;
            cbTxt.GetComponent<RectTransform>().offsetMin = Vector2.zero; cbTxt.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        }

        // 4. Gün Sonu Paneli
        GameObject eodPanel = new GameObject("EndOfDayPanel");
        eodPanel.transform.SetParent(canvas.transform, false);
        RectTransform eodRect = eodPanel.AddComponent<RectTransform>();
        eodRect.anchorMin = Vector2.zero; eodRect.anchorMax = Vector2.one;
        eodRect.offsetMin = Vector2.zero; eodRect.offsetMax = Vector2.zero;
        Image eodBg = eodPanel.AddComponent<Image>();
        eodBg.color = new Color(0, 0, 0, 0.9f);

        GameObject eodBox = new GameObject("Box");
        eodBox.transform.SetParent(eodPanel.transform, false);
        RectTransform eodbRect = eodBox.AddComponent<RectTransform>();
        eodbRect.anchorMin = new Vector2(0.5f, 0.5f); eodbRect.anchorMax = new Vector2(0.5f, 0.5f);
        eodbRect.anchoredPosition = Vector2.zero; eodbRect.sizeDelta = new Vector2(600, 500);
        Image eodbImg = eodBox.AddComponent<Image>();
        eodbImg.color = new Color(0.15f, 0.15f, 0.15f, 1f);

        TextMeshProUGUI eodTitle = AddText("Title", eodBox.transform, new Vector2(0, 200), new Vector2(600, 60), 40, TextAlignmentOptions.Center);
        eodTitle.text = "GÜN SONU";

        uiManager.endOfDayTotalKuruMamaText = AddText("EOD_Kuru", eodBox.transform, new Vector2(0, 80), new Vector2(500, 40), 28, TextAlignmentOptions.Center);
        uiManager.endOfDayTotalYasMamaText = AddText("EOD_Yas", eodBox.transform, new Vector2(0, 30), new Vector2(500, 40), 28, TextAlignmentOptions.Center);
        uiManager.endOfDayTotalAcBitirText = AddText("EOD_Ac", eodBox.transform, new Vector2(0, -20), new Vector2(500, 40), 28, TextAlignmentOptions.Center);
        uiManager.endOfDayTotalKuruEtText = AddText("EOD_Et", eodBox.transform, new Vector2(0, -70), new Vector2(500, 40), 28, TextAlignmentOptions.Center);

        GameObject eodBtnObj = new GameObject("EndDayButton");
        eodBtnObj.transform.SetParent(eodBox.transform, false);
        RectTransform ebRect = eodBtnObj.AddComponent<RectTransform>();
        ebRect.anchorMin = new Vector2(0.5f, 0); ebRect.anchorMax = new Vector2(0.5f, 0);
        ebRect.anchoredPosition = new Vector2(0, 60); ebRect.sizeDelta = new Vector2(300, 80);
        Image ebImg = eodBtnObj.AddComponent<Image>();
        ebImg.color = new Color(0.1f, 0.8f, 0.2f, 1f); // Yeşil
        uiManager.endOfDayCloseButton = eodBtnObj.AddComponent<Button>();

        TextMeshProUGUI ebTxt = AddText("Text", eodBtnObj.transform, Vector2.zero, new Vector2(300, 80), 32, TextAlignmentOptions.Center);
        ebTxt.text = "GÜNÜ BİTİR";
        ebTxt.GetComponent<RectTransform>().anchorMin = Vector2.zero; ebTxt.GetComponent<RectTransform>().anchorMax = Vector2.one;
        ebTxt.GetComponent<RectTransform>().offsetMin = Vector2.zero; ebTxt.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        uiManager.endOfDayPanel = eodPanel;
        eodPanel.SetActive(false);

        EditorUtility.SetDirty(uiManager);
        Debug.Log("Mama Sistemi ve Gün Sonu UI'ı başarıyla eklendi! UIManager güncellendi.");
    }
}
