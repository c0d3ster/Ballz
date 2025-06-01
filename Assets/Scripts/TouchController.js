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
  
  function Start() {
    // Store the starting position of the inner circle
    startPos = innerCircle.anchoredPosition;
    // Calculate the maximum distance the joystick can move
    movementRadius = (outerCircle.sizeDelta.x - innerCircle.sizeDelta.x) / 2;
    UIManager.ShowTouchController(true);
  } 
  
  function Update() {
    if (SystemInfo.deviceType == DeviceType.Desktop) {
      if (Input.GetMouseButton(0)) {
        HandleMouseInput();
      } else {
        // Reflect keyboard input
        var keyHorizontal = Input.GetAxis("Horizontal");
        var keyVertical = Input.GetAxis("Vertical");
        if (keyHorizontal != 0 || keyVertical != 0) {
          moveDirection = new Vector2(keyHorizontal, keyVertical);
          innerCircle.anchoredPosition = moveDirection * movementRadius;
        } else {
          moveDirection = Vector2.zero;
          innerCircle.anchoredPosition = startPos;
        }
      }
    } else {
      if (Input.touchCount > 0) {
        HandleTouchInput();
      } else {
        // Reflect accelerometer input when not touching
        var accelX = Mathf.Clamp(Input.acceleration.x * 2, -1, 1);
        var accelY = Mathf.Clamp(Input.acceleration.y * 2, -1, 1);
        moveDirection = new Vector2(accelX, accelY);
        // Move inner circle to reflect accelerometer
        innerCircle.anchoredPosition = moveDirection * movementRadius;
      }
    }
  }
  
  function HandleMouseInput() {
    var mousePos : Vector2 = Input.mousePosition;
    var localPoint : Vector2;
    
    if (Input.GetMouseButton(0)) { // Left mouse button
      if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
        outerCircle, mousePos, null, localPoint)) {
        
        if (!isPressed) {
          isPressed = true;
          touchOffset = localPoint - innerCircle.anchoredPosition;
        }
        
        var newPos = localPoint - touchOffset;
        if (newPos.magnitude > movementRadius) {
          newPos = newPos.normalized * movementRadius;
        }
        
        innerCircle.anchoredPosition = newPos;
        moveDirection = newPos / movementRadius;
      }
    } else {
      isPressed = false;
      innerCircle.anchoredPosition = startPos;
      moveDirection = Vector2.zero;
    }
  }
  
  function HandleTouchInput() {
    var touch : Touch = Input.GetTouch(0);
    var touchPos : Vector2 = touch.position;
    
    // Convert touch position to local space
    var localPoint : Vector2;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
      outerCircle, touchPos, null, localPoint);
      
    switch (touch.phase) {
      case TouchPhase.Began:
        if (RectTransformUtility.RectangleContainsScreenPoint(
          outerCircle, touchPos, null)) {
          isPressed = true;
          touchOffset = localPoint - innerCircle.anchoredPosition;
        }
        break;
        
      case TouchPhase.Moved:
        if (isPressed) {
          var newPos = localPoint - touchOffset;
          // Clamp to circle
          if (newPos.magnitude > movementRadius) {
            newPos = newPos.normalized * movementRadius;
          }
          innerCircle.anchoredPosition = newPos;
          // Calculate movement direction (-1 to 1 range)
          moveDirection = newPos / movementRadius;
        }
        break;
        
      case TouchPhase.Ended:
      case TouchPhase.Canceled:
        isPressed = false;
        // Don't reset to center, let accelerometer take over
        break;
    }
  }
}