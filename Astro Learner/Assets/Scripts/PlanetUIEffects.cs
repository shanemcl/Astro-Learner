using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetUIEffects : MonoBehaviour
{
    // Glow effect variables
    public float glowSpeed = 1.0f;         // Speed at which the planet glows
    public float minBrightness = 0.5f;     // Minimum brightness (lower color intensity)
    public float maxBrightness = 1.5f;     // Maximum brightness (higher color intensity)
    
    // Bounce effect variables
    public float floatAmplitude = 10f;     // How much the planet moves up and down (adjust for UI units)
    public float floatSpeed = 1.0f;        // How fast the planet floats

    // Randomized offsets to desynchronize planets
    private float randomGlowOffset;        // Random time offset for glow
    private float randomFloatOffset;       // Random time offset for float

    private Image planetImage;             // Reference to the UI Image component
    private Color originalColor;           // To store the original color of the planet's image
    private RectTransform rectTransform;   // To handle the UI element's position

    private Vector3 startPos;              // To store the initial position of the planet

    void Start()
    {
        // Get the Image component attached to the UI object
        planetImage = GetComponent<Image>();

        // Get the RectTransform component for UI movement
        rectTransform = GetComponent<RectTransform>();

        // Store the original color of the planet
        originalColor = planetImage.color;

        // Capture the starting position of the planet for the floating effect
        startPos = rectTransform.anchoredPosition;

        // Add some randomness to the glow and float speed
        glowSpeed += Random.Range(-0.5f, 0.5f); // Randomize glow speed slightly
        floatSpeed += Random.Range(-0.2f, 0.2f); // Randomize float speed slightly
        floatAmplitude += Random.Range(-5f, 5f); // Randomize float amplitude slightly (adjust for UI scale)

        // Generate random time offsets to desynchronize the effects between planets
        randomGlowOffset = Random.Range(0f, 2f * Mathf.PI); // Random phase shift for glow
        randomFloatOffset = Random.Range(0f, 2f * Mathf.PI); // Random phase shift for float
    }

    void Update()
    {
        // Handle the glow effect with random offset
        HandleGlow();

        // Handle the floating/bouncing effect with random offset
        HandleBounce();
    }

    // Method to handle the glow effect
    void HandleGlow()
    {
        // Calculate brightness using Mathf.PingPong with a random offset to desynchronize glow timing
        float brightness = Mathf.Lerp(minBrightness, maxBrightness, Mathf.PingPong((Time.time + randomGlowOffset) * glowSpeed, 1));

        // Adjust the color's brightness without altering the alpha (transparency)
        planetImage.color = new Color(originalColor.r * brightness, originalColor.g * brightness, originalColor.b * brightness, originalColor.a);
    }

    // Method to handle the floating/bouncing effect
    void HandleBounce()
    {
        // Create a smooth bouncing effect using a sine wave with a random offset to desynchronize float timing
        float newY = startPos.y + Mathf.Sin((Time.time + randomFloatOffset) * floatSpeed) * floatAmplitude;

        // Apply the new Y position to the planet while keeping the X position the same
        rectTransform.anchoredPosition = new Vector3(startPos.x, newY, 0);
    }
}
