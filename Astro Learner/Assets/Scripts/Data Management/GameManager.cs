using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int totalEnemies = 10;
    public int enemiesDefeated = 0;

    [Header("UI Settings")]
    public GameObject gameOverUI;
    public GameObject victoryUI;

    [Header("Scene Transition Settings")]
    [SerializeField] private string galaxyMapSceneName = "GalaxyMap"; // Name of the Galaxy Map scene

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        // Check if the player presses Enter while a UI is active
        if ((gameOverUI.activeSelf || victoryUI.activeSelf) && Input.GetKeyDown(KeyCode.Return))
        {
            ProceedToGalaxyMap();
        }
    }

    public void EnemyDefeated()
    {
        enemiesDefeated++;
        if (enemiesDefeated >= totalEnemies)
        {
            TriggerVictory();
        }
    }

    public void TriggerGameOver()
    {
        // Pause the game and display the Game Over UI
        Time.timeScale = 0f;
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    public void TriggerVictory()
    {
        // Pause the game and display the Victory UI
        Time.timeScale = 0f;
        if (victoryUI != null)
        {
            victoryUI.SetActive(true);
        }
    }

    public void ProceedToGalaxyMap()
    {
        // Unpause the game and load the Galaxy Map scene
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(galaxyMapSceneName))
        {
            SceneManager.LoadScene(galaxyMapSceneName);
        }
        else
        {
            Debug.LogWarning("Galaxy Map scene name is not set!");
        }
    }
}
