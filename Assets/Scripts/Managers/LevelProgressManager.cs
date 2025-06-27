using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Enums;

public class LevelProgressManager : MonoBehaviour
{
  // Level progression counters
  private int pushLevel = 1;
  private int collectLevel = 1;
  private int balanceLevel = 1;
  private int dodgeLevel = 1;
  private int jumpLevel = 1;

  // UI elements for locked game modes
  public GameObject[] balanceLockedItems;
  public GameObject[] dodgeLockedItems;
  public GameObject[] jumpLockedItems;
  public GameObject[] pushLockedItems;

  // Lock objects for each game mode
  public GameObject balanceLock;
  public GameObject dodgeLock;
  public GameObject jumpLock;
  public GameObject pushLock;

  // Singleton instance
  private static LevelProgressManager instance;
  public static LevelProgressManager Instance
  {
    get { return instance; }
  }

  private void Awake()
  {
    if (instance != null)
    {
      Destroy(instance.gameObject);
    }
    instance = this;
    DontDestroyOnLoad(gameObject);
    UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
  }

  private void OnDestroy()
  {
    UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    HotkeyManager.OnResetConfirmed -= ResetProgress;
    HotkeyManager.OnCompleteLevelPressed -= CompleteCurrentLevel;
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    if (scene.name == "Active Main Menu")
    {
      LoadProgress();
      UpdateGameModeVisibility();
    }
  }

  private void Start()
  {
    LoadProgress();
    UpdateGameModeVisibility();

    // Subscribe to hotkey events
    HotkeyManager.OnResetConfirmed += ResetProgress;
    HotkeyManager.OnCompleteLevelPressed += CompleteCurrentLevel;
  }

  private void Update()
  {
    // Hotkey handling moved to HotkeyManager
  }

  public void UpdateGameModeVisibility()
  {
    Debug.Log($"[LevelProgress] Levels - Collect: {collectLevel}, Balance: {balanceLevel}, Dodge: {dodgeLevel}, Jump: {jumpLevel}, Push: {pushLevel}");

    // Balance is unlocked after completing Collect level 1
    bool balanceUnlocked = collectLevel > 1;
    SetGameModeState(balanceLockedItems, balanceLock, balanceUnlocked);

    // Push is unlocked after completing Balance level 1
    bool pushUnlocked = balanceLevel > 1;
    SetGameModeState(pushLockedItems, pushLock, pushUnlocked);

    // Jump is unlocked after completing Push level 1
    bool jumpUnlocked = pushLevel > 1;
    SetGameModeState(jumpLockedItems, jumpLock, jumpUnlocked);

    // Dodge is unlocked after completing Jump level 1
    bool dodgeUnlocked = jumpLevel > 1;
    SetGameModeState(dodgeLockedItems, dodgeLock, dodgeUnlocked);
  }

  private void SetGameModeState(GameObject[] lockedItems, GameObject lockObject, bool isUnlocked)
  {
    // Show/hide the lock object
    if (lockObject != null)
    {
      lockObject.SetActive(!isUnlocked);
    }

    // Handle all other locked items
    if (lockedItems != null)
    {
      foreach (GameObject item in lockedItems)
      {
        if (item != null)
        {
          item.SetActive(isUnlocked);
        }
      }
    }
  }

  public void CompleteCurrentLevel()
  {
    // Handle progress management (our responsibility)
    CompleteLevel(SceneLoader.Instance.currentScene);

    // Handle scene loading (SceneLoader's responsibility)
    SceneLoader.Instance.LoadWinScene();
  }

  // Level progression methods
  public void CompleteLevel(string sceneName)
  {
    GameMode? gameMode = SceneLoader.Instance.DetermineGameMode(sceneName);
    if (!gameMode.HasValue)
    {
      Debug.LogWarning($"[LevelProgress] Could not determine game mode for scene: {sceneName}");
      return;
    }

    string baseName = $"Ball {gameMode}{SceneLoader.Instance.GetGameModeSuffix(gameMode.Value)}";
    string currentMaxLevelName = $"{baseName} {GetHighestLevelNumber(gameMode.Value)}";

    if (sceneName == currentMaxLevelName)
    {
      switch (gameMode.Value)
      {
        case GameMode.Collect:
          collectLevel++;
          break;
        case GameMode.Balance:
          balanceLevel++;
          break;
        case GameMode.Dodge:
          dodgeLevel++;
          break;
        case GameMode.Jump:
          jumpLevel++;
          break;
        case GameMode.Push:
          pushLevel++;
          break;
      }
      UpdateGameModeVisibility();
      SaveProgress();
    }
  }

  public void SaveProgress()
  {
    // Save progress to AccountManager
    AccountManager accountManager = AccountManager.Instance;
    if (accountManager != null)
    {
      accountManager.UpdateLevel(GameMode.Collect, collectLevel);
      accountManager.UpdateLevel(GameMode.Balance, balanceLevel);
      accountManager.UpdateLevel(GameMode.Dodge, dodgeLevel);
      accountManager.UpdateLevel(GameMode.Jump, jumpLevel);
      accountManager.UpdateLevel(GameMode.Push, pushLevel);
    }
  }

  private void LoadProgress()
  {
    // Load progress from AccountManager
    AccountManager accountManager = AccountManager.Instance;
    if (accountManager != null)
    {
      collectLevel = accountManager.GetLevel(GameMode.Collect);
      balanceLevel = accountManager.GetLevel(GameMode.Balance);
      dodgeLevel = accountManager.GetLevel(GameMode.Dodge);
      jumpLevel = accountManager.GetLevel(GameMode.Jump);
      pushLevel = accountManager.GetLevel(GameMode.Push);
    }
    else
    {
      // Fallback to default values if AccountManager not available
      collectLevel = 1;
      balanceLevel = 1;
      dodgeLevel = 1;
      jumpLevel = 1;
      pushLevel = 1;
    }
  }

  public void ResetProgress()
  {
    // Reset all level counters to 1
    pushLevel = 1;
    collectLevel = 1;
    balanceLevel = 1;
    dodgeLevel = 1;
    jumpLevel = 1;

    // Save reset progress to AccountManager
    SaveProgress();

    // Update visibility
    UpdateGameModeVisibility();
  }

  [ContextMenu("Clear All Game Data")]
  public void ClearAllGameData()
  {
    // Clear all data through AccountManager
    AccountManager accountManager = AccountManager.Instance;
    if (accountManager != null)
    {
      accountManager.ClearAccount();
    }

    // Reload the current scene to apply changes
    UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
  }

  public int GetHighestLevelNumber(GameMode gameMode)
  {
    switch (gameMode)
    {
      case GameMode.Collect:
        return collectLevel;
      case GameMode.Balance:
        return balanceLevel;
      case GameMode.Dodge:
        return dodgeLevel;
      case GameMode.Jump:
        return jumpLevel;
      case GameMode.Push:
        return pushLevel;
      default:
        return 1;
    }
  }
}