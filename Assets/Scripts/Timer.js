#pragma strict
import UnityEngine.UI;

//timer for game modes with time constraints
var timer: float = 15.0;
var timerText: Text;

// add options for difficulty
function Start () {
	if (Options.diff != 0)
	{
		timer = timer * Options.diff;
	}
  SetTimerText();
}

//
function Update () {
  if(timer > 0) {
    timer -= Time.deltaTime;
  }
  SetTimerText();
}

function SetTimerText(){
    timerText.text = "Time Remaining: " + Mathf.Round(timer * 100f) / 100f;
    if(timer < 0){
      SceneLoader.GameOver();
    }
}