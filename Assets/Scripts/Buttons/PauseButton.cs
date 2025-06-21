using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class PauseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  private Button button;
  private Image buttonImage;

  void Start()
  {
    button = GetComponent<Button>();
    buttonImage = GetComponent<Image>();
    button.onClick.AddListener(OnPauseClick);

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
      buttonImage.enabled = !SceneLoader.Instance.isPaused;
      button.enabled = !SceneLoader.Instance.isPaused;
    }
  }

  void OnPauseClick()
  {
    // Use HotkeyManager to trigger pause - this ensures consistent behavior
    // whether pause is triggered by keyboard (Escape) or UI button
    if (HotkeyManager.Instance != null)
    {
      HotkeyManager.Instance.TriggerPause();
    }
    else
    {
      Debug.LogWarning("HotkeyManager instance not found!");
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