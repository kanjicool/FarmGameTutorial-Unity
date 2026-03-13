using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    // Singleton
    public static UIManager Instance {  get; private set; }

    [Header("Goal UI")]
    [SerializeField] private Slider goalSlider;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI goalAmountText;

    [Header("UI Elements")]
    [SerializeField] private CanvasGroup faderCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;


    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Water UI")]
    [SerializeField] private TextMeshProUGUI waterText;
    [SerializeField] private Button waterButton;   
    [SerializeField] private Image waterIconImage;  
    [SerializeField] private WaterMiniGame waterMiniGame; 


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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (faderCanvasGroup != null)
        {
            faderCanvasGroup.alpha = 1f;
            StartCoroutine(Fade(1f, 0f));
        }

        if (waterButton != null)
        {
            waterButton.onClick.AddListener(() => {
                int current = FarmManager.Instance.currentWater;
                int max = FarmManager.Instance.maxWater;

                if (current > max)
                {
                    Debug.Log("น้ำเต็มอยู่ครับ! ไม่ต้องเติม");
                    return;
                }


                if (waterMiniGame != null)
                {
                    waterMiniGame.OpenRefillGame();
                }
            });
        }

        UpdateMoneyText(FarmManager.Instance.GetCurrentMoney());
    }

    public IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);

            if (faderCanvasGroup != null)
                faderCanvasGroup.alpha = newAlpha;

            yield return null; // รอเฟรมถัดไป
        }

        if (faderCanvasGroup != null)
            faderCanvasGroup.alpha = endAlpha;
    }

    public void UpdateMoneyText(int newMoney)
    {
        if (moneyText != null)
        {
            moneyText.text = $"$ {newMoney}";
        }
    }

    public void UpdateTimerText(float timeInSeconds)
    {
        if (timerText != null)
        {
            // แปลงวินาทีเป็น นาที:วินาที (เช่น 05:00)
            int minutes = Mathf.FloorToInt(timeInSeconds / 60);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    public void UpdateWaterUI(int current, int max)
    {
        if (waterText != null) waterText.text = $"{current}/{max}";

        if (waterIconImage != null)
        {
            if (current <= 0)
            {
                waterIconImage.color = Color.red;
                waterButton.interactable = true;
            }

            else if (current < max)
            {
                waterIconImage.color = Color.white;
                waterButton.interactable = true; // ให้กดได้
            }

            else
            {
                waterIconImage.color = Color.white; // หรือสีฟ้า
                 waterButton.interactable = false;
            }
        }
    }

    public void UpdateGoalProgress(int currentMoney, int targetMoney)
    {
        if (goalSlider != null)
        {
            // คำนวณเป็นค่า 0 ถึง 1 (เช่น 50/100 = 0.5)
            float progress = (float)currentMoney / targetMoney;
            goalSlider.value = progress;
        }

        if (goalText != null)
        {
            float percent = ((float)currentMoney / targetMoney) * 100;
            goalText.text = $"{percent:F0}%"; // F0 คือทศนิยม 0 ตำแหน่ง
        }

        if (goalAmountText != null)
        {

            // แบบ A: โชว์แค่เป้าหมาย "Goal: 500"
            // goalAmountText.text = $"Goal: ${targetMoney}";

            // แบบ B: โชว์ความคืบหน้า "100 / 500" (แนะนำอันนี้)
            goalAmountText.text = $"{currentMoney} / {targetMoney}";
        }
    }

    // --- Control Panels ---
    public void ShowPausePanel(bool isShow)
    {
        if (pausePanel != null) pausePanel.SetActive(isShow);
    }

    public void ShowWinPanel()
    {
        if (winPanel != null) winPanel.SetActive(true);
    }

    public void ShowLosePanel()
    {
        if (losePanel != null) losePanel.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
