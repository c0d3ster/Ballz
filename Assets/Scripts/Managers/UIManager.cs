using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
  public static UIManager Instance { get; private set; }
  [Header("UI References")]
  public Canvas gameUICanvas;

  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);

      // Find canvas if not assigned
      if (gameUICanvas == null)
      {
        gameUICanvas = GetComponentInChildren<Canvas>();
        if (gameUICanvas == null)
        {
          Debug.LogError("No Canvas found in UIManager prefab!");
          return;
        }
      }

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
    if (gameUICanvas == null) return;

    if (SceneLoader.Instance.IsCurrentSceneNonInteractive)
    {
      gameUICanvas.enabled = false;
    }
    else
    {
      gameUICanvas.enabled = true;
    }
  }

  void OnDestroy()
  {
    if (Instance == this)
    {
      SceneManager.sceneLoaded -= OnSceneLoaded;
    }
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
    if (SceneLoader.Instance.IsCurrentSceneNonInteractive)
    {
      return;
    }

    if (!SceneLoader.Instance.isPaused)
    {
      // Pause the game
      Time.timeScale = 0;
      SceneLoader.Instance.isPaused = true;
      SceneLoader.Instance.Pause();

      // Pause the timer
      if (TimerManager.Instance != null)
      {
        TimerManager.Instance.PauseTimer();
      }
    }
    else
    {
      // Unpause the game
      Time.timeScale = 1;
      SceneLoader.Instance.isPaused = false;
      UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("PAUSE");

      // Resume the timer
      if (TimerManager.Instance != null)
      {
        TimerManager.Instance.ResumeTimer();
      }
    }
  }
}