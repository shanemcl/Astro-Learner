using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public TextMeshProUGUI tokensText;
    public TextMeshProUGUI healthLevelText;
    public TextMeshProUGUI speedLevelText;
    public TextMeshProUGUI weaponsLevelText;

    private int healthLevel = 0;
    private float speedLevel = 0;
    private float weaponsLevel = 0;

    private void Start()
    {
        UpdateUI();
    }

    public void UpgradeHealth()
    {
        if (Player.Instance.GetTokens() > 0)
        {
            Player.Instance.UpgradeHealth(1);
            healthLevel++;
            Player.Instance.AddTokens(-1);
            UpdateUI();
        }
    }

    public void UpgradeSpeed()
    {
        if (Player.Instance.GetTokens() > 0)
        {
            Player.Instance.UpgradeMovementSpeed(0.5f);
            speedLevel += 0.5f;
            Player.Instance.AddTokens(-1);
            UpdateUI();
        }
    }

    public void UpgradeWeapons()
    {
        if (Player.Instance.GetTokens() > 0)
        {
            Player.Instance.UpgradeShootSpeed(0.1f);
            weaponsLevel += 0.1f;
            Player.Instance.AddTokens(-1);
            UpdateUI();
        }
    }

public void Refund()
{
    // Convert speedLevel and weaponsLevel to integers
    int speedTokensToRefund = Mathf.RoundToInt(speedLevel);
    int weaponsTokensToRefund = Mathf.RoundToInt(weaponsLevel);

    // Refund tokens for all upgrades
    int tokensToRefund = healthLevel + speedTokensToRefund + weaponsTokensToRefund;
    Player.Instance.AddTokens(tokensToRefund);

    // Reset the player's stats
    Player.Instance.RefundUpgrades(healthLevel, speedTokensToRefund, weaponsTokensToRefund);

    // Reset levels to 0
    healthLevel = 0;
    speedLevel = 0;
    weaponsLevel = 0;

    // Update the UI
    UpdateUI();
    Debug.Log($"Refund successful! Tokens refunded: {tokensToRefund}");
}

    private void UpdateUI()
    {
        tokensText.text = $"Tokens: {Player.Instance.GetTokens()}";
        healthLevelText.text = $"Health LVL: {healthLevel}";
        speedLevelText.text = $"Speed LVL: {speedLevel}";
        weaponsLevelText.text = $"Weapons LVL: {weaponsLevel}";
    }
}
