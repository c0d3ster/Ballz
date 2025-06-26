using UnityEngine
using UnityEngine.UI
using TMPro
using System

public class FirebaseAuthUI : MonoBehaviour
{
  [Header("UI References")]
  [SerializeField]
  private GameObject signInPanel
  [SerializeField] private GameObject signUpPanel
  [SerializeField] private GameObject userPanel
  [SerializeField] private GameObject usernamePanel

  // Sign In UI
  [SerializeField] private TMP_InputField signInEmailField
  [SerializeField] private TMP_InputField signInPasswordField
  [SerializeField] private Button signInButton
  [SerializeField] private Button signInAnonymousButton
  [SerializeField] private Button showSignUpButton

  // Sign Up UI
  [SerializeField] private TMP_InputField signUpEmailField
  [SerializeField] private TMP_InputField signUpPasswordField
  [SerializeField] private TMP_InputField signUpUsernameField
  [SerializeField] private Button signUpButton
  [SerializeField] private Button showSignInButton

  // User Panel UI
  [SerializeField] private TextMeshProUGUI usernameText
  [SerializeField] private TextMeshProUGUI emailText
  [SerializeField] private Button changeUsernameButton
  [SerializeField] private Button signOutButton

  // Username Change UI
  [SerializeField] private TMP_InputField newUsernameField
  [SerializeField] private Button confirmUsernameButton
  [SerializeField] private Button cancelUsernameButton
  [SerializeField] private TextMeshProUGUI usernameStatusText

  [Header("Settings")]
  [SerializeField]
  private bool showInMainMenu = true
  [SerializeField] private bool autoSignInAnonymous = true

  private void Start()
  {
    // Subscribe to Firebase events
    FirebaseSaveManager.OnFirebaseInitialized += OnFirebaseInitialized
    FirebaseSaveManager.OnUserSignedIn += OnUserSignedIn
    FirebaseSaveManager.OnUserSignedOut += OnUserSignedOut
    FirebaseSaveManager.OnUsernameConflict += OnUsernameConflict

    // Set up button listeners
    SetupButtonListeners()

    // Initial UI state
    UpdateUIState()

    // Auto sign in anonymous if enabled
    if (autoSignInAnonymous && FirebaseSaveManager.Instance != null && FirebaseSaveManager.Instance.IsInitialized)
    {
      FirebaseSaveManager.Instance.SignInAnonymously()
    }
  }

  private void OnDestroy()
  {
    // Unsubscribe from events
    FirebaseSaveManager.OnFirebaseInitialized -= OnFirebaseInitialized
    FirebaseSaveManager.OnUserSignedIn -= OnUserSignedIn
    FirebaseSaveManager.OnUserSignedOut -= OnUserSignedOut
    FirebaseSaveManager.OnUsernameConflict -= OnUsernameConflict

    // Remove button listeners
    RemoveButtonListeners()
  }

  private void SetupButtonListeners()
  {
    // Sign In buttons
    if (signInButton != null)
      signInButton.onClick.AddListener(OnSignInButtonClicked)


    if (signInAnonymousButton != null)
      signInAnonymousButton.onClick.AddListener(OnSignInAnonymousButtonClicked)


    if (showSignUpButton != null)
      showSignUpButton.onClick.AddListener(OnShowSignUpButtonClicked)

    // Sign Up buttons
    if (signUpButton != null)
      signUpButton.onClick.AddListener(OnSignUpButtonClicked)


    if (showSignInButton != null)
      showSignInButton.onClick.AddListener(OnShowSignInButtonClicked)

    // User panel buttons
    if (changeUsernameButton != null)
      changeUsernameButton.onClick.AddListener(OnChangeUsernameButtonClicked)


    if (signOutButton != null)
      signOutButton.onClick.AddListener(OnSignOutButtonClicked)

    // Username change buttons
    if (confirmUsernameButton != null)
      confirmUsernameButton.onClick.AddListener(OnConfirmUsernameButtonClicked)


    if (cancelUsernameButton != null)
      cancelUsernameButton.onClick.AddListener(OnCancelUsernameButtonClicked)
  }

