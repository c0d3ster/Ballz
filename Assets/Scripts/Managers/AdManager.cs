using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Mediation;
using System;
using System.Threading.Tasks;

public class AdManager : MonoBehaviour
{
  public static AdManager Instance { get; private set; }

  [Header("Ad Settings")]
  [SerializeField] public string androidGameId = "5882068";
  [SerializeField] public string iosGameId = "5882069";
  [SerializeField] public string rewardedAdUnitId = "Rewarded_Android";

  public bool IsInitialized { get; private set; }
  public bool IsAdReady { get; private set; }

  public event Action OnAdCompleted;
  public event Action OnAdFailed;
  public event Action OnAdSkipped;

  // LevelPlay mediation objects
  private IRewardedAd rewardedAd;
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

      // Initialize LevelPlay mediation
      await MediationService.Initialize(gameId);

      IsInitialized = true;
      Debug.Log("[AdManager] LevelPlay initialization complete.");

      // Load the first ad
      await LoadRewardedAd();
    }
    catch (Exception e)
    {
      Debug.LogError($"[AdManager] Initialization failed: {e.Message}");
      IsInitialized = false;
    }
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

      // Create the rewarded ad
      rewardedAd = MediationService.CreateRewardedAd(rewardedAdUnitId);

      // Set up event handlers
      rewardedAd.OnLoaded += OnRewardedAdLoaded;
      rewardedAd.OnFailedLoad += OnRewardedAdFailedLoad;
      rewardedAd.OnUserRewarded += OnRewardedAdUserRewarded;
      rewardedAd.OnClosed += OnRewardedAdClosed;
      rewardedAd.OnClicked += OnRewardedAdClicked;
      rewardedAd.OnImpressionRecorded += OnRewardedAdImpressionRecorded;

      // Load the ad
      await rewardedAd.LoadAsync();
    }
    catch (Exception e)
    {
      Debug.LogError($"[AdManager] Error loading rewarded ad: {e.Message}");
      IsAdReady = false;
    }
  }

  public async Task ShowRewardedAd()
  {
    if (!IsAdReady || rewardedAd == null)
    {
      Debug.LogWarning("[AdManager] Ad not ready! Loading ad first...");
      await LoadRewardedAd();
      return;
    }

    try
    {
      Debug.Log("[AdManager] Showing rewarded ad...");
      await rewardedAd.ShowAsync();
    }
    catch (Exception e)
    {
      Debug.LogError($"[AdManager] Error showing rewarded ad: {e.Message}");
      OnAdFailed?.Invoke();
    }
  }

  // LevelPlay Event Handlers
  private void OnRewardedAdLoaded(object sender, LoadAdEventArgs args)
  {
    Debug.Log("[AdManager] Rewarded ad loaded successfully");
    IsAdReady = true;
  }

  private void OnRewardedAdFailedLoad(object sender, LoadErrorEventArgs args)
  {
    Debug.LogError($"[AdManager] Rewarded ad failed to load: {args.Message}");
    IsAdReady = false;
  }

  private void OnRewardedAdUserRewarded(object sender, RewardEventArgs args)
  {
    Debug.Log($"[AdManager] User rewarded: {args.Type} {args.Amount}");
    OnAdCompleted?.Invoke();
  }

  private void OnRewardedAdClosed(object sender, EventArgs args)
  {
    Debug.Log("[AdManager] Rewarded ad closed");
    IsAdReady = false;

    // Load the next ad
    _ = LoadRewardedAd();
  }

  private void OnRewardedAdClicked(object sender, EventArgs args)
  {
    Debug.Log("[AdManager] Rewarded ad clicked");
  }

  private void OnRewardedAdImpressionRecorded(object sender, ImpressionEventArgs args)
  {
    Debug.Log($"[AdManager] Rewarded ad impression recorded: {args.ImpressionData}");
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
    if (rewardedAd != null)
    {
      rewardedAd.OnLoaded -= OnRewardedAdLoaded;
      rewardedAd.OnFailedLoad -= OnRewardedAdFailedLoad;
      rewardedAd.OnUserRewarded -= OnRewardedAdUserRewarded;
      rewardedAd.OnClosed -= OnRewardedAdClosed;
      rewardedAd.OnClicked -= OnRewardedAdClicked;
      rewardedAd.OnImpressionRecorded -= OnRewardedAdImpressionRecorded;
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