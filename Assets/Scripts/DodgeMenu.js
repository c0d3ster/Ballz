#pragma strict

var destination : Transform;
private var originalPosition : Vector3;
var moveSpeed : float = .5;
 
function Start () {
    originalPosition = transform.position;  // grabs original position
}
 
function Update () {
    transform.position = Vector3.Lerp(originalPosition, destination.position, Mathf.PingPong(Time.time * moveSpeed, 1.0));  //moves object back and forth
}


function OnTriggerEnter(other : Collider) {  //destroys player if it touches
  if (other.gameObject.CompareTag("Player")) {
    //add change color, loud sound, and explode then timer to load
    other.gameObject.SetActive (false);
    SceneLoader.ChangeScene("Active Main Menu");
  }
}