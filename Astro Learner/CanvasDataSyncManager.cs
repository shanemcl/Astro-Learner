using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CanvasDataSyncManager : MonoBehaviour
{
    // Canvas API configuration
    private const string BASE_URL = "https://fsu.instructure.com";
    private const string API_KEY = "YOUR_API_KEY_HERE"; //ask me if you need help setting this instance up with api key,course id, and user id, for testing and I can help (probs takes 5 mins)
    private const string COURSE_ID = "YOUR_COURSE_ID_HERE";
    private const string USER_ID = "YOUR_USER_ID_HERE";

    // Sync configuration
    public float syncInterval = 300f; // 5 minutes
    public int maxRetries = 3;
    public float retryDelay = 5f;

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

        yield return StartCoroutine(FetchAssignments());
        yield return StartCoroutine(FetchModules());
        yield return StartCoroutine(FetchQuizzes());
        yield return StartCoroutine(FetchGrades());

        Debug.Log("Canvas Data Sync Completed");
    }

    private IEnumerator FetchAssignments()
    {
        Debug.Log("\nFetching Assignments:");
        string url = $"{BASE_URL}/api/v1/courses/{COURSE_ID}/assignments";
        yield return StartCoroutine(FetchData<List<Assignment>>(url, (assignments) =>
        {
            if (assignments != null)
            {
                gameState.assignments = assignments;
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

    private IEnumerator FetchModules()
    {
        Debug.Log("\nFetching Modules:");
        string url = $"{BASE_URL}/api/v1/courses/{COURSE_ID}/modules?include[]=items";
        yield return StartCoroutine(FetchData<List<Module>>(url, (modules) =>
        {
            if (modules != null)
            {
                gameState.modules = modules;
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

    private IEnumerator FetchQuizzes()
    {
        Debug.Log("\nFetching Quizzes:");
        string url = $"{BASE_URL}/api/v1/courses/{COURSE_ID}/quizzes";
        yield return StartCoroutine(FetchData<List<Quiz>>(url, (quizzes) =>
        {
            if (quizzes != null)
            {
                gameState.quizzes = quizzes;
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

    private IEnumerator FetchGrades()
    {
        Debug.Log("\nFetching Grades:");
        string url = $"{BASE_URL}/api/v1/courses/{COURSE_ID}/students/submissions?student_ids[]={USER_ID}&include[]=assignment";
        yield return StartCoroutine(FetchData<List<dynamic>>(url, (submissions) =>
        {
            if (submissions != null)
            {
                gameState.grades.Clear();
                foreach (var submission in submissions)
                {
                    int assignmentId = submission.assignment_id;
                    gameState.grades[assignmentId] = new Grade
                    {
                        score = submission.score,
                        grade = submission.grade,
                        submitted = submission.workflow_state != "unsubmitted",
                        submitted_at = submission.submitted_at,
                        workflow_state = submission.workflow_state,
                        late_policy_status = submission.late_policy_status
                    };
                    Debug.Log($"- {submission.assignment.name}: Score: {submission.score}, Grade: {submission.grade}");
                }
            }
            else
            {
                Debug.Log("Failed to fetch grades.");
            }
        }));
    }

    private IEnumerator FetchData<T>(string url, Action<T> callback)
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
                callback(result);
            }
            else
            {
                Debug.LogError($"Error fetching data from {url}: {webRequest.error}");
                Debug.LogError($"Response content: {webRequest.downloadHandler.text}");
                callback(default);
            }
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
}
