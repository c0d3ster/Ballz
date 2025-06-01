#pragma strict

function OnTriggerEnter(other : Collider) {
  if (other.gameObject.CompareTag("Player")) {
  	// make delay for next level for flatten ball animation
    other.gameObject.SetActive (false);
    if (SceneLoader.currentScene == "Active Main Menu"){
    	SceneLoader.ChangeScene("Ball Balancer " + SceneLoader.balanceCounter);
    }
    else {
    	SceneLoader.Win();
    }
  }
}