using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class WaterMiniGame : MonoBehaviour
{
    [Header("UI References")]
    public GameObject refillPanel;      // หน้าต่างมินิเกม
    public Slider qteSlider;            // หลอดวิ่ง
    public RectTransform targetArea;    // พื้นที่สีเขียว (เป้าหมาย)
    public GameObject jumpScarePanel;   // รูปผี

    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Settings")]
    public int targetSuccess = 5;       // ต้องกดให้โดนกี่ครั้ง
    public int maxFails = 3;            // พลาดได้กี่ครั้ง
    public float baseSpeed = 2f;        // ความเร็วพื้นฐาน

    private bool isPlaying = false;
    private int currentSuccess = 0;
    private int currentFails = 0;
    private float currentSpeed;
    private float time;

    public void OpenRefillGame()
    {
        // เปิดหน้าต่าง รีเซ็ตค่า
        refillPanel.SetActive(true);
        jumpScarePanel.SetActive(false);

        currentSuccess = 0;
        currentFails = 0;
        isPlaying = true;

        UpdateProgressText();

        RandomizeSpeed();
    }

    public void CloseRefillGame()
    {
        refillPanel.SetActive(false);
        isPlaying = false;
    }

    void Update()
    {
        if (!isPlaying) return;

        // ทำให้หลอดวิ่งไปกลับ (PingPong) ค่า 0 ถึง 1
        time += Time.deltaTime * currentSpeed;
        qteSlider.value = Mathf.PingPong(time, 1f);
    }

    private void UpdateProgressText()
    {
        if (progressText != null)
        {
            // แสดงผลเช่น "Success: 2/5"
            progressText.text = $"Success: {currentSuccess}/{targetSuccess}";

            // (ลูกเล่นเสริม) เปลี่ยนสีถ้าใกล้ครบแล้ว
            if (currentSuccess == targetSuccess - 1)
                progressText.color = Color.green;
            else
                progressText.color = Color.white;
        }
    }

    // ผูกปุ่มนี้กับปุ่ม "CLICK!" ในหน้า Refill
    public void OnClickButton()
    {
        if (!isPlaying) return;

        // เช็คว่าค่า Slider อยู่ในโซนเป้าหมายไหม (สมมติเป้าอยู่ตรงกลาง 0.4 - 0.6)
        // คุณอาจปรับ logic นี้ให้สัมพันธ์กับตำแหน่ง targetArea จริงๆ ได้
        float hitValue = qteSlider.value;
        bool isHit = (hitValue >= 0.4f && hitValue <= 0.6f);

        if (isHit)
        {
            currentSuccess++;
            UpdateProgressText();
            Debug.Log($"Success {currentSuccess}/{targetSuccess}");

            if (currentSuccess >= targetSuccess)
            {
                // ชนะ: เติมน้ำเต็ม
                FarmManager.Instance.RefillWaterFull();
                CloseRefillGame();
            }
            else
            {
                // ยังไม่ครบ: สุ่มความเร็วใหม่ให้ยากขึ้น
                RandomizeSpeed();
            }
        }
        else
        {
            currentFails++;
            Debug.Log($"Fail {currentFails}/{maxFails}");

            if (currentFails >= maxFails)
            {
                // แพ้: โดนหลอก + เงินหาย
                StartCoroutine(TriggerJumpScare());
            }
        }
    }

    void RandomizeSpeed()
    {
        // สุ่มความเร็วระหว่าง 1x ถึง 3x
        currentSpeed = Random.Range(baseSpeed, baseSpeed * 3f);
    }

    IEnumerator TriggerJumpScare()
    {
        isPlaying = false;

        // 1. เงินหายหมด
        FarmManager.Instance.Bankrupt();

        // 2. แสดงรูปผี
        jumpScarePanel.SetActive(true);

        // (Optional) สั่งสั่นหน้าจอ หรือเล่นเสียงกรี๊ดตรงนี้

        // 3. รอ 2 วินาทีแล้วปิด
        yield return new WaitForSeconds(2f);

        jumpScarePanel.SetActive(false);
        CloseRefillGame();

        // หมายเหตุ: แพ้แล้วน้ำไม่เต็มนะ ต้องกดเติมใหม่!
    }
}