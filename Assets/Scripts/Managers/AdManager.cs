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
  private LevelPlayRewardedAd rewardedAd;

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

    // Create Rewarded Ad object after successful initialization
    CreateRewardedAd();

    // Load the first ad
    LoadRewardedAd();
  }

  private void OnLevelPlayInitFailed(LevelPlayInitError error)
  {
    Debug.LogError($"[AdManager] LevelPlay initialization failed: {error}");
    IsInitialized = false;
  }

  private void CreateRewardedAd()
  {
    try
    {
      // Create Rewarded Ad instance
      rewardedAd = new LevelPlayRewardedAd(rewardedAdUnitId);

      // Subscribe to Rewarded Ad events
      rewardedAd.OnAdLoaded += OnRewardedAdLoaded;
      rewardedAd.OnAdLoadFailed += OnRewardedAdLoadFailed;
      rewardedAd.OnAdDisplayed += OnRewardedAdDisplayed;
      rewardedAd.OnAdDisplayFailed += OnRewardedAdDisplayFailed;
      rewardedAd.OnAdClicked += OnRewardedAdClicked;
      rewardedAd.OnAdClosed += OnRewardedAdClosed;
      rewardedAd.OnAdRewarded += OnRewardedAdRewarded;
      rewardedAd.OnAdInfoChanged += OnRewardedAdInfoChanged;

      Debug.Log($"[AdManager] Rewarded Ad created with unit ID: {rewardedAdUnitId}");
    }
    catch (Exception e)
    {
      Debug.LogError($"[AdManager] Error creating rewarded ad: {e.Message}");
    }
  }

  private string GetProjectIdFromServices()
  {
    try
    {
      // Use the appropriate game ID based on platform
      // Unity Services doesn't expose initialization state directly, so we'll use the game IDs
      string gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
          ? iosGameId
          : androidGameId;

      if (!string.IsNullOrEmpty(gameId) && gameId != "unused")
      {
        Debug.Log($"[AdManager] Using Game ID from configuration: {gameId}");
        return gameId;
      }

      Debug.LogWarning("[AdManager] Game ID not configured - using manual configuration");
      return null;
    }
    catch (Exception e)
    {
      Debug.LogError($"[AdManager] Error retrieving Game ID: {e.Message}");
      return null;
    }
  }

  public void LoadRewardedAd()
  {
    if (!IsInitialized || rewardedAd == null)
    {
      Debug.LogWarning("[AdManager] Ads not initialized yet or rewarded ad not created!");
      return;
    }

    try
    {
      Debug.Log("[AdManager] Loading rewarded ad...");
      IsAdReady = false;

      // Load rewarded ad using LevelPlay API
      rewardedAd.LoadAd();
    }
    catch (Exception e)
    {
      Debug.LogError($"[AdManager] Error loading rewarded ad: {e.Message}");
      IsAdReady = false;
    }
  }

  public void ShowRewardedAd()
  {
    if (!IsAdReady || rewardedAd == null)
    {
      Debug.LogWarning("[AdManager] Ad not ready! Loading ad first...");
      LoadRewardedAd();
      return;
    }

    try
    {
      Debug.Log("[AdManager] Showing rewarded ad...");

      // Check if ad is ready before showing
      if (rewardedAd.IsAdReady())
      {
        rewardedAd.ShowAd();
      }
      else
      {
        Debug.LogWarning("[AdManager] Ad is not ready to show");
        OnAdFailed?.Invoke();
      }
    }
    catch (Exception e)
    {
      Debug.LogError($"[AdManager] Error showing rewarded ad: {e.Message}");
      OnAdFailed?.Invoke();
    }
  }

  // LevelPlay Rewarded Ad Event Handlers
  private void OnRewardedAdLoaded(LevelPlayAdInfo adInfo)
  {
    Debug.Log($"[AdManager] Rewarded ad loaded successfully - Placement: {adInfo.PlacementName}");
    IsAdReady = true;
  }

  private void OnRewardedAdLoadFailed(LevelPlayAdError error)
  {
    Debug.LogError($"[AdManager] Rewarded ad failed to load: {error}");
    IsAdReady = false;
  }

  private void OnRewardedAdDisplayed(LevelPlayAdInfo adInfo)
  {
    Debug.Log($"[AdManager] Rewarded ad displayed - Placement: {adInfo.PlacementName}");
  }

  private void OnRewardedAdDisplayFailed(LevelPlayAdDisplayInfoError error)
  {
    Debug.LogError($"[AdManager] Rewarded ad display failed: {error}");
    OnAdFailed?.Invoke();
  }

  private void OnRewardedAdClicked(LevelPlayAdInfo adInfo)
  {
    Debug.Log($"[AdManager] Rewarded ad clicked - Placement: {adInfo.PlacementName}");
  }

  private void OnRewardedAdClosed(LevelPlayAdInfo adInfo)
  {
    Debug.Log($"[AdManager] Rewarded ad closed - Placement: {adInfo.PlacementName}");
    IsAdReady = false;

    // Load the next ad
    LoadRewardedAd();
  }

  private void OnRewardedAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
  {
    Debug.Log($"[AdManager] User rewarded: {reward.Name} - {reward.Amount} - Placement: {adInfo.PlacementName}");
    OnAdCompleted?.Invoke();
  }

  private void OnRewardedAdInfoChanged(LevelPlayAdInfo adInfo)
  {
    Debug.Log($"[AdManager] Rewarded ad info changed - Placement: {adInfo.PlacementName}");
  }

  // Helper method to check if ads are available
  public bool CanShowAd()
  {
    return IsInitialized && IsAdReady && rewardedAd != null && rewardedAd.IsAdReady();
  }

  // Method to add lives via ad reward
  public void RequestLivesViaAd()
  {
    if (CanShowAd())
    {
      ShowRewardedAd();
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

    // Clean up rewarded ad event handlers and dispose
    if (rewardedAd != null)
    {
      rewardedAd.OnAdLoaded -= OnRewardedAdLoaded;
      rewardedAd.OnAdLoadFailed -= OnRewardedAdLoadFailed;
      rewardedAd.OnAdDisplayed -= OnRewardedAdDisplayed;
      rewardedAd.OnAdDisplayFailed -= OnRewardedAdDisplayFailed;
      rewardedAd.OnAdClicked -= OnRewardedAdClicked;
      rewardedAd.OnAdClosed -= OnRewardedAdClosed;
      rewardedAd.OnAdRewarded -= OnRewardedAdRewarded;
      rewardedAd.OnAdInfoChanged -= OnRewardedAdInfoChanged;

      rewardedAd.Dispose();
      rewardedAd = null;
    }
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
    Debug.Log($"[AdManager] Application Identifier: {Application.identifier}");
    Debug.Log($"[AdManager] Product Name: {Application.productName}");
    Debug.Log($"[AdManager] Version: {Application.version}");
    
    // For Android, you can get the advertising ID using Google Play Services
#if UNITY_ANDROID && !UNITY_EDITOR
    try
    {
      using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
      {
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        
        // Try to get advertising ID from Google Play Services
        AndroidJavaClass advertisingIdClient = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
        AndroidJavaObject adInfo = advertisingIdClient.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", context);
        string advertisingId = adInfo.Call<string>("getId");
        
        Debug.Log($"[AdManager] Android Advertising ID: {advertisingId}");
        Debug.Log($"[AdManager] Is Limit Ad Tracking Enabled: {adInfo.Call<bool>("isLimitAdTrackingEnabled")}");
      }
    }
    catch (Exception e)
    {
      Debug.LogWarning($"[AdManager] Could not get Android Advertising ID: {e.Message}");
    }
#endif
    
    // For iOS, advertising ID is not accessible directly from Unity
#if UNITY_IOS && !UNITY_EDITOR
    Debug.Log($"[AdManager] iOS: Advertising ID cannot be retrieved directly from Unity");
    Debug.Log($"[AdManager] iOS: Check Settings > Privacy & Security > Tracking");
#endif
    
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