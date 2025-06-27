using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class PAUSE : MonoBehaviour
{
  public float heightUnit;
  public float widthUnit;
  public virtual void Start()
  {
    Time.timeScale = 0;
    this.heightUnit = Screen.height / 10;
    this.widthUnit = Screen.width / 10;
  }

  //creates two buttons for main menu and try again
  public virtual void OnGUI()
  {
    float middleButtonWidth = Screen.width * 0.25f;
    float rightButtonWidth = Screen.width * 0.3f; // Increased from 0.2f to 0.3f (30% of screen width)
    float buttonHeight = Screen.height * 0.15f; // 20% for control options
    float checkboxSize = buttonHeight * 0.4f; // Back to original size since we're not using radio buttons
    float rightPosition = Screen.width * 0.65f; // Adjusted from 0.75f to 0.65f to accommodate wider rect
    float middlePosition = Screen.width * 0.375f; // Right side position
    int boxPadding = 3; // Left side position

    // Define colors for control options
    Color darkGreen = new Color(0, 0.65f, 0, 1);
    Color darkRed = new Color(0.65f, 0, 0, 1);

    // Create styles // Reduced padding around boxes
    GUIStyle style = GUI.skin.button;
    style.fontSize = (int)(middleButtonWidth * 0.06f);

    GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
    labelStyle.fontSize = (int)(rightButtonWidth * 0.05f);
    labelStyle.normal.textColor = Color.white;

    GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
    headerStyle.fontSize = (int)(buttonHeight * 0.3f); // 50% bigger than normal text
    headerStyle.fontStyle = FontStyle.Bold;
    headerStyle.normal.textColor = Color.white;
    headerStyle.hover = headerStyle.normal; // Prevent hover effect

    // Create button styles for control options with gradient and hover effects
    GUIStyle enabledButtonStyle = new GUIStyle(GUI.skin.button);
    enabledButtonStyle.fontSize = (int)(checkboxSize * 0.3f);
    enabledButtonStyle.normal.textColor = Color.white;
    enabledButtonStyle.hover.textColor = Color.white;
    enabledButtonStyle.active.textColor = Color.white;

    // Set darker green colors for enabled state with gradient effect
    enabledButtonStyle.normal.background = GUI.skin.button.normal.background;
    enabledButtonStyle.hover.background = GUI.skin.button.hover.background;
    enabledButtonStyle.active.background = GUI.skin.button.active.background;

    GUIStyle disabledButtonStyle = new GUIStyle(GUI.skin.button);
    disabledButtonStyle.fontSize = (int)(checkboxSize * 0.3f);
    disabledButtonStyle.normal.textColor = Color.white;
    disabledButtonStyle.hover.textColor = Color.white;
    disabledButtonStyle.active.textColor = Color.white;

    // Set darker red colors for disabled state with gradient effect
    disabledButtonStyle.normal.background = GUI.skin.button.normal.background;
    disabledButtonStyle.hover.background = GUI.skin.button.hover.background;
    disabledButtonStyle.active.background = GUI.skin.button.active.background;

    // middle - existing menu buttons
    if (GUI.Button(new Rect(middlePosition, Screen.height * 0.2f, middleButtonWidth, buttonHeight), "Resume Play", style))
    {
      Time.timeScale = 1;
      SceneLoader.Instance.isPaused = false;
      UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("PAUSE");
    }
    if (GUI.Button(new Rect(middlePosition, Screen.height * 0.4f, middleButtonWidth, buttonHeight), "Restart Level", style))
    {
      Time.timeScale = 1;
      SceneLoader.Instance.isPaused = false;
      SceneLoader.Instance.ReloadScene();
    }
    if (GUI.Button(new Rect(middlePosition, Screen.height * 0.6f, middleButtonWidth, buttonHeight), "Change Difficulty: " + Optionz.DisplayDifficulty(), style))
    {
      Optionz.ChangeDifficulty();
    }
    if (GUI.Button(new Rect(middlePosition, Screen.height * 0.8f, middleButtonWidth, buttonHeight), "Main Menu", style))
    {
      Time.timeScale = 1;
      SceneLoader.Instance.isPaused = false;
      SceneLoader.Instance.LoadMainMenu();
    }
    // Right side - Control Optionz header
    GUI.Label(new Rect(rightPosition, Screen.height * 0.2f, rightButtonWidth, buttonHeight * 0.5f), "Control Optionz", headerStyle);

    float boxHeight = checkboxSize * 0.85f; // Reduce box height to minimize bottom padding

    // Create clickable buttons for each option (no visual checkbox, just clickable areas)
    bool targetEnabled = Optionz.useTarget;
    bool joystickEnabled = Optionz.useJoystick;
    bool accelEnabled = Optionz.useAccelerometer;
    bool keyboardEnabled = Optionz.useKeyboard;

    // Target option - use proper button styling with gradient and hover effects
    Color originalColor = GUI.color;
    GUI.color = targetEnabled ? darkGreen : darkRed;
    if (GUI.Button(new Rect(rightPosition - boxPadding, (Screen.height * 0.3f) - boxPadding, rightButtonWidth + (boxPadding * 2), boxHeight + (boxPadding * 2)), "", targetEnabled ? enabledButtonStyle : disabledButtonStyle))
    {
      targetEnabled = !targetEnabled;
      Debug.Log($"[PAUSE] Target toggled to: {targetEnabled}");
    }

    // Joystick option
    GUI.color = joystickEnabled ? darkGreen : darkRed;
    if (GUI.Button(new Rect(rightPosition - boxPadding, (Screen.height * 0.4f) - boxPadding, rightButtonWidth + (boxPadding * 2), boxHeight + (boxPadding * 2)), "", joystickEnabled ? enabledButtonStyle : disabledButtonStyle))
    {
      joystickEnabled = !joystickEnabled;
      Debug.Log($"[PAUSE] Joystick toggled to: {joystickEnabled}");
    }

    // Third option depends on platform
    if (SystemInfo.deviceType == DeviceType.Handheld)
    {
      GUI.color = accelEnabled ? darkGreen : darkRed;
      if (GUI.Button(new Rect(rightPosition - boxPadding, (Screen.height * 0.5f) - boxPadding, rightButtonWidth + (boxPadding * 2), boxHeight + (boxPadding * 2)), "", accelEnabled ? enabledButtonStyle : disabledButtonStyle))
      {
        accelEnabled = !accelEnabled;
        Debug.Log($"[PAUSE] Accelerometer toggled to: {accelEnabled}");
      }
    }
    else
    {
      GUI.color = keyboardEnabled ? darkGreen : darkRed;
      if (GUI.Button(new Rect(rightPosition - boxPadding, (Screen.height * 0.5f) - boxPadding, rightButtonWidth + (boxPadding * 2), boxHeight + (boxPadding * 2)), "", keyboardEnabled ? enabledButtonStyle : disabledButtonStyle))
      {
        keyboardEnabled = !keyboardEnabled;
        Debug.Log($"[PAUSE] Keyboard toggled to: {keyboardEnabled}");
      }
    }

    GUI.color = originalColor; // Reset color

    // Draw the labels
    GUI.Label(new Rect(rightPosition + 20, Screen.height * 0.3f, rightButtonWidth - 20, checkboxSize), "Target", labelStyle);
    GUI.Label(new Rect(rightPosition + 20, Screen.height * 0.4f, rightButtonWidth - 20, checkboxSize), "Joystick", labelStyle);

    // Third option label depends on platform
    if (SystemInfo.deviceType == DeviceType.Handheld)
    {
      GUI.Label(new Rect(rightPosition + 20, Screen.height * 0.5f, rightButtonWidth - 20, checkboxSize), "Accelerometer", labelStyle);
    }
    else
    {
      GUI.Label(new Rect(rightPosition + 20, Screen.height * 0.5f, rightButtonWidth - 20, checkboxSize), "Keyboard", labelStyle);
    }

    // If joystick state changed
    if (joystickEnabled != Optionz.useJoystick)
    {
      Optionz.useJoystick = joystickEnabled;
      // Find and update both controller objects
      GameObject outer = GameObject.Find("TouchControllerOuter");
      GameObject inner = GameObject.Find("TouchControllerInner");
      if (outer && inner)
      {
        // Get the Image components
        UnityEngine.UI.Image outerImage = outer.GetComponent<UnityEngine.UI.Image>();
        UnityEngine.UI.Image innerImage = inner.GetComponent<UnityEngine.UI.Image>();
        if (outerImage && innerImage)
        {
          // Store current colors
          Color outerColor = outerImage.color;
          Color innerColor = innerImage.color;
          // Update alpha but keep other color values
          outerColor.a = joystickEnabled ? 0.5f : 0;
          innerColor.a = joystickEnabled ? 0.5f : 0;
          // Apply colors
          outerImage.color = outerColor;
          innerImage.color = innerColor;
          // Update raycast targets
          outerImage.raycastTarget = joystickEnabled;
          innerImage.raycastTarget = joystickEnabled;
          Debug.Log("Setting TouchController opacity to: " + (joystickEnabled ? "visible" : "invisible"));
        }
      }
      else
      {
        Debug.LogWarning("TouchController objects not found!");
      }
    }

    // Update options after all buttons are processed
    bool settingsChanged = false;
    if (targetEnabled != Optionz.useTarget)
    {
      Optionz.useTarget = targetEnabled;
      settingsChanged = true;
    }
    if (joystickEnabled != Optionz.useJoystick)
    {
      Optionz.useJoystick = joystickEnabled;
      settingsChanged = true;
    }

    // Update platform-specific options
    if (SystemInfo.deviceType == DeviceType.Handheld)
    {
      if (accelEnabled != Optionz.useAccelerometer)
      {
        Optionz.useAccelerometer = accelEnabled;
        settingsChanged = true;
      }
      Optionz.useKeyboard = false; // Force off on mobile
    }
    else
    {
      if (keyboardEnabled != Optionz.useKeyboard)
      {
        Optionz.useKeyboard = keyboardEnabled;
        settingsChanged = true;
      }
      Optionz.useAccelerometer = false; // Force off on desktop
    }

    // Save settings if any changed
    if (settingsChanged)
    {
      Optionz.SaveControlSettings();
    }

    // Ensure at least one control method is enabled
    if (((!Optionz.useTarget && !Optionz.useAccelerometer) && !Optionz.useJoystick) && !Optionz.useKeyboard)
    {
      if (SystemInfo.deviceType == DeviceType.Handheld)
      {
        Optionz.useTarget = true; // Default to target on mobile
      }
      else
      {
        Optionz.useKeyboard = true; // Default to keyboard on desktop
      }
      Optionz.SaveControlSettings(); // Save the enforced default
    }
  }
}