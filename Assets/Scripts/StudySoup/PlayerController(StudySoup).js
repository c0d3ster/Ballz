#pragma strict
import UnityEngine.UI;

var rb: Rigidbody;
var speed: float;
private var totalBoxes: GameObject[];

var count: int;
var countText: Text;


function Start() {
  rb = GetComponent.<Rigidbody>();
  totalBoxes = GameObject.FindGameObjectsWithTag("Pick Up");  // gets total number of collectables on scene

  count = 0;
  SetCountText();
  SceneLoader.currentScene = "Ball Collector 1 (StudySoup)";
}

function FixedUpdate () {
  var moveHorizontal: float = Input.GetAxis ("Horizontal");
  var moveVertical: float = Input.GetAxis ("Vertical");

  var movement: Vector3 = new Vector3 (moveHorizontal, 0.0, moveVertical);

  rb.AddForce(movement * speed);

  if (rb.transform.position.y <= -10)
  {
  	SceneLoader.GameOver();
  }
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
  countText.text = "Count: " + count.ToString() + " out of " + totalBoxes.length;
  //increase size of countText.text for mobile and various dimensions
  if (count >= totalBoxes.length) {
    //add another scene (with options for next level try again and main menu), freeze time, increment counter
    SceneLoader.Win();
  }
}