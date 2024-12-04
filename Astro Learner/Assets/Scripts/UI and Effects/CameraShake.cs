using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    public float defaultShakeDuration = 0.2f;
    public float defaultShakeMagnitude = 0.1f;
    public float dampingSpeed = 1.0f;

    private Vector3 initialPosition;
    private float currentShakeDuration = 0f;
    private float shakeMagnitude;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if (currentShakeDuration > 0)
    {
        transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
        currentShakeDuration -= Time.deltaTime * dampingSpeed;
    }
    else
    {
        if (currentShakeDuration != 0f)
        {
            Debug.Log("Shaking ended. Resetting camera position.");
        }
        currentShakeDuration = 0f;
        transform.localPosition = initialPosition;
    }
    }

    public void Shake(float duration = -1, float magnitude = -1)
    {
      currentShakeDuration = duration > 0 ? duration : defaultShakeDuration;
    shakeMagnitude = magnitude > 0 ? magnitude : defaultShakeMagnitude;

    Debug.Log($"Shake triggered. Duration: {currentShakeDuration}, Magnitude: {shakeMagnitude}");
    }
}
