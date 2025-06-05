using UnityEngine.UI;
using UnityEngine;
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
    // Movement limits
    private float movementRadius;
    public static Vector2 moveDirection; // Make static so PlayerController can access it
    private bool startedInCircle;
    private Image innerImage;
    private float normalAlpha; // 50% opacity
    private float activeAlpha; // 75% opacity
    public virtual void Start()
    {
        this.innerImage = this.innerCircle.GetComponent<Image>();
        // Get player's material color and apply it to inner circle
        GameObject player = GameObject.FindWithTag("Player");
        if (player)
        {
            Renderer playerRenderer = player.GetComponent<Renderer>();
            if (playerRenderer)
            {
                Color playerColor = playerRenderer.material.color;
                Color circleColor = this.innerImage.color;
                circleColor.r = playerColor.r;
                circleColor.g = playerColor.g;
                circleColor.b = playerColor.b;
                circleColor.a = this.normalAlpha;
                this.innerImage.color = circleColor;
            }
        }
        // Store the starting position of the inner circle
        this.startPos = this.innerCircle.anchoredPosition;
        // Calculate the maximum distance the joystick can move
        this.movementRadius = (this.outerCircle.sizeDelta.x - this.innerCircle.sizeDelta.x) / 2;
    }

    public virtual void Update()
    {
        if (SceneLoader.isPaused)
        {
            this.ResetMovement();
            return;
        }
        // Update joystick opacity based on movement
        if (Optionz.useJoystick)
        {
            this.UpdateJoystickOpacity();
        }
        // Handle keyboard input first if enabled
        if (Optionz.useKeyboard && (SystemInfo.deviceType == DeviceType.Desktop))
        {
            this.HandleKeyboardInput();
        }
        // Handle touch/mouse input (can override keyboard)
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetMouseButton(0))
            {
                this.HandlePointerInput(Input.mousePosition);
            }
            else
            {
                if (!Input.anyKey || !Optionz.useKeyboard)
                {
                    this.ResetMovement();
                }
            }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                this.HandlePointerInput(Input.GetTouch(0).position);
            }
            else
            {
                this.ResetMovement();
            }
        }
    }

    public virtual void UpdateJoystickOpacity()
    {
        Color color = this.innerImage.color;
        color.a = MoveController.moveDirection != Vector2.zero ? this.activeAlpha : this.normalAlpha;
        this.innerImage.color = color;
    }

    public virtual void HandleKeyboardInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if ((horizontal != 0) || (vertical != 0))
        {
            MoveController.moveDirection = new Vector2(horizontal, vertical);
            if (MoveController.moveDirection.magnitude > 1)
            {
                MoveController.moveDirection = MoveController.moveDirection.normalized;
            }
            // Update joystick visual if enabled
            if (Optionz.useJoystick)
            {
                this.innerCircle.anchoredPosition = MoveController.moveDirection * this.movementRadius;
            }
        }
    }

    public virtual void HandlePointerInput(Vector2 pointerPos)
    {
        Vector2 localPoint = default(Vector2);
        // Handle joystick input if enabled and pointer is in joystick area
        if (Optionz.useJoystick && RectTransformUtility.ScreenPointToLocalPointInRectangle(this.outerCircle, pointerPos, null, out localPoint))
        {
            if (!this.isPressed)
            {
                this.startedInCircle = RectTransformUtility.RectangleContainsScreenPoint(this.outerCircle, pointerPos, null);
            }
            if (this.startedInCircle)
            {
                this.isPressed = true;
                MoveController.moveDirection = localPoint / this.movementRadius;
                if (MoveController.moveDirection.magnitude > 1)
                {
                    MoveController.moveDirection = MoveController.moveDirection.normalized;
                }
                this.innerCircle.anchoredPosition = MoveController.moveDirection * this.movementRadius;
                return;
            }
        }
        // Handle touch/click input outside joystick area
        if (Optionz.useTarget)
        {
            Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(GameObject.FindWithTag("Player").transform.position);
            Vector2 directionToTarget = new Vector2(pointerPos.x - playerScreenPos.x, pointerPos.y - playerScreenPos.y);
            float distance = directionToTarget.magnitude;
            float normalizedDistance = Mathf.Clamp01(distance / (Screen.height * 0.5f));
            MoveController.moveDirection = directionToTarget.normalized * normalizedDistance;
            // Update joystick visual if enabled
            if (Optionz.useJoystick)
            {
                this.innerCircle.anchoredPosition = MoveController.moveDirection * this.movementRadius;
            }
        }
    }

    public virtual void ResetMovement()
    {
        this.isPressed = false;
        this.startedInCircle = false;
        MoveController.moveDirection = Vector2.zero;
        this.innerCircle.anchoredPosition = this.startPos;
    }

    public MoveController()
    {
        this.normalAlpha = 0.5f;
        this.activeAlpha = 0.7f;
    }

    static MoveController()
    {
        MoveController.moveDirection = Vector2.zero;
    }

}