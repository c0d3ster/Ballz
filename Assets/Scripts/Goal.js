#pragma strict
import UnityEngine.UI;

var totalBoxes: GameObject[];
var count: int;
var countText: Text;


function Start() {
  totalBoxes = GameObject.FindGameObjectsWithTag("Pick Up");

  count = 0;
  SetCountText();
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
  if (count >= totalBoxes.length) {
  	
    SceneLoader.Win();
  }
}