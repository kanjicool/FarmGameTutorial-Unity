using UnityEngine;

public class IsometricZ : MonoBehaviour
{
    [Tooltip("ค่า Offset: ใส่ค่าติดลบเพื่อให้ลอยมาข้างหน้า (เช่น -5 สำหรับเหรียญ)")]
    public float zOffset = 0f;

    void Start()
    {
        // เอาค่า Y มาคำนวณ Z แล้วบวกด้วย Offset ที่เราตั้งไว้
        float depth = (transform.position.y / 100f) + zOffset;

        transform.position = new Vector3(transform.position.x, transform.position.y, depth);
    }
}