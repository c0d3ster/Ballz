using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Canvas touchControllerCanvas;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Find canvas if not assigned
            if (touchControllerCanvas == null)
            {
                touchControllerCanvas = GetComponentInChildren<Canvas>();
                if (touchControllerCanvas == null)
                {
                    Debug.LogError("No Canvas found in UIManager prefab!");
                    return;
                }
            }

            // Make canvas a root object if it isn't already
            if (touchControllerCanvas.transform.parent != null)
            {
                // Store the local position and scale before unparenting
                Vector3 localPos = touchControllerCanvas.transform.localPosition;
                Vector3 localScale = touchControllerCanvas.transform.localScale;
                
                touchControllerCanvas.transform.SetParent(null);
                
                // Restore the local position and scale
                touchControllerCanvas.transform.localPosition = localPos;
                touchControllerCanvas.transform.localScale = localScale;
            }
            
            DontDestroyOnLoad(touchControllerCanvas.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (touchControllerCanvas == null) return;

        if (scene.name == "GAME OVER" || scene.name == "WIN" || scene.name == "SPLASH")
        {
            touchControllerCanvas.enabled = false;
        }
        else if (scene.name == "Active Main Menu" || scene.name.Contains("Ball"))
        {
            touchControllerCanvas.enabled = true;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public static void ShowTouchController(bool show)
    {
        if (Instance == null)
        {
            Debug.LogWarning("No UIManager instance exists!");
            return;
        }

        if (Instance.touchControllerCanvas == null)
        {
            Debug.LogWarning("Canvas reference is missing!");
            return;
        }

        Instance.touchControllerCanvas.enabled = show;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        // If we're in a scene where pause isn't allowed, do nothing
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[Pause] Current scene: {currentScene}, isPaused: {SceneLoader.isPaused}");

        if (currentScene == "Splash Screen" || currentScene == "GAME OVER" || currentScene == "WIN")
        {
            Debug.Log("[Pause] Ignoring pause toggle in non-gameplay scene");
            return;
        }

        // Check if PAUSE scene is already loaded
        bool pauseSceneLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == "PAUSE")
            {
                pauseSceneLoaded = true;
                break;
            }
        }
        Debug.Log($"[Pause] PAUSE scene is {(pauseSceneLoaded ? "already loaded" : "not loaded")}");

        if (!SceneLoader.isPaused && !pauseSceneLoaded)
        {
            // Pause the game
            Debug.Log("[Pause] Pausing game and loading PAUSE scene");
            Time.timeScale = 0;
            SceneLoader.isPaused = true;
            SceneLoader.Pause();
        }
        else if (SceneLoader.isPaused && pauseSceneLoaded)
        {
            // Unpause the game
            Debug.Log("[Pause] Unpausing game and unloading PAUSE scene");
            Time.timeScale = 1;
            SceneLoader.isPaused = false;
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("PAUSE");
        }
        else
        {
            Debug.LogWarning($"[Pause] Inconsistent state - isPaused: {SceneLoader.isPaused}, pauseSceneLoaded: {pauseSceneLoaded}");
        }
    }
}