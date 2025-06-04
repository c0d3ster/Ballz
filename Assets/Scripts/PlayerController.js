#pragma strict
import UnityEngine.UI;

class PlayerController extends MonoBehaviour {
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
    }
    
    if (Options.diff != 0) {
      speed = speed / Options.diff; // makes player move slower if difficulty is low and vice versa
    }
    
    jump = false;
    canJump = true;
    var levelNumber = SceneLoader.GetLevelNumber();
    Debug.Log("Level Number: " + levelNumber);
    if (levelNumber <= 1) {
      canJump = false;
    }
    
    //================= Starting Camera Position ==========//
    cam.transform.position = cam.transform.position + transform.position;
    camShift = cam.transform.position - transform.position;
    camOffset = Vector3(0, -2.5, 5);
  }
  
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
    
    // Handle jumping
    if (Input.GetKey("space") && !jump && canJump) {
      rb.AddForce(Vector3.up * 300);
      jump = true;
    }
    
    if (jump) {
      rb.AddForce(Vector3.down * 15);
    }
  }
  
  function FixedUpdate() {
    // Get movement direction from MoveController
    var moveDirection = MoveController.moveDirection;
    
    // Apply movement force
    var movement = new Vector3(moveDirection.x, 0.0, moveDirection.y);
    rb.AddForce(movement * speed);
    
    // Check if player fell off the map
    if (rb.transform.position.y <= -10) {
      if (!SceneLoader.currentScene || SceneLoader.currentScene == "Active Main Menu") {
        SceneLoader.ChangeScene("Active Main Menu");
      } else {
        SceneLoader.GameOver();
      }
    }
  }
  
  function LateUpdate() {
    cam.transform.position = transform.position + camShift;
    cam.transform.LookAt(transform);
    cam.transform.position += camOffset;
  }
  
  function OnTriggerEnter(other: Collider) {
    if (other.gameObject.CompareTag("Pick Up")) {
      other.gameObject.SetActive(false);
      count = count + 1;
      SetCountText();
    }
    
    if (other.gameObject.CompareTag("Ground")) {
      jump = false;
    }
  }
  
  function SetCountText() {
    countText.text = "Count: " + count.ToString() + "/" + totalBoxes.Length.ToString();
    
    if (count >= totalBoxes.Length) {
      SceneLoader.Win();
    }
  }
}