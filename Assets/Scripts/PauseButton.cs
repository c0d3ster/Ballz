using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class PauseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private UIManager uiManager;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnPauseClick);
        
        // Find UIManager instance
        uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogWarning("UIManager instance not found!");
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