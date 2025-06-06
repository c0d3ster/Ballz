using UnityEngine.UI;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    private bool jump;
    private bool canJump;
    public int count;
    public Text countText;
    private GameObject[] totalBoxes;
    public Camera cam;
    public static Vector3 camOffset;
    public Vector3 camShift;
    public float dirOffset;
    public float jumpForce = 350f; 
    public float gravityMultiplier = 2f; 
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
        this.totalBoxes = GameObject.FindGameObjectsWithTag("Pick Up"); // gets total number of collectables on scene
        this.count = 0;
        if (this.countText)
        {
            this.countText.text = "";
            this.SetCountText();
        }
        if (Optionz.diff != 0)
        {
            this.speed = (float) (this.speed / Optionz.diff); // makes player move slower if difficulty is low and vice versa
        }
        this.jump = false;
        this.canJump = true;
        int levelNumber = SceneLoader.GetLevelNumber();
        Debug.Log("Level Number: " + levelNumber);
        if (levelNumber <= 1)
        {
            this.canJump = false;
        }
        //================= Starting Camera Position ==========//
        this.cam.transform.position = this.cam.transform.position + this.transform.position;
        this.camShift = this.cam.transform.position - this.transform.position;
        PlayerController.camOffset = new Vector3(0, -2.5f, 5);
    }

    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!SceneLoader.isPaused)
            {
                Time.timeScale = 0;
                SceneLoader.isPaused = true;
                SceneLoader.Pause();
            }
            else
            {
                Time.timeScale = 1;
                SceneLoader.isPaused = false;
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("PAUSE");
            }
        }
        // Handle jumping
        if (Input.GetKey("space") && IsGrounded() && this.canJump)
        {
            this.rb.AddForce(Vector3.up * jumpForce);
            this.jump = true;
        }
        // Apply extra gravity when falling
        if (!IsGrounded())
        {
            this.rb.AddForce(Physics.gravity * gravityMultiplier);
        }
        else
        {
            this.jump = false;
        }
    }

    public virtual void FixedUpdate()
    {
        // Get movement direction from MoveController
        Vector2 moveDirection = MoveController.moveDirection;
        // Apply movement force
        Vector3 movement = new Vector3(moveDirection.x, 0f, moveDirection.y);
        this.rb.AddForce(movement * this.speed);
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
        
        this.countText.text = (("Count: " + this.count.ToString()) + "/") + this.totalBoxes.Length.ToString();
        if (this.count >= this.totalBoxes.Length)
        {
            SceneLoader.Win();
        }
    }

    private bool IsGrounded()
    {
        // Reduced check distance for tighter ground detection
        return Physics.Raycast(transform.position, Vector3.down, 0.6f);
    }

}