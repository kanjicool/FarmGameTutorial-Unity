using UnityEngine;

public class Coin : MonoBehaviour
{
    private int coinValue = 0;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    public void Initialized(int value)
    {
        coinValue = value;
    }

    private void OnMouseDown()
    {
        FarmManager.Instance.AddMoney(coinValue);
        Debug.Log($"เก็บเหรียญได้เงิน: {coinValue}");
        Destroy(gameObject);
    }

    //void Update()
    //{
        
    //}
}
