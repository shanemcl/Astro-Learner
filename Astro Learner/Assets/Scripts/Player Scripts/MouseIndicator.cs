using UnityEngine;

public class MouseIndicator : MonoBehaviour
{
    [Header("Sprite Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    [SerializeField] private Sprite indicatorSprite; // Sprite for the mouse indicator

    private void Start()
    {
        // Ensure the SpriteRenderer is assigned or fetch it from the GameObject
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Assign the indicator sprite if not already assigned
        if (indicatorSprite != null)
        {
            spriteRenderer.sprite = indicatorSprite;
        }

        // Set sorting layer and order to ensure the indicator is always on top
        spriteRenderer.sortingLayerName = "UI"; // Ensure this layer exists in your project
        spriteRenderer.sortingOrder = 100; // Set a high order value to always render on top

        // Optionally hide the default hardware cursor
        Cursor.visible = false;
    }

    private void Update()
    {
        // Follow the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = -1; // Ensure the Z position is always in front of other objects
        transform.position = mousePosition;
    }
}
