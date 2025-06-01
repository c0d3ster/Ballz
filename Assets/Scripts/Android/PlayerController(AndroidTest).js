#pragma strict
import UnityEngine.UI;

var rb: Rigidbody;
var speed: float;
private var totalBoxes: GameObject[];

var count: int;
var countText: Text;
var winText: Text;


function Start() {
  rb = GetComponent.<Rigidbody>();
  totalBoxes = GameObject.FindGameObjectsWithTag("Pick Up");

  count = 0;
  countText.text = "";
  SetCountText();
  winText.text = "";
}

function FixedUpdate () {
		var dir : Vector3 = Vector3.zero;

		// we assume that device is held parallel to the ground
		// and Home button is in the right hand
		
		// remap device acceleration axis to game coordinates:
		//  1) XY plane of the device is mapped onto XZ plane
		//  2) rotated 90 degrees around Y axis
		dir.x = Input.acceleration.x;
		dir.z = Input.acceleration.y;
		Debug.Log(dir);
		
		// clamp acceleration vector to unit sphere
//		if (dir.sqrMagnitude > 1)
//			dir.Normalize();
		
		// Make it move 10 meters per second instead of 10 meters per frame...
		//dir *= Time.deltaTime;
			
		// Move object
		rb.AddForce(dir * speed);

}

function OnTriggerEnter(other : Collider) {
  if (other.gameObject.CompareTag("Pick Up")) {
    other.gameObject.SetActive (false);
    count++;
    SetCountText();
  }
}

function SetCountText()
{
  countText.text = "Count: " + count.ToString();
  if (count >= totalBoxes.length) {
    winText.text = "You Win!";
  }
}