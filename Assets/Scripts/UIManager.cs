using UnityEngine;
using System.Collections;

[System.Serializable]
public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }
    
    // Reference to your touch controller canvas
    public Canvas touchControllerCanvas;

    public virtual void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Make sure canvas persists too
            if (touchControllerCanvas)
            {
                DontDestroyOnLoad(touchControllerCanvas.gameObject);
                // Ensure controls are shown by default in gameplay scenes
                touchControllerCanvas.enabled = true;
            }

            // Initialize any other UI elements here
            SetupPauseHandling();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupPauseHandling()
    {
        // Make sure we start unpaused
        Time.timeScale = 1;
        SceneLoader.isPaused = false;
    }

    // Function to toggle touch controller visibility
    public static void ShowTouchController(bool show)
    {
        if (instance && instance.touchControllerCanvas)
        {
            instance.touchControllerCanvas.enabled = show;
        }
    }

    void Update()
    {
        // Handle pause functionality
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (!SceneLoader.isPaused)
        {
            Time.timeScale = 0;
            SceneLoader.isPaused = true;
            SceneLoader.Pause();
        }
        else
        {
            Time.timeScale = 1;
            SceneLoader.isPaused = false;
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("PAUSE");
        }
    }
}