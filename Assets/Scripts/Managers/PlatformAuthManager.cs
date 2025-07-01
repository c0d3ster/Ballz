using UnityEngine;
using System;
using System.Threading.Tasks;

public class PlatformAuthManager : MonoBehaviour
{
  public static PlatformAuthManager Instance { get; private set; }

  [Header("Platform Settings")]
  [SerializeField] private bool enablePlatformAuth = true;

  // Events
  public event Action<string, string> OnAuthenticationSuccess; // email, platform
  public event Action<string> OnAuthenticationFailed;
  public event Action OnAuthenticationCancelled;

  // Platform state
  private bool isAuthenticated = false;
  private string userEmail = "";
  private string platform = "";

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
      InitializePlatformAuth();
    }
  }

  private void InitializePlatformAuth()
  {
    if (!enablePlatformAuth)
    {
      Debug.Log("[PlatformAuthManager] Platform authentication disabled");
      return;
    }

    // Set platform based on build target
#if UNITY_ANDROID
        platform = "Android";
        Debug.Log("[PlatformAuthManager] Android platform detected");
#elif UNITY_IOS
        platform = "iOS";
        Debug.Log("[PlatformAuthManager] iOS platform detected");
#else
    platform = "Editor";
    Debug.Log("[PlatformAuthManager] Editor platform detected");
#endif

    // Initialize the appropriate platform for Unity's Social API
    InitializeSocialPlatform();
  }

  private void InitializeSocialPlatform()
  {
#if UNITY_ANDROID
    // Initialize Google Play Games (requires the plugin to be installed)
    try
    {
      // This assumes the Google Play Games plugin is installed and configured
      // The plugin should be initialized elsewhere in your game startup
      Debug.Log("[PlatformAuthManager] Google Play Games platform should be initialized");
    }
    catch (Exception e)
    {
      Debug.LogWarning($"[PlatformAuthManager] Google Play Games not available: {e.Message}");
    }
#elif UNITY_IOS
    // Game Center is built into Unity, no additional setup needed
    Debug.Log("[PlatformAuthManager] Game Center platform ready");
#endif
  }

  public async Task<bool> AuthenticateWithPlatform()
  {
    if (!enablePlatformAuth)
    {
      Debug.Log("[PlatformAuthManager] Platform authentication disabled, using fallback");
      return await AuthenticateFallback();
    }

    try
    {
      // Use Unity's Social API directly - this is the proper way!
      return await AuthenticateWithSocialAPI();
    }
    catch (Exception e)
    {
      Debug.LogError($"[PlatformAuthManager] Authentication failed: {e.Message}");
      OnAuthenticationFailed?.Invoke(e.Message);
      return false;
    }
  }

  private async Task<bool> AuthenticateWithSocialAPI()
  {
    Debug.Log("[PlatformAuthManager] Starting authentication with Unity Social API");

    // Check if Social platform is available
    if (Social.Active == null)
    {
      Debug.LogWarning("[PlatformAuthManager] No Social platform available, using fallback");
      return await AuthenticateFallback();
    }

    try
    {
      bool authenticationComplete = false;
      bool authenticationSuccess = false;

      // This is the proper Unity Social API call - works on all platforms!
      Social.localUser.Authenticate((bool success) =>
      {
        authenticationSuccess = success;
        authenticationComplete = true;
        Debug.Log($"[PlatformAuthManager] Social API authentication result: {success}");
      });

      // Wait for authentication to complete
      while (!authenticationComplete)
      {
        await Task.Delay(100);
      }

      if (authenticationSuccess)
      {
        // Get user info from Unity's Social API
        userEmail = GetUserEmailFromSocialAPI();
        isAuthenticated = true;

        Debug.Log($"[PlatformAuthManager] Authentication successful: {userEmail}");
        OnAuthenticationSuccess?.Invoke(userEmail, platform);
        return true;
      }
      else
      {
        Debug.LogWarning("[PlatformAuthManager] Social API authentication failed, using fallback");
        return await AuthenticateFallback();
      }
    }
    catch (Exception e)
    {
      Debug.LogError($"[PlatformAuthManager] Social API authentication error: {e.Message}");
      return await AuthenticateFallback();
    }
  }

  private string GetUserEmailFromSocialAPI()
  {
    try
    {
      if (Social.localUser.authenticated)
      {
        // Try to get email from the platform
        // Note: Unity's Social API doesn't guarantee email access
        // Different platforms may provide different information

        string userId = Social.localUser.id;
        string userName = Social.localUser.userName;

        // For now, create a platform-specific email format
        // In a real implementation, you might need platform-specific code to get actual email
        return $"{userId}@{platform.ToLower()}.user";
      }
    }
    catch (Exception e)
    {
      Debug.LogError($"[PlatformAuthManager] Failed to get user email: {e.Message}");
    }

    return "";
  }

  private async Task<bool> AuthenticateFallback()
  {
    Debug.Log("[PlatformAuthManager] Using fallback authentication");

    try
    {
      // Generate fallback email using device ID
      string deviceId = SystemInfo.deviceUniqueIdentifier;
      userEmail = $"user_{deviceId.Substring(0, Math.Min(8, deviceId.Length))}@{platform.ToLower()}.fallback.com";

      isAuthenticated = true;
      Debug.Log($"[PlatformAuthManager] Fallback authentication successful: {userEmail}");
      OnAuthenticationSuccess?.Invoke(userEmail, platform);
      return true;
    }
    catch (Exception e)
    {
      Debug.LogError($"[PlatformAuthManager] Fallback authentication failed: {e.Message}");
      OnAuthenticationFailed?.Invoke(e.Message);
      return false;
    }
  }

  // Public properties
  public bool IsAuthenticated => isAuthenticated;
  public string UserEmail => userEmail;
  public string Platform => platform;

  // Sign out method
  public void SignOut()
  {
    // Note: Unity's Social API doesn't have a SignOut method
    // Platform-specific sign out would need to be implemented separately
    // For now, just clear our local state

    isAuthenticated = false;
    userEmail = "";
    Debug.Log("[PlatformAuthManager] User signed out");
  }

  // Check if platform authentication is supported
  public bool IsPlatformSupported()
  {
    return Social.Active != null;
  }
}