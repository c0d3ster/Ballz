#pragma strict

function OnTriggerEnter(other : Collider) {
  if (other.gameObject.CompareTag("Pick Up")) {
    other.gameObject.SetActive (false);
    SceneLoader.ChangeScene("Ball Pusher " + SceneLoader.pushCounter);
  }
}