  private void RemoveButtonListeners()
  {
    // Sign In buttons
    if (signInButton != null)
      signInButton.onClick.RemoveListener(OnSignInButtonClicked)


    if (signInAnonymousButton != null)
      signInAnonymousButton.onClick.RemoveListener(OnSignInAnonymousButtonClicked)


    if (showSignUpButton != null)
      showSignUpButton.onClick.RemoveListener(OnShowSignUpButtonClicked)

    // Sign Up buttons
    if (signUpButton != null)
      signUpButton.onClick.RemoveListener(OnSignUpButtonClicked)


    if (showSignInButton != null)
      showSignInButton.onClick.RemoveListener(OnShowSignInButtonClicked)

    // User panel buttons
    if (changeUsernameButton != null)
      changeUsernameButton.onClick.RemoveListener(OnChangeUsernameButtonClicked)


    if (signOutButton != null)
      signOutButton.onClick.RemoveListener(OnSignOutButtonClicked)

    // Username change buttons
    if (confirmUsernameButton != null)
      confirmUsernameButton.onClick.RemoveListener(OnConfirmUsernameButtonClicked)


    if (cancelUsernameButton != null)
      cancelUsernameButton.onClick.RemoveListener(OnCancelUsernameButtonClicked)
  }

  private void UpdateUIState()
  {
    if (FirebaseSaveManager.Instance == null) return

    bool isSignedIn = FirebaseSaveManager.Instance.IsSignedIn

    // Show appropriate panel
    if (signInPanel != null)
      signInPanel.SetActive(!isSignedIn)


    if (signUpPanel != null)
      signUpPanel.SetActive(false) // Always hidden initially


    if (userPanel != null)
      userPanel.SetActive(isSignedIn)


    if (usernamePanel != null)
      usernamePanel.SetActive(false) // Hidden by default

    // Update user info if signed in
    if (isSignedIn)
    {
      if (usernameText != null)
        usernameText.text = $"Username: {FirebaseSaveManager.Instance.CurrentUsername}"


      if (emailText != null)
        emailText.text = $"Email: {FirebaseSaveManager.Instance.CurrentUserId}"
    }
  }

  private void OnFirebaseInitialized()
  {
    Debug.Log("[FirebaseAuthUI] Firebase initialized")
    UpdateUIState()
  }

  private void OnUserSignedIn()
  {
    Debug.Log("[FirebaseAuthUI] User signed in")
    UpdateUIState()
  }

  private void OnUserSignedOut()
  {
    Debug.Log("[FirebaseAuthUI] User signed out")
    UpdateUIState()
  }

  private void OnUsernameConflict(string username)
  {
    Debug.Log($"[FirebaseAuthUI] Username conflict: {username}")
    if (usernameStatusText != null)
    {
      usernameStatusText.text = $"Username '{username}' is already taken"
      usernameStatusText.color = Color.red
    }
  }

  // Button event handlers
  private async void OnSignInButtonClicked()
  {
    if (signInEmailField == null || signInPasswordField == null) return

    string email = signInEmailField.text
    string password = signInPasswordField.text

    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
    {
      Debug.LogWarning("[FirebaseAuthUI] Email and password required")
      return
    }

    bool success = await FirebaseSaveManager.Instance.SignInWithEmail(email, password)
    if (!success)
    {
      Debug.LogError("[FirebaseAuthUI] Sign in failed")
    }
  }

  private async void OnSignInAnonymousButtonClicked()
  {
    bool success = await FirebaseSaveManager.Instance.SignInAnonymously()
    if (!success)
    {
      Debug.LogError("[FirebaseAuthUI] Anonymous sign in failed")
    }
  }

