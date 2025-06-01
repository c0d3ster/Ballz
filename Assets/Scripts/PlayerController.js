#pragma strict
import UnityEngine.UI;

var rb: Rigidbody;
var speed: float;
private var jump : boolean;
private var canJump : boolean;

var count: int;
var countText: Text;
private var totalBoxes: GameObject[];

var cam : Camera;
static var camOffset : Vector3;
var camShift : Vector3;

var dirOffset : float;

function Awake() {
    // Try to get camera reference immediately
    if (!cam) {
        cam = Camera.main;
        if (!cam) {
            Debug.LogError("No camera assigned and no Main Camera found!");
            enabled = false; // Disable this component if no camera
            return;
        }
    }
}

function Start() {
  // Get camera if not assigned
  if (!cam) {
    cam = Camera.main;
    if (!cam) {
      Debug.LogError("No camera assigned and no Main Camera found!");
      return;
    }
  }

  rb = GetComponent.<Rigidbody>();
  totalBoxes = GameObject.FindGameObjectsWithTag("Pick Up");  // gets total number of collectables on scene

  count = 0;
  if (countText) {
    countText.text = "";
    SetCountText();
  } else {
    Debug.LogWarning("No Count Text UI reference assigned to PlayerController!");
  }

  if (Options.diff != 0)
  {
    speed = speed / Options.diff; // makes player move slower if difficulty is low and vice versa
  }

  jump = false;
  canJump = true;
  var levelNumber = SceneLoader.GetLevelNumber();
  Debug.Log("Level Number: " + SceneLoader.GetLevelNumber());
  if (levelNumber == "1" || levelNumber == "u")
  { //disable jump for first set of levels and mennu
  	canJump = false;
  }


  //================= Starting Camera Position ==========//
  cam.transform.position = cam.transform.position + transform.position;
  camShift = cam.transform.position - transform.position;
  camOffset = Vector3(0, -2.5, 5);

  //================ Starting Accelerometer Position =====//
  dirOffset = Mathf.Clamp(Input.acceleration.y*2,-1,1);

}

//------------------------------------------------------------------------------
function Update() {
    if(Input.GetKeyDown(KeyCode.Escape)) { 
        if (!SceneLoader.isPaused) {
            Time.timeScale = 0;
            SceneLoader.isPaused = true;
            SceneLoader.Pause();
        } else {
            Time.timeScale = 1;
            SceneLoader.isPaused = false;
            SceneManager.UnloadSceneAsync("PAUSE");
        }
    }

	if (SystemInfo.deviceType == DeviceType.Desktop)
	{
		if (Input.GetKey("space") && jump == false && canJump == true) {
		  rb.AddForce(Vector3.up * 300);
			jump = true;
		}

	}
	else
	{
		if (Input.GetKey("space") && jump == false && canJump == true) {
		  rb.AddForce(Vector3.up * 300);
			jump = true;
		}

	}

	if (jump == true) {
    rb.AddForce(Vector3.down * 15);
  }
}

//------------------------------------------------------------------------------
function FixedUpdate () {

	//if (SystemInfo.deviceType == DeviceType.Desktop)
	//{
	  //axes set to WASD and arrows by default
	  var moveHorizontal: float = Input.GetAxis ("Horizontal");
	  var moveVertical: float = Input.GetAxis ("Vertical");

	  if (moveHorizontal != 0) {
	  	var vector : Vector3 = Vector3(cam.transform.right.x, 0, cam.transform.right.z);
	  	rb.AddForce(vector * moveHorizontal * speed);
	  }

	  if (moveVertical != 0) {
	  	var vector2 : Vector3 = Vector3(cam.transform.forward.x, 0, cam.transform.forward.z);
	  	rb.AddForce(vector2 * moveVertical * speed);
	  }
  //}
  //else
  //{
  	var dir : Vector3 = Vector3.zero;
  	//ensures acceleration value is between -1 and 1 but multiplies by a sensitivity variable (2)
  	//makes it easier for user to move around with mobile
  	dir.x = Mathf.Clamp(Input.acceleration.x*2,-1,1);
  	dir.z = Mathf.Clamp(Input.acceleration.y*2-dirOffset,-1,1);

		rb.AddForce(dir * speed);
 // }

 // if you fall off the map
 if (rb.transform.position.y <= -10) {
  if (SceneLoader.currentScene == "Active Main Menu") {
      SceneLoader.ChangeScene("Active Main Menu");
  } else {
      SceneLoader.GameOver();
  }
}
}

//------------------------------------------------------------------------------
function LateUpdate() {

	/*if (cam.transform.position != transform.position + camOffset) {
		cam.transform.RotateAround(transform.position, Vector3.up, 80*Time.deltaTime);
		cam.transform.LookAt(transform);
	}*/

	cam.transform.position = transform.position + camShift;
	cam.transform.LookAt(transform);
	cam.transform.position += camOffset;
}

//------------------------------------------------------------------------------
function OnTriggerEnter(other : Collider) {
  if (other.gameObject.CompareTag("Pick Up")) {
    other.gameObject.SetActive (false);
    count++;
    SetCountText();
  }
}

//------------------------------------------------------------------------------
function OnCollisionStay() {
	jump = false;
}

//------------------------------------------------------------------------------
function SetCountText()
{
  if (totalBoxes.length > 0 && countText)
  {
	  countText.text = "Count: " + count.ToString() + " out of " + totalBoxes.length;
	  //increase size of countText.text for mobile and various dimensions
	  if (count >= totalBoxes.length) {
	    SceneLoader.Win();
	  }
  }
}

