using UnityEngine;

public class EnemyParallax : MonoBehaviour
{
    public float speed = 5f;               // Speed at which the enemy ship moves downward
    public float resetHeight = 10f;        // Height at which the enemy resets above the screen
    public float bottomLimit = -5f;        // Y position where the enemy ship goes off-screen and should reset
    public Vector3 resetOffset;            // Offset position to reset above screen (X and Z axis)

    private Vector3 startPosition;         // Original starting position of the enemy ship

    void Start()
    {
        // Store the initial position of the enemy ship
        startPosition = transform.position;
    }

    void Update()
    {
        // Move the ship downward
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // If the enemy ship moves below the bottom limit, reset its position above the screen
        if (transform.position.y <= bottomLimit)
        {
            // Reset the ship's position to just above the screen at a specified offset
            ResetPosition();
        }
    }

    void ResetPosition()
    {
        // Reset position back to above the screen, maintaining the starting X and Z positions with an offset
        transform.position = new Vector3(startPosition.x + resetOffset.x, resetHeight, startPosition.z + resetOffset.z);
    }
}
