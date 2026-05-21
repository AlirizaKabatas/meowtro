using UnityEngine;
using TMPro;

public class CatPanelManager : MonoBehaviour
{
    public static CatPanelManager Instance { get; private set; }

    [Header("Main Panel")]
    public GameObject panel;

    [Header("Text References - Info")]
    public TextMeshProUGUI genderText;
    public TextMeshProUGUI primaryClassText;
    public TextMeshProUGUI secondaryClassText;

    [Header("Text References - Stats")]
    public TextMeshProUGUI efficiencyText;
    public TextMeshProUGUI cutenessText;
    public TextMeshProUGUI charismaText;
    public TextMeshProUGUI meowingText;
    public TextMeshProUGUI rhythmText;
    public TextMeshProUGUI actingText;
    public TextMeshProUGUI sleightOfPawText;
    public TextMeshProUGUI nurturingText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && panel != null && panel.activeSelf)
        {
            ClosePanel();
        }
    }

    public void OpenPanel(CatStats stats)
    {
        if (panel == null) return;

        // Info
        if (genderText != null) genderText.text = "Gender: " + stats.gender.ToString();
        if (primaryClassText != null) primaryClassText.text = "Class: " + stats.primaryClass.ToString();
        if (secondaryClassText != null) 
        {
            if (stats.secondaryClass == CatClass.None)
                secondaryClassText.text = "";
            else
                secondaryClassText.text = "Class 2: " + stats.secondaryClass.ToString();
        }

        // Stats
        if (efficiencyText != null) efficiencyText.text = "Efficiency: " + stats.efficiency;
        if (cutenessText != null) cutenessText.text = "Cuteness: " + stats.cuteness;
        if (charismaText != null) charismaText.text = "Charisma: " + stats.charisma;
        if (meowingText != null) meowingText.text = "Meowing: " + stats.meowing;
        if (rhythmText != null) rhythmText.text = "Rhythm: " + stats.rhythm;
        if (actingText != null) actingText.text = "Acting: " + stats.acting;
        if (sleightOfPawText != null) sleightOfPawText.text = "Sleight Of Paw: " + stats.sleightOfPaw;
        if (nurturingText != null) nurturingText.text = "Nurturing: " + stats.nurturing;

        panel.SetActive(true);
    }

    public void ClosePanel()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
