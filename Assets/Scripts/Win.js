#pragma strict

function Start(){
  Time.timeScale = 0;
  SceneLoader.IncrementLevel();
}

//creates two buttons for main menu and try again
function OnGUI () {
  // Calculate button dimensions relative to screen size
  var buttonWidth = Screen.width * 0.25;    // 25% of screen width
  var buttonHeight = Screen.height * 0.15;   // 15% of screen height
  var verticalPosition = Screen.height * 0.7;  // 70% down the screen
  
  // Main Menu button - left position
  if (GUI.Button (Rect (
    Screen.width * 0.08,  // 8% from left
    verticalPosition,
    buttonWidth,
    buttonHeight
  ), "Main Menu")) {
    Time.timeScale = 1;
    if (SceneLoader.currentScene === "Ball Collector 1 (StudySoup)")
    {
    	SceneLoader.ReloadScene();
    }
    else
    {
    	SceneLoader.ChangeScene("Active Main Menu");
    }
  }

  // Try For Better Time button - center position
  if (GUI.Button (Rect (
    Screen.width * 0.375,  // 37.5% from left
    verticalPosition,
    buttonWidth,
    buttonHeight
  ), "Try For A Better Time")) {
    Time.timeScale = 1;
    SceneLoader.ReloadScene();
  }

  // Next Level button - right position
  if (GUI.Button (Rect (
    Screen.width * 0.67,  // 67% from left
    verticalPosition,
    buttonWidth,
    buttonHeight
  ), "Next Level")) {
    Time.timeScale = 1;
    SceneLoader.NextLevel();
  }
}

function Update () {

}