using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
  public static AdManager Instance { get; private set; }

  [Header("Ad Settings")]
  [SerializeField] public string androidGameId = "378f8017-1f54-49c2-b1a4-1e121871862b"; // Will be replaced with your Project ID
  [SerializeField] public string iosGameId = "378f8017-1f54-49c2-b1a4-1e121871862b"; // Will be replaced with your Project ID
  [SerializeField] public bool testMode = true;
  [SerializeField] public string rewardedAdUnitId = "Rewarded_Android";

  public bool IsInitialized { get; private set; }
  public bool IsAdReady { get; private set; }

  public event Action OnAdCompleted;
  public event Action OnAdFailed;
  public event Action OnAdSkipped;

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
      InitializeAds();
    }
  }

  private void InitializeAds()
  {
    // Try to get the Project ID from Unity Services first
    string projectId = GetProjectIdFromServices();

    string gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
        ? iosGameId
        : androidGameId;

    // If we have a Project ID from Services, use it
    if (!string.IsNullOrEmpty(projectId) && (gameId == "unused" || string.IsNullOrEmpty(gameId)))
    {
      gameId = projectId;
      Debug.Log($"[AdManager] Using Project ID from Unity Services: {gameId}");
    }

    if (string.IsNullOrEmpty(gameId) || gameId == "unused")
    {
      Debug.LogWarning("[AdManager] Game ID is not set! Ads will not work. Please configure in Unity Services or set manually.");
      return;
    }

    Advertisement.Initialize(gameId, testMode, this);
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

  public void LoadRewardedAd()
  {
    if (!IsInitialized)
    {
      Debug.LogWarning("[AdManager] Ads not initialized yet!");
      return;
    }

    Debug.Log("[AdManager] Loading rewarded ad...");
    Advertisement.Load(rewardedAdUnitId, this);
  }

  public void ShowRewardedAd()
  {
    if (!IsAdReady)
    {
      Debug.LogWarning("[AdManager] Ad not ready! Loading ad first...");
      LoadRewardedAd();
      return;
    }

    Debug.Log("[AdManager] Showing rewarded ad...");
    Advertisement.Show(rewardedAdUnitId, this);
  }

  // IUnityAdsInitializationListener
  public void OnInitializationComplete()
  {
    Debug.Log("[AdManager] Unity Ads initialization complete.");
    IsInitialized = true;
    LoadRewardedAd();
  }

  public void OnInitializationFailed(UnityAdsInitializationError error, string message)
  {
    Debug.LogError($"[AdManager] Unity Ads Initialization Failed: {error} - {message}");
    IsInitialized = false;
  }

  // IUnityAdsLoadListener
  public void OnUnityAdsAdLoaded(string placementId)
  {
    Debug.Log($"[AdManager] Ad loaded: {placementId}");
    IsAdReady = true;
  }

  public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
  {
    Debug.LogError($"[AdManager] Error loading Ad Unit {placementId}: {error} - {message}");
    IsAdReady = false;
  }

  // IUnityAdsShowListener
  public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
  {
    if (placementId.Equals(rewardedAdUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
    {
      Debug.Log("[AdManager] Ad completed successfully!");
      OnAdCompleted?.Invoke();
    }
    else if (showCompletionState.Equals(UnityAdsShowCompletionState.SKIPPED))
    {
      Debug.Log("[AdManager] Ad was skipped!");
      OnAdSkipped?.Invoke();
    }
    else
    {
      // Handle any other completion states (including ERROR in newer versions)
      Debug.LogError($"[AdManager] Ad completed with state: {showCompletionState}");
      OnAdFailed?.Invoke();
    }

    // Load the next ad
    LoadRewardedAd();
  }

  public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
  {
    Debug.LogError($"[AdManager] Error showing Ad Unit {placementId}: {error} - {message}");
    OnAdFailed?.Invoke();
  }

  public void OnUnityAdsShowStart(string placementId)
  {
    Debug.Log($"[AdManager] Ad show start: {placementId}");
  }

  public void OnUnityAdsShowClick(string placementId)
  {
    Debug.Log($"[AdManager] Ad show click: {placementId}");
  }

  // Helper method to check if ads are available
  public bool CanShowAd()
  {
    return IsInitialized && IsAdReady;
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