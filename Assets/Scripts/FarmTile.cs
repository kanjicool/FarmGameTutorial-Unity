using UnityEngine;

public class FarmTile : MonoBehaviour
{
    [Header("Configuration")]
    // ลาก FarmPlotData Asset มาใส่ตรงนี้เพื่อกำหนดราคาและสี
    [SerializeField] private FarmPlotData plotData;
    [SerializeField] private GameObject plantPrefab; // ลาก Prefab ต้นไม้ที่จะปลูกมาใส่

    [Header("State")]
    [SerializeField] private bool isLocked = true; // เริ่มต้นมาล็อคอยู่ไหม?
    private bool isPlanted = false;

    // -- Water System Variables
    public bool isWet = false;
    private float waterDuration = 10f;
    private float currentWaterTimer = 0f;

    // References
    private SpriteRenderer spriteRenderer;
    private Plant currentPlant; // อ้างอิงไปยังสคริปต์ Plant ของต้นไม้ที่ปลูกอยู่

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // ตั้งค่าสีเริ่มต้นตอนรันเกม
        UpdateTileVisuals();
    }

    private void Update()
    {
        if (!isLocked && isWet)
        {
            currentWaterTimer -= Time.deltaTime;
            if (currentWaterTimer <= 0)
            {
                DryOut();
            }
        }
    }

    public void Interact()
    {
        if (isLocked)
        {
            TryUnlockPlot();
            return;
        }

        if (currentPlant != null && currentPlant.isDead)
        {
            if (FarmManager.Instance.isHoeSelected)
            {
                RemoveDeadPlant();
            }
            else
            {
               Debug.LogWarning("ต้นไม้ตายแล้ว! ต้องใช้จอบขุดออก (ไปเลือกจอบก่อน)");
            }
            return;
        }

        if (!isPlanted)
        {
            if (!FarmManager.Instance.isHoeSelected)
            {
                PlantSeed();
            }

        }
        else
        {
            if (currentPlant != null && currentPlant.IsReadyForHarvest())
            {
                HarvestCrop();
            }
            else if (!isWet)
            {
                WaterTile();
            }
        }
    }

    private void RemoveDeadPlant()
    {
        int removalCost = 30;

        if (FarmManager.Instance.TrySubtractMoney(removalCost))
        {
            Destroy(currentPlant.gameObject);
            currentPlant = null;
            isPlanted = false;
            isWet = false;
            UpdateTileVisuals();

            

            Debug.Log($"ใช้จอบขุดต้นไม้ตาย เสียเงิน {removalCost} บาท");
        }
        else
        {
            Debug.Log("เงินไม่พอค่าขุด (100 บาท)!");
        }

    }

    private void OnMouseDown()
    {
        Interact();
    }

    private void WaterTile()
    {
        if (FarmManager.Instance.TryUseWater())
        {
            isWet = true;
            currentWaterTimer = waterDuration;
            UpdateTileVisuals();
            Debug.Log("รดน้ำสำเร็จ!");
        }
        else
        {
            Debug.Log("น้ำหมด! กรุณาเติมน้ำที่ถัง");
            // อาจจะใส่เสียง "ติ๊ดๆ" แจ้งเตือนตรงนี้
        }
    }

    private void DryOut()
    {
        isWet = false;
        currentWaterTimer = 0;
        UpdateTileVisuals();
        Debug.Log("ดินแห้งแล้ว... (พืชหยุดโต)");
    }
        
    private void TryUnlockPlot()
    {
        if (plotData == null)
        {
            Debug.LogError("[FarmTile] ยังไม่ได้ใส่ FarmPlotData ใน Inspector!");
            return;
        }

        int price = plotData.purchasePrice;

        // เรียก Singleton FarmManager เพื่อเช็คและตัดเงิน
        if (FarmManager.Instance.TrySubtractMoney(price))
        {
            // ปลดล็อคสำเร็จ
            isLocked = false;
            UpdateTileVisuals(); // เปลี่ยนสี

            if (UIManager.Instance != null)
            {
                int currentMoney = FarmManager.Instance.GetCurrentMoney();
                UIManager.Instance.UpdateMoneyText(currentMoney);
            }


            Debug.Log($"<color=green>ปลดล็อคแปลงสำเร็จ! จ่ายไป {price} เหรียญ</color>");

            // TODO: ใส่เสียงเอฟเฟกต์ซื้อที่ตรงนี้ได้
        }
        else
        {
            Debug.LogWarning($"เงินไม่พอ! ต้องการ {price} แต่มี {FarmManager.Instance.GetCurrentMoney()}");
            // TODO: เด้ง UI แจ้งเตือนเงินไม่พอตรงนี้
        }
    }

    private void PlantSeed()
    {

        PlantData selectedData = FarmManager.Instance.GetSelectedPlan();

        if (selectedData == null || plantPrefab == null)
        {
            Debug.LogWarning("ยังไม่ได้เลือกพืช หรือไม่มี Plant Prefab");
            return;
        }

        if (FarmManager.Instance.TrySubtractMoney(selectedData.buyPrice))
        {
            GameObject plantObj = Instantiate(plantPrefab, transform.position, Quaternion.identity, transform);
            currentPlant = plantObj.GetComponent<Plant>();

            if (currentPlant != null)
            {
                isPlanted = true;
                currentPlant.ResetPlant();
                Debug.Log($"{currentPlant} -> ปลูกพืชเรียบร้อย!");

                // Optional: ปลูกเสร็จใหม่ๆ ให้ดินแห้งหรือเปียก? (ที่นี่ให้แห้งก่อน ต้องรดน้ำเอง)
                //isWet = false;
                //UpdateTileVisuals();
            }
            else
            {
                Debug.LogError("Prefab ที่ใส่มาไม่มีสคริปต์ 'Plant' แปะอยู่!");
                // TODO: ใส่เสียงแจ้งเตือน หรือ UI เด้งแจ้งเตือนตรงนี้
            }
        }

    }

    private void HarvestCrop()
    {
        if (currentPlant != null)
        {
            bool willRegrow = currentPlant.data.isRegrowable;

            // สั่งให้ต้นไม้ทำการเก็บเกี่ยว (เช่น ให้เงิน, เล่น Effect, ทำลายตัวเอง)
            currentPlant.Harvest();

            if (willRegrow)
            {
                isWet = false;
                UpdateTileVisuals();
                Debug.Log("เก็บเกี่ยวพืชงอกใหม่แล้ว! (ดินแห้งลง -> รดน้ำเพื่อให้โตต่อ)");
            }
            else
            {
                currentPlant = null;
                isPlanted = false;
                isWet = false;
                Debug.Log("เก็บเกี่ยวเสร็จสิ้น แปลงว่างแล้ว");
            }

        }
    }

    private void UpdateTileVisuals()
    {
        if (spriteRenderer == null || plotData == null) return;

        if (isLocked)
        {
            spriteRenderer.color = plotData.lockedColor;
        }
        else
        {
            spriteRenderer.color = isWet ? plotData.wetColor : plotData.unlockedColor;
        }
    }
}