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
  [SerializeField] public string androidGameId = "5882069";
  [SerializeField] public string iosGameId = "5882068";

  [SerializeField] public string levelplayAppKey = "22829285d";
  [SerializeField] public string rewardedAdUnitId = "5uofehvzjnm63tkp";
  [SerializeField] private bool testMode = true;
  [SerializeField] private bool shouldShowOnLoad = false;

  public bool IsInitialized { get; private set; }

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
      Debug.Log("[AdManager] Duplicate instance found, destroying this one");
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

      // Add a small delay to ensure Unity Services is fully ready
      await Task.Delay(1000);

      // Get the gameId for the appropriate platform
      gameId = GetGameIdForPlatform();

      // Fallback to manual configuration if needed
      if (string.IsNullOrEmpty(gameId) || gameId == "unused")
      {
        Debug.LogError("[AdManager] Game ID is not set! Ads will not work. Please configure in Unity Services or set manually.");
        return;
      }

      // Register LevelPlay events BEFORE initialization
      LevelPlay.OnInitSuccess += OnLevelPlayInitSuccess;
      LevelPlay.OnInitFailed += OnLevelPlayInitFailed;

      // Initialize LevelPlay
      LevelPlay.Init(levelplayAppKey);
    }
    catch (Exception e)
    {
      Debug.LogError($"[AdManager] Initialization failed: {e.Message}");
      IsInitialized = false;
    }
  }

  private void OnLevelPlayInitSuccess(LevelPlayConfiguration config)
  {
    IsInitialized = true;

    // Create Rewarded Ad object after successful initialization
    CreateRewardedAd();

    // Load the first ad immediately so it's ready for the user
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
    }
    catch (Exception e)
    {
      Debug.LogError($"[AdManager] Error creating rewarded ad: {e.Message}");
    }
  }

  private string GetGameIdForPlatform()
  {
    try
    {
      // Use the appropriate game ID based on platform
      string gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
          ? iosGameId
          : androidGameId;

      if (!string.IsNullOrEmpty(gameId) && gameId != "unused")
      {
        return gameId;
      }

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
      // Load rewarded ad using LevelPlay API
      rewardedAd.LoadAd();
    }
    catch (Exception e)
    {
      Debug.LogError($"[AdManager] Error loading rewarded ad: {e.Message}");
    }
  }

  public void ShowRewardedAd()
  {
    try
    {
      if (CanShowAd())
      {
        rewardedAd.ShowAd();
      }
      else
      {
        // Set flag to true so the callback will show the ad when it loads
        shouldShowOnLoad = true;
        LoadRewardedAd();
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
    Debug.Log("[AdManager] Ad loaded and ready");

    // Only show automatically if shouldShowOnLoad is enabled
    if (shouldShowOnLoad)
    {
      rewardedAd.ShowAd();
      shouldShowOnLoad = false;
    }
  }

  private void OnRewardedAdLoadFailed(LevelPlayAdError error)
  {
    Debug.LogError($"[AdManager] Ad failed to load: {error}");
  }

  private void OnRewardedAdDisplayed(LevelPlayAdInfo adInfo)
  {
    // Ad displayed successfully
  }

  private void OnRewardedAdDisplayFailed(LevelPlayAdDisplayInfoError error)
  {
    Debug.LogError($"[AdManager] Ad display failed: {error}");
    OnAdFailed?.Invoke();
  }

  private void OnRewardedAdClicked(LevelPlayAdInfo adInfo)
  {
    // Ad clicked
  }

  private void OnRewardedAdClosed(LevelPlayAdInfo adInfo)
  {
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
    return IsInitialized && rewardedAd != null && rewardedAd.IsAdReady();
  }

  // Method to add lives via ad reward
  public void RequestLivesViaAd()
  {
    Debug.Log("[AdManager] Requesting lives via ad...");
    ShowRewardedAd();
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
#endif
}