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
            
            DontDestroyOnLoad(touchControllerCanvas.gameObject);

            // Subscribe to scene changes
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (touchControllerCanvas == null) return;

        if (SceneLoader.IsCurrentSceneNonInteractive) {
            touchControllerCanvas.enabled = false;
        }
        else {
            touchControllerCanvas.enabled = true;
        }
    }

    void OnDestroy() {
        if (Instance == this) {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        Debug.Log($"[Pause] Current scene: {SceneLoader.currentScene}, isPaused: {SceneLoader.isPaused}");

        if (SceneLoader.IsCurrentSceneNonInteractive) {
            Debug.Log("[Pause] Ignoring pause toggle in non-interactive scene");
            return;
        }

        if (!SceneLoader.isPaused) {
            // Pause the game
            Debug.Log("[Pause] Pausing game and loading PAUSE scene");
            Time.timeScale = 0;
            SceneLoader.isPaused = true;
            SceneLoader.Pause();
        }
        else {
            // Unpause the game
            Debug.Log("[Pause] Unpausing game and unloading PAUSE scene");
            Time.timeScale = 1;
            SceneLoader.isPaused = false;
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("PAUSE");
        }
    }
}