  private void OnShowSignUpButtonClicked()
  {
    if (signInPanel != null)
      signInPanel.SetActive(false)


    if (signUpPanel != null)
      signUpPanel.SetActive(true)
  }

  private async void OnSignUpButtonClicked()
  {
    if (signUpEmailField == null || signUpPasswordField == null || signUpUsernameField == null) return

    string email = signUpEmailField.text
    string password = signUpPasswordField.text
    string username = signUpUsernameField.text

    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
    {
      Debug.LogWarning("[FirebaseAuthUI] Email, password, and username required")
      return
    }

    bool success = await FirebaseSaveManager.Instance.CreateAccount(email, password, username)
    if (!success)
    {
      Debug.LogError("[FirebaseAuthUI] Account creation failed")
    }
  }

  private void OnShowSignInButtonClicked()
  {
    if (signUpPanel != null)
      signUpPanel.SetActive(false)


    if (signInPanel != null)
      signInPanel.SetActive(true)
  }

  private void OnChangeUsernameButtonClicked()
  {
    if (usernamePanel != null)
      usernamePanel.SetActive(true)


    if (userPanel != null)
      userPanel.SetActive(false)
  }

  private void OnSignOutButtonClicked()
  {
    FirebaseSaveManager.Instance.SignOut()
  }

  private async void OnConfirmUsernameButtonClicked()
  {
    if (newUsernameField == null) return

    string newUsername = newUsernameField.text

    if (string.IsNullOrEmpty(newUsername))
    {
      Debug.LogWarning("[FirebaseAuthUI] Username required")
      return
    }

    bool success = await FirebaseSaveManager.Instance.SetUsername(newUsername)
    if (success)
    {
      // Hide username panel and show user panel
      if (usernamePanel != null)
        usernamePanel.SetActive(false)


      if (userPanel != null)
        userPanel.SetActive(true)


      UpdateUIState()
    }
  }

  private void OnCancelUsernameButtonClicked()
  {
    if (usernamePanel != null)
      usernamePanel.SetActive(false)


    if (userPanel != null)
      userPanel.SetActive(true)
  }

  // Public methods for external control
  public void ShowSignInPanel()
  {
    if (signInPanel != null)
      signInPanel.SetActive(true)


    if (signUpPanel != null)
      signUpPanel.SetActive(false)


    if (userPanel != null)
      userPanel.SetActive(false)


    if (usernamePanel != null)
      usernamePanel.SetActive(false)
  }

  public void ShowSignUpPanel()
  {
    if (signInPanel != null)
      signInPanel.SetActive(false)


    if (signUpPanel != null)
      signUpPanel.SetActive(true)


    if (userPanel != null)
      userPanel.SetActive(false)


    if (usernamePanel != null)
      usernamePanel.SetActive(false)
  }

  public void ShowUserPanel()
  {
    if (signInPanel != null)
      signInPanel.SetActive(false)


    if (signUpPanel != null)
      signUpPanel.SetActive(false)


    if (userPanel != null)
      userPanel.SetActive(true)


    if (usernamePanel != null)
      usernamePanel.SetActive(false)
  }

  public void HideAllPanels()
  {
    if (signInPanel != null)
      signInPanel.SetActive(false)


    if (signUpPanel != null)
      signUpPanel.SetActive(false)


    if (userPanel != null)
      userPanel.SetActive(false)


    if (usernamePanel != null)
      usernamePanel.SetActive(false)
  }

#if UNITY_EDITOR
  [ContextMenu("Test Anonymous Sign In")]
  private async void TestAnonymousSignIn()
  {
    if (FirebaseSaveManager.Instance != null)
    {
      await FirebaseSaveManager.Instance.SignInAnonymously()
    }
  }

  [ContextMenu("Test Sign Out")]
  private void TestSignOut()
  {
    if (FirebaseSaveManager.Instance != null)
    {
      FirebaseSaveManager.Instance.SignOut()
    }
  }
#endif
}