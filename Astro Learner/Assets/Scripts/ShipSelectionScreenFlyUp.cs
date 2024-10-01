using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelectionFlyIn : MonoBehaviour
{
    public RectTransform shipSelectionRect;  // The RectTransform of the entire ShipSelection object (parent)
    public Vector2 offScreenPosition;        // Starting position off the screen (below the canvas)
    public Vector2 onScreenPosition;         // Target position on the screen (center or desired position)
    public float flyDuration = 0.5f;         // Duration of the fly animation (for both fly-in and fly-out)
    public Button triggerButton;             // Button to trigger the fly-in and fly-out animation

    private bool isFlyingIn = false;         // Track whether the UI is currently flown in
    private bool isAnimating = false;        // Prevent multiple triggers during animation

    void Start()
    {
        // Set the initial position of the ship selection screen to off-screen
        shipSelectionRect.anchoredPosition = offScreenPosition;

        // Add a listener to the button to toggle the fly-in and fly-out
        triggerButton.onClick.AddListener(ToggleFly);
    }

    // Method to toggle between flying in and flying out
    void ToggleFly()
    {
        if (!isAnimating)  // Ensure we don't trigger the animation multiple times
        {
            if (isFlyingIn)
            {
                StartCoroutine(FlyOutAnimation());  // Fly out if already in
            }
            else
            {
                StartCoroutine(FlyInAnimation());   // Fly in if off-screen
            }
        }
    }

    // Coroutine to handle the smooth fly-in animation
    IEnumerator FlyInAnimation()
    {
        isAnimating = true;   // Lock the animation state

        float elapsedTime = 0f;
        Vector2 startingPosition = shipSelectionRect.anchoredPosition;  // Start from the current position

        // Fly in for the given duration
        while (elapsedTime < flyDuration)
        {
            elapsedTime += Time.deltaTime;

            // Lerp the position smoothly from off-screen to the on-screen position over time
            shipSelectionRect.anchoredPosition = Vector2.Lerp(startingPosition, onScreenPosition, elapsedTime / flyDuration);

            yield return null;  // Wait for the next frame
        }

        // Ensure the position is exactly at the target position
        shipSelectionRect.anchoredPosition = onScreenPosition;

        isFlyingIn = true;    // Now the UI is in the "on-screen" state
        isAnimating = false;  // Unlock the animation state
    }

    // Coroutine to handle the smooth fly-out animation
    IEnumerator FlyOutAnimation()
    {
        isAnimating = true;  // Lock the animation state

        float elapsedTime = 0f;
        Vector2 startingPosition = shipSelectionRect.anchoredPosition;  // Start from the current position

        // Fly out for the given duration
        while (elapsedTime < flyDuration)
        {
            elapsedTime += Time.deltaTime;

            // Lerp the position smoothly from on-screen to the off-screen position over time
            shipSelectionRect.anchoredPosition = Vector2.Lerp(startingPosition, offScreenPosition, elapsedTime / flyDuration);

            yield return null;  // Wait for the next frame
        }

        // Ensure the position is exactly at the off-screen position
        shipSelectionRect.anchoredPosition = offScreenPosition;

        isFlyingIn = false;   // Now the UI is in the "off-screen" state
        isAnimating = false;  // Unlock the animation state
    }
}
