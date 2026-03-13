using UnityEngine;
using UnityEngine.UI;

public class PlantSelectionButton : MonoBehaviour
{
    // DRAG the Scriptable Object (Tomato, Corn, or Sunflower) into this slot!
    public PlantData plantData;

    void Start()
    {
        // Setup listener for the standard UI click
        GetComponent<Button>().onClick.AddListener(SelectPlant);
    }

    private void SelectPlant()
    {
        // Passes the specific Scriptable Object instance to the Manager.
        FarmManager.Instance.SelectPlant(plantData);
        Debug.Log($"Currently selected: {plantData.plantName}");
    }
}

