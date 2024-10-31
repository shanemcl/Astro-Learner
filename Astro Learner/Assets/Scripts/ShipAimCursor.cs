using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAimCursor : MonoBehaviour
{
    private Transform m_transform;
    private void Start()
    {
        m_transform = this.transform;
    }

    private void CursorAim()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        m_transform.rotation = rotation;
    }

    // Update is called once per frame
    void Update()
    {
        CursorAim();
    }
}
