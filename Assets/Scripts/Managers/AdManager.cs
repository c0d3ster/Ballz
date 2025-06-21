using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.LevelPlay;
using System;
using System.Threading.Tasks;

public class AdManager : MonoBehaviour
{
  public static AdManager Instance { get; private set; }

  [Header("Ad Settings")]
  [SerializeField] public string androidGameId = "5882068";
  [SerializeField] public string iosGameId = "5882069";
  [SerializeField] public string rewardedAdUnitId = "Rewarded_Android";
#pragma warning disable CS0414 // The field 'AdManager.monetizationApiKey' is assigned but its value is never used
  [SerializeField] private string monetizationApiKey = "ee6876694eddc0893e1c5fc2a67576f921073c9d161185c8ceddaf88dcb1fec1";
#pragma warning restore CS0414
  [SerializeField] private bool testMode = true;

  public bool IsInitialized { get; private set; }
  public bool IsAdReady { get; private set; }

  public event Action OnAdCompleted;
  public event Action OnAdFailed;

  // LevelPlay mediation objects
  private string gameId;

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

  private async void Start()
  {
    if (Instance == this)
    {
      await InitializeAds();
    }
  }

  private async Task InitializeAds()
  {
    try
    {
      // Enable test suite if in test mode
      if (testMode)
      {
        LevelPlay.SetMetaData("is_test_suite", "enable");
      }

      // Initialize Unity Services
      var options = new InitializationOptions()
        .SetEnvironmentName(testMode ? "development" : "production");

      await UnityServices.InitializeAsync(options);

      // Get the Project ID from Unity Services
      gameId = GetProjectIdFromServices();

      // Fallback to manual configuration if needed
      if (string.IsNullOrEmpty(gameId) || gameId == "unused")
      {
        gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? iosGameId
            : androidGameId;
      }

      if (string.IsNullOrEmpty(gameId) || gameId == "unused")
      {
        Debug.LogWarning("[AdManager] Game ID is not set! Ads will not work. Please configure in Unity Services or set manually.");
        return;
      }

      Debug.Log($"[AdManager] Initializing LevelPlay with Game ID: {gameId}");

      // Register LevelPlay events BEFORE initialization
      LevelPlay.OnInitSuccess += OnLevelPlayInitSuccess;
      LevelPlay.OnInitFailed += OnLevelPlayInitFailed;

      // Initialize LevelPlay
      LevelPlay.Init(gameId);
    }
    catch (Exception e)
    {
      Debug.LogError($"[AdManager] Initialization failed: {e.Message}");
      IsInitialized = false;
    }
  }

  private void OnLevelPlayInitSuccess(LevelPlayConfiguration config)
  {
    Debug.Log("[AdManager] LevelPlay initialization successful!");
    IsInitialized = true;

    // Launch test suite if in test mode
    if (testMode)
    {
      LevelPlay.LaunchTestSuite();
    }

    // Load the first ad
    _ = LoadRewardedAd();
  }

  private void OnLevelPlayInitFailed(LevelPlayInitError error)
  {
    Debug.LogError($"[AdManager] LevelPlay initialization failed: {error}");
    IsInitialized = false;
  }

  private string GetProjectIdFromServices()
  {
    // Try to get the Project ID from Unity Services
    // This is a simplified approach - in a real implementation you might use Unity's API
    try
    {
      // For now, we'll rely on manual configuration
      // In the future, you could use Unity's Services API to get this automatically
      return null;
    }
    catch
    {
      return null;
    }
  }

  public async Task LoadRewardedAd()
  {
    if (!IsInitialized)
    {
      Debug.LogWarning("[AdManager] Ads not initialized yet!");
      return;
    }

    try
    {
      Debug.Log("[AdManager] Loading rewarded ad...");

      // LevelPlay is a mediation platform - we need to use the proper mediation API
      // For now, we'll simulate the loading process since the exact API needs to be confirmed
      // TODO: Replace with actual LevelPlay mediation API calls
      await Task.Delay(100); // Simulate async loading

      Debug.Log("[AdManager] Ad loading simulated - LevelPlay mediation API needs to be implemented");
      IsAdReady = true;
    }
    catch (Exception e)
    {
      Debug.LogError($"[AdManager] Error loading rewarded ad: {e.Message}");
      IsAdReady = false;
    }
  }

