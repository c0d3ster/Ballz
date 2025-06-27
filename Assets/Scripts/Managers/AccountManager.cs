using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
using Enums;

public class AccountManager : MonoBehaviour
{
  [System.Serializable]
  public class UserAccount
  {
    public string email;
    public string username;
    public string platform;
    public DateTime createdDate;
    public bool isNewGame;
  }

  [System.Serializable]
  public class GameData
  {
    // Account info
    public string userEmail;
    public string username;
    public string platform;
    public long createdDateTicks;

    // Game progress
    public int collectLevel = 1;
    public int balanceLevel = 1;
    public int dodgeLevel = 1;
    public int jumpLevel = 1;
    public int pushLevel = 1;

    // Settings
    public float difficulty = 1.0f;
    public bool useTarget = true;
    public bool useAccelerometer = true;
    public bool useJoystick = true;
    public bool useKeyboard = true;

    // Lives system
    public int currentLives = 5;
    public long lastLifeLostTicks = 0;

    // Timestamps
    public long lastSaveTime = 0;
    public long lastLoadTime = 0;
  }

  public static AccountManager Instance { get; private set; }

  [Header("Account Settings")]
  public UserAccount currentAccount;
  public bool isAuthenticated = false;
  public bool isNewGame = false;

  [Header("Platform Detection")]
  public string currentPlatform;
  public string userEmail;

  [Header("Cloud Save Settings")]
  public bool enableCloudSave = true;

  // Events
  public event Action<UserAccount> OnAccountLoaded;
  public event Action<UserAccount> OnAccountCreated;
  public event Action<string> OnAuthenticationFailed;
  public event Action OnDataSaved;
  public event Action OnDataLoaded;
  public event Action OnCloudSaveFailed;

