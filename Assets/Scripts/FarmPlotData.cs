using UnityEngine;

[CreateAssetMenu(fileName = "NewPlotData", menuName = "Farm Game/Plot Data")]
public class FarmPlotData : ScriptableObject
{
    [Header("Plot Purchase Information")]
    public int purchasePrice = 100; // ราคาที่ต้องใช้ซื้อเพื่อปลดล็อคแปลงนี้

    [Header("Visuals")]
    public Color lockedColor = Color.red;
    public Color unlockedColor = new Color(1f, 1f, 1f);
    public Color wetColor = new Color(0.6f, 0.6f, 0.6f);
}
