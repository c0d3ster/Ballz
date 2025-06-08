using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class JumpButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Image buttonImage;
    private PlayerController playerController;

    void Start()
    {
        Debug.Log("JumpButton Start called");
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        button.onClick.AddListener(OnJumpClick);
        Debug.Log("JumpButton onClick listener added");
    }

    void FindPlayerController()
    {
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogWarning("PlayerController not found in scene!");
            }
            else
            {
                Debug.Log("JumpButton found PlayerController");
            }
        }
    }

    void OnJumpClick()
    {
        Debug.Log("JumpButton OnJumpClick called");
        FindPlayerController();
        if (playerController != null)
        {
            Debug.Log("JumpButton attempting to jump");
            playerController.Jump();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("JumpButton pointer enter");
        // Unity's button will handle the cursor change automatically
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("JumpButton pointer exit");
        // Unity's button will handle the cursor change automatically
    }

    void OnEnable()
    {
        // Clear the reference when enabled (like when switching scenes)
        playerController = null;
        Debug.Log("JumpButton enabled, cleared PlayerController reference");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Clear player reference and find new one
        playerController = null;
        FindPlayerController();
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnJumpClick);
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        // Skip update in non-interactive scenes since UIManager handles canvas visibility
        if (SceneLoader.IsCurrentSceneNonInteractive)
        {
            return;
        }

        // Find player controller if needed
        if (playerController == null)
        {
            FindPlayerController();
        }

        // Update visibility based on canJump state
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = (playerController != null && playerController.canJump) ? 0.8f : 0f;
            buttonImage.color = color;
        }
    }
} 