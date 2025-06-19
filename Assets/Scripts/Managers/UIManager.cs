using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

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

    if (SceneLoader.Instance.IsCurrentSceneNonInteractive)
    {
      touchControllerCanvas.enabled = false;
      Debug.Log("[UIManager] Disabling canvas for non-interactive scene");
    }
    else
    {
      touchControllerCanvas.enabled = true;
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
      rectTransform.anchoredPosition = new Vector2(35, -35);

      livesContainer = containerObj.transform;
      Debug.Log("[UIManager] Created LivesContainer at position (20, -20)");
    }

    // Add LivesDisplay component
    livesDisplay = gameObject.AddComponent<LivesDisplay>();
    Debug.Log("[UIManager] Added LivesDisplay component");
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