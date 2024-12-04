using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;

        // Optional: Change the fill color dynamically based on health percentage
        Image fillImage = slider.fillRect.GetComponent<Image>();
        if (fillImage != null)
        {
            float healthPercentage = (float)health / slider.maxValue;
            if (healthPercentage > 0.5f)
                fillImage.color = Color.Lerp(Color.yellow, Color.green, (healthPercentage - 0.5f) * 2);
            else
                fillImage.color = Color.Lerp(Color.red, Color.yellow, healthPercentage * 2);
        }
    }
}
