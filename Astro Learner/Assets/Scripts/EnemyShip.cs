using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public class EnemyShip : MonoBehaviour
{
    public float speed = 3f;               // Speed of the enemy ship
    public int health = 1;                 // Health of the enemy ship
    public float firingRate = 1f;          // Time between shots
    public GameObject projectilePrefab;     // Prefab for the projectile
    public MovementType movementType;       // Selected movement type
    private float lastFiredTime;

    private float zigzagFrequency = 2f;    // Frequency for zigzag movement
    private float zigzagAmplitude = 1f;     // Amplitude for zigzag movement
    private float circularRadius = 1f;      // Radius for circular movement
    private float angle = 0f;               // Angle for circular movement

    void Update()
    {
        Move();
        CheckFire();
    }

    void Move()
    {
        switch (movementType)
        {
            case MovementType.StraightDown:
                transform.Translate(Vector3.down * speed * Time.deltaTime);
                break;

            case MovementType.ZigZag:
                float zigzagOffset = Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;
                transform.Translate(new Vector3(zigzagOffset, -speed * Time.deltaTime, 0));
                break;

            case MovementType.Circular:
                angle += speed * Time.deltaTime; // Increase angle based on speed
                float x = Mathf.Cos(angle) * circularRadius;
                float y = Mathf.Sin(angle) * circularRadius - speed * Time.deltaTime; // Move down
                transform.position = new Vector3(x, transform.position.y + y, 0);
                break;

            case MovementType.Sinusoidal:
                float sinusoidalY = Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;
                transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
                transform.position = new Vector3(transform.position.x, transform.position.y + sinusoidalY, 0);
                break;
        }

        // Check if off-screen
        if (transform.position.y < -6f) // Adjust according to your screen height
        {
            Destroy(gameObject); // Destroy the ship if it goes off-screen
        }
    }

    void CheckFire()
    {
        if (Time.time - lastFiredTime >= firingRate)
        {
            FireProjectile();
            lastFiredTime = Time.time;
        }
    }

    void FireProjectile() //Needs separate class
    {
        // Instantiate a new projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        // You can set projectile's speed or other properties here if needed
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject); // Destroy the ship if health is zero
        }
    }
}