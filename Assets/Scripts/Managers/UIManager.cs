using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
  public static UIManager Instance { get; private set; }
  public Canvas touchControllerCanvas;

  [Header("Lives Display")]
  [SerializeField] private Transform livesContainer;

  private LivesDisplay livesDisplay;

  void Awake()
  {
    Debug.Log($"[UIManager] Awake called, parent: {transform.parent?.name ?? "null"}");
    if (Instance == null)
    {
      Debug.Log("[UIManager] Setting instance and DontDestroyOnLoad");
      Instance = this;
      DontDestroyOnLoad(gameObject);

      // Find canvas if not assigned
      if (touchControllerCanvas == null)
      {
        Debug.Log("[UIManager] Looking for Canvas in children");
        touchControllerCanvas = GetComponentInChildren<Canvas>();
        if (touchControllerCanvas == null)
        {
          Debug.LogError("No Canvas found in UIManager prefab!");
          return;
        }
      }

      // Setup lives display
      SetupLivesDisplay();

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
    if (touchControllerCanvas == null) return;

    Debug.Log($"[UIManager] Scene loaded: {scene.name}");

    if (SceneLoader.IsCurrentSceneNonInteractive)
    {
      touchControllerCanvas.enabled = false;
      Debug.Log("[UIManager] Disabling canvas for non-interactive scene");
    }
    else
    {
      touchControllerCanvas.enabled = true;

      // Hide lives display on main menu
      if (scene.name == "Active Main Menu")
      {
        if (livesContainer != null)
        {
          livesContainer.gameObject.SetActive(false);
          Debug.Log("[UIManager] Hiding lives display on main menu");
        }
      }
      else
      {
        if (livesContainer != null)
        {
          livesContainer.gameObject.SetActive(true);
          Debug.Log("[UIManager] Showing lives display on game scene");
        }
      }
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

  private void SetupLivesDisplay()
  {
    // Create lives container if it doesn't exist
    if (livesContainer == null)
    {
      GameObject containerObj = new GameObject("LivesContainer");
      containerObj.transform.SetParent(touchControllerCanvas.transform);

      RectTransform rectTransform = containerObj.AddComponent<RectTransform>();
      rectTransform.anchorMin = new Vector2(0, 1); // Top left
      rectTransform.anchorMax = new Vector2(0, 1);
      rectTransform.pivot = new Vector2(0, 1);
      // Position below CountText: CountText is at y: -20, size 100, so we start at y: -130
      rectTransform.anchoredPosition = new Vector2(20, -130);

      livesContainer = containerObj.transform;
      Debug.Log("[UIManager] Created LivesContainer at position (20, -130)");
    }

    // Add LivesDisplay component
    livesDisplay = gameObject.AddComponent<LivesDisplay>();
    Debug.Log("[UIManager] Added LivesDisplay component");
  }

  public void TogglePause()
  {
    Debug.Log($"[Pause] Current scene: {SceneLoader.currentScene}, isPaused: {SceneLoader.isPaused}");

    if (SceneLoader.IsCurrentSceneNonInteractive)
    {
      Debug.Log("[Pause] Ignoring pause toggle in non-interactive scene");
      return;
    }

    if (!SceneLoader.isPaused)
    {
      // Pause the game
      Debug.Log("[Pause] Pausing game and loading PAUSE scene");
      Time.timeScale = 0;
      SceneLoader.isPaused = true;
      SceneLoader.Pause();
    }
    else
    {
      // Unpause the game
      Debug.Log("[Pause] Unpausing game and unloading PAUSE scene");
      Time.timeScale = 1;
      SceneLoader.isPaused = false;
      UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("PAUSE");
    }
  }
}