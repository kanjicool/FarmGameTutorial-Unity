using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Configuration")]
    public List<PlantData> plantDataList;
    public GameObject plantItemPrefab;
    public Transform contentHolder;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateShopItems();   
    }

    void GenerateShopItems()
    {
        foreach (Transform child in contentHolder)
        {
            Destroy(child.gameObject);
        }

        foreach (PlantData data in plantDataList)
        {
            GameObject newItemObj = Instantiate(plantItemPrefab, contentHolder);

            PlantStoreItem itemScript = newItemObj.GetComponent<PlantStoreItem>();

            if (itemScript != null )
            {
                itemScript.plantData = data;
                itemScript.UpdateUI();
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
