using UnityEngine;

public class WorldScroller : MonoBehaviour
{
    [Header("Scroller Settings")]
    public float Speed = 10f; // Speed at which the world scrolls

    [Header("Reset Limits")]
    public float ResetZ = -15f; // Z position at which the world resets
    public float StartZ = 30f; // Z position to reset to start

    void Update()
    {
        if (GameManager.isGameOver) return;
        
        transform.Translate(Vector3.back * GameManager.CurrentSpeed * Time.deltaTime);

        if (transform.position.z <= ResetZ)
        {
            Vector3 resetPos = new Vector3(transform.position.x, transform.position.y, StartZ);
            transform.position = resetPos;
        }
    }
}
