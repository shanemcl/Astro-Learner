using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class RandomizedBackgroundGlow : MonoBehaviour
{
    public float baseGlowSpeed = 1f;       // Base speed for the glow effect
    public float baseGlowIntensity = 0.2f; // Base intensity for the glow effect
    public float glowSpeedVariance = 0.5f; // How much to randomize the speed (+/-)
    public float glowIntensityVariance = 0.1f; // How much to randomize the intensity (+/-)
    
    private SpriteRenderer spriteRenderer;  // Reference to the SpriteRenderer component
    private Color originalColor;            // Store the original color of the background
    private bool isBrightening = true;      // Flag to control brightening and dimming

    void Start()
    {
        // Get the SpriteRenderer component attached to the background
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Store the original color of the background sprite
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        // Randomize the glow speed and intensity slightly
        float randomGlowSpeed = baseGlowSpeed + Random.Range(-glowSpeedVariance, glowSpeedVariance);
        float randomGlowIntensity = baseGlowIntensity + Random.Range(-glowIntensityVariance, glowIntensityVariance);

        // Get the current color of the background sprite
        Color currentColor = spriteRenderer.color;

        // Control whether we are brightening or dimming the background
        if (isBrightening)
        {
            // Gradually brighten the background by increasing RGB values
            currentColor.r += randomGlowIntensity * randomGlowSpeed * Time.deltaTime;
            currentColor.g += randomGlowIntensity * randomGlowSpeed * Time.deltaTime;
            currentColor.b += randomGlowIntensity * randomGlowSpeed * Time.deltaTime;

            // Cap the brightness so it doesn't exceed the limit
            if (currentColor.r >= originalColor.r + baseGlowIntensity)
            {
                isBrightening = false; // Switch to dimming once the max brightness is reached
            }
        }
        else
        {
            // Gradually dim the background by decreasing RGB values
            currentColor.r -= randomGlowIntensity * randomGlowSpeed * Time.deltaTime;
            currentColor.g -= randomGlowIntensity * randomGlowSpeed * Time.deltaTime;
            currentColor.b -= randomGlowIntensity * randomGlowSpeed * Time.deltaTime;

            // Stop dimming when the original color is restored
            if (currentColor.r <= originalColor.r)
            {
                isBrightening = true;  // Switch to brightening
                currentColor = originalColor; // Reset to the original color exactly
            }
        }

        // Apply the modified color back to the SpriteRenderer
        spriteRenderer.color = currentColor;
    }
}