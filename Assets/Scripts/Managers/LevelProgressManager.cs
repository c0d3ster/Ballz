using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Enums;

[System.Serializable]
public class LevelProgressManager : MonoBehaviour
{
  // Level progression counters
  private int pushLevel = 1;
  private int collectLevel = 1;
  private int balanceLevel = 1;
  private int dodgeLevel = 1;
  private int jumpLevel = 1;

  // Arrays for locked state visuals (including portals)
  public GameObject[] balanceLockedItems;
  public GameObject[] dodgeLockedItems;
  public GameObject[] jumpLockedItems;
  public GameObject[] pushLockedItems;

  // References to lock objects
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

  // Level progression methods
  public void CompleteLevel(GameMode gameMode)
  {
    string currentScene = SceneLoader.Instance.currentScene;
    string baseName = $"Ball {gameMode}{SceneLoader.Instance.GetGameModeSuffix(gameMode)}";
    string currentLevelName = $"{baseName} {GetHighestLevelNumber(gameMode)}";

    if (currentScene == currentLevelName)
    {
      switch (gameMode)
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
    PlayerPrefs.SetInt("CollectLevel", collectLevel);
    PlayerPrefs.SetInt("BalanceLevel", balanceLevel);
    PlayerPrefs.SetInt("DodgeLevel", dodgeLevel);
    PlayerPrefs.SetInt("JumpLevel", jumpLevel);
    PlayerPrefs.SetInt("PushLevel", pushLevel);
    PlayerPrefs.Save();
  }

  private void LoadProgress()
  {
    collectLevel = PlayerPrefs.GetInt("CollectLevel", 1);
    balanceLevel = PlayerPrefs.GetInt("BalanceLevel", 1);
    dodgeLevel = PlayerPrefs.GetInt("DodgeLevel", 1);
    jumpLevel = PlayerPrefs.GetInt("JumpLevel", 1);
    pushLevel = PlayerPrefs.GetInt("PushLevel", 1);
  }

  public void ResetProgress()
  {
    // Reset all level counters to 1
    pushLevel = 1;
    collectLevel = 1;
    balanceLevel = 1;
    dodgeLevel = 1;
    jumpLevel = 1;

    // Clear saved progress
    PlayerPrefs.DeleteKey("CollectLevel");
    PlayerPrefs.DeleteKey("BalanceLevel");
    PlayerPrefs.DeleteKey("DodgeLevel");
    PlayerPrefs.DeleteKey("JumpLevel");
    PlayerPrefs.DeleteKey("PushLevel");
    PlayerPrefs.Save();

    // Update visibility
    UpdateGameModeVisibility();
  }

  [ContextMenu("Clear All Game Data")]
  public void ClearAllGameData()
  {
    PlayerPrefs.DeleteAll();
    PlayerPrefs.Save();

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

  private void CompleteCurrentLevel()
  {
    GameMode? gameMode = SceneLoader.Instance.DetermineGameMode(SceneLoader.Instance.currentScene);
    if (gameMode.HasValue)
    {
      Debug.Log($"[LevelProgress] C key pressed - Current scene: {SceneLoader.Instance.currentScene}");
      Debug.Log($"[LevelProgress] Determined game mode: {gameMode}");
      Debug.Log($"[LevelProgress] Completing level for game mode: {gameMode.Value}");
      SceneLoader.Instance.Win();
    }
  }
}