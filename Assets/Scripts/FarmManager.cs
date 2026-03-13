using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.Collections;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance { get; private set; }

    [Header("Game Settings")]
    public int targetMoneyToWin = 1000;
    public float totalGameTime = 300f; // 5 นาที
    public int passiveIncomeAmount = 10;
    public float passiveIncomeInterval = 10f;

    [Header("Player Economy")]
    [SerializeField] private int currentMoney = 150;

    [Header("Resources")]
    public int maxWater = 10;
    public int currentWater = 10;

    [Header("State")]
    public float currentTime;
    private float incomeTimer;
    public bool isGameActive = true;
    public bool isPaused = false;

    [Header("Current Plant Selection")]
    [SerializeField] private PlantData selectedPlant;

    public bool isHoeSelected = false;

    
    public PlantData GetSelectedPlan()
    {
        return selectedPlant;
    }

    private void Awake()
    {
        // Enforce the Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Optional: Keep the manager alive across scenes
        }
        else
        {
            // Destroy any extra instances
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        currentTime = totalGameTime;
        incomeTimer = passiveIncomeInterval;
        isGameActive = true;
        isPaused = false;
        Time.timeScale = 1f;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateMoneyText(currentMoney);
            UIManager.Instance.UpdateTimerText(currentMoney);

            UIManager.Instance.UpdateGoalProgress(currentMoney, targetMoneyToWin);
        }

        currentWater = maxWater;
        UpdateWaterUI();
    }

    private void Update()
    {
        // ถ้าเกมจบ หรือ หยุดเกมอยู่ ไม่ต้องทำอะไร
        if (!isGameActive || isPaused) return;

        currentTime -= Time.deltaTime;
        if (UIManager.Instance != null) UIManager.Instance.UpdateTimerText(currentTime);

        if (currentTime <= 0)
        {
            GameOver();
        }

        incomeTimer -= Time.deltaTime;
        if (incomeTimer <= 0)
        {
            AddMoney(passiveIncomeAmount);
            incomeTimer = passiveIncomeInterval;
            Debug.Log($"[System] เงินช่วยเหลือเข้าบัญชี {passiveIncomeAmount} บาท");
        }
    }

    // ---------------- System Functions ----------------
    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            Time.timeScale = 0f;
            UIManager.Instance.ShowPausePanel(true);
        }
        else
        {
            Time.timeScale = 1f;
            UIManager.Instance.ShowPausePanel(false);
        }
    }

    public void RestartGame()
    {
        StartCoroutine(RestartSequence());
    }

    private IEnumerator RestartSequence()
    {
        // 1. คืนค่าเวลาก่อน (เผื่อเกม Pause อยู่)
        Time.timeScale = 1f;

        // 2. สั่ง UIManager ให้ Fade ดำ (จากใส 0 -> ดำ 1)
        if (UIManager.Instance != null)
        {
            // รอจนกว่า Fade จะเสร็จ
            yield return UIManager.Instance.Fade(0f, 1f);
        }

        // 3. โหลด Scene เดิม
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    private void GameOver()
    {
        isGameActive = false;
        Time.timeScale = 0f;
        UIManager.Instance.ShowLosePanel();
        Debug.Log("Game Over! เวลาหมด");
    }

    private void WinGame()
    {
        isGameActive = false;
        Time.timeScale = 0f;
        UIManager.Instance.ShowWinPanel();
        Debug.Log("You Win! เงินครบตามเป้าหมาย");
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadMenuSequence());
    }

    private IEnumerator LoadMenuSequence()
    {
        Time.timeScale = 1f;
        if (UIManager.Instance != null)
        {
            yield return UIManager.Instance.Fade(0f, 1f);
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void SelectPlant(PlantData newPlant)
    {
        if (newPlant == null)
        {
            Debug.LogError("Attempted to select a null PlantData.");
            return;
        }

        selectedPlant = newPlant;
        isHoeSelected = false;
        Debug.Log($"เลือกปลูก: {selectedPlant.plantName}");
    }

    public void SelectHoe()
    {
        isHoeSelected = true;
        selectedPlant = null; // ถ้าเลือกจอบ ต้องวางเมล็ด
        Debug.Log("เลือกใช้จอบ (Hoe Selected)");
    }


    public PlantData GetSelectedPlant()
    {
        // The FarmTile will receive the Tomato, Corn, or Sunflower asset based on the selection.
        return selectedPlant;
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        currentMoney += amount;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateMoneyText(currentMoney);
            UIManager.Instance.UpdateGoalProgress(currentMoney, targetMoneyToWin);

        } 

        Debug.Log($"[FarmManager] Added {amount} gold. Total: {currentMoney}");

        if (isGameActive && currentMoney >= targetMoneyToWin)
        {
            WinGame();
        }
    
        
    }    


    public bool TryUseWater()
    {
        if (currentWater > 0)
        {
            currentWater--;
            UpdateWaterUI();
            return true;
        }
        return false;
    }

    public void RefillWaterFull()
    {
        currentWater = maxWater;
        UpdateWaterUI();
    }

    private void UpdateWaterUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateWaterUI(currentWater, maxWater);
        }
    }

    public void Bankrupt()
    {
        currentMoney = 0; // เงินหายหมด
        if (UIManager.Instance != null) UIManager.Instance.UpdateMoneyText(currentMoney);
        Debug.Log("ล้มละลาย! เงินเหลือ 0");
    }


    public bool TrySubtractMoney(int amount)
    {
        if (amount <= 0 || currentMoney < amount)
        {
            Debug.LogWarning($"[FarmManager] Not enough money to buy. Required: {amount}, Current: {currentMoney}");
            return false;
        }

        currentMoney -= amount;
        Debug.Log($"[FarmManager] Subtracted {amount} gold. Total: {currentMoney}");

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateMoneyText(currentMoney);
            UIManager.Instance.UpdateGoalProgress(currentMoney, targetMoneyToWin);
        }

        return true;
    
    }

    public void DeselectAll()
    {
        selectedPlant = null;
        isHoeSelected=false;
        Debug.Log("ยกเลิกการเลือกทั้งหมด (มือว่าง)");
    }

}
