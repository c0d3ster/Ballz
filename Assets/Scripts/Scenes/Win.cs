using UnityEngine;
using System.Collections;
using Enums;

[System.Serializable]
public partial class Win : MonoBehaviour
{
  public virtual void Start()
  {
    Time.timeScale = 0;

    // Determine which game mode was completed based on the current scene
    GameMode? gameMode = SceneLoader.Instance.DetermineGameMode(SceneLoader.Instance.currentScene);
    if (gameMode.HasValue)
    {
      LevelProgressManager.Instance.CompleteLevel(SceneLoader.Instance.currentScene);
    }
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

    // Next Level button - right position
    if (GUI.Button(new Rect(Screen.width * 0.67f, verticalPosition, buttonWidth, buttonHeight), "Next Level", buttonStyle)) // 67% from left
    {
      Time.timeScale = 1;
      SceneLoader.Instance.NextLevel();
    }
  }
}