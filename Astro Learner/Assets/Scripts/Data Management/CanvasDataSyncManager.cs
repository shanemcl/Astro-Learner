using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class CanvasDataSyncManager : MonoBehaviour
{
    // Canvas API configuration
     private const string BASE_URL = "https://fsu.instructure.com/";
     private const string API_KEY = "10284~NaEXWCLCfuEwf9ZY7YYwKZ97AmtNGMU8N89GMyc7QeVmfy4GRT6YvzUzTF3J43Da";
     private const string COURSE_ID = "284766";
     private const string USER_ID = "225739";

    // Sync configuration
    public float syncInterval = 300f; // 5 minutes
    public int maxRetries = 3;
    public float retryDelay = 5f;

    // Offline data
    private const string OFFLINE_DATA_FILENAME = "canvas_offline_data.json";

    private CanvasData gameState = new CanvasData();

    [System.Serializable]
    public class CanvasData
    {
        public List<Assignment> assignments = new List<Assignment>();
        public List<Module> modules = new List<Module>();
        public List<Quiz> quizzes = new List<Quiz>();
        public Dictionary<int, Grade> grades = new Dictionary<int, Grade>();
    }

    [System.Serializable]
    public class Assignment
    {
        public int id;
        public string name;
        public string description;
        public DateTime? due_at;
        public float points_possible;
    }

    [System.Serializable]
    public class Module
    {
        public int id;
        public string name;
        public List<ModuleItem> items = new List<ModuleItem>();
    }

    [System.Serializable]
    public class ModuleItem
    {
        public int id;
        public string title;
        public string type;
        public int content_id;
    }

    [System.Serializable]
    public class Quiz
    {
        public int id;
        public string title;
        public string quiz_type;
        public int time_limit;
        public int allowed_attempts;
        public float points_possible;
    }

    [System.Serializable]
    public class Grade
    {
        public float? score;
        public string grade;
        public bool submitted;
        public DateTime? submitted_at;
        public string workflow_state;
        public string late_policy_status;
    }

    private void Start()
    {
        StartCoroutine(SyncRoutine());
    }

    private IEnumerator SyncRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(SyncAllData());
            yield return new WaitForSeconds(syncInterval);
        }
    }

    private IEnumerator SyncAllData()
    {
        Debug.Log("Starting Canvas Data Sync");
        Debug.Log($"Base URL: {BASE_URL}");
        Debug.Log($"Course ID: {COURSE_ID}");
        Debug.Log($"User ID: {USER_ID}");

        if (CheckInternetConnection())
        {
            CanvasData updatedGameState = new CanvasData();

            yield return StartCoroutine(FetchAssignments(updatedGameState));
            yield return StartCoroutine(FetchModules(updatedGameState));
            yield return StartCoroutine(FetchQuizzes(updatedGameState));
            yield return StartCoroutine(FetchGrades(updatedGameState));

            gameState = updatedGameState;
            SaveOfflineData(gameState);
            Debug.Log("Canvas Data Sync Completed and Saved Offline");
        }
        else
        {
            Debug.Log("No internet connection. Loading offline data.");
            LoadOfflineData();
        }
    }

    private IEnumerator FetchAssignments(CanvasData updatedGameState)
    {
        Debug.Log("\nFetching Assignments:");
        string url = $"{BASE_URL}/api/v1/courses/{COURSE_ID}/assignments";
        yield return StartCoroutine(FetchDataWithRetry<List<Assignment>>(url, (assignments) =>
        {
            if (assignments != null)
            {
                updatedGameState.assignments = assignments;
                foreach (var assignment in assignments)
                {
                    Debug.Log($"- {assignment.name} (ID: {assignment.id})");
                }
            }
            else
            {
                Debug.Log("Failed to fetch assignments.");
            }
        }));
    }

    private IEnumerator FetchModules(CanvasData updatedGameState)
    {
        Debug.Log("\nFetching Modules:");
        string url = $"{BASE_URL}/api/v1/courses/{COURSE_ID}/modules?include[]=items";
        yield return StartCoroutine(FetchDataWithRetry<List<Module>>(url, (modules) =>
        {
            if (modules != null)
            {
                updatedGameState.modules = modules;
                foreach (var module in modules)
                {
                    Debug.Log($"- {module.name} (ID: {module.id})");
                    foreach (var item in module.items)
                    {
                        Debug.Log($"  - {item.title} (Type: {item.type})");
                    }
                }
            }
            else
            {
                Debug.Log("Failed to fetch modules.");
            }
        }));
    }

    private IEnumerator FetchQuizzes(CanvasData updatedGameState)
    {
        Debug.Log("\nFetching Quizzes:");
        string url = $"{BASE_URL}/api/v1/courses/{COURSE_ID}/quizzes";
        yield return StartCoroutine(FetchDataWithRetry<List<Quiz>>(url, (quizzes) =>
        {
            if (quizzes != null)
            {
                updatedGameState.quizzes = quizzes;
                foreach (var quiz in quizzes)
                {
                    Debug.Log($"- {quiz.title} (ID: {quiz.id})");
                }
            }
            else
            {
                Debug.Log("Failed to fetch quizzes.");
            }
        }));
    }

    private IEnumerator FetchGrades(CanvasData updatedGameState)
{
    Debug.Log("\nFetching Grades:");
    string url = $"{BASE_URL}/api/v1/courses/{COURSE_ID}/students/submissions?" +
                 $"student_ids[]={USER_ID}" +
                 "&include[]=assignment" +
                 "&include[]=submission" +
                 "&include[]=total_scores" +
                 "&order=graded_at" +
                 "&per_page=100";

    yield return StartCoroutine(FetchDataWithRetry<List<dynamic>>(url, (submissions) =>
    {
        if (submissions != null)
        {
            updatedGameState.grades = new Dictionary<int, Grade>();
            Debug.Log($"Found {submissions.Count} submissions");

            foreach (var submission in submissions)
            {
                try 
                {
                    int assignmentId = submission.assignment_id;
                    string assignmentName = submission.assignment.name;
                    
                    updatedGameState.grades[assignmentId] = new Grade
                    {
                        score = submission.score,
                        grade = submission.grade,
                        submitted = submission.workflow_state != "unsubmitted",
                        submitted_at = submission.submitted_at,
                        workflow_state = submission.workflow_state,
                        late_policy_status = submission.late_policy_status
                    };

                    Debug.Log($"- {assignmentName}:");
                    Debug.Log($"    Score: {submission.score}");
                    Debug.Log($"    Grade: {submission.grade}");
                    Debug.Log($"    Status: {submission.workflow_state}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error processing submission: {e.Message}");
                }
            }
        }
        else
        {
            Debug.Log("Failed to fetch grades.");
        }
        
    }));
if (CanvasTokenTracker.Instance != null)
    {
        CanvasTokenTracker.Instance.CheckForNewCompletions();
    }
}

    private IEnumerator FetchDataWithRetry<T>(string url, Action<T> callback)
    {
        int retries = 0;
        bool success = false;

        while (!success && retries < maxRetries)
        {
            yield return StartCoroutine(FetchData<T>(url, (result, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    callback(result);
                    success = true;
                }
                else
                {
                    Debug.LogWarning($"Attempt {retries + 1} failed: {error}");
                    retries++;
                }
            }));

            if (!success && retries < maxRetries)
            {
                yield return new WaitForSeconds(retryDelay);
            }
        }

        if (!success)
        {
            Debug.LogError($"Failed to fetch data after {maxRetries} attempts.");
            callback(default);
        }
    }

    private IEnumerator FetchData<T>(string url, Action<T, string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Authorization", $"Bearer {API_KEY}");
            Debug.Log($"Requesting: {url}");

            yield return webRequest.SendWebRequest();

            Debug.Log($"Response Status: {webRequest.responseCode}");

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = webRequest.downloadHandler.text;
                T result = JsonConvert.DeserializeObject<T>(jsonResult);
                callback(result, null);
            }
            else
            {
                Debug.LogError($"Error fetching data from {url}: {webRequest.error}");
                callback(default, webRequest.error);
            }
        }
    }

    private bool CheckInternetConnection()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    private void SaveOfflineData(CanvasData data)
    {
        string json = JsonConvert.SerializeObject(data);
        string path = Path.Combine(Application.persistentDataPath, OFFLINE_DATA_FILENAME);
        File.WriteAllText(path, json);
        Debug.Log("Offline data saved.");
    }

    private void LoadOfflineData()
    {
        string path = Path.Combine(Application.persistentDataPath, OFFLINE_DATA_FILENAME);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            gameState = JsonConvert.DeserializeObject<CanvasData>(json);
            Debug.Log("Offline data loaded.");
        }
        else
        {
            Debug.LogWarning("No offline data available.");
        }
    }

    public CanvasData GetGameState()
    {
        return gameState;
    }

    public void ForceSyncNow()
    {
        StartCoroutine(SyncAllData());
    }

    public bool IsOffline()
    {
        return !CheckInternetConnection();
    }
}
