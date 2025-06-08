using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class MoveController : MonoBehaviour
{
    // UI Elements
    public RectTransform outerCircle; // The background/boundary circle
    public RectTransform innerCircle; // The movable joystick circle
    // Control variables
    private Vector2 touchOffset;
    private bool isPressed;
    private Vector2 startPos;
    private bool usingJoystickMode; // Track which control mode we started with
    // Movement limits
    private float movementRadius;
    public static Vector2 moveDirection; // Make static so PlayerController can access it
    private Image innerImage;
    private Image outerImage;
    private float normalAlpha = 0.5f; // 50% opacity
    private float activeAlpha = 0.7f; // 75% opacity
    private bool isInitialized = false;
    
    // Accelerometer settings
    private Vector3 accelerometerRestPosition;
    private const float ACCELEROMETER_SENSITIVITY = 1.5f;

    private bool isClickingButton = false; // Track if current click/touch started on a button

    void Awake()
    {
        InitializeController();
    }

    void OnEnable()
    {
        // Subscribe to scene change events
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from scene change events
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Recalibrate immediately when any scene is loaded
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            CalibrateAccelerometer();
        }
    }

    private void CalibrateAccelerometer()
    {
        accelerometerRestPosition = Input.acceleration;
        Debug.Log($"Accelerometer calibrated to rest position: {accelerometerRestPosition}");
    }

    public virtual void Start()
    {
        InitializeController();
        // Initial calibration
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            CalibrateAccelerometer();
        }
    }

    private void InitializeController()
    {
        if (isInitialized) return;
        
        if (!outerCircle || !innerCircle)
        {
            Debug.LogWarning("MoveController missing circle references!");
            enabled = false;
            return;
        }

        innerImage = innerCircle.GetComponent<Image>();
        if (!innerImage)
        {
            Debug.LogWarning("Inner circle missing Image component!");
            enabled = false;
            return;
        }

        // Get player's material color and apply it to inner circle
        GameObject player = GameObject.FindWithTag("Player");
        if (player)
        {
            Renderer playerRenderer = player.GetComponent<Renderer>();
            if (playerRenderer)
            {
                Color playerColor = playerRenderer.material.color;
                Color circleColor = innerImage.color;
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
        // Add half the inner circle's size to allow it to extend halfway out
        movementRadius = (outerCircle.sizeDelta.x + innerCircle.sizeDelta.x) / 2;
        
        isInitialized = true;
    }

    private bool IsClickingButton()
    {
        if (EventSystem.current == null) return false;

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Check if initial click was on a button
                var pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                isClickingButton = results.Any(result => result.gameObject.GetComponent<Button>() != null);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isClickingButton = false;
            }
            return isClickingButton;
        }
        else if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // Check if initial touch was on a button
                var pointerData = new PointerEventData(EventSystem.current) { position = touch.position };
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                isClickingButton = results.Any(result => result.gameObject.GetComponent<Button>() != null);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isClickingButton = false;
            }
            return isClickingButton;
        }
        return false;
    }

    public virtual void Update()
    {
        if (!isInitialized)
        {
            InitializeController();
            if (!isInitialized) return;
        }

        // If paused, ignore all input
        if (SceneLoader.isPaused)
        {
            ResetMovement();
            return;
        }

        // Update joystick opacity based on movement
        if (Optionz.useJoystick)
        {
            UpdateJoystickOpacity();
        }

        bool isUsingKeyboard = false;
        // Handle keyboard input first if enabled
        if (Optionz.useKeyboard)
        {
            HandleKeyboardInput();
            isUsingKeyboard = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        }

        // Handle touch/mouse input if there is input (highest priority)
        bool usingTouchInput = false;
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetMouseButton(0) && !IsClickingButton())
            {
                HandlePointerInput(Input.mousePosition);
                usingTouchInput = true;
            }
        }
        else if (Input.touchCount == 1 && !IsClickingButton()) // Single touch for joystick/target
        {
            HandlePointerInput(Input.GetTouch(0).position);
            usingTouchInput = true;
        }
        else if (Input.touchCount == 2) // Two finger touch for accelerometer recalibration
        {
            if (Optionz.useAccelerometer)
            {
                CalibrateAccelerometer();
            }
        }

        // Handle accelerometer input if enabled on mobile and not using touch
        bool usingAccelerometer = false;
        if (Optionz.useAccelerometer && SystemInfo.deviceType == DeviceType.Handheld && !usingTouchInput)
        {
            HandleAccelerometerInput();
            usingAccelerometer = true;
        }

        // Reset movement if no input method is being used
        if (!isUsingKeyboard && !usingTouchInput && !usingAccelerometer)
        {
            ResetMovement();
        }
    }

    public virtual void HandlePointerInput(Vector2 pointerPos)
    {
        // Ignore input if paused
        if (SceneLoader.isPaused) return;

        if (!isInitialized || !outerCircle || !innerCircle) return;

        // First, determine if this is a joystick input
        if (!isPressed)
        {
            Vector2 localPoint;
            usingJoystickMode = Optionz.useJoystick && 
                RectTransformUtility.ScreenPointToLocalPointInRectangle(outerCircle, pointerPos, null, out localPoint) &&
                RectTransformUtility.RectangleContainsScreenPoint(outerCircle, pointerPos, null);
            isPressed = true;
        }

        // Handle input based on the mode
        if (usingJoystickMode)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(outerCircle, pointerPos, null, out localPoint))
            {
                // Calculate direction and magnitude
                moveDirection = localPoint / (movementRadius - innerCircle.sizeDelta.x / 2);
                float magnitude = moveDirection.magnitude;
                
                // Normalize if beyond max range
                if (magnitude > 1)
                {
                    moveDirection = moveDirection.normalized;
                }
                
                // Calculate the actual position of the inner circle
                Vector2 position = moveDirection * (movementRadius - innerCircle.sizeDelta.x / 2);
                innerCircle.anchoredPosition = position;
            }
        }
        else if (Optionz.useTarget)
        {
            // Handle target movement
            GameObject player = GameObject.FindWithTag("Player");
            if (!player) return;

            Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(player.transform.position);
            Vector2 directionToTarget = new Vector2(pointerPos.x - playerScreenPos.x, pointerPos.y - playerScreenPos.y);
            float distance = directionToTarget.magnitude;
            float normalizedDistance = Mathf.Clamp01(distance / (Screen.height * 0.33f));
            moveDirection = directionToTarget.normalized * normalizedDistance;

            // Update joystick visual if enabled
            if (Optionz.useJoystick)
            {
                innerCircle.anchoredPosition = moveDirection * (movementRadius - innerCircle.sizeDelta.x / 2);
            }
        }
    }

    public virtual void UpdateJoystickOpacity()
    {
        if (!isInitialized || !innerImage) return;

        Color color = innerImage.color;
        color.a = moveDirection != Vector2.zero ? activeAlpha : normalAlpha;
        innerImage.color = color;
    }

    public virtual void HandleKeyboardInput()
    {
        // Ignore input if paused
        if (SceneLoader.isPaused) return;

        if (!isInitialized) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (horizontal != 0 || vertical != 0)
        {
            moveDirection = new Vector2(horizontal, vertical);
            if (moveDirection.magnitude > 1)
            {
                moveDirection = moveDirection.normalized;
            }
            // Update joystick visual if enabled
            if (Optionz.useJoystick && innerCircle)
            {
                innerCircle.anchoredPosition = moveDirection * (movementRadius - innerCircle.sizeDelta.x / 2);
            }
        }
    }

    public virtual void ResetMovement()
    {
        if (!isInitialized) return;

        isPressed = false;
        usingJoystickMode = false;
        moveDirection = Vector2.zero;
        if (innerCircle)
        {
            innerCircle.anchoredPosition = startPos;
        }
    }

    public virtual void HandleAccelerometerInput()
    {
        // Get accelerometer data relative to rest position
        Vector3 currentTilt = Input.acceleration;
        Vector3 relativeTilt = currentTilt - accelerometerRestPosition;
        
        // Convert acceleration to movement direction
        // Apply sensitivity multiplier
        moveDirection = new Vector2(relativeTilt.x, relativeTilt.y) * ACCELEROMETER_SENSITIVITY;
        
        // Normalize if magnitude is greater than 1
        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }
        
        // Update joystick visual if enabled
        if (Optionz.useJoystick && innerCircle)
        {
            innerCircle.anchoredPosition = moveDirection * (movementRadius - innerCircle.sizeDelta.x / 2);
        }
    }

    static MoveController()
    {
        moveDirection = Vector2.zero;
    }
}