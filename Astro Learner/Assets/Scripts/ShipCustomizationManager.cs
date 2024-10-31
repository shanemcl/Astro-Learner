using UnityEngine;
using UnityEngine.UI;

public class ShipCustomizationManager : MonoBehaviour
{
    // References to UI preview panels
    public Image cockpitPreview;
    public Image bodyPreview;
    public Image weaponsPreview;
    public Image enginePreview;

    // Player prefab image reference
    public Image playerShipImage;

    // Selected parts
    private ShipPart selectedCockpit;
    private ShipPart selectedBody;
    private ShipPart selectedWeapons;
    private ShipPart selectedEngine;

    // Methods to set selected parts
    public void SelectCockpit(ShipPart part)
    {
        selectedCockpit = part;
        cockpitPreview.sprite = part.partSprite;  // Using the correct partSprite field
        UpdatePlayerShip();
    }

    public void SelectBody(ShipPart part)
    {
        selectedBody = part;
        bodyPreview.sprite = part.partSprite;  // Using the correct partSprite field
        UpdatePlayerShip();
    }

    public void SelectWeapons(ShipPart part)
    {
        selectedWeapons = part;
        weaponsPreview.sprite = part.partSprite;  // Using the correct partSprite field
        UpdatePlayerShip();
    }

    public void SelectEngine(ShipPart part)
    {
        selectedEngine = part;
        enginePreview.sprite = part.partSprite;  // Using the correct partSprite field
        UpdatePlayerShip();
    }

    // Update the player ship with selected parts
    private void UpdatePlayerShip()
    {
        if (selectedCockpit && selectedBody && selectedWeapons && selectedEngine)
        {
            // Combine all selected parts into a single sprite (using layering or parenting)
            Transform cockpit = playerShipImage.transform.Find("Cockpit");
            Transform body = playerShipImage.transform.Find("Body");
            Transform weapons = playerShipImage.transform.Find("Weapons");
            Transform engine = playerShipImage.transform.Find("Engine");

            cockpit.GetComponent<Image>().sprite = selectedCockpit.partSprite;
            body.GetComponent<Image>().sprite = selectedBody.partSprite;
            weapons.GetComponent<Image>().sprite = selectedWeapons.partSprite;
            engine.GetComponent<Image>().sprite = selectedEngine.partSprite;
        }
    }
}
