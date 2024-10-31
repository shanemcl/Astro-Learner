using UnityEngine;
using UnityEngine.UI;

public class ShipPartButton : MonoBehaviour
{
    public ShipPart shipPart;
    public ShipCustomizationManager customizationManager;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(SelectPart);
    }

    private void SelectPart()
    {
        // Determine which category this part belongs to and send it to the customization manager
        switch (shipPart.partName)
        {
            case "Cockpit":
                customizationManager.SelectCockpit(shipPart);
                break;
            case "Body":
                customizationManager.SelectBody(shipPart);
                break;
            case "Weapons":
                customizationManager.SelectWeapons(shipPart);
                break;
            case "Engine":
                customizationManager.SelectEngine(shipPart);
                break;
        }
    }
}
