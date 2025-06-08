using UnityEngine.UI;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 10f;
    public bool canJump;
    public int count;
    public Text countText;
    private GameObject[] totalBoxes;
    public Camera cam;
    public static Vector3 camOffset;
    public Vector3 camShift;
    public float dirOffset;
    protected float jumpForce = 500f;
    public float gravityMultiplier = .5f;
    [Range(0.1f, 1f)]
    public float minSpeedMultiplier = 0.3f; // Minimum speed multiplier for small movements

    public void Jump()
    {
        Debug.Log($"Jump called - canJump: {canJump}, isGrounded: {IsGrounded()}, scene: {SceneLoader.currentScene}");
        if (IsGrounded() && canJump)
        {
            Debug.Log("Jump triggered");
            rb.AddForce(Vector3.up * jumpForce);
        }
    }

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
        Debug.Log("PlayerController Start called");
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
        this.totalBoxes = GameObject.FindGameObjectsWithTag("Pick Up"); // gets total number of collectables on scene
        Debug.Log("Total Boxes: " + this.totalBoxes.Length);
        this.count = 0;

        // Debug joystick setup
        GameObject outer = GameObject.Find("TouchControllerOuter");
        GameObject inner = GameObject.Find("TouchControllerInner");
        Debug.Log("Found joystick objects - Outer: " + (outer != null) + ", Inner: " + (inner != null));
        
        if (outer && inner)
        {
            UnityEngine.UI.Image outerImage = outer.GetComponent<UnityEngine.UI.Image>();
            UnityEngine.UI.Image innerImage = inner.GetComponent<UnityEngine.UI.Image>();
            if (outerImage && innerImage)
            {
                Debug.Log("Setting joystick visibility");
                outerImage.color = new Color(outerImage.color.r, outerImage.color.g, outerImage.color.b, 0.5f);
                innerImage.color = new Color(innerImage.color.r, innerImage.color.g, innerImage.color.b, 0.5f);
                outerImage.raycastTarget = true;
                innerImage.raycastTarget = true;
                Optionz.useJoystick = true;
            }
        }

        if (this.countText)
        {
            this.countText.text = "";
            this.SetCountText();
        }
        if (Optionz.diff != 0)
        {
            this.speed = (float) (this.speed / Optionz.diff); // makes player move slower if difficulty is low and vice versa
        }
       
        this.canJump = SceneLoader.GetLevelNumber() >= 1;
        Debug.Log($"Jump enabled: {this.canJump} for level {SceneLoader.GetLevelNumber()}");
        
        //================= Starting Camera Position ==========//
        this.cam.transform.position = this.cam.transform.position + this.transform.position;
        this.camShift = this.cam.transform.position - this.transform.position;
        PlayerController.camOffset = new Vector3(0, -1.75f, 5.25f);
    }

    public virtual void Update()
    {
        // Debug input state
        Vector2 moveDir = MoveController.moveDirection;

        // Handle jumping
        if (Input.GetKeyDown("space") && IsGrounded() && this.canJump)
        {
            Jump();
        }
        
        // Apply extra gravity when falling
        if (!IsGrounded())
        {
            this.rb.AddForce(Physics.gravity * gravityMultiplier);
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
            if (!!string.IsNullOrEmpty(SceneLoader.currentScene) || (SceneLoader.currentScene == "Active Main Menu"))
            {
                SceneLoader.ChangeScene("Active Main Menu");
            }
            else
            {
                SceneLoader.GameOver();
            }
        }
    }

    public virtual void LateUpdate()
    {
        this.cam.transform.position = this.transform.position + this.camShift;
        this.cam.transform.LookAt(this.transform);
        this.cam.transform.position = this.cam.transform.position + PlayerController.camOffset;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            this.count = this.count + 1;
            if (this.countText != null)
            {
                this.SetCountText();
            }
        }
    }

    public virtual void SetCountText()
    {
        if (this.countText == null) return;
        
        if (this.totalBoxes.Length == 0)
        {
            this.countText.text = "";
            return;
        }

        this.countText.text = (("Count: " + this.count.ToString()) + "/") + this.totalBoxes.Length.ToString();
        if (this.totalBoxes.Length > 0 && this.count >= this.totalBoxes.Length)
        {
            SceneLoader.Win();
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, .6f);
    }

}