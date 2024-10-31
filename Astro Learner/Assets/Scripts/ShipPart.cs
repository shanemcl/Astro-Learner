using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShipPart", menuName = "Ship/Part")]
public class ShipPart : ScriptableObject
{
    public string partName;  // Name of the part (e.g., Cockpit, Body, etc.)
    public Sprite partSprite;  // The actual sprite to display for this part
}
