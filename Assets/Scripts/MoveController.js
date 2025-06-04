#pragma strict
import UnityEngine.UI;

class MoveController extends MonoBehaviour {
  // UI Elements
  var outerCircle : RectTransform;  // The background/boundary circle
  var innerCircle : RectTransform;  // The movable joystick circle
  
  // Control variables
  private var touchOffset : Vector2;
  private var isPressed : boolean = false;
  private var startPos : Vector2;
  
  // Movement limits
  private var movementRadius : float;
  static var moveDirection : Vector2 = Vector2.zero;  // Make static so PlayerController can access it
  
  private var startedInCircle : boolean = false;
  private var innerImage : Image;
  private var normalAlpha : float = 0.5;  // 50% opacity
  private var activeAlpha : float = 0.70; // 75% opacity
  
  function Start() {
    innerImage = innerCircle.GetComponent.<Image>();
    
    // Get player's material color and apply it to inner circle
    var player = GameObject.FindWithTag("Player");
    if (player) {
      var playerRenderer = player.GetComponent.<Renderer>();
      if (playerRenderer) {
        var playerColor = playerRenderer.material.color;
        var circleColor = innerImage.color;
        circleColor.r = playerColor.r;
        circleColor.g = playerColor.g;
        circleColor.b = playerColor.b;
        circleColor.a = normalAlpha;
        innerImage.color = circleColor;
      }
    }
    
    // Store the starting position of the inner circle
    startPos = innerCircle.anchoredPosition;
    // Calculate the maximum distance the joystick can move
    movementRadius = (outerCircle.sizeDelta.x - innerCircle.sizeDelta.x) / 2;
    
    // Set initial visibility based on platform and options
    UpdateControllerVisibility();
  }
  
  function Update() {
    if (SceneLoader.isPaused) {
      ResetMovement();
      return;
    }
    
    // Update joystick opacity based on movement
    UpdateJoystickOpacity();
    
    // Handle keyboard input first
    if (SystemInfo.deviceType == DeviceType.Desktop) {
      HandleKeyboardInput();
    }
    
    // Handle touch/mouse input (can override keyboard)
    if (SystemInfo.deviceType == DeviceType.Desktop) {
      if (Input.GetMouseButton(0)) {
        HandlePointerInput(Input.mousePosition);
      } else if (!Input.anyKey) {
        ResetMovement();
      }
    } else if (Input.touchCount > 0) {
      HandlePointerInput(Input.GetTouch(0).position);
    } else {
      ResetMovement();
    }
  }
  
  function UpdateControllerVisibility() {
    var showJoystick = Optionz.useJoystick;
    innerCircle.gameObject.SetActive(showJoystick);
    outerCircle.gameObject.SetActive(showJoystick);
  }
  
  function UpdateJoystickOpacity() {
    var color = innerImage.color;
    color.a = (moveDirection != Vector2.zero) ? activeAlpha : normalAlpha;
    innerImage.color = color;
  }
  
  function HandleKeyboardInput() {
    var horizontal = Input.GetAxis("Horizontal");
    var vertical = Input.GetAxis("Vertical");
    
    if (horizontal != 0 || vertical != 0) {
      moveDirection = new Vector2(horizontal, vertical);
      if (moveDirection.magnitude > 1) {
        moveDirection = moveDirection.normalized;
      }
      
      // Update joystick visual if enabled
      if (Optionz.useJoystick) {
        innerCircle.anchoredPosition = moveDirection * movementRadius;
      }
    }
  }
  
  function HandlePointerInput(pointerPos : Vector2) {
    var localPoint : Vector2;
    
    // Handle joystick input if enabled and pointer is in joystick area
    if (Optionz.useJoystick && RectTransformUtility.ScreenPointToLocalPointInRectangle(
      outerCircle, pointerPos, null, localPoint)) {
      
      if (!isPressed) {
        startedInCircle = RectTransformUtility.RectangleContainsScreenPoint(
          outerCircle, pointerPos, null);
      }
      
      if (startedInCircle) {
        isPressed = true;
        moveDirection = localPoint / movementRadius;
        if (moveDirection.magnitude > 1) {
          moveDirection = moveDirection.normalized;
        }
        innerCircle.anchoredPosition = moveDirection * movementRadius;
        return;
      }
    }
    
    // Handle touch/click input outside joystick area
    if (Optionz.useTouch) {
      var playerScreenPos = Camera.main.WorldToScreenPoint(GameObject.FindWithTag("Player").transform.position);
      var directionToPointer = new Vector2(pointerPos.x - playerScreenPos.x, pointerPos.y - playerScreenPos.y);
      var distance = directionToPointer.magnitude;
      var normalizedDistance = Mathf.Clamp01(distance / (Screen.height * 0.5));
      moveDirection = directionToPointer.normalized * normalizedDistance;
      
      // Update joystick visual if enabled
      if (Optionz.useJoystick) {
        innerCircle.anchoredPosition = moveDirection * movementRadius;
      }
    }
  }
  
  function ResetMovement() {
    isPressed = false;
    startedInCircle = false;
    moveDirection = Vector2.zero;
    innerCircle.anchoredPosition = startPos;
  }
}