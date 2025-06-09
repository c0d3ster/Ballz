using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class PauseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private UIManager uiManager;
    private Image buttonImage;

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        button.onClick.AddListener(OnPauseClick);
        
        // Find UIManager instance
        uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogWarning("UIManager instance not found!");
        }

        // Disable UI navigation
        Navigation nav = new Navigation();
        nav.mode = Navigation.Mode.None;
        button.navigation = nav;
    }

    void Update()
    {
        // Hide button when game is paused
        if (buttonImage != null)
        {
            buttonImage.enabled = !SceneLoader.isPaused;
            button.enabled = !SceneLoader.isPaused;
        }
    }

    void OnPauseClick()
    {
        if (uiManager != null)
        {
            uiManager.TogglePause();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Unity's button will handle the cursor change automatically
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Unity's button will handle the cursor change automatically
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnPauseClick);
        }
    }
} 