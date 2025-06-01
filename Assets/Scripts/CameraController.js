#pragma strict

var player: GameObject;
var offset: Vector3;
var lower: Vector3;

function Start () {
  lower.z = -5;  //shows less of behind the player object
  lower.y = 2.5; //gets camera closer to player object
  transform.position = transform.position + player.transform.position;  //adjusts camera for non-zero starting position on player
  offset = transform.position - player.transform.position - lower;  //creates the initial offset distance from player
}

function LateUpdate () {
  transform.position = player.transform.position + offset;
}