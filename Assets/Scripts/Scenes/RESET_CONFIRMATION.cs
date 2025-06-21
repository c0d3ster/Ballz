using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class RESET_CONFIRMATION : MonoBehaviour
{
  private void Start()
  {
    // No need to set up button listeners since we're using OnGUI
  }

  public virtual void OnGUI()
  {
    // Calculate button dimensions relative to screen size
    float buttonWidth = Screen.width * 0.35f;
    float buttonHeight = Screen.height * 0.15f; // 15% of screen height
    float verticalPosition = Screen.height * 0.7f; // 70% down the screen

    // Create button style with larger text
    GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
    buttonStyle.fontSize = (int)(buttonWidth * 0.06f);

    // Create title text style
    GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
    titleStyle.fontSize = (int)(Screen.height * 0.12f);
    titleStyle.alignment = TextAnchor.MiddleCenter;
    titleStyle.normal.textColor = Color.white;
    titleStyle.hover = titleStyle.normal;
    titleStyle.active = titleStyle.normal;

    // Create message text style
    GUIStyle messageStyle = new GUIStyle(GUI.skin.label);
    messageStyle.fontSize = (int)(Screen.height * 0.04f);
    messageStyle.alignment = TextAnchor.MiddleCenter;
    messageStyle.normal.textColor = Color.white;
    messageStyle.hover = messageStyle.normal;
    messageStyle.active = messageStyle.normal;

    // Create red confirm button style
    GUIStyle confirmButtonStyle = new GUIStyle(GUI.skin.button);
    confirmButtonStyle.fontSize = (int)(buttonWidth * 0.06f);
    confirmButtonStyle.normal.textColor = Color.white;
    confirmButtonStyle.hover.textColor = Color.white;
    confirmButtonStyle.active.textColor = Color.white;

    // Set red colors for confirm button
    ColorBlock colors = new ColorBlock();
    colors.normalColor = new Color(0.8f, 0.2f, 0.2f, 1f); // Red
    colors.highlightedColor = new Color(0.9f, 0.3f, 0.3f, 1f); // Lighter red
    colors.pressedColor = new Color(0.7f, 0.1f, 0.1f, 1f); // Darker red
    colors.selectedColor = new Color(0.8f, 0.2f, 0.2f, 1f);
    colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    colors.colorMultiplier = 1f;
    colors.fadeDuration = 0.1f;

    // Apply colors to button style
    confirmButtonStyle.normal.background = GUI.skin.button.normal.background;
    confirmButtonStyle.hover.background = GUI.skin.button.hover.background;
    confirmButtonStyle.active.background = GUI.skin.button.active.background;

    // Draw title text
    GUI.Label(new Rect(0, Screen.height * 0.25f, Screen.width, Screen.height * 0.15f), "Reset Progress", titleStyle);

    // Draw warning message
    string messageText = "Are you sure you want to reset all progress?\nThis will clear all level progress and lives data.\nThis action cannot be undone.";
    GUI.Label(new Rect(0, Screen.height * 0.4f, Screen.width, Screen.height * 0.2f), messageText, messageStyle);

    // Confirm button - positioned on the right (red)
    if (GUI.Button(new Rect(Screen.width * 0.55f, verticalPosition, buttonWidth, buttonHeight), "Confirm Reset", confirmButtonStyle))
    {
      Debug.Log("[RESET_CONFIRMATION] Reset confirmed - clearing all progress");

      // Trigger the actual reset functionality
      HotkeyManager.Instance.TriggerResetConfirmed();

      // Close the confirmation dialog
      UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("RESET_CONFIRMATION");
    }

    // Cancel button - positioned on the left
    if (GUI.Button(new Rect(Screen.width * 0.1f, verticalPosition, buttonWidth, buttonHeight), "Cancel", buttonStyle))
    {
      Debug.Log("[RESET_CONFIRMATION] Reset cancelled");

      // Close the confirmation dialog
      UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("RESET_CONFIRMATION");
    }
  }

  // Handle escape key to cancel
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      Debug.Log("[RESET_CONFIRMATION] Reset cancelled (Escape key)");

      // Close the confirmation dialog
      UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("RESET_CONFIRMATION");
    }
  }
}