#pragma strict

function OnTriggerEnter(other : Collider) {
  if (other.gameObject.CompareTag("Player")) {
    // make delay for next level for expansion animation
    other.gameObject.SetActive (false);
    if (SceneLoader.currentScene == "Active Main Menu"){
    	SceneLoader.ChangeScene("Ball Jumper " + SceneLoader.jumpCounter);
    }
    else {
    	SceneLoader.Win();
    }
  }
}