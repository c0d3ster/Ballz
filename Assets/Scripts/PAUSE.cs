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
        float leftButtonWidth = Screen.width * 0.25f;
        float rightButtonWidth = Screen.width * 0.3f; // Increased from 0.2f to 0.3f (30% of screen width)
        float buttonHeight = Screen.height * 0.15f; // 20% for control options
        float checkboxSize = buttonHeight * 0.4f;
        float rightPosition = Screen.width * 0.65f; // Adjusted from 0.75f to 0.65f to accommodate wider rect
        float leftPosition = Screen.width * 0.375f; // Right side position
        int boxPadding = 3; // Left side position
        // Create styles // Reduced padding around boxes
        GUIStyle style = GUI.skin.button;
        style.fontSize = (int) (buttonHeight * 0.2f);
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = (int) (buttonHeight * 0.2f);
        labelStyle.normal.textColor = Color.white;
        GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.fontSize = (int) (buttonHeight * 0.3f); // 50% bigger than normal text
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.white;
        headerStyle.hover = headerStyle.normal; // Prevent hover effect
        GUIStyle toggleStyle = new GUIStyle(GUI.skin.toggle);
        toggleStyle.fontSize = (int) (buttonHeight * 0.2f);
        toggleStyle.normal.textColor = Color.white;
        toggleStyle.onNormal.textColor = Color.white;
        toggleStyle.hover.textColor = Color.white;
        toggleStyle.onHover.textColor = Color.white;
        toggleStyle.active.textColor = Color.white;
        toggleStyle.onActive.textColor = Color.white;
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.textColor = Color.white;
        // Left side - existing menu buttons
        if (GUI.Button(new Rect(leftPosition, Screen.height * 0.2f, leftButtonWidth, buttonHeight), "Resume Play", style))
        {
            Time.timeScale = 1;
            SceneLoader.isPaused = false;
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("PAUSE");
        }
        if (GUI.Button(new Rect(leftPosition, Screen.height * 0.4f, leftButtonWidth, buttonHeight), "Restart Level", style))
        {
            Time.timeScale = 1;
            SceneLoader.isPaused = false;
            SceneLoader.ReloadScene();
        }
        if (GUI.Button(new Rect(leftPosition, Screen.height * 0.6f, leftButtonWidth, buttonHeight), "Change Difficulty: " + Optionz.DisplayDifficulty(), style))
        {
            Optionz.ChangeDifficulty();
        }
        if (GUI.Button(new Rect(leftPosition, Screen.height * 0.8f, leftButtonWidth, buttonHeight), "Main Menu", style))
        {
            Time.timeScale = 1;
            SceneLoader.isPaused = false;
            SceneLoader.ChangeScene("Active Main Menu");
        }
        // Right side - Control Optionz header
        GUI.Label(new Rect(rightPosition, Screen.height * 0.2f, rightButtonWidth, buttonHeight * 0.5f), "Control Optionz", headerStyle);
        
        float checkboxVerticalOffset = checkboxSize * 0.12f; // Add a small offset to vertically center checkboxes
        float boxHeight = checkboxSize * 0.85f; // Reduce box height to minimize bottom padding
        
        // Draw background boxes for better visibility with less padding
        GUI.Box(new Rect(rightPosition - boxPadding, (Screen.height * 0.3f) - boxPadding, rightButtonWidth + (boxPadding * 2), boxHeight + (boxPadding * 2)), "", boxStyle);
        GUI.Box(new Rect(rightPosition - boxPadding, (Screen.height * 0.4f) - boxPadding, rightButtonWidth + (boxPadding * 2), boxHeight + (boxPadding * 2)), "", boxStyle);
        GUI.Box(new Rect(rightPosition - boxPadding, (Screen.height * 0.5f) - boxPadding, rightButtonWidth + (boxPadding * 2), boxHeight + (boxPadding * 2)), "", boxStyle);
        
        Color originalColor = GUI.color;
        GUI.color = new Color(0, 0.5f, 0, 1); // Darker green (50% green)
        
        // Checkboxes for control options - now with vertical offset
        bool targetEnabled = GUI.Toggle(new Rect(rightPosition, Screen.height * 0.3f + checkboxVerticalOffset, rightButtonWidth, checkboxSize), Optionz.useTarget, "", toggleStyle);
        bool joystickEnabled = GUI.Toggle(new Rect(rightPosition, Screen.height * 0.4f + checkboxVerticalOffset, rightButtonWidth, checkboxSize), Optionz.useJoystick, "", toggleStyle);
        
        // Third option depends on platform
        bool accelEnabled = false;
        bool keyboardEnabled = false;
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            accelEnabled = GUI.Toggle(new Rect(rightPosition, Screen.height * 0.5f + checkboxVerticalOffset, rightButtonWidth, checkboxSize), Optionz.useAccelerometer, "", toggleStyle);
        }
        else
        {
            keyboardEnabled = GUI.Toggle(new Rect(rightPosition, Screen.height * 0.5f + checkboxVerticalOffset, rightButtonWidth, checkboxSize), Optionz.useKeyboard, "", toggleStyle);
        }
        
        GUI.color = originalColor;
        
        // Draw the labels - keep them at original vertical position for proper alignment
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
        
        // Update options after all toggles are drawn
        Optionz.useTarget = targetEnabled;
        Optionz.useJoystick = joystickEnabled;
        // Update platform-specific options
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            Optionz.useAccelerometer = accelEnabled;
            Optionz.useKeyboard = false; // Force off on mobile
        }
        else
        {
            Optionz.useKeyboard = keyboardEnabled;
            Optionz.useAccelerometer = false; // Force off on desktop
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
        }
    }
}