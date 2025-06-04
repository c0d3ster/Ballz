#pragma strict

class UIManager extends MonoBehaviour {
  private static var instance : UIManager;
  
  // Reference to your touch controller canvas
  var touchControllerCanvas : Canvas;
  
  function Awake() {
    // Singleton pattern
    if (instance == null) {
      instance = this;
      DontDestroyOnLoad(gameObject);
      
      // Make sure canvas persists too
      if (touchControllerCanvas) {
        DontDestroyOnLoad(touchControllerCanvas.gameObject);
      }
    } else {
      // If an instance already exists, destroy this one
      Destroy(gameObject);
    }
  }
  
  // Function to toggle touch controller visibility
  static function ShowTouchController(show : boolean) {
    if (instance && instance.touchControllerCanvas) {
      instance.touchControllerCanvas.enabled = show;
    }
  }
}