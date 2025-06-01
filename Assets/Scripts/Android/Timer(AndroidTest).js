#pragma strict
import UnityEngine.UI;

var timer: float = 15.0;
var timerText: Text;

function Start () {
  SetTimerText();
}

function Update () {
  if(timer > 0) {
    timer -= Time.deltaTime;
  }
  SetTimerText();
}

function SetTimerText(){
    timerText.text = "Time Remaining: " + timer;

    if(timer < 0){
      SceneLoader.ChangeScene("GAME OVER (Android Test)");
    }
}