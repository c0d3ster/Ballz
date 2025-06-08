using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

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
    private float normalAlpha = 0.5f; // 50% opacity
    private float activeAlpha = 0.7f; // 75% opacity
    private bool isInitialized = false;

    void Awake()
    {
        InitializeController();
    }

    public virtual void Start()
    {
        InitializeController();
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

    private bool IsPointerOverUI()
    {
        // Check if pointer is over UI element
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            return EventSystem.current && EventSystem.current.IsPointerOverGameObject();
        }
        else if (Input.touchCount > 0)
        {
            return EventSystem.current && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
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

        // Handle keyboard input first if enabled
        if (Optionz.useKeyboard && (SystemInfo.deviceType == DeviceType.Desktop))
        {
            HandleKeyboardInput();
        }

        // Handle touch/mouse input (can override keyboard)
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetMouseButton(0))
            {
                HandlePointerInput(Input.mousePosition);
            }
            else if (!Input.anyKey || !Optionz.useKeyboard)
            {
                ResetMovement();
            }
        }
        else if (Input.touchCount > 0)
        {
            HandlePointerInput(Input.GetTouch(0).position);
        }
        else
        {
            ResetMovement();
        }
    }

    public virtual void HandlePointerInput(Vector2 pointerPos)
    {
        // Ignore input if paused
        if (SceneLoader.isPaused) return;

        if (!isInitialized || !outerCircle || !innerCircle) return;

        // If we haven't started input yet, determine the mode
        if (!isPressed)
        {
            Vector2 localPoint;
            usingJoystickMode = Optionz.useJoystick && 
                RectTransformUtility.ScreenPointToLocalPointInRectangle(outerCircle, pointerPos, null, out localPoint) &&
                RectTransformUtility.RectangleContainsScreenPoint(outerCircle, pointerPos, null);
            isPressed = true;
        }

        // Handle input based on the initial mode
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

    static MoveController()
    {
        moveDirection = Vector2.zero;
    }
}