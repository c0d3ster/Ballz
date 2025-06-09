using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Image buttonImage;
    private PlayerController playerController;
    private bool isHolding = false;
    private Coroutine holdCoroutine;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        FindPlayerController();
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
        FindPlayerController();
    }

    private void FindPlayerController()
    {
        // Try to find player by tag first
        GameObject player = GameObject.FindWithTag("Player");
        if (player)
        {
            playerController = player.GetComponent<PlayerController>();
        }
        else
        {
            // Fallback to finding any PlayerController
            playerController = FindFirstObjectByType<PlayerController>();
        }
    }

    void Update()
    {
        if (SceneLoader.IsCurrentSceneNonInteractive) return;

        if (playerController == null)
        {
            FindPlayerController();
            return;
        }

        // Set alpha based on canJump state
        Color color = buttonImage.color;
        color.a = playerController.canJump ? 0.8f : 0f;
        buttonImage.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerController != null && playerController.canJump)
        {
            isHolding = true;
            holdCoroutine = StartCoroutine(HoldJump());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        if (holdCoroutine != null)
        {
            StopCoroutine(holdCoroutine);
            holdCoroutine = null;
        }
    }

    private IEnumerator HoldJump()
    {
        while (isHolding)
        {
            if (playerController != null && playerController.canJump)
            {
                playerController.Jump();
            }
            yield return new WaitForSeconds(0.1f); // Small delay between jumps
        }
    }
} 