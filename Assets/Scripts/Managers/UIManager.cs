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
    Debug.Log($"[UIManager] Awake called, parent: {transform.parent?.name ?? "null"}");
    if (Instance == null)
    {
      Debug.Log("[UIManager] Setting instance and DontDestroyOnLoad");
      Instance = this;
      DontDestroyOnLoad(gameObject);

      // Find canvas if not assigned
      if (gameUICanvas == null)
      {
        Debug.Log("[UIManager] Looking for Canvas in children");
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
      Debug.Log("[UIManager] Instance already exists, destroying this object");
      Destroy(gameObject);
    }
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    if (gameUICanvas == null) return;

    Debug.Log($"[UIManager] Scene loaded: {scene.name}");

    if (SceneLoader.Instance.IsCurrentSceneNonInteractive)
    {
      gameUICanvas.enabled = false;
      Debug.Log("[UIManager] Disabling canvas for non-interactive scene");
    }
    else
    {
      gameUICanvas.enabled = true;
      Debug.Log("[UIManager] Enabling canvas for interactive scene");
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
    Debug.Log($"[Pause] Current scene: {SceneLoader.Instance.currentScene}, isPaused: {SceneLoader.Instance.isPaused}");

    if (SceneLoader.Instance.IsCurrentSceneNonInteractive)
    {
      Debug.Log("[Pause] Ignoring pause toggle in non-interactive scene");
      return;
    }

    if (!SceneLoader.Instance.isPaused)
    {
      // Pause the game
      Debug.Log("[Pause] Pausing game and loading PAUSE scene");
      Time.timeScale = 0;
      SceneLoader.Instance.isPaused = true;
      SceneLoader.Instance.Pause();
    }
    else
    {
      // Unpause the game
      Debug.Log("[Pause] Unpausing game and unloading PAUSE scene");
      Time.timeScale = 1;
      SceneLoader.Instance.isPaused = false;
      UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("PAUSE");
    }
  }
}