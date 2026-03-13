using System.Runtime.CompilerServices;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [HideInInspector] public PlantData data;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] Transform coinPoin;

    private float currentGrowthTime;
    private SpriteRenderer spriteRenderer;

    // --- Reference to parent tile ---
    private FarmTile parentTile;

    private int harvestTimeLeft;
    public bool isDead = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        parentTile = GetComponentInParent<FarmTile>();

    }


    private void OnMouseDown()
    {
        if (parentTile != null)
        {
            parentTile.Interact();
        }

    }

    public void ResetPlant()
    {
        data = FarmManager.Instance.GetSelectedPlant();
        if (data == null)
        {
            Debug.LogWarning("[Plant] Cannot reset, PlantData not assigned");
            return;
        }
        currentGrowthTime = 0f;

        // Reset sprite to first growth stage
        if (data.growthStages != null && data.growthStages.Length > 0)
        {
            spriteRenderer.sprite = data.growthStages[0];

            harvestTimeLeft = Random.Range(data.minHarvestTimes, data.maxHarvestTimes + 1);
            isDead = false;
        }

        // Ensure plant is visible
        gameObject.SetActive(true);

        Debug.Log($"[Plant] Growth reset: {data.plantName}");
    }

    private void Update()
    {
        if (data == null) return;

        // Growth progression
        if (parentTile == null || !parentTile.isWet)
        {
            return;
        }
        currentGrowthTime += Time.deltaTime;

        float progress = Mathf.Clamp01(currentGrowthTime / data.timeToGrow);
        int stageIndex = Mathf.FloorToInt(progress * data.growthStages.Length);
        stageIndex = Mathf.Clamp(stageIndex, 0, data.growthStages.Length - 1);

        spriteRenderer.sprite = data.growthStages[stageIndex];
    }

    public bool IsReadyForHarvest()
    {
        return currentGrowthTime >= data.timeToGrow;
    }

    public void Harvest()
    {
        if (!IsReadyForHarvest() || isDead) return;

        if (coinPrefab != null)
        {
            Vector3 spawPos = coinPoin != null ? coinPoin.position : transform.position;
            spawPos.z = -5f;
            GameObject coinObj = Instantiate(coinPrefab, coinPoin.position, Quaternion.identity);
            Coin coinScript = coinObj.GetComponent<Coin>();
            if (coinScript != null)
            {
                coinScript.Initialized(data.sellPrice);
            }
            else
            {
                Debug.LogError("Coin Prefab ไม่มีสคริปต์ 'Coin' แปะอยู่!");
            }

            //gameObject.SetActive(false);
            //currentGrowthTime = 0f;

        }

        if (data.isRegrowable)
        {
            harvestTimeLeft--;

            if (harvestTimeLeft > 0)
            {
                // ถ้าเป็นพืชงอกใหม่ได้
                // สูตร: (เวลาทั้งหมด / จำนวนภาพ) * ลำดับภาพที่จะย้อนไป
                float timePerStage = data.timeToGrow / data.growthStages.Length;
                currentGrowthTime = timePerStage * data.regrowthStageIndex;

                spriteRenderer.sprite = data.growthStages[data.regrowthStageIndex];
                Debug.Log($"[Plant] Harvested {data.plantName} -> Regrowing from stage {data.regrowthStageIndex}");
            }
            else
            {
                Die();
            }

        }
        else
        {
            Debug.Log($"[Plant] Harvested {data.plantName} -> Destroying");
            Destroy(gameObject);

        }
        
        

    }

    private void Die()
    {
        isDead = true;
        if (data.deadSprite != null)
        {
            spriteRenderer.sprite = data.deadSprite;
        }
        Debug.Log("ต้นไม้ตายแล้ว! ต้องใช้จอบขุดออก");
    }
}
