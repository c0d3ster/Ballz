#pragma strict
import UnityEngine.UI;

class TouchController extends MonoBehaviour {
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
  
  // Add this with the other private variables at the top
  private var startedInCircle : boolean = false;
  
  // Add these with the other variables at the top
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
    UIManager.ShowTouchController(true);
  } 
  
  function Update() {
    // Don't process any input if paused
    if (SceneLoader.isPaused) {
      ResetJoystick();
      return;
    }
    
    // Update joystick visual feedback
    var color = innerImage.color;
    color.a = (moveDirection != Vector2.zero) ? activeAlpha : normalAlpha;
    innerImage.color = color;
    
    // Show/hide joystick based on useJoystick option
    var show = Optionz.useJoystick;
    innerCircle.gameObject.SetActive(show);
    outerCircle.gameObject.SetActive(show);
    
    if (SystemInfo.deviceType == DeviceType.Desktop) {
      HandleMouseInput();
    } else {
      HandleTouchInput();
    }
  }
  
  function HandleMouseInput() {
    var mousePos : Vector2 = Input.mousePosition;
    var localPoint : Vector2;
    
    if (Input.GetMouseButton(0)) {
      if (Optionz.useJoystick) {
        // Check if input started in joystick area
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
          outerCircle, mousePos, null, localPoint)) {
          
          if (!isPressed) {
            startedInCircle = RectTransformUtility.RectangleContainsScreenPoint(
              outerCircle, mousePos, null);
          }
          
          if (startedInCircle) {
            isPressed = true;
            moveDirection = localPoint / movementRadius;
            if (moveDirection.magnitude > 1) {
              moveDirection = moveDirection.normalized;
            }
          }
        }
      }
      
      // Handle touch-style input if not using joystick or clicked outside joystick
      if (Optionz.useTouch && !startedInCircle) {
        var playerScreenPos = Camera.main.WorldToScreenPoint(GameObject.FindWithTag("Player").transform.position);
        var directionToClick = new Vector2(mousePos.x - playerScreenPos.x, mousePos.y - playerScreenPos.y);
        var distance = directionToClick.magnitude;
        var normalizedDistance = Mathf.Clamp01(distance / (Screen.height * 0.5));
        moveDirection = directionToClick.normalized * normalizedDistance;
      }
      
      // Always update joystick position to reflect current movement
      if (moveDirection != Vector2.zero) {
        innerCircle.anchoredPosition = moveDirection * movementRadius;
      }
    } else {
      ResetJoystick();
    }
  }
  
  function HandleTouchInput() {
    var touch = Input.GetTouch(0);
    var touchPos : Vector2 = touch.position;
    var localPoint : Vector2;
    
    if (Optionz.useJoystick) {
      // Check if touch started in joystick area
      if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
        outerCircle, touchPos, null, localPoint)) {
        
        if (!isPressed) {
          startedInCircle = RectTransformUtility.RectangleContainsScreenPoint(
            outerCircle, touchPos, null);
        }
        
        if (startedInCircle) {
          isPressed = true;
          moveDirection = localPoint / movementRadius;
          if (moveDirection.magnitude > 1) {
            moveDirection = moveDirection.normalized;
          }
        }
      }
    }
    
    // Handle touch-style input if not using joystick or touched outside joystick
    if (Optionz.useTouch && !startedInCircle) {
      var playerScreenPos = Camera.main.WorldToScreenPoint(GameObject.FindWithTag("Player").transform.position);
      var directionToTouch = new Vector2(touchPos.x - playerScreenPos.x, touchPos.y - playerScreenPos.y);
      var distance = directionToTouch.magnitude;
      var normalizedDistance = Mathf.Clamp01(distance / (Screen.height * 0.5));
      moveDirection = directionToTouch.normalized * normalizedDistance;
    }
    
    // Always update joystick position to reflect current movement
    if (moveDirection != Vector2.zero) {
      innerCircle.anchoredPosition = moveDirection * movementRadius;
    }
    
    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
      ResetJoystick();
    }
  }
  
  function ResetJoystick() {
    isPressed = false;
    startedInCircle = false;
    moveDirection = Vector2.zero;
    innerCircle.anchoredPosition = startPos;
  }
}