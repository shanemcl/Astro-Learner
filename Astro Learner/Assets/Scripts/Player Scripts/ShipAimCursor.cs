using UnityEngine;

public class ShipAimCursor : MonoBehaviour
{
    private Transform _transform;

    private void Awake()
    {
        _transform = transform; // Cache the transform for performance
    }

    private void Update()
    {
        RotateTowardsCursor();
    }

    private void RotateTowardsCursor()
    {
        // Get mouse position in world space
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate direction from the ship to the mouse position
        Vector2 direction = mouseWorldPosition - _transform.position;

        // Calculate the angle to rotate
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the rotation to the ship
        _transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f); // Offset by 90 degrees if the ship's forward direction is up
    }
}
