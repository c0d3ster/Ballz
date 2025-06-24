using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
  public static UIManager Instance { get; private set; }
  [Header("UI References")]
  public Canvas gameUICanvas;

  [Header("Mobile UI Scaling")]
  [SerializeField] private float mobileScaleMultiplier = 1.1f;
  [SerializeField] private bool enableMobileScaling = true;

  private CanvasScaler canvasScaler;
  private bool isMobilePlatform = false;

  // Property for current scale multiplier
  public float currentScaleMultiplier => isMobilePlatform && enableMobileScaling ? mobileScaleMultiplier : 1f;

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

      // Get canvas scaler for mobile scaling
      canvasScaler = gameUICanvas.GetComponent<CanvasScaler>();
      if (canvasScaler == null)
      {
        Debug.LogError("No CanvasScaler found on gameUICanvas!");
        return;
      }

      // Check if we're on a mobile platform
      isMobilePlatform = SystemInfo.deviceType == DeviceType.Handheld;

      // Apply mobile scaling if enabled and on mobile
      if (enableMobileScaling && isMobilePlatform)
      {
        ApplyMobileScaling();
      }

      // Subscribe to scene changes
      SceneManager.sceneLoaded += OnSceneLoaded;
      SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    else
    {
      Destroy(gameObject);
    }
  }

  void Start()
  {
    // Subscribe to hotkey events
    HotkeyManager.OnPausePressed += TogglePause;
  }

  private void ApplyMobileScaling()
  {
    if (canvasScaler == null) return;

    Debug.Log($"[UIManager] Applying mobile scaling ({mobileScaleMultiplier}x) for {SystemInfo.deviceModel}");

    // Scale up existing UI elements immediately
    ScaleUIElementsRecursively(gameUICanvas.transform);
  }

  private void ScaleUIElementsRecursively(Transform parent)
  {
    // Scale all direct children
    foreach (Transform child in parent)
    {
      // Scale this element if it has a RectTransform
      RectTransform rectTransform = child.GetComponent<RectTransform>();
      if (rectTransform != null)
      {
        // Scale size by full multiplier, position by half multiplier
        float positionMultiplier = 1f + ((mobileScaleMultiplier - 1f) * 0.5f);
        rectTransform.sizeDelta *= mobileScaleMultiplier;
        rectTransform.anchoredPosition *= positionMultiplier;
        Debug.Log($"[UIManager] Scaled {child.name} - Size: {rectTransform.sizeDelta}, Position: {rectTransform.anchoredPosition}, SizeMult: {mobileScaleMultiplier}, PosMult: {positionMultiplier}");
      }

      // Special handling for TextMeshPro font size
      TMPro.TextMeshProUGUI tmpText = child.GetComponent<TMPro.TextMeshProUGUI>();
      if (tmpText != null)
      {
        tmpText.fontSize *= mobileScaleMultiplier;
        Debug.Log($"[UIManager] Scaled {child.name} font size to {tmpText.fontSize}");
      }

      // Recursively scale children (for nested UI elements)
      ScaleUIElementsRecursively(child);
    }
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    if (gameUICanvas == null) return;

    if (SceneLoader.Instance.IsCurrentSceneNonInteractive)
    {
      // Check if this is specifically the GAME_OVER scene
      if (scene.name == "GAME_OVER")
      {
        // Show only LivesContainer on GAME_OVER scene
        ShowOnlyLivesContainer();
      }
      else
      {
        // Hide everything on other non-interactive scenes
        gameUICanvas.enabled = false;
      }
    }
    else
    {
      // Show all UI elements on interactive scenes
      ShowAllUIElements();
    }
  }

  void OnSceneUnloaded(Scene scene)
  {
    if (gameUICanvas == null) return;

    // When any scene is unloaded, show all UI elements
    // The OnSceneLoaded event will handle hiding/showing specific elements
    ShowAllUIElements();
  }

  private void ShowOnlyLivesContainer()
  {
    if (gameUICanvas == null) return;

    // Hide all UI elements except LivesContainer
    foreach (Transform child in gameUICanvas.transform)
    {
      if (child.name == "LivesContainer")
      {
        child.gameObject.SetActive(true);
      }
      else
      {
        child.gameObject.SetActive(false);
      }
    }

    // Keep the canvas enabled so LivesContainer remains visible
    gameUICanvas.enabled = true;
  }

  private void ShowAllUIElements()
  {
    if (gameUICanvas == null) return;

    // Show all UI elements
    foreach (Transform child in gameUICanvas.transform)
    {
      child.gameObject.SetActive(true);
    }

    gameUICanvas.enabled = true;
  }

  void OnDestroy()
  {
    if (Instance == this)
    {
      SceneManager.sceneLoaded -= OnSceneLoaded;
      SceneManager.sceneUnloaded -= OnSceneUnloaded;
      HotkeyManager.OnPausePressed -= TogglePause;
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

  // Public method to check if mobile scaling is applied
  public bool IsMobileScalingApplied()
  {
    return isMobilePlatform && enableMobileScaling;
  }

  // Public method for dynamically created components to trigger scaling
  public void ScaleNewUIElement(Transform element)
  {
    if (!isMobilePlatform || !enableMobileScaling) return;

    RectTransform rectTransform = element.GetComponent<RectTransform>();
    if (rectTransform != null)
    {
      // Scale size by full multiplier, position by half multiplier
      float positionMultiplier = 1f + ((mobileScaleMultiplier - 1f) * 0.5f);
      rectTransform.sizeDelta *= mobileScaleMultiplier;
      rectTransform.anchoredPosition *= positionMultiplier;
      Debug.Log($"[UIManager] Scaled new element {element.name} - Size: {rectTransform.sizeDelta}, Position: {rectTransform.anchoredPosition}");
    }

    // Special handling for TextMeshPro font size
    TMPro.TextMeshProUGUI tmpText = element.GetComponent<TMPro.TextMeshProUGUI>();
    if (tmpText != null)
    {
      tmpText.fontSize *= mobileScaleMultiplier;
      Debug.Log($"[UIManager] Scaled {element.name} font size to {tmpText.fontSize}");
    }
  }
}