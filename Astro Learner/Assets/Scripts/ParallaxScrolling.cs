using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    public float scrollSpeed = 2.0f; // Speed of background scrolling
    public float resetPositionY = -10f; // Y position where the background should reset
    public float startPositionY = 10f; // Starting Y position after reset

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Move the background down
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        // If the background moves below the reset position, move it back to the start
        if (transform.position.y <= resetPositionY)
        {
            transform.position = new Vector3(startPosition.x, startPositionY, startPosition.z);
        }
    }
}
