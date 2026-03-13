using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantStoreItem : MonoBehaviour
{
    [Header("Data")]
    public PlantData plantData;

    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button selectButton;

    private void Start()
    {
        UpdateUI();
        selectButton.onClick.AddListener(OnSelectItem);
    }

    public void UpdateUI()
    {
        if (plantData == null) return;

        if (iconImage != null) iconImage.sprite = plantData.icon;
        if (nameText != null) nameText.text = plantData.plantName;
        if (priceText != null) priceText.text = $"${plantData.buyPrice}";

    }

    private void OnSelectItem()
    {
        if (plantData != null)
        {
            FarmManager.Instance.SelectPlant(plantData);

            Debug.Log($"Selected Store Item: {plantData.plantName}");
        }
    }
}
