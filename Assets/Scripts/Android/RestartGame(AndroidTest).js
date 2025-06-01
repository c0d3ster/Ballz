#pragma strict

function OnGUI () {
  if (GUI.Button (Rect (Screen.width / 2 - 80, Screen.height / 2 + 100, 160, 40), "Try Again")) {
    SceneLoader.ChangeScene("Ball Collector Level 1 (Android Test)");
  }
}

function Update () {

}