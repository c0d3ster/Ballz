#pragma strict

function OnTriggerEnter(other : Collider) {
  if (other.gameObject.CompareTag("Player")) {
    other.gameObject.SetActive (false);
    SceneLoader.ChangeScene("Ball Collector " + SceneLoader.collectCounter);
  }
}