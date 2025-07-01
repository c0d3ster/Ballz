using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class LivesManager : MonoBehaviour
{
  public static LivesManager Instance { get; private set; }

  [Header("Lives Settings")]
  [SerializeField] private int maxLives = 5;
  [SerializeField] private float regenerationTimeMinutes = 15f;

  [Header("Easter Egg Settings")]
  [SerializeField] private int tapsRequiredForEasterEgg = 5;
  [SerializeField] private float maxTimeBetweenTaps = 2f; // Reset counter if more than 2 seconds between taps

  public int CurrentLives { get; private set; }
  public int MaxLives => maxLives;
  public float TimeUntilNextLife => GetTimeUntilNextLife();

  public event Action<int> OnLivesChanged;

  private DateTime lastLifeLostTime;
  private const string LIVES_KEY = "PlayerLives";
  private const string LAST_LIFE_LOST_KEY = "LastLifeLostTime";
  private LivesDisplay livesDisplay;
  private Transform livesContainer;
  private bool isInitialized = false;

  // Easter egg variables
  private int consecutiveTapCount = 0;
  private float lastTapTime = 0f;
  private int targetLifeCount = 0; // The life count to restore to when easter egg triggers

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
      InitializeLivesManager();
      LoadLives();
      SceneManager.sceneLoaded += OnSceneLoaded;

      // Subscribe to hotkey events
      HotkeyManager.OnResetConfirmed += ClearLivesData;
    }
  }

  private void OnDestroy()
  {
    if (Instance == this)
    {
      SceneManager.sceneLoaded -= OnSceneLoaded;
      HotkeyManager.OnResetConfirmed -= ClearLivesData;
    }
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    // Only process non-additive scene loads
    if (mode != LoadSceneMode.Additive)
    {
      // Notify LivesDisplay that scene has loaded so it can find player material
      if (livesDisplay != null)
      {
        livesDisplay.RefreshLives();
      }
    }
  }

  private void InitializeLivesManager()
  {
    if (isInitialized) return;

    // Create LivesDisplay
    CreateLivesDisplay();

    isInitialized = true;
  }

  private void CreateLivesDisplay()
  {
    // Create lives container first
    CreateLivesContainer();

    // Add LivesDisplay component to this GameObject
    livesDisplay = gameObject.AddComponent<LivesDisplay>();
  }

  private void CreateLivesContainer()
  {
    Canvas canvas = UIManager.Instance?.gameUICanvas;
    if (canvas == null)
    {
      Debug.LogError("[LivesManager] No canvas found for lives container!");
      return;
    }

    // Create lives container
    GameObject containerObj = new GameObject("LivesContainer");
    containerObj.transform.SetParent(canvas.transform, false);

    RectTransform rectTransform = containerObj.AddComponent<RectTransform>();
    rectTransform.anchorMin = new Vector2(0, 1); // Top left
    rectTransform.anchorMax = new Vector2(0, 1);
    rectTransform.pivot = new Vector2(0, 1);
    rectTransform.anchoredPosition = new Vector2(35, -35);

    livesContainer = containerObj.transform;

    // Scale the lives container if on mobile
    UIManager.Instance?.ScaleNewUIElement(containerObj.transform);
  }

  private void Update()
  {
    CheckLifeRegeneration();
    CheckEasterEggTapTimeout();
    // Reset lives handling moved to HotkeyManager
  }

  private void LoadLives()
  {
    // Get AccountManager reference
    AccountManager accountManager = AccountManager.Instance;
    if (accountManager == null)
    {
      Debug.LogError("[LivesManager] AccountManager not found!");
      return;
    }

    // Check if this is the first time running (no saved data)
    bool isFirstRun = accountManager.GetCurrentLives() == 5 && accountManager.GetLastLifeLostTime() == DateTime.UtcNow;

    if (isFirstRun)
    {
      // First time running - start with max lives
      CurrentLives = maxLives;
      lastLifeLostTime = DateTime.UtcNow;
      Debug.Log($"[LivesManager] First run detected. Initializing with {CurrentLives}/{maxLives} lives");
    }
    else
    {
      // Load saved data from AccountManager
      CurrentLives = accountManager.GetCurrentLives();
      lastLifeLostTime = accountManager.GetLastLifeLostTime();

      Debug.Log($"[LivesManager] Loading saved lives: {CurrentLives}/{maxLives}");

      // Check if enough time has passed to regenerate lives
      CheckAndRegenerateLivesOnLoad();
    }

    // Save the current state
    SaveLives();
  }

  private void CheckAndRegenerateLivesOnLoad()
  {
    TimeSpan timeSinceLastLife = DateTime.UtcNow - lastLifeLostTime;
    double regenerationTimeSeconds = regenerationTimeMinutes * 60.0;

    if (timeSinceLastLife.TotalSeconds >= regenerationTimeSeconds)
    {
      int livesToRegenerate = (int)(timeSinceLastLife.TotalSeconds / regenerationTimeSeconds);
      int oldLives = CurrentLives;
      CurrentLives = Mathf.Min(maxLives, CurrentLives + livesToRegenerate);

      if (livesToRegenerate > 0)
      {
        // Update the last life lost time to account for regenerated lives
        if (CurrentLives < maxLives)
        {
          // Calculate when the next life will be ready
          double remainingTime = timeSinceLastLife.TotalSeconds % regenerationTimeSeconds;
          lastLifeLostTime = DateTime.UtcNow.AddSeconds(-remainingTime);
        }
        else
        {
          // At max lives, set to current time
          lastLifeLostTime = DateTime.UtcNow;
        }
      }
    }
  }

  private void SaveLives()
  {
    // Save lives data to AccountManager
    AccountManager accountManager = AccountManager.Instance;
    if (accountManager != null)
    {
      accountManager.UpdateLivesWithTimestamp(CurrentLives, lastLifeLostTime);
    }
  }

  private void CheckLifeRegeneration()
  {
    if (CurrentLives >= maxLives) return;

    TimeSpan timeSinceLastLife = DateTime.UtcNow - lastLifeLostTime;
    double regenerationTimeSeconds = regenerationTimeMinutes * 60.0;

    if (timeSinceLastLife.TotalSeconds >= regenerationTimeSeconds)
    {
      CurrentLives++;
      lastLifeLostTime = DateTime.UtcNow;
      SaveLives();

      Debug.Log($"[LivesManager] Life regenerated! Current: {CurrentLives}/{maxLives}");
      OnLivesChanged?.Invoke(CurrentLives);
    }
  }

  private float GetTimeUntilNextLife()
  {
    if (CurrentLives >= maxLives) return 0f;

    TimeSpan timeSinceLastLife = DateTime.UtcNow - lastLifeLostTime;
    double regenerationTimeSeconds = regenerationTimeMinutes * 60.0;
    double timeUntilNext = regenerationTimeSeconds - timeSinceLastLife.TotalSeconds;

    return Mathf.Max(0f, (float)timeUntilNext);
  }

  public void LoseLife()
  {
    if (CurrentLives <= 0)
    {
      Debug.LogWarning("[LivesManager] Attempted to lose life when already at 0!");
      return;
    }

    bool wasAtMaxLives = CurrentLives >= maxLives;
    CurrentLives--;

    // Only reset the timer if we were at max lives before losing this life
    // If we already had a timer running, continue from where it was
    if (wasAtMaxLives)
    {
      lastLifeLostTime = DateTime.UtcNow;
      Debug.Log($"[LivesManager] Life lost from max lives - timer reset to 15 minutes");
    }
    else
    {
      Debug.Log($"[LivesManager] Life lost - timer continues from previous life loss");
    }

    SaveLives();

    Debug.Log($"[LivesManager] Life lost! Current: {CurrentLives}/{maxLives}");
    OnLivesChanged?.Invoke(CurrentLives);
  }

  public void AddLife()
  {
    if (CurrentLives >= maxLives)
    {
      Debug.LogWarning("[LivesManager] Attempted to add life when already at max!");
      return;
    }

    CurrentLives++;
    SaveLives();

    Debug.Log($"[LivesManager] Life added! Current: {CurrentLives}/{maxLives}");
    OnLivesChanged?.Invoke(CurrentLives);
  }

  public void AddLivesViaAd(int livesToAdd = 2)
  {
    if (CurrentLives >= maxLives)
    {
      Debug.LogWarning("[LivesManager] Attempted to add lives when already at max!");
      return;
    }

    int oldLives = CurrentLives;
    CurrentLives = Mathf.Min(maxLives, CurrentLives + livesToAdd);
    int actualLivesAdded = CurrentLives - oldLives;

    SaveLives();

    Debug.Log($"[LivesManager] Added {actualLivesAdded} lives via ad! Current: {CurrentLives}/{maxLives}");
    OnLivesChanged?.Invoke(CurrentLives);
  }

  public void SetMaxLives(int newMaxLives)
  {
    if (newMaxLives <= 0)
    {
      Debug.LogError("[LivesManager] Max lives must be greater than 0!");
      return;
    }

    maxLives = newMaxLives;
    CurrentLives = Mathf.Min(CurrentLives, maxLives);
    SaveLives();

    Debug.Log($"[LivesManager] Max lives updated to {maxLives}. Current: {CurrentLives}/{maxLives}");
    OnLivesChanged?.Invoke(CurrentLives);
  }

  public void ResetLives(int newLives)
  {
    if (newLives < 0 || newLives > maxLives)
    {
      Debug.LogError("[LivesManager] Invalid number of lives to reset to!");
      return;
    }

    CurrentLives = newLives;
    lastLifeLostTime = DateTime.UtcNow;
    SaveLives();

    Debug.Log($"[LivesManager] Lives reset to {CurrentLives}/{maxLives}");
    OnLivesChanged?.Invoke(CurrentLives);
  }

  public bool HasLives()
  {
    return CurrentLives > 0;
  }

  [ContextMenu("Reset Lives to Max")]
  public void ResetLivesToMax()
  {
    ResetLives(maxLives);
  }

  [ContextMenu("Clear All Lives Data")]
  public void ClearLivesData()
  {
    Debug.Log("[LivesManager] Clearing all lives data...");

    // Reset lives data in AccountManager
    AccountManager accountManager = AccountManager.Instance;
    if (accountManager != null)
    {
      accountManager.UpdateLivesWithTimestamp(maxLives, DateTime.UtcNow);
    }

    Debug.Log("[LivesManager] Cleared all lives data");

    // Force reset to max lives
    CurrentLives = maxLives;
    lastLifeLostTime = DateTime.UtcNow;
    SaveLives();
    OnLivesChanged?.Invoke(CurrentLives);
    Debug.Log($"[LivesManager] Reset to {CurrentLives}/{maxLives} lives");
  }

  [ContextMenu("Force Reset to 5 Lives")]
  public void ForceResetToFiveLives()
  {
    Debug.Log("[LivesManager] Force resetting to 5 lives");
    CurrentLives = 5;
    lastLifeLostTime = DateTime.UtcNow;
    SaveLives();
    OnLivesChanged?.Invoke(CurrentLives);
    Debug.Log($"[LivesManager] Force reset complete: {CurrentLives}/{maxLives} lives");
  }

  public void OnLifeIconClicked(int lifeNumber)
  {
    float currentTime = Time.time;

    // Check if too much time has passed since last tap
    if (currentTime - lastTapTime > maxTimeBetweenTaps)
    {
      consecutiveTapCount = 0;
    }

    consecutiveTapCount++;
    lastTapTime = currentTime;
    targetLifeCount = lifeNumber; // Store which life count to restore to

    Debug.Log($"[LivesManager] Life icon {lifeNumber} clicked! Consecutive taps: {consecutiveTapCount}/{tapsRequiredForEasterEgg}");

    // Check if easter egg should trigger
    if (consecutiveTapCount >= tapsRequiredForEasterEgg)
    {
      TriggerEasterEgg();
    }
  }

  private void CheckEasterEggTapTimeout()
  {
    // Reset counter if too much time has passed since last tap
    if (Time.time - lastTapTime > maxTimeBetweenTaps && consecutiveTapCount > 0)
    {
      consecutiveTapCount = 0;
      Debug.Log("[LivesManager] Easter egg tap counter reset due to timeout");
    }
  }

  private void TriggerEasterEgg()
  {
    Debug.Log($"[LivesManager] 🥚 EASTER EGG TRIGGERED! Resetting lives to {targetLifeCount}!");

    // Reset the tap counter
    consecutiveTapCount = 0;

    // Call the ResetLives method with the target life count
    ResetLives(targetLifeCount);
  }
}