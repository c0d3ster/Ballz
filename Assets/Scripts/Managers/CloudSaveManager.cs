using UnityEngine
using System
using System.Collections.Generic
using System.Threading.Tasks
using Unity.Services.Core
using Unity.Services.CloudSave
using Unity.Services.Authentication

public class CloudSaveManager : MonoBehaviour
{
  public static CloudSaveManager Instance { get; private set; }

  [Header("Cloud Save Settings")]
  [SerializeField]
  private bool enableCloudSave = true
  [SerializeField] private bool enableOfflineMode = true
  [SerializeField] private float saveRetryDelay = 5f

  // Events
  public static event Action OnCloudSaveInitialized
  public static event Action OnCloudSaveFailed
  public static event Action OnDataLoaded
  public static event Action OnDataSaved

  // State tracking
  private bool isInitialized = false
  private bool isSignedIn = false
  private bool isCloudSaveAvailable = false
  private Dictionary<string, object> pendingSaves = new Dictionary<string, object>()
  private float lastSaveTime = 0f

  // Data structure for all game data
  [Serializable]
  public class GameData
  {
    // Settings
    public float difficulty = 1.0f
    public bool useTarget = true
    public bool useAccelerometer = true
    public bool useJoystick = true
    public bool useKeyboard = true

    // Progress
    public int collectLevel = 1
    public int balanceLevel = 1
    public int dodgeLevel = 1
    public int jumpLevel = 1
    public int pushLevel = 1

    // Lives system
    public int currentLives = 5
    public long lastLifeLostTicks = 0

    // Timestamps
    public long lastSaveTime = 0
    public long lastLoadTime = 0
  }