  // State tracking
  private bool isCloudSaveAvailable = false;
  private GameData gameData = new GameData();

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
      InitializePlatform();
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
      InitializeCloudSave();
    }
  }

  private void InitializePlatform()
  {
#if UNITY_ANDROID
    currentPlatform = "Android";
#elif UNITY_IOS
    currentPlatform = "iOS";
#else
    currentPlatform = "Editor";
#endif

    Debug.Log($"Platform detected: {currentPlatform}");
  }

  private async void InitializeCloudSave()
  {
    if (!enableCloudSave)
    {
      Debug.Log("Cloud Save disabled, using local storage only");
      return;
    }

    try
    {
      // Initialize Unity Services
      await UnityServices.InitializeAsync();

      // Sign in anonymously
      await AuthenticationService.Instance.SignInAnonymouslyAsync();

      Debug.Log("Cloud Save initialized successfully");
      isCloudSaveAvailable = true;
    }
    catch (Exception e)
    {
      Debug.LogWarning($"Cloud Save initialization failed: {e.Message}. Using local storage only.");
      isCloudSaveAvailable = false;
    }
  }

  public void StartAuthentication()
  {
    Debug.Log("Starting platform authentication...");
    StartCoroutine(AuthenticateWithPlatform());
  }

  private IEnumerator AuthenticateWithPlatform()
  {
    // For now, we'll simulate getting the email
    // In a real implementation, you'd integrate with Google Play Games Services or Game Center
    yield return new WaitForSeconds(1f); // Simulate network delay

#if UNITY_ANDROID
      // TODO: Integrate with Google Play Games Services
      userEmail = "android.user@example.com";
#elif UNITY_IOS
      // TODO: Integrate with Game Center
      userEmail = "ios.user@example.com";
#else
    userEmail = "editor.user@example.com";
#endif

    if (!string.IsNullOrEmpty(userEmail))
    {
      isAuthenticated = true;
      Debug.Log($"Authenticated with email: {userEmail}");
      CheckForExistingAccount();
    }
    else
    {
      OnAuthenticationFailed?.Invoke("Failed to get user email from platform");
    }
  }

  private void CheckForExistingAccount()
  {
    // Check if we have a saved account for this email
    string savedEmail = PlayerPrefs.GetString("UserEmail", "");

    if (savedEmail == userEmail)
    {
      // Load existing account
      LoadAccount();
    }
    else
    {
      // New user, will need to create account
      Debug.Log("New user detected, will create account after username input");
    }
  }

  public void CreateNewAccount(string username)
  {
    if (!isAuthenticated)
    {
      Debug.LogError("Cannot create account: not authenticated");
      return;
    }

    currentAccount = new UserAccount
    {
      email = userEmail,
      username = username,
      platform = currentPlatform,
      createdDate = DateTime.Now,
      isNewGame = true
    };

    // Initialize game data
    gameData.userEmail = userEmail;
    gameData.username = username;
    gameData.platform = currentPlatform;
    gameData.createdDateTicks = DateTime.Now.Ticks;
    gameData.lastSaveTime = DateTime.Now.Ticks;

    SaveAccount();
    isNewGame = true;
    OnAccountCreated?.Invoke(currentAccount);

    Debug.Log($"New account created: {username} ({userEmail})");
  }

  public void LoadExistingAccount()
  {
    LoadAccount();
    isNewGame = false;
  }

  private void LoadAccount()
  {
    string savedEmail = PlayerPrefs.GetString("UserEmail", "");
    string savedUsername = PlayerPrefs.GetString("Username", "");
    string savedPlatform = PlayerPrefs.GetString("Platform", "");

    if (!string.IsNullOrEmpty(savedEmail) && savedEmail == userEmail)
    {
      currentAccount = new UserAccount
      {
        email = savedEmail,
        username = savedUsername,
        platform = savedPlatform,
        createdDate = DateTime.Parse(PlayerPrefs.GetString("CreatedDate", DateTime.Now.ToString())),
        isNewGame = false
      };

      // Load game data
      LoadGameData();

      OnAccountLoaded?.Invoke(currentAccount);
      Debug.Log($"Account loaded: {savedUsername} ({savedEmail})");
    }
    else
    {
      Debug.Log("No existing account found for this email");
    }
  }

  private void SaveAccount()
  {
    if (currentAccount != null)
    {
      PlayerPrefs.SetString("UserEmail", currentAccount.email);
      PlayerPrefs.SetString("Username", currentAccount.username);
      PlayerPrefs.SetString("Platform", currentAccount.platform);
      PlayerPrefs.SetString("CreatedDate", currentAccount.createdDate.ToString());
      PlayerPrefs.Save();

      // Save game data
      SaveGameData();
    }
  }

  private async void SaveGameData()
  {
    if (gameData == null) return;

    gameData.lastSaveTime = DateTime.Now.Ticks;

    // Save to PlayerPrefs as backup
    string jsonData = JsonUtility.ToJson(gameData);
    PlayerPrefs.SetString("GameData", jsonData);
    PlayerPrefs.Save();

    // Save to cloud if available
    if (isCloudSaveAvailable && enableCloudSave)
    {
      try
      {
        var data = new Dictionary<string, object>
        {
          { "GameData", jsonData }
        };

        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        OnDataSaved?.Invoke();
        Debug.Log("Game data saved to cloud");
      }
      catch (Exception e)
      {
        Debug.LogWarning($"Cloud save failed: {e.Message}");
        OnCloudSaveFailed?.Invoke();
      }
    }
  }

  private async void LoadGameData()
  {
    // Try to load from cloud first
    if (isCloudSaveAvailable && enableCloudSave)
    {
      try
      {
        var cloudData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "GameData" });

        if (cloudData.ContainsKey("GameData"))
        {
          string jsonData = cloudData["GameData"].Value.GetAsString();
          gameData = JsonUtility.FromJson<GameData>(jsonData);
          gameData.lastLoadTime = DateTime.Now.Ticks;
          OnDataLoaded?.Invoke();
          Debug.Log("Game data loaded from cloud");
          return;
        }
      }
      catch (Exception e)
      {
        Debug.LogWarning($"Cloud load failed: {e.Message}");
      }
    }

    // Fallback to local data
    string localData = PlayerPrefs.GetString("GameData", "");
    if (!string.IsNullOrEmpty(localData))
    {
      try
      {
        gameData = JsonUtility.FromJson<GameData>(localData);
        gameData.lastLoadTime = DateTime.Now.Ticks;
        OnDataLoaded?.Invoke();
        Debug.Log("Game data loaded from local storage");
      }
      catch (Exception e)
      {
        Debug.LogError($"Failed to parse local game data: {e.Message}");
        gameData = new GameData();
      }
    }
    else
    {
      // Initialize new game data
      gameData = new GameData();
      gameData.userEmail = currentAccount.email;
      gameData.username = currentAccount.username;
      gameData.platform = currentAccount.platform;
      gameData.lastLoadTime = DateTime.Now.Ticks;
    }
  }

  public bool HasExistingAccount()
  {
    string savedEmail = PlayerPrefs.GetString("UserEmail", "");
    return savedEmail == userEmail && !string.IsNullOrEmpty(savedEmail);
  }

  public void ClearAccount()
  {
    PlayerPrefs.DeleteKey("UserEmail");
    PlayerPrefs.DeleteKey("Username");
    PlayerPrefs.DeleteKey("Platform");
    PlayerPrefs.DeleteKey("CreatedDate");
    PlayerPrefs.DeleteKey("GameData");
    PlayerPrefs.Save();

    currentAccount = null;
    gameData = new GameData();
    isAuthenticated = false;
    isNewGame = false;
  }

  // Game data access methods
  public GameData GetGameData()
  {
    return gameData;
  }

  public void SaveGameProgress()
  {
    SaveGameData();
  }

  public void UpdateLives(int lives)
  {
    if (gameData != null)
    {
      gameData.currentLives = lives;
      gameData.lastLifeLostTicks = DateTime.Now.Ticks;
      SaveGameData();
    }
  }

  public void UpdateLivesWithTimestamp(int lives, DateTime lastLifeLostTime)
  {
    if (gameData != null)
    {
      gameData.currentLives = lives;
      gameData.lastLifeLostTicks = lastLifeLostTime.Ticks;
      SaveGameData();
    }
  }

  public int GetCurrentLives()
  {
    return gameData?.currentLives ?? 5;
  }

  public DateTime GetLastLifeLostTime()
  {
    if (gameData != null && gameData.lastLifeLostTicks > 0)
    {
      return new DateTime(gameData.lastLifeLostTicks, DateTimeKind.Utc);
    }
    return DateTime.UtcNow;
  }

  public void UpdateLevel(GameMode gameMode, int level)
  {
    if (gameData != null)
    {
      switch (gameMode)
      {
        case GameMode.Collect:
          gameData.collectLevel = level;
          break;
        case GameMode.Balance:
          gameData.balanceLevel = level;
          break;
        case GameMode.Dodge:
          gameData.dodgeLevel = level;
          break;
        case GameMode.Jump:
          gameData.jumpLevel = level;
          break;
        case GameMode.Push:
          gameData.pushLevel = level;
          break;
      }
      SaveGameData();
    }
  }

  public int GetLevel(GameMode gameMode)
  {
    if (gameData != null)
    {
      switch (gameMode)
      {
        case GameMode.Collect:
          return gameData.collectLevel;
        case GameMode.Balance:
          return gameData.balanceLevel;
        case GameMode.Dodge:
          return gameData.dodgeLevel;
        case GameMode.Jump:
          return gameData.jumpLevel;
        case GameMode.Push:
          return gameData.pushLevel;
        default:
          return 1;
      }
    }
    return 1;
  }

  public void UpdateSettings(float difficulty, bool useTarget, bool useAccelerometer, bool useJoystick, bool useKeyboard)
  {
    if (gameData != null)
    {
      gameData.difficulty = difficulty;
      gameData.useTarget = useTarget;
      gameData.useAccelerometer = useAccelerometer;
      gameData.useJoystick = useJoystick;
      gameData.useKeyboard = useKeyboard;
      SaveGameData();
    }
  }

  public float GetDifficulty()
  {
    return gameData?.difficulty ?? 1.0f;
  }

  public bool GetUseTarget()
  {
    return gameData?.useTarget ?? true;
  }

  public bool GetUseAccelerometer()
  {
    return gameData?.useAccelerometer ?? true;
  }

  public bool GetUseJoystick()
  {
    return gameData?.useJoystick ?? true;
  }

  public bool GetUseKeyboard()
  {
    return gameData?.useKeyboard ?? true;
  }
}