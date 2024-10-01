using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor; // Required to use SceneAsset in the Inspector

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneAsset sceneToLoad; // Allows you to drag the scene in the Inspector

    // This method is called when the button is clicked
    public void LoadScene()
    {
        if (sceneToLoad != null)
        {
            // Load the scene by its name
            SceneManager.LoadScene(sceneToLoad.name);
        }
        else
        {
            Debug.LogError("No scene assigned! Please drag a scene into the inspector.");
        }
    }
}
