using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using Enums;

[System.Serializable]
public class PlayerController : MonoBehaviour
{
  public Rigidbody rb;
  public float speed = 10f;
  public bool canJump;
  public Camera cam;
  public static Vector3 camOffset;
  public Vector3 camShift;
  public float dirOffset;
  protected float jumpForce = 500f;
  public float gravityMultiplier = .5f;
  [Range(0.1f, 1f)]
  public float minSpeedMultiplier = 0.3f; // Minimum speed multiplier for small movements

  private bool isHoldingSpace = false;
  private Coroutine spaceHoldCoroutine;

  public virtual void Awake()
  {
    // Try to get camera reference immediately
    if (!this.cam)
    {
      this.cam = Camera.main;
      if (!this.cam)
      {
        Debug.LogError("No camera assigned and no Main Camera found!");
        this.enabled = false; // Disable this component if no camera
        return;
      }
    }
  }

  public virtual void Start()
  {
    // Get camera if not assigned
    if (!this.cam)
    {
      this.cam = Camera.main;
      if (!this.cam)
      {
        Debug.LogError("No camera assigned and no Main Camera found!");
        return;
      }
    }
    this.rb = this.GetComponent<Rigidbody>();

    // Debug joystick setup
    GameObject outer = GameObject.Find("TouchControllerOuter");
    GameObject inner = GameObject.Find("TouchControllerInner");

    if (outer && inner)
    {
      UnityEngine.UI.Image outerImage = outer.GetComponent<UnityEngine.UI.Image>();
      UnityEngine.UI.Image innerImage = inner.GetComponent<UnityEngine.UI.Image>();
      if (outerImage && innerImage)
      {
        outerImage.color = new Color(outerImage.color.r, outerImage.color.g, outerImage.color.b, 0.5f);
        innerImage.color = new Color(innerImage.color.r, innerImage.color.g, innerImage.color.b, 0.5f);
        outerImage.raycastTarget = true;
        innerImage.raycastTarget = true;
        Optionz.useJoystick = true;
      }
    }

    if (Optionz.diff != 0)
    {
      this.speed = (float)(this.speed / Optionz.diff); // makes player move slower if difficulty is low and vice versa
    }

    // Check if jump is unlocked (Push level > 1)
    this.canJump = LevelProgressManager.Instance.GetHighestLevelNumber(GameMode.Push) > 1;

    //================= Starting Camera Position ==========//
    this.cam.transform.position = this.cam.transform.position + this.transform.position;
    this.camShift = this.cam.transform.position - this.transform.position;
    PlayerController.camOffset = new Vector3(0, -1.75f, 5.25f);

    // Subscribe to hotkey events
    HotkeyManager.OnJumpPressed += OnJumpPressed;
    HotkeyManager.OnJumpReleased += OnJumpReleased;
  }

  private void OnDestroy()
  {
    HotkeyManager.OnJumpPressed -= OnJumpPressed;
    HotkeyManager.OnJumpReleased -= OnJumpReleased;
  }

  private void OnJumpPressed()
  {
    if (IsGrounded() && this.canJump)
    {
      isHoldingSpace = true;
      if (spaceHoldCoroutine != null)
      {
        StopCoroutine(spaceHoldCoroutine);
      }
      spaceHoldCoroutine = StartCoroutine(HoldSpaceJump());
    }
  }

  private void OnJumpReleased()
  {
    isHoldingSpace = false;
    if (spaceHoldCoroutine != null)
    {
      StopCoroutine(spaceHoldCoroutine);
      spaceHoldCoroutine = null;
    }
  }

  public virtual void Update()
  {
    // Debug input state
    Vector2 moveDir = MoveController.moveDirection;

    // Jump handling moved to HotkeyManager

    // Apply extra gravity when falling
    if (!IsGrounded())
    {
      this.rb.AddForce(Physics.gravity * gravityMultiplier);
    }
  }

  private IEnumerator HoldSpaceJump()
  {
    while (isHoldingSpace)
    {
      if (IsGrounded() && this.canJump)
      {
        Jump();
      }
      yield return new WaitForSeconds(0.1f); // Small delay between jumps
    }
  }

  public virtual void FixedUpdate()
  {
    // Get movement direction from MoveController
    Vector2 moveDirection = MoveController.moveDirection;

    // Calculate speed multiplier based on input magnitude
    float inputMagnitude = moveDirection.magnitude;
    float speedMultiplier = Mathf.Lerp(minSpeedMultiplier, 1f, inputMagnitude);

    // Apply movement force with smooth sensitivity
    Vector3 movement = new Vector3(moveDirection.x, 0f, moveDirection.y);
    this.rb.AddForce(movement * this.speed * speedMultiplier);

    // Check if player fell off the map
    if (this.rb.transform.position.y <= -10)
    {
      if (string.IsNullOrEmpty(SceneLoader.Instance.currentScene) || (SceneLoader.Instance.currentScene == "Active Main Menu"))
      {
        SceneLoader.Instance.ChangeScene("Active Main Menu");
      }
      else
      {
        SceneLoader.Instance.GameOver();
      }
    }
  }

  public virtual void LateUpdate()
  {
    this.cam.transform.position = this.transform.position + this.camShift;
    this.cam.transform.LookAt(this.transform);
    this.cam.transform.position = this.cam.transform.position + PlayerController.camOffset;
  }

  public void Jump()
  {
    if (IsGrounded() && canJump)
    {
      rb.AddForce(Vector3.up * jumpForce);
    }
  }

  public bool IsGrounded()
  {
    return Physics.Raycast(transform.position, Vector3.down, .6f);
  }
}