#pragma strict

function Start () {
	StartCoroutine("Load");
}

function Load () {
  yield WaitForSeconds (1);
  SceneManager.LoadScene("Active Main Menu");
}