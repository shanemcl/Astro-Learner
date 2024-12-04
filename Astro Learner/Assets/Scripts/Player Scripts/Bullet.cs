using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private bool isActive = false; // Indestructible state until fired

    private void Start()
    {
        if (!isActive)
        {
            Debug.Log("Bullet is in an indestructible state.");
        }
    }

    private void Update()
    {
        if (isActive)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
    }

    public void Activate()
    {
        isActive = true;
        Debug.Log("Bullet is now active and destructible.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return; // Ignore collisions if inactive

        // Handle collision with other objects
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Bullet hit the enemy.");
            Destroy(gameObject); // Destroy the bullet
        }
    }
}
