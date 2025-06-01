#pragma strict

function Update () {
  transform.Rotate (new Vector3 (0, Random.Range(15, 45), 0) * Time.deltaTime);
}