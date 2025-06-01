#pragma strict

var destination : Transform;
private var originalPosition : Vector3;
var moveSpeed : float = .5;
private var timer : float = 0;
 
function Start () {
	if (Options.diff != 0)
	{
		moveSpeed = moveSpeed / Options.diff;
	}
  originalPosition = transform.position;  // grabs original position
}
 
function Update () {
	timer += Time.deltaTime;
  transform.position = Vector3.Lerp(originalPosition, destination.position, Mathf.PingPong(timer * moveSpeed, 1.0));  //moves object back and forth
}


function OnTriggerEnter(other : Collider) {  //destroys player if it touches
  if (other.gameObject.CompareTag("Player")) {
    //add change color, loud sound, and explode then timer to load
    other.gameObject.SetActive (false);
    SceneLoader.ReloadScene();
  }
}