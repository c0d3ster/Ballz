#pragma strict

//creates two buttons for main menu and try again
function OnGUI () {
  // Calculate button dimensions relative to screen size
  var buttonWidth = Screen.width * 0.35;    // 25% of screen width
  var buttonHeight = Screen.height * 0.15;   // 10% of screen height
  var verticalPosition = Screen.height * 0.7;  // 70% down the screen
  
  // Try Again button - positioned on the right
  if (GUI.Button (Rect (
    Screen.width * 0.55,  // 60% from left
    verticalPosition,
    buttonWidth,
    buttonHeight
  ), "Try Again")) {
    SceneLoader.LoadLastScene();
  }

  // Main Menu button - positioned on the left
  if (GUI.Button (Rect (
    Screen.width * 0.10,  // 15% from left
    verticalPosition,
    buttonWidth,
    buttonHeight
  ), "Main Menu")) {
    SceneLoader.ChangeScene("Active Main Menu");
  }
}

function Update () {

}