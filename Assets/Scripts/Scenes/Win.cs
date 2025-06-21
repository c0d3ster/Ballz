using UnityEngine;
using System.Collections;
using Enums;

[System.Serializable]
public partial class Win : MonoBehaviour
{
  private LivesManager livesManager;
  private AdManager adManager;
  private bool isOutOfLives = false;
  private bool adRequested = false;

  public virtual void Start()
  {
    Time.timeScale = 0;

    // Get references to managers
    livesManager = LivesManager.Instance;
    adManager = AdManager.Instance;

    // Check if player is out of lives
    isOutOfLives = livesManager != null && !livesManager.HasLives();

    // Subscribe to ad events if out of lives
    if (isOutOfLives && adManager != null)
    {
      adManager.OnAdCompleted += OnAdCompleted;
      adManager.OnAdFailed += OnAdFailed;
    }

    // Determine which game mode was completed based on the current scene
    GameMode? gameMode = SceneLoader.Instance.DetermineGameMode(SceneLoader.Instance.currentScene);
    if (gameMode.HasValue)
    {
      LevelProgressManager.Instance.CompleteLevel(SceneLoader.Instance.currentScene);
    }
  }

  private void OnDestroy()
  {
    // Unsubscribe from ad events
    if (adManager != null)
    {
      adManager.OnAdCompleted -= OnAdCompleted;
      adManager.OnAdFailed -= OnAdFailed;
    }
  }

  private void OnAdCompleted()
  {
    Debug.Log("[Win] Ad completed - adding lives!");
    if (livesManager != null)
    {
      livesManager.AddLivesViaAd(2);
    }
    adRequested = false;
  }

  private void OnAdFailed()
  {
    Debug.Log("[Win] Ad failed or skipped");
    adRequested = false;
  }

  public virtual void OnGUI()
  {
    // Calculate button dimensions relative to screen size
    float buttonWidth = Screen.width * 0.25f;
    float buttonHeight = Screen.height * 0.15f; // 15% of screen height
    float verticalPosition = Screen.height * 0.7f; // 70% down the screen

    // Create button style with larger text
    GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
    buttonStyle.fontSize = (int)(buttonWidth * 0.06f);

    // Create warning text style for out of lives
    GUIStyle warningStyle = new GUIStyle(GUI.skin.label);
    warningStyle.fontSize = (int)(Screen.height * 0.04f);
    warningStyle.alignment = TextAnchor.MiddleCenter;
    warningStyle.normal.textColor = Color.red;
    warningStyle.fontStyle = FontStyle.Bold;

    // Show warning if out of lives
    if (isOutOfLives)
    {
      string warningText = "You're out of lives! Watch an ad to continue.";
      GUI.Label(new Rect(0, Screen.height * 0.45f, Screen.width, Screen.height * 0.1f), warningText, warningStyle);
    }

    // Main Menu button - left position
    if (GUI.Button(new Rect(Screen.width * 0.08f, verticalPosition, buttonWidth, buttonHeight), "Main Menu", buttonStyle)) // 8% from left
    {
      Time.timeScale = 1;
      SceneLoader.Instance.ChangeScene("Active Main Menu");
    }

    // Try For Better Time button - center position
    if (GUI.Button(new Rect(Screen.width * 0.375f, verticalPosition, buttonWidth, buttonHeight), "Try For A Better Time", buttonStyle)) // 37.5% from left
    {
      Time.timeScale = 1;
      SceneLoader.Instance.ReloadScene();
    }

    // Next Level button - right position (modified based on lives status)
    string nextLevelText = isOutOfLives ? "Watch Ad to Continue" : "Next Level";
    bool canProceed = !isOutOfLives || livesManager.HasLives();

    if (GUI.Button(new Rect(Screen.width * 0.67f, verticalPosition, buttonWidth, buttonHeight), nextLevelText, buttonStyle)) // 67% from left
    {
      if (isOutOfLives && !adRequested)
      {
        // Request ad for lives
        adRequested = true;
        if (adManager != null)
        {
          adManager.RequestLivesViaAd();
        }
        else
        {
          Debug.LogWarning("[Win] AdManager not found!");
          adRequested = false;
        }
      }
      else if (canProceed)
      {
        // Proceed to next level
        Time.timeScale = 1;
        SceneLoader.Instance.NextLevel();
      }
    }

    // Disable Next Level button if out of lives and ad not available
    if (isOutOfLives && !canProceed && adManager != null && !adManager.CanShowAd())
    {
      GUI.enabled = false;
      GUI.Button(new Rect(Screen.width * 0.67f, verticalPosition, buttonWidth, buttonHeight), "Ad Not Available", buttonStyle);
      GUI.enabled = true;
    }
  }

  public virtual void Update()
  {
    // Update out of lives status
    if (livesManager != null)
    {
      isOutOfLives = !livesManager.HasLives();
    }
  }
}