  private GameData gameData = new GameData()

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this
      DontDestroyOnLoad(gameObject)
      Debug.Log("[CloudSaveManager] Instance created")
    }
    else
    {
      Debug.Log("[CloudSaveManager] Duplicate instance found, destroying")
      Destroy(gameObject)
    }
  }

  private async void Start()
  {
    if (Instance == this)
    {
      await InitializeCloudSave()
    }
  }

  private async Task InitializeCloudSave()
  {
    if (!enableCloudSave)
    {
      Debug.Log("[CloudSaveManager] Cloud save disabled, using local storage only")
      LoadFromLocalStorage()
      return
    }

    try
    {
      Debug.Log("[CloudSaveManager] Initializing cloud save...")

      // Initialize Unity Services
      await UnityServices.InitializeAsync()

      // Sign in anonymously
      await SignInAnonymously()

      // Check cloud save availability
      isCloudSaveAvailable = await CheckCloudSaveAvailability()

      if (isCloudSaveAvailable)
      {
        Debug.Log("[CloudSaveManager] Cloud save available, loading from cloud...")
        await LoadFromCloud()
      }
      else
      {
        Debug.Log("[CloudSaveManager] Cloud save not available, using local storage...")
        LoadFromLocalStorage()
      }

      isInitialized = true
      OnCloudSaveInitialized?.Invoke()
    }
    catch (Exception e)
    {
      Debug.LogError($"[CloudSaveManager] Initialization failed: {e.Message}")
      LoadFromLocalStorage()
      OnCloudSaveFailed?.Invoke()
    }
  }

  private async Task SignInAnonymously()
  {
    try
    {
      if (!AuthenticationService.Instance.IsSignedIn)
      {
        await AuthenticationService.Instance.SignInAnonymouslyAsync()
        isSignedIn = true
        Debug.Log("[CloudSaveManager] Signed in anonymously")
      }
    }
    catch (Exception e)
    {
      Debug.LogError($"[CloudSaveManager] Sign in failed: {e.Message}")
      throw
    }
  }

  private async Task<bool> CheckCloudSaveAvailability()
  {
    try
    {
      // Try to access cloud save to check availability
      var data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "test" })
      return true
    }
    catch
    {
      return false
    }
  }

  // Save data to cloud with fallback to local
  public async void SaveData()
  {
    if (!isInitialized)
    {
      Debug.LogWarning("[CloudSaveManager] Not initialized, queuing save...")
      QueueSave()
      return
    }

    try
    {
      // Update game data from current managers
      UpdateGameDataFromManagers()

      // Save to local storage first (immediate backup)
      SaveToLocalStorage()

      // Try to save to cloud if available
      if (isCloudSaveAvailable && isSignedIn)
      {
        await SaveToCloud()
        Debug.Log("[CloudSaveManager] Data saved to cloud successfully")
      }
      else
      {
        Debug.Log("[CloudSaveManager] Cloud save not available, saved to local storage only")
      }

      lastSaveTime = Time.time
      OnDataSaved?.Invoke()
    }
    catch (Exception e)
    {
      Debug.LogError($"[CloudSaveManager] Save failed: {e.Message}")
      // Save to local storage as fallback
      SaveToLocalStorage()
    }
  }

  private async Task SaveToCloud()
  {
    try
    {
      var data = new Dictionary<string, object>
      {
        { "gameData", JsonUtility.ToJson(gameData) },
        { "saveTime", DateTime.UtcNow.Ticks.ToString() }
      }

      await CloudSaveService.Instance.Data.SaveAsync(data)
    }
    catch (Exception e)
    {
      Debug.LogError($"[CloudSaveManager] Cloud save failed: {e.Message}")
      throw
    }
  }

  private async Task LoadFromCloud()
  {
    try
    {
      var keys = new HashSet<string> { "gameData" }
      var data = await CloudSaveService.Instance.Data.LoadAsync(keys)

      if (data.ContainsKey("gameData"))
      {
        string jsonData = data["gameData"].Value.GetAsString()
        gameData = JsonUtility.FromJson<GameData>(jsonData)
        Debug.Log("[CloudSaveManager] Data loaded from cloud")
      }
      else
      {
        Debug.Log("[CloudSaveManager] No cloud data found, using defaults")
        LoadFromLocalStorage()
      }

      ApplyGameDataToManagers()
      OnDataLoaded?.Invoke()
    }
    catch (Exception e)
    {
      Debug.LogError($"[CloudSaveManager] Cloud load failed: {e.Message}")
      LoadFromLocalStorage()
    }
  }

  private void LoadFromLocalStorage()
  {
    Debug.Log("[CloudSaveManager] Loading from local storage...")

    // Load settings
    gameData.difficulty = PlayerPrefs.GetFloat("Optionz_Difficulty", 1.0f)
    gameData.useTarget = PlayerPrefs.GetInt("Optionz_UseTarget", 1) == 1
    gameData.useAccelerometer = PlayerPrefs.GetInt("Optionz_UseAccelerometer", 1) == 1
    gameData.useJoystick = PlayerPrefs.GetInt("Optionz_UseJoystick", 1) == 1
    gameData.useKeyboard = PlayerPrefs.GetInt("Optionz_UseKeyboard", 1) == 1

    // Load progress
    gameData.collectLevel = PlayerPrefs.GetInt("CollectLevel", 1)
    gameData.balanceLevel = PlayerPrefs.GetInt("BalanceLevel", 1)
    gameData.dodgeLevel = PlayerPrefs.GetInt("DodgeLevel", 1)
    gameData.jumpLevel = PlayerPrefs.GetInt("JumpLevel", 1)
    gameData.pushLevel = PlayerPrefs.GetInt("PushLevel", 1)

    // Load lives
    gameData.currentLives = PlayerPrefs.GetInt("LivesManager_CurrentLives", 5)
    string lastLifeLostTicksString = PlayerPrefs.GetString("LivesManager_LastLifeLost", "0")
    if (long.TryParse(lastLifeLostTicksString, out long ticks))
    {
      gameData.lastLifeLostTicks = ticks
    }

    ApplyGameDataToManagers()
    OnDataLoaded?.Invoke()
  }

  private void SaveToLocalStorage()
  {
    // Save settings
    PlayerPrefs.SetFloat("Optionz_Difficulty", gameData.difficulty)
    PlayerPrefs.SetInt("Optionz_UseTarget", gameData.useTarget ? 1 : 0)
    PlayerPrefs.SetInt("Optionz_UseAccelerometer", gameData.useAccelerometer ? 1 : 0)
    PlayerPrefs.SetInt("Optionz_UseJoystick", gameData.useJoystick ? 1 : 0)
    PlayerPrefs.SetInt("Optionz_UseKeyboard", gameData.useKeyboard ? 1 : 0)

    // Save progress
    PlayerPrefs.SetInt("CollectLevel", gameData.collectLevel)
    PlayerPrefs.SetInt("BalanceLevel", gameData.balanceLevel)
    PlayerPrefs.SetInt("DodgeLevel", gameData.dodgeLevel)
    PlayerPrefs.SetInt("JumpLevel", gameData.jumpLevel)
    PlayerPrefs.SetInt("PushLevel", gameData.pushLevel)

    // Save lives
    PlayerPrefs.SetInt("LivesManager_CurrentLives", gameData.currentLives)
    PlayerPrefs.SetString("LivesManager_LastLifeLost", gameData.lastLifeLostTicks.ToString())

    PlayerPrefs.Save()
    Debug.Log("[CloudSaveManager] Data saved to local storage")
  }

  private void UpdateGameDataFromManagers()
  {
    // Update from Optionz
    gameData.difficulty = (float)Optionz.diff
    gameData.useTarget = Optionz.useTarget
    gameData.useAccelerometer = Optionz.useAccelerometer
    gameData.useJoystick = Optionz.useJoystick
    gameData.useKeyboard = Optionz.useKeyboard

    // Update from LevelProgressManager
    if (LevelProgressManager.Instance != null)
    {
      // Note: We'd need to add getter methods to LevelProgressManager
      // For now, we'll rely on the existing PlayerPrefs system
    }

    // Update from LivesManager
    if (LivesManager.Instance != null)
    {
      gameData.currentLives = LivesManager.Instance.CurrentLives
      // Note: We'd need to add a getter for lastLifeLostTime in LivesManager
    }

    gameData.lastSaveTime = DateTime.UtcNow.Ticks
  }

  private void ApplyGameDataToManagers()
  {
    // Apply to Optionz
    Optionz.diff = gameData.difficulty
    Optionz.useTarget = gameData.useTarget
    Optionz.useAccelerometer = gameData.useAccelerometer
    Optionz.useJoystick = gameData.useJoystick
    Optionz.useKeyboard = gameData.useKeyboard

    // Apply to LevelProgressManager
    if (LevelProgressManager.Instance != null)
    {
      // Note: We'd need to add setter methods to LevelProgressManager
      // For now, we'll rely on the existing PlayerPrefs system
    }

    // Apply to LivesManager
    if (LivesManager.Instance != null)
    {
      // Note: We'd need to add setter methods to LivesManager
      // For now, we'll rely on the existing PlayerPrefs system
    }

    gameData.lastLoadTime = DateTime.UtcNow.Ticks
  }

  private void QueueSave()
  {
    // Queue save for later when initialized
    Invoke(nameof(SaveData), saveRetryDelay)
  }

  // Public methods for manual save/load
  public void ForceSave()
  {
    SaveData()
  }

  public async void ForceLoad()
  {
    if (isCloudSaveAvailable && isSignedIn)
    {
      await LoadFromCloud()
    }
    else
    {
      LoadFromLocalStorage()
    }
  }

  // Clear all data (both cloud and local)
  public async void ClearAllData()
  {
    try
    {
      // Clear cloud data
      if (isCloudSaveAvailable && isSignedIn)
      {
        await CloudSaveService.Instance.Data.DeleteAsync(new HashSet<string> { "gameData" })
      }

      // Clear local data
      PlayerPrefs.DeleteAll()
      PlayerPrefs.Save()

      // Reset game data
      gameData = new GameData()

      Debug.Log("[CloudSaveManager] All data cleared")
    }
    catch (Exception e)
    {
      Debug.LogError($"[CloudSaveManager] Clear data failed: {e.Message}")
    }
  }

  // Get save status
  public bool IsCloudSaveAvailable => isCloudSaveAvailable
  public bool IsInitialized => isInitialized
  public bool IsSignedIn => isSignedIn
  public float TimeSinceLastSave => Time.time - lastSaveTime

  private void OnApplicationPause(bool pauseStatus)
  {
    if (pauseStatus)
    {
      // Save when app is paused (going to background)
      SaveData()
    }
  }

  private void OnApplicationFocus(bool hasFocus)
  {
    if (!hasFocus)
    {
      // Save when app loses focus
      SaveData()
    }
  }

  private void OnDestroy()
  {
    // Save on destroy
    if (isInitialized)
    {
      SaveData()
    }
  }

#if UNITY_EDITOR
  [ContextMenu("Test Cloud Save")]
  public async void TestCloudSave()
  {
    Debug.Log("[CloudSaveManager] Testing cloud save...")
    SaveData()
  }

  [ContextMenu("Test Cloud Load")]
  public async void TestCloudLoad()
  {
    Debug.Log("[CloudSaveManager] Testing cloud load...")
    ForceLoad()
  }
#endif
}