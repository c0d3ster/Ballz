#pragma strict

function OnTriggerEnter(other : Collider) {
  if (other.gameObject.CompareTag("Player")) {
    // make delay for next level for ball up into portal/fade animation
    other.gameObject.SetActive (false);
    if (SceneLoader.currentScene == "Active Main Menu"){
    	SceneLoader.ChangeScene("Ball Dodger " + SceneLoader.dodgeCounter);
    	//SceneLoader.LevelSelect();
    }
    else {
    	SceneLoader.Win();
    }

  }
}