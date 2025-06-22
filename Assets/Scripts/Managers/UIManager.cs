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
  [SerializeField] private float mobileScaleMultiplier = 1.2f;
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

    // Don't scale the canvas scaler - we'll scale individual elements instead
    // canvasScaler.scaleFactor *= mobileScaleMultiplier;

    // Scale up existing UI elements immediately
    ScaleUIElementsRecursively(gameUICanvas.transform);
  }

  private System.Collections.IEnumerator ScaleAllUIElementsDelayed()
  {
    // Wait a frame to ensure all dynamic elements (lives container, countdown text) are created
    yield return null;

    if (gameUICanvas == null) yield break;

    Debug.Log($"[UIManager] Scaling all UI elements in UICanvas by {mobileScaleMultiplier}x");

    // Scale all UI elements that are direct children of the UICanvas
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
      gameUICanvas.enabled = false;
    }
    else
    {
      gameUICanvas.enabled = true;
    }
  }

  void OnSceneUnloaded(Scene scene)
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