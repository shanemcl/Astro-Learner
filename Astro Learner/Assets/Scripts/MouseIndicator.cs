using UnityEngine;

public class MouseIndicator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;  // The sprite renderer attached to the mouse indicator
    public Sprite indicatorSprite;         // The sprite you want to use for the indicator

    void Start()
    {
        // Ensure the SpriteRenderer has the correct sprite
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (indicatorSprite != null)
            spriteRenderer.sprite = indicatorSprite;

        // Hide the default hardware cursor (optional)
        Cursor.visible = false;
    }

    void Update()
    {
        // Convert the mouse position to world position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Set the Z coordinate to 0 to ensure it's on the correct plane
        mousePosition.z = 0;

        // Update the position of the indicator to follow the mouse
        transform.position = mousePosition;
    }
}