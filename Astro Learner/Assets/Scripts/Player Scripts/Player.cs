using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private HealthBar healthBar;

    [Header("Player Stats")]
    [SerializeField] private int tokens = 0;
    [SerializeField] private int healthLevel = 0;
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float shootSpeed = 1.0f;
    [SerializeField] private int currentHealth;

    [Header("Hit Effects")]
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private float invincibilityDuration = 2.0f;

    private bool isInvincible;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        Debug.Log($"Player took damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            TriggerGameOver();
        }
        else
        {
            TriggerHitEffects();
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        Debug.Log($"Player healed. Current health: {currentHealth}");
    }

    private void TriggerHitEffects()
    {
        if (hitEffect != null)
    {
        hitEffect.transform.position = transform.position;
        hitEffect.Play();
        Debug.Log("Hit effect triggered.");
    }

    if (cameraShake != null)
    {
        Debug.Log("Triggering camera shake.");
        cameraShake.Shake(0.2f, 0.3f); // Ensure duration and magnitude are correct.
    }
    else
    {
        Debug.LogWarning("CameraShake reference is null!");
    }
    }

    private void TriggerGameOver()
    {
        Debug.Log("Player is defeated. Game Over.");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float blinkInterval = 0.2f;
        float elapsedTime = 0;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        while (elapsedTime < invincibilityDuration)
        {
            if (renderer != null)
            {
                renderer.enabled = !renderer.enabled;
            }
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        if (renderer != null)
        {
            renderer.enabled = true;
        }
        isInvincible = false;
    }

    public void UpgradeHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    public void UpgradeMovementSpeed(float amount)
    {
        movementSpeed += amount;
        ShipSmoothMovement shipMovement = GetComponent<ShipSmoothMovement>();
        if (shipMovement != null)
        {
            shipMovement.UpdateSpeed(movementSpeed);
        }
    }

    public void UpgradeShootSpeed(float amount)
    {
        shootSpeed += amount;
        PlayerShoot playerShoot = GetComponent<PlayerShoot>();
        if (playerShoot != null)
        {
            playerShoot.UpdateFireRate(shootSpeed);
        }
    }

    public int GetTokens() => tokens;

    public void AddTokens(int amount)
    {
        tokens += amount;
        Debug.Log($"Tokens added. Current tokens: {tokens}");
    }

    public void SpendTokens(int amount)
    {
        if (tokens >= amount)
        {
            tokens -= amount;
            Debug.Log($"Tokens spent. Current tokens: {tokens}");
        }
        else
        {
            Debug.LogWarning("Not enough tokens to spend.");
        }
    }

    public void ResetStats()
    {
        maxHealth = 5;
        movementSpeed = 5.0f;
        shootSpeed = 1.0f;
        currentHealth = maxHealth;
        tokens = 0;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }

        Debug.Log("Player stats reset to default.");
    }

    // Movement speed retrieval
public float GetMovementSpeed()
{
    return movementSpeed;
}

// Shoot speed retrieval
public float GetShootSpeed()
{
    return shootSpeed;
}

  public void RefundUpgrades(int healthTokens, int movementTokens, int shootTokens)
{
    // Add the tokens back
    tokens += healthTokens + movementTokens + shootTokens;

    // Reset stats to their default values
    maxHealth = 5;
    currentHealth = maxHealth;
    movementSpeed = 5.0f;
    shootSpeed = 1.0f;

    // Update the health bar UI
    if (healthBar != null)
    {
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
    }

    // Update the movement and shooting components
    ShipSmoothMovement shipMovement = GetComponent<ShipSmoothMovement>();
    if (shipMovement != null)
    {
        shipMovement.UpdateSpeed(movementSpeed);
    }

    PlayerShoot playerShoot = GetComponent<PlayerShoot>();
    if (playerShoot != null)
    {
        playerShoot.UpdateFireRate(shootSpeed);
    }

    // Log the results
    Debug.Log($"Refund complete. Tokens refunded: {healthTokens + movementTokens + shootTokens}. Current tokens: {tokens}");
    Debug.Log($"Stats reset: Max Health = {maxHealth}, Movement Speed = {movementSpeed}, Shoot Speed = {shootSpeed}");
}

}
