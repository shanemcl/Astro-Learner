using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float fireRate = 1.0f; // Base fire rate

    private float nextFireTime;

    private Player player; // Reference to Player script

    private void Start()
    {
        Debug.Log($"PlayerShoot initialized with fire rate: {fireRate}");
        player = FindObjectOfType<Player>(); // Link to Player script
        if (player != null)
        {
            UpdateFireRate(player.GetShootSpeed()); // Sync fire rate with Player's shoot speed
        }
    }

    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

 private void FireBullet()
{
    if (bulletPrefab != null && bulletSpawnPoint != null)
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        
        Rigidbody2D bulletRigidbody = bulletInstance.GetComponent<Rigidbody2D>();
        if (bulletRigidbody != null)
        {
            bulletRigidbody.velocity = bulletSpawnPoint.up * 10f; // Adjust speed as needed
        }
        else
        {
            Debug.LogWarning("Bullet prefab does not have a Rigidbody2D component.");
        }

        Debug.Log("Bullet fired.");
    }
    else
    {
        Debug.LogWarning("Bullet prefab or spawn point is not assigned.");
    }
}

    public void UpdateFireRate(float newFireRate)
    {
        fireRate = Mathf.Max(0.1f, newFireRate);
        Debug.Log($"Fire rate updated: {fireRate}");
    }

    public float GetFireRate()
    {
        return fireRate;
    }

    public void ResetFireRate()
    {
        fireRate = 1.0f; // Reset to default fire rate
        Debug.Log("Fire rate reset to default.");
    }
}
