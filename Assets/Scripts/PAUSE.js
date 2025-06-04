#pragma strict
var heightUnit : float;
var widthUnit : float;


function Start(){
  Time.timeScale = 0;
  heightUnit = Screen.height / 10;
  widthUnit = Screen.width / 10;
}

//creates two buttons for main menu and try again
function OnGUI () {
  var leftButtonWidth = Screen.width * 0.25;    // 25% of screen width
  var rightButtonWidth = Screen.width * 0.2;    // 20% for control options
  var buttonHeight = Screen.height * 0.15;
  var checkboxSize = buttonHeight * 0.4;        // Reduced checkbox size
  var rightPosition = Screen.width * 0.75;      // Right side position
  var leftPosition = Screen.width * 0.375;      // Left side position
  var boxPadding = 3;                          // Reduced padding around boxes

  // Create styles
  var style = GUI.skin.button;
  style.fontSize = buttonHeight * 0.2;
  
  var labelStyle = new GUIStyle(GUI.skin.label);
  labelStyle.fontSize = buttonHeight * 0.2;
  labelStyle.normal.textColor = Color.white;

  var headerStyle = new GUIStyle(GUI.skin.label);
  headerStyle.fontSize = buttonHeight * 0.3;  // 50% bigger than normal text
  headerStyle.fontStyle = FontStyle.Bold;
  headerStyle.normal.textColor = Color.white;

  var toggleStyle = new GUIStyle(GUI.skin.toggle);
  toggleStyle.fontSize = buttonHeight * 0.2;
  toggleStyle.normal.textColor = Color.white;
  toggleStyle.onNormal.textColor = Color.white;
  toggleStyle.hover.textColor = Color.white;
  toggleStyle.onHover.textColor = Color.white;
  toggleStyle.active.textColor = Color.white;
  toggleStyle.onActive.textColor = Color.white;

  var boxStyle = new GUIStyle(GUI.skin.box);
  boxStyle.normal.textColor = Color.white;
  
  // Left side - existing menu buttons
  if (GUI.Button(Rect(leftPosition, Screen.height * 0.2, leftButtonWidth, buttonHeight), "Resume Play", style)) {
    Time.timeScale = 1;
    SceneLoader.isPaused = false;
    SceneManager.UnloadSceneAsync("PAUSE");
  }

  if (GUI.Button(Rect(leftPosition, Screen.height * 0.4, leftButtonWidth, buttonHeight), "Restart Level", style)) {
    Time.timeScale = 1;
    SceneLoader.isPaused = false;
    SceneLoader.ReloadScene();
  }

  if (GUI.Button(Rect(leftPosition, Screen.height * 0.6, leftButtonWidth, buttonHeight), "Change Difficulty: " + Optionz.DisplayDifficulty(), style)) {
    Optionz.ChangeDifficulty();
  }

  if (GUI.Button(Rect(leftPosition, Screen.height * 0.8, leftButtonWidth, buttonHeight), "Main Menu", style)) {
    Time.timeScale = 1;
    SceneLoader.isPaused = false;
    SceneLoader.ChangeScene("Active Main Menu");
  }

  // Right side - Control Optionz header
  GUI.Label(Rect(rightPosition, Screen.height * 0.2, rightButtonWidth, buttonHeight * 0.5), "Control Optionz", headerStyle);

  // Draw background boxes for better visibility with less padding
  GUI.Box(Rect(rightPosition - boxPadding, Screen.height * 0.3 - boxPadding, rightButtonWidth + (boxPadding * 2), checkboxSize + (boxPadding * 2)), "", boxStyle);
  GUI.Box(Rect(rightPosition - boxPadding, Screen.height * 0.4 - boxPadding, rightButtonWidth + (boxPadding * 2), checkboxSize + (boxPadding * 2)), "", boxStyle);
  GUI.Box(Rect(rightPosition - boxPadding, Screen.height * 0.5 - boxPadding, rightButtonWidth + (boxPadding * 2), checkboxSize + (boxPadding * 2)), "", boxStyle);

  var originalColor = GUI.color;
  GUI.color = new Color(0, 0.4, 0, 1);  // Darker green (40% green)
  
  // Checkboxes for control options
  var touchEnabled = GUI.Toggle(
    Rect(rightPosition, Screen.height * 0.3, rightButtonWidth, checkboxSize),
    Optionz.useTouch,
    "",
    toggleStyle
  );
  
  var accelEnabled = GUI.Toggle(
    Rect(rightPosition, Screen.height * 0.4, rightButtonWidth, checkboxSize),
    Optionz.useAccelerometer,
    "",
    toggleStyle
  );
  
  var joystickEnabled = GUI.Toggle(
    Rect(rightPosition, Screen.height * 0.5, rightButtonWidth, checkboxSize),
    Optionz.useJoystick,
    "",
    toggleStyle
  );
  
  GUI.color = originalColor;
  
  // Draw the labels
  GUI.Label(Rect(rightPosition + 20, Screen.height * 0.3, rightButtonWidth - 20, checkboxSize), "Touch", labelStyle);
  GUI.Label(Rect(rightPosition + 20, Screen.height * 0.4, rightButtonWidth - 20, checkboxSize), "Accelerometer", labelStyle);
  GUI.Label(Rect(rightPosition + 20, Screen.height * 0.5, rightButtonWidth - 20, checkboxSize), "Joystick", labelStyle);

  // Update options after all toggles are drawn
  Optionz.useTouch = touchEnabled;
  Optionz.useAccelerometer = accelEnabled;
  Optionz.useJoystick = joystickEnabled;

  // Ensure at least one control method is enabled
  if (!Optionz.useTouch && !Optionz.useAccelerometer && !Optionz.useJoystick) {
    Optionz.useTouch = true; // Default to touch if nothing selected
  }
}

function Update () {

}