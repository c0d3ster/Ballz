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
  var buttonWidth = Screen.width * 0.25;    // 25% of screen width
  var buttonHeight = Screen.height * 0.15;   // 15% of screen height
  var verticalSpacing = Screen.height * 0.2; // 20% of screen height between buttons
  var horizontalPosition = Screen.width * 0.375; // Centered at 37.5% from left

  // Create button style with proportional font size
  var style = GUI.skin.button;
  style.fontSize = buttonHeight * 0.2; // Font size is 15% of button height

  if (GUI.Button (Rect (horizontalPosition, Screen.height * 0.2, buttonWidth, buttonHeight), "Resume Play", style)) {
    Time.timeScale = 1;
    SceneLoader.isPaused = false;
    SceneManager.UnloadSceneAsync("PAUSE");
  }

  if (GUI.Button (Rect (horizontalPosition, Screen.height * 0.4, buttonWidth, buttonHeight), "Restart Level", style)) {
    Time.timeScale = 1;
    SceneLoader.isPaused = false;
    SceneLoader.ReloadScene();
  }

  if (GUI.Button (Rect (horizontalPosition, Screen.height * 0.6, buttonWidth, buttonHeight), "Change Difficulty: " + Options.DisplayDifficulty(), style)) {
    Options.ChangeDifficulty();
  }

  if (GUI.Button (Rect (horizontalPosition, Screen.height * 0.8, buttonWidth, buttonHeight), "Main Menu", style)) {
    Time.timeScale = 1;
    SceneLoader.isPaused = false;
    SceneLoader.ChangeScene("Active Main Menu");
  }
}

function Update () {

}