#pragma strict
var speed : float;

function Update () {
  transform.Rotate (new Vector3 (0, speed, 0) * Time.deltaTime);
}

function OnTriggerEnter(other : Collider) {  //loads option screen if touched
  if (other.gameObject.CompareTag("Player") && this.gameObject.CompareTag("Optionz")) {
    SceneLoader.Pause();
  }
}