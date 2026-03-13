using UnityEngine;

public class PlotManager : MonoBehaviour
{
    bool isPlanted = false;
    public SpriteRenderer plant;
    BoxCollider2D plantCollider;

    public Sprite[] plantStages;
    int plantStage = 0;
    float timeBtwStage = 2f;
    float timer;

    void Start()
    {
        plant = transform.GetChild(0).GetComponent<SpriteRenderer>();
        plantCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();

        Debug.Log($"plant : {plant}");
        Debug.Log($"plantCollider : {plantCollider}");
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlanted)
        {
            timer -= Time.deltaTime;
            if (timer < 0 && plantStage <= plantStages.Length - 1)
            {
                timer = timeBtwStage;
                plantStage++;
                UpdatePlant();
            }
        }
    }
    private void OnMouseDown()
    {
        if (isPlanted)
        {
            if (plantStage == plantStages.Length - 1)
            {
                Harvest();
            }
        }
        else
        {
            Plant();
        }
        Debug.Log("Click");
    }
    void Harvest()
    {
        //Debug.Log("Harvested");
        isPlanted = false;
        plant.gameObject.SetActive(false);
    }
    void Plant()
    {
        //Debug.Log("Planted");
        isPlanted = true;
        plantStage = 0;
        UpdatePlant();
        timer = timeBtwStage;
        plant.gameObject.SetActive(true);

    }
    void UpdatePlant()
    {
        //Debug.Log($">>> {plantStage}");

        int currentStateIndex = Mathf.Clamp(plantStage, 0, plantStages.Length - 1);
        plant.sprite = plantStages[currentStateIndex];

        plantCollider.size = plant.sprite.bounds.size;
        plantCollider.offset = new Vector2(0, plant.bounds.size.y/2);
    }
}
