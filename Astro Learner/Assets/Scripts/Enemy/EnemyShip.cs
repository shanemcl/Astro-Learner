using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    public int health = 1;
    public float speed = 2.0f;
    public Vector2 movementDirection = Vector2.down;

    public enum MovementType { Straight, Zigzag, Circular }
    public MovementType movementType;
    public float zigzagAmplitude = 1.0f;
    public float circularRadius = 1.0f;

    private float movementTimer = 0f;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position; // Save starting position
    }

    private void Update()
    {
        Move();

        if (IsBelowCameraBounds())
        {
            WarpToInitialPosition();
        }
    }

    private void Move()
    {
        movementTimer += Time.deltaTime;

        switch (movementType)
        {
            case MovementType.Straight:
                transform.Translate(movementDirection * speed * Time.deltaTime);
                break;

            case MovementType.Zigzag:
                float zigzagOffset = Mathf.Sin(movementTimer * speed) * zigzagAmplitude;
                transform.Translate(new Vector2(zigzagOffset, -speed) * Time.deltaTime);
                break;

            case MovementType.Circular:
                  // Calculate circular movement
            float x = Mathf.Cos(movementTimer * speed) * circularRadius;
            float y = Mathf.Sin(movementTimer * speed) * circularRadius;

            // Add downward movement
            transform.Translate(new Vector2(x, -speed + y) * Time.deltaTime);
            break;
        }
    }

    private bool IsBelowCameraBounds()
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        return viewportPosition.y < 0;
    }

    private void WarpToInitialPosition()
    {
        transform.position = initialPosition;
        Debug.Log("Enemy warped to initial position.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
        {
            Debug.Log("Collision object is null, ignoring.");
            return;
        }

        if (collision.CompareTag("PlayerBullet"))
        {
            Debug.Log("Enemy hit by PlayerBullet.");
            TakeDamage(1);
            Destroy(collision.gameObject); // Destroy the bullet
        }
        else if (collision.CompareTag("Player"))
        {
            Debug.Log("Enemy collided with Player.");
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(1); // Damage the player
            }
        }
        else
        {
            Debug.Log($"Enemy collided with {collision.gameObject.name}, tag: {collision.tag}. Ignoring.");
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"Enemy took damage. Current health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy defeated.");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.enemiesDefeated++;
            Debug.Log($"Enemies defeated: {GameManager.Instance.enemiesDefeated}/{GameManager.Instance.totalEnemies}");

            if (GameManager.Instance.enemiesDefeated >= GameManager.Instance.totalEnemies)
            {
                GameManager.Instance.TriggerVictory();
            }
        }
        Destroy(gameObject);
    }

       public void SetMovementType(MovementType type)
    {
        movementType = type;
    }
}
