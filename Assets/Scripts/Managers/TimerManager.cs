using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using TMPro;

public class TimerManager : MonoBehaviour
{
  public static TimerManager Instance { get; private set; }

  [Header("Timer Settings")]
  [SerializeField] private string timerFormat = "Time Remaining: {0:F2}";
  [SerializeField] private int fontSize = 64;
  [SerializeField] private Color textColor = Color.red;
  [SerializeField] private Vector2 timerPosition = new Vector2(0, -35);

  public float CurrentTime { get; private set; }
  public float MaxTime { get; private set; }
  public bool IsTimerActive { get; private set; }
  public bool IsTimeUp => CurrentTime <= 0;

  private TextMeshProUGUI timerText;
  private bool isInitialized = false;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void Start()
  {
    if (Instance == this)
    {
      InitializeTimerManager();
      SceneManager.sceneLoaded += OnSceneLoaded;
    }
  }

  private void OnDestroy()
  {
    if (Instance == this)
    {
      SceneManager.sceneLoaded -= OnSceneLoaded;
    }
  }

  private void InitializeTimerManager()
  {
    if (isInitialized) return;

    // Create timer display
    CreateTimerDisplay();

    isInitialized = true;
  }

  private void CreateTimerDisplay()
  {
    Canvas canvas = UIManager.Instance?.gameUICanvas;
    if (canvas == null)
    {
      Debug.LogError("[TimerManager] No canvas found for timer display!");
      return;
    }

    // Create timer text GameObject
    GameObject timerObj = new GameObject("TimerText");
    timerObj.transform.SetParent(canvas.transform, false);
    timerObj.layer = canvas.gameObject.layer;

    // Add TextMeshProUGUI component
    timerText = timerObj.AddComponent<TextMeshProUGUI>();
    timerText.text = "Time Remaining: 00.00";
    timerText.fontSize = fontSize;
    timerText.color = textColor;
    timerText.alignment = TextAlignmentOptions.Top;
    timerText.textWrappingMode = TextWrappingModes.NoWrap;

    // Set up RectTransform
    RectTransform rectTransform = timerObj.GetComponent<RectTransform>();
    rectTransform.anchorMin = new Vector2(0.5f, 1);
    rectTransform.anchorMax = new Vector2(0.5f, 1);
    rectTransform.pivot = new Vector2(0.5f, 1);
    rectTransform.anchoredPosition = timerPosition;
    rectTransform.sizeDelta = new Vector2(300, 100);
    rectTransform.localScale = Vector3.one;

    // Start with timer display hidden - only show when timer is active
    timerObj.SetActive(false);
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    // Only process non-additive scene loads
    if (mode != LoadSceneMode.Additive)
    {
      InitializeTimerForScene(scene.name);
    }
  }

  private void InitializeTimerForScene(string sceneName)
  {
    // Check if this scene has a time limit configured
    if (SceneTimerConfig.HasTimeLimit(sceneName))
    {
      float timeLimit = SceneTimerConfig.GetTimeLimit(sceneName);
      MaxTime = timeLimit;
      CurrentTime = MaxTime;
      IsTimerActive = true;

      // Apply difficulty modifier if available
      if (Optionz.diff != 0)
      {
        CurrentTime = MaxTime * (float)Optionz.diff;
        MaxTime = CurrentTime;
      }

      UpdateTimerDisplay();
    }
    else
    {
      // No timer for this scene
      IsTimerActive = false;
      CurrentTime = 0;
      MaxTime = 0;
    }

    // Show/hide timer display based on whether timer is active
    if (timerText != null)
    {
      timerText.gameObject.SetActive(IsTimerActive);
    }
  }

  private void Update()
  {
    if (IsTimerActive && CurrentTime > 0)
    {
      CurrentTime -= Time.deltaTime;
      UpdateTimerDisplay();

      if (CurrentTime <= 0)
      {
        CurrentTime = 0;
        IsTimerActive = false;
        SceneLoader.Instance.GameOver();
      }
    }
  }

  private void UpdateTimerDisplay()
  {
    if (timerText != null && IsTimerActive)
    {
      timerText.text = string.Format(timerFormat, CurrentTime);
    }
  }

  // Public methods for external control
  public void PauseTimer()
  {
    IsTimerActive = false;
  }

  public void ResumeTimer()
  {
    if (MaxTime > 0)
    {
      IsTimerActive = true;
    }
  }

  public void ResetTimer()
  {
    if (MaxTime > 0)
    {
      CurrentTime = MaxTime;
      IsTimerActive = true;
      UpdateTimerDisplay();
    }
  }


  public void SetTimeLimit(string sceneName, float timeLimit)
  {
    SceneTimerConfig.AddSceneTimeLimit(sceneName, timeLimit);
  }

  public float GetTimeLimit(string sceneName)
  {
    return SceneTimerConfig.GetTimeLimit(sceneName);
  }

  public bool HasTimeLimit(string sceneName)
  {
    return SceneTimerConfig.HasTimeLimit(sceneName);
  }
}