  public async Task ShowRewardedAd()
  {
    if (!IsAdReady)
    {
      Debug.LogWarning("[AdManager] Ad not ready! Loading ad first...");
      await LoadRewardedAd();
      return;
    }

    try
    {
      Debug.Log("[AdManager] Showing rewarded ad...");

      // LevelPlay is a mediation platform - we need to use the proper mediation API
      // For now, we'll simulate the showing process since the exact API needs to be confirmed
      // TODO: Replace with actual LevelPlay mediation API calls
      await Task.Delay(100); // Simulate async showing

      Debug.Log("[AdManager] Ad showing simulated - LevelPlay mediation API needs to be implemented");
      OnAdCompleted?.Invoke();
    }
    catch (Exception e)
    {
      Debug.LogError($"[AdManager] Error showing rewarded ad: {e.Message}");
      OnAdFailed?.Invoke();
    }
  }

  // LevelPlay Event Handlers - These will be called by the mediation platform
  private void OnRewardedAdLoaded()
  {
    Debug.Log("[AdManager] Rewarded ad loaded successfully");
    IsAdReady = true;
  }

  private void OnRewardedAdFailedLoad(string errorMessage)
  {
    Debug.LogError($"[AdManager] Rewarded ad failed to load: {errorMessage}");
    IsAdReady = false;
  }

  private void OnRewardedAdUserRewarded(string type, double amount)
  {
    Debug.Log($"[AdManager] User rewarded: {type} {amount}");
    OnAdCompleted?.Invoke();
  }

  private void OnRewardedAdClosed()
  {
    Debug.Log("[AdManager] Rewarded ad closed");
    IsAdReady = false;

    // Load the next ad
    _ = LoadRewardedAd();
  }

  private void OnRewardedAdClicked()
  {
    Debug.Log("[AdManager] Rewarded ad clicked");
  }

  private void OnRewardedAdImpressionRecorded(string impressionData)
  {
    Debug.Log($"[AdManager] Rewarded ad impression recorded: {impressionData}");
  }

  // Helper method to check if ads are available
  public bool CanShowAd()
  {
    return IsInitialized && IsAdReady;
  }

  // Method to add lives via ad reward
  public async void RequestLivesViaAd()
  {
    if (CanShowAd())
    {
      await ShowRewardedAd();
    }
    else
    {
      Debug.LogWarning("[AdManager] Cannot show ad - not ready or not initialized");
      OnAdFailed?.Invoke();
    }
  }

  private void OnDestroy()
  {
    // Clean up event handlers
    LevelPlay.OnInitSuccess -= OnLevelPlayInitSuccess;
    LevelPlay.OnInitFailed -= OnLevelPlayInitFailed;
  }

#if UNITY_EDITOR
  // Test method for editor - simulates ad completion
  [ContextMenu("Test Ad Completion")]
  public void TestAdCompletion()
  {
    Debug.Log("[AdManager] Testing ad completion in editor");
    OnAdCompleted?.Invoke();
  }

  // Test method for editor - simulates ad failure
  [ContextMenu("Test Ad Failure")]
  public void TestAdFailure()
  {
    Debug.Log("[AdManager] Testing ad failure in editor");
    OnAdFailed?.Invoke();
  }

  // Get advertising ID for testing
  [ContextMenu("Get Advertising ID")]
  public void GetAdvertisingId()
  {
    string deviceId = SystemInfo.deviceUniqueIdentifier;
    Debug.Log($"[AdManager] Device ID: {deviceId}");
    Debug.Log($"[AdManager] To get your Advertising ID:");
    Debug.Log($"[AdManager] Android: Use 'adb shell settings get secure advertising_id'");
    Debug.Log($"[AdManager] iOS: Check Settings > Privacy & Security > Tracking");
    Debug.Log($"[AdManager] Or install 'Device ID' app from app store");
    Debug.Log($"[AdManager] Add the Advertising ID (not Device ID) to Unity Services test devices");
  }

  // Setup method for testing
  [ContextMenu("Setup for Testing")]
  public void SetupForTesting()
  {
    androidGameId = "test_android_id";
    iosGameId = "test_ios_id";
    testMode = true;
    rewardedAdUnitId = "Rewarded_Android";
    Debug.Log("[AdManager] Setup for testing complete!");
  }
#endif
}