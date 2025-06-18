using UnityEngine;
using System;

public class LivesManager : MonoBehaviour
{
  public static LivesManager Instance { get; private set; }

  [Header("Lives Settings")]
  [SerializeField] private int maxLives = 5;
  [SerializeField] private float regenerationTimeMinutes = 15f;

  public int CurrentLives { get; private set; }
  public int MaxLives => maxLives;
  public float TimeUntilNextLife => Mathf.Max(0f, regenerationTimeMinutes * 60f - (Time.time - lastLifeLostTime));

  public event Action<int> OnLivesChanged;

  private float lastLifeLostTime;
  private const string LIVES_KEY = "PlayerLives";
  private const string LAST_LIFE_LOST_KEY = "LastLifeLostTime";

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
      Debug.Log($"[LivesManager] Awake - Setting instance. GameObject: {gameObject.name}");
    }
    else
    {
      Debug.Log($"[LivesManager] Awake - Instance already exists, destroying {gameObject.name}");
      Destroy(gameObject);
    }
  }

  private void Start()
  {
    if (Instance == this)
    {
      Debug.Log("[LivesManager] Start - Loading lives");
      LoadLives();
      Debug.Log($"[LivesManager] Start - Initialized with {CurrentLives}/{maxLives} lives");
    }
  }

  private void Update()
  {
    CheckLifeRegeneration();

    // Reset lives when R is pressed (only in editor)
#if UNITY_EDITOR
    if (Input.GetKeyDown(KeyCode.R))
    {
      Debug.Log("[LivesManager] R key pressed - Resetting lives");
      ClearLivesData();
    }
#endif
  }

  private void LoadLives()
  {
    // Check if this is the first time running (no saved data)
    bool isFirstRun = !PlayerPrefs.HasKey(LIVES_KEY);

    Debug.Log($"[LivesManager] LoadLives - isFirstRun: {isFirstRun}");
    Debug.Log($"[LivesManager] LoadLives - PlayerPrefs.HasKey({LIVES_KEY}): {PlayerPrefs.HasKey(LIVES_KEY)}");
    Debug.Log($"[LivesManager] LoadLives - PlayerPrefs.GetInt({LIVES_KEY}, {maxLives}): {PlayerPrefs.GetInt(LIVES_KEY, maxLives)}");

    if (isFirstRun)
    {
      // First time running - start with max lives
      CurrentLives = maxLives;
      lastLifeLostTime = 0f;
      Debug.Log($"[LivesManager] First run detected. Initializing with {CurrentLives}/{maxLives} lives");
    }
    else
    {
      // Load saved data
      CurrentLives = PlayerPrefs.GetInt(LIVES_KEY, maxLives);
      lastLifeLostTime = PlayerPrefs.GetFloat(LAST_LIFE_LOST_KEY, 0f);
      Debug.Log($"[LivesManager] Loading saved lives: {CurrentLives}/{maxLives}");

      // Check if enough time has passed to regenerate lives
      float timeSinceLastLife = Time.time - lastLifeLostTime;
      if (timeSinceLastLife >= regenerationTimeMinutes * 60f)
      {
        int livesToRegenerate = Mathf.FloorToInt(timeSinceLastLife / (regenerationTimeMinutes * 60f));
        CurrentLives = Mathf.Min(maxLives, CurrentLives + livesToRegenerate);

        if (livesToRegenerate > 0)
        {
          Debug.Log($"[LivesManager] Regenerated {livesToRegenerate} lives during load. Current: {CurrentLives}/{maxLives}");
        }
      }
    }

    // Save the current state
    SaveLives();
  }

  private void SaveLives()
  {
    PlayerPrefs.SetInt(LIVES_KEY, CurrentLives);
    PlayerPrefs.SetFloat(LAST_LIFE_LOST_KEY, lastLifeLostTime);
    PlayerPrefs.Save();
  }

  private void CheckLifeRegeneration()
  {
    if (CurrentLives >= maxLives) return;

    float timeSinceLastLife = Time.time - lastLifeLostTime;
    if (timeSinceLastLife >= regenerationTimeMinutes * 60f)
    {
      CurrentLives++;
      lastLifeLostTime = Time.time;
      SaveLives();

      Debug.Log($"[LivesManager] Life regenerated! Current: {CurrentLives}/{maxLives}");
      OnLivesChanged?.Invoke(CurrentLives);
    }
  }

  public void LoseLife()
  {
    if (CurrentLives <= 0)
    {
      Debug.LogWarning("[LivesManager] Attempted to lose life when already at 0!");
      return;
    }

    CurrentLives--;
    lastLifeLostTime = Time.time;
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

  public void ResetLives()
  {
    CurrentLives = maxLives;
    lastLifeLostTime = 0f;
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
    ResetLives();
  }

  [ContextMenu("Clear All Lives Data")]
  public void ClearLivesData()
  {
    Debug.Log("[LivesManager] Clearing all lives data...");
    PlayerPrefs.DeleteKey(LIVES_KEY);
    PlayerPrefs.DeleteKey(LAST_LIFE_LOST_KEY);
    PlayerPrefs.Save();
    Debug.Log("[LivesManager] Cleared all lives data");

    // Force reset to max lives
    CurrentLives = maxLives;
    lastLifeLostTime = 0f;
    SaveLives();
    OnLivesChanged?.Invoke(CurrentLives);
    Debug.Log($"[LivesManager] Reset to {CurrentLives}/{maxLives} lives");
  }

  [ContextMenu("Force Reset to 5 Lives")]
  public void ForceResetToFiveLives()
  {
    Debug.Log("[LivesManager] Force resetting to 5 lives");
    CurrentLives = 5;
    lastLifeLostTime = 0f;
    SaveLives();
    OnLivesChanged?.Invoke(CurrentLives);
    Debug.Log($"[LivesManager] Force reset complete: {CurrentLives}/{maxLives} lives");
  }

  // Debug method to print current state
  [ContextMenu("Print Current State")]
  public void PrintCurrentState()
  {
    Debug.Log($"[LivesManager] Current State:");
    Debug.Log($"  - Instance: {Instance != null}");
    Debug.Log($"  - Current Lives: {CurrentLives}");
    Debug.Log($"  - Max Lives: {maxLives}");
    Debug.Log($"  - Has Lives: {HasLives()}");
    Debug.Log($"  - Last Life Lost Time: {lastLifeLostTime}");
    Debug.Log($"  - Time Until Next Life: {TimeUntilNextLife}");
    Debug.Log($"  - PlayerPrefs LIVES_KEY: {PlayerPrefs.GetInt(LIVES_KEY, -1)}");
    Debug.Log($"  - PlayerPrefs LAST_LIFE_LOST_KEY: {PlayerPrefs.GetFloat(LAST_LIFE_LOST_KEY, -1f)}");
  }
}