using UnityEngine;
using System.Collections.Generic;
using System;

public class CanvasTokenTracker : MonoBehaviour 
{
    public static CanvasTokenTracker Instance { get; private set; }
    
    public CanvasDataSyncManager canvasManager;
    private HashSet<int> processedAssignments = new HashSet<int>();
    private int tokens = 0;
    private const string TOKENS_SAVE_KEY = "canvas_tokens";
    private const string PROCESSED_ASSIGNMENTS_KEY = "processed_assignments";

    // Event that other scripts can subscribe to for token changes
    public event Action<int> OnTokensChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadTokenData();
    }

    public void CheckForNewCompletions()
    {
        var gameState = canvasManager.GetGameState();
        bool foundNewCompletion = false;

        // Check through all grades
        foreach (var grade in gameState.grades)
        {
            // If we haven't processed this completed assignment yet
            if (!processedAssignments.Contains(grade.Key) && 
                grade.Value.submitted && 
                grade.Value.workflow_state == "graded")
            {
                // Add it to our processed list
                processedAssignments.Add(grade.Key);
                // Award a token
                tokens++;
                foundNewCompletion = true;

                Debug.Log($"New assignment completed! Token awarded. Total tokens: {tokens}");
                OnTokensChanged?.Invoke(tokens);
            }
        }

        if (foundNewCompletion)
        {
            SaveTokenData();
        }
    }

    private void SaveTokenData()
    {
        PlayerPrefs.SetInt(TOKENS_SAVE_KEY, tokens);
        string processedData = JsonUtility.ToJson(new SerializableIntSet(processedAssignments));
        PlayerPrefs.SetString(PROCESSED_ASSIGNMENTS_KEY, processedData);
        PlayerPrefs.Save();
    }

    public void LoadTokenData()
    {
        tokens = PlayerPrefs.GetInt(TOKENS_SAVE_KEY, 0);
        string processedData = PlayerPrefs.GetString(PROCESSED_ASSIGNMENTS_KEY, "");
        if (!string.IsNullOrEmpty(processedData))
        {
            var loadedSet = JsonUtility.FromJson<SerializableIntSet>(processedData);
            processedAssignments = new HashSet<int>(loadedSet.numbers);
        }
        OnTokensChanged?.Invoke(tokens);

         Debug.Log("LoadTokenDataCompleted");
    }

    // Public methods to access and modify tokens
    public int GetTokens()
    {
        return tokens;
    }

    public bool SpendTokens(int amount)
    {
        if (tokens >= amount)
        {
            tokens -= amount;
            SaveTokenData();
            OnTokensChanged?.Invoke(tokens);
            return true;
        }
        return false;
    }

    public void AddTokens(int amount)
    {
        tokens += amount;
        SaveTokenData();
        OnTokensChanged?.Invoke(tokens);
    }

    // Helper class for serializing HashSet<int>
    [System.Serializable]
    private class SerializableIntSet
    {
        public List<int> numbers;

        public SerializableIntSet(HashSet<int> set)
        {
            numbers = new List<int>(set);
        }
    }
}
