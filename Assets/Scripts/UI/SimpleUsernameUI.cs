using UnityEngine
using UnityEngine.UI
using TMPro

public class SimpleUsernameUI : MonoBehaviour
{
  [Header("UI References")]
  [SerializeField]
  private GameObject usernamePanel
  [SerializeField] private TMP_InputField usernameField
  [SerializeField] private Button setUsernameButton
  [SerializeField] private Button skipButton
  [SerializeField] private TextMeshProUGUI currentUsernameText
  [SerializeField] private TextMeshProUGUI statusText
  [SerializeField] private TextMeshProUGUI infoText

  [Header("Settings")]
  [SerializeField]
  private bool showOnStart = true
  [SerializeField] private bool requireUsername = false
  [SerializeField] private string infoMessage = "Set a username for leaderboards and multiplayer features"

  private void Start()
  {
    // Set up button listeners
    if (setUsernameButton != null)
      setUsernameButton.onClick.AddListener(OnSetUsernameClicked)


    if (skipButton != null)
      skipButton.onClick.AddListener(OnSkipClicked)

    // Subscribe to events
    SimpleUserManager.OnUserManagerInitialized += OnUserManagerInitialized
    SimpleUserManager.OnUsernameSet += OnUsernameSet

    // Show username panel if needed
    if (showOnStart)
    {
      CheckUsernameStatus()
    }
  }

  private void OnDestroy()
  {
    // Remove button listeners
    if (setUsernameButton != null)
      setUsernameButton.onClick.RemoveListener(OnSetUsernameClicked)


    if (skipButton != null)
      skipButton.onClick.RemoveListener(OnSkipClicked)

    // Unsubscribe from events
    SimpleUserManager.OnUserManagerInitialized -= OnUserManagerInitialized
    SimpleUserManager.OnUsernameSet -= OnUsernameSet
  }

  private void OnUserManagerInitialized()
  {
    CheckUsernameStatus()
  }

  private void OnUsernameSet(string username)
  {
    UpdateUI()
    if (statusText != null)
    {
      statusText.text = $"Username set to: {username}"
      statusText.color = Color.green
    }
  }

  private void CheckUsernameStatus()
  {
    if (SimpleUserManager.Instance == null) return

    bool hasUsername = SimpleUserManager.Instance.HasUsername

    if (usernamePanel != null)
    {
      // Show panel if no username and requireUsername is true, or if showOnStart is true
      bool shouldShow = (!hasUsername && requireUsername) || (showOnStart && !hasUsername)
      usernamePanel.SetActive(shouldShow)
    }

    UpdateUI()
  }

  private void UpdateUI()
  {
    if (SimpleUserManager.Instance == null) return

    // Update current username display
    if (currentUsernameText != null)
    {
      string username = SimpleUserManager.Instance.GetUsername()
      if (!string.IsNullOrEmpty(username))
      {
        currentUsernameText.text = $"Current Username: {username}"
      }
      else
      {
        currentUsernameText.text = "No username set"
      }
    }

    // Update info text
    if (infoText != null)
    {
      infoText.text = infoMessage
    }

    // Update button states
    if (setUsernameButton != null)
    {
      setUsernameButton.interactable = SimpleUserManager.Instance.IsInitialized
    }

    if (skipButton != null)
    {
      skipButton.interactable = !requireUsername
    }
  }

  private async void OnSetUsernameClicked()
  {
    if (usernameField == null) return

    string username = usernameField.text.Trim()

    if (string.IsNullOrEmpty(username))
    {
      if (statusText != null)
      {
        statusText.text = "Please enter a username"
        statusText.color = Color.red
      }
      return
    }

    // Validate username
    if (!SimpleUserManager.Instance.IsValidUsername(username))
    {
      if (statusText != null)
      {
        statusText.text = "Invalid username. Use only letters, numbers, and underscores (max 20 characters)"
        statusText.color = Color.red
      }
      return
    }

    // Check if username is available
    bool isAvailable = await SimpleUserManager.Instance.IsUsernameAvailable(username)
    if (!isAvailable)
    {
      if (statusText != null)
      {
        statusText.text = $"Username '{username}' is already taken"
        statusText.color = Color.red
      }
      return
    }

    // Set username
    bool success = await SimpleUserManager.Instance.SetUsername(username)
    if (success)
    {
      // Clear input field
      usernameField.text = ""

      // Hide panel if username is now set
      if (usernamePanel != null && !requireUsername)
      {
        usernamePanel.SetActive(false)
      }
    }
    else
    {
      if (statusText != null)
      {
        statusText.text = "Failed to set username. Please try again."
        statusText.color = Color.red
      }
    }
  }

  private void OnSkipClicked()
  {
    if (usernamePanel != null)
    {
      usernamePanel.SetActive(false)
    }

    if (statusText != null)
    {
      statusText.text = "Username setup skipped"
      statusText.color = Color.yellow
    }
  }

  // Public methods for external control
  public void ShowUsernamePanel()
  {
    if (usernamePanel != null)
    {
      usernamePanel.SetActive(true)
      UpdateUI()
    }
  }

  public void HideUsernamePanel()
  {
    if (usernamePanel != null)
    {
      usernamePanel.SetActive(false)
    }
  }

  public void SetRequireUsername(bool required)
  {
    requireUsername = required
    CheckUsernameStatus()
  }

  public bool HasValidUsername()
  {
    return SimpleUserManager.Instance != null && SimpleUserManager.Instance.HasUsername
  }

  public string GetCurrentUsername()
  {
    return SimpleUserManager.Instance?.GetUsername() ?? ""
  }

#if UNITY_EDITOR
  [ContextMenu("Test Show Panel")]
  private void TestShowPanel()
  {
    ShowUsernamePanel()
  }

  [ContextMenu("Test Hide Panel")]
  private void TestHidePanel()
  {
    HideUsernamePanel()
  }

  [ContextMenu("Test Set Username")]
  private async void TestSetUsername()
  {
    if (SimpleUserManager.Instance != null)
    {
      await SimpleUserManager.Instance.SetUsername("TestUser" + UnityEngine.Random.Range(1000, 9999))
    }
  }
#endif
}