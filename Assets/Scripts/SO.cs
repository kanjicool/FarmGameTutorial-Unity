using UnityEngine;

// Menu path: Right-click -> Create -> Farm Game/Plant Data
[CreateAssetMenu(fileName = "NewPlantData", menuName = "Farm Game/Plant Data")]
public class PlantData : ScriptableObject
{
    // These fields hold the unique data for Tomato, Corn, and Sunflower.
    [Header("Identity")]
    public string plantName = "New Plant";
    public Sprite icon;

    [Header("Economy")]
    public int buyPrice = 10;
    public int sellPrice = 20;

    [Header("Growth & Harvest")]
    public float timeToGrow = 2f; // Time in seconds for full growth
    public Sprite[] growthStages;

    [Header("Regrowable Settings")]
    public bool isRegrowable = false;
    public int regrowthStageIndex = 3;

    [Header("Lifespan Settings")]
    public int minHarvestTimes = 3;
    public int maxHarvestTimes = 5;
    public Sprite deadSprite;
}
