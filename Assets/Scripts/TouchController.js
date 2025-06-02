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
    // Add this at the start of the Update function
    var color = innerImage.color;
    color.a = (moveDirection != Vector2.zero) ? activeAlpha : normalAlpha;
    innerImage.color = color;
    
    if (SystemInfo.deviceType == DeviceType.Desktop) {
      // Handle WASD input first
      var horizontal = Input.GetAxis("Horizontal");
      var vertical = Input.GetAxis("Vertical");
      
      if ((horizontal != 0 || vertical != 0) && !isPressed) {
        // Show WASD movement on joystick
        moveDirection = new Vector2(horizontal, vertical);
        if (moveDirection.magnitude > 1) {
          moveDirection = moveDirection.normalized;
        }
        innerCircle.anchoredPosition = moveDirection * movementRadius;
      } else if (!isPressed) {
        // Reset if no input and not being controlled by mouse
        moveDirection = Vector2.zero;
        innerCircle.anchoredPosition = startPos;
      }
      
      // Handle mouse input second (will override WASD if clicking)
      HandleMouseInput();
    } else {
      if (Input.touchCount > 0) {
        HandleTouchInput();
      } else {
        moveDirection = Vector2.zero;
        innerCircle.anchoredPosition = startPos;
      }
    }
  }
  
  function HandleMouseInput() {
    var mousePos : Vector2 = Input.mousePosition;
    var localPoint : Vector2;
    
    if (Input.GetMouseButton(0)) { // Left mouse button
      if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
        outerCircle, mousePos, null, localPoint)) {
        
        // If just starting to press
        if (!isPressed) {
          startedInCircle = RectTransformUtility.RectangleContainsScreenPoint(
            outerCircle, mousePos, null);
        }
        
        isPressed = true;
        if (startedInCircle) {
          // Use joystick-style movement if we started in the circle
          moveDirection = localPoint / movementRadius;
          if (moveDirection.magnitude > 1) {
            moveDirection = moveDirection.normalized;
          }
        } else {
          // Outside circle - calculate direction and magnitude relative to player position
          var playerScreenPos = Camera.main.WorldToScreenPoint(GameObject.FindWithTag("Player").transform.position);
          var directionToClick = mousePos - playerScreenPos;
          var distance = directionToClick.magnitude;
          var normalizedDistance = Mathf.Clamp01(distance / (Screen.height * 0.5));
          moveDirection = new Vector2(directionToClick.x, directionToClick.y).normalized * normalizedDistance;
        }
        innerCircle.anchoredPosition = moveDirection * movementRadius;
      }
    } else {
      isPressed = false;
      startedInCircle = false;
    }
  }
  
  function HandleTouchInput() {
    var touch : Touch = Input.GetTouch(0);
    var touchPos : Vector2 = touch.position;
    var localPoint : Vector2;
    
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
      outerCircle, touchPos, null, localPoint);
      
    switch (touch.phase) {
      case TouchPhase.Began:
        startedInCircle = RectTransformUtility.RectangleContainsScreenPoint(
          outerCircle, touchPos, null);
        if (startedInCircle) {
          isPressed = true;
          moveDirection = localPoint / movementRadius;
          if (moveDirection.magnitude > 1) {
            moveDirection = moveDirection.normalized;
          }
          innerCircle.anchoredPosition = moveDirection * movementRadius;
        }
        break;
        
      case TouchPhase.Moved:
        if (isPressed || !startedInCircle) {
          if (startedInCircle) {
            moveDirection = localPoint / movementRadius;
            if (moveDirection.magnitude > 1) {
              moveDirection = moveDirection.normalized;
            }
          } else {
            // Outside circle - calculate direction and magnitude relative to player position
            var playerScreenPos = Camera.main.WorldToScreenPoint(GameObject.FindWithTag("Player").transform.position);
            var directionToTouch = touchPos - playerScreenPos;
            var distance = directionToTouch.magnitude;
            var normalizedDistance = Mathf.Clamp01(distance / (Screen.height * 0.5));
            moveDirection = new Vector2(directionToTouch.x, directionToTouch.y).normalized * normalizedDistance;
          }
          innerCircle.anchoredPosition = moveDirection * movementRadius;
        }
        break;
        
      case TouchPhase.Ended:
      case TouchPhase.Canceled:
        isPressed = false;
        startedInCircle = false;
        moveDirection = Vector2.zero;
        innerCircle.anchoredPosition = startPos;
        break;
    }
  }
}