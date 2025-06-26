using UnityEngine
using UnityEngine.UI
using TMPro
using System.Collections

public class SplashScreenUI : MonoBehaviour
{
  [Header("UI Panels")]
  public GameObject loadingPanel
    public GameObject authenticationPanel
    public GameObject newGamePanel
    public GameObject loadGamePanel
    public GameObject usernameInputPanel

    [Header("Loading Panel")]
    public TextMeshProUGUI loadingText
    public Slider loadingProgress

    [Header("Authentication Panel")]
    public TextMeshProUGUI authStatusText

    [Header("New Game Panel")]
    public Button newGameButton
    public TextMeshProUGUI newGameInfoText

    [Header("Load Game Panel")]
    public Button loadGameButton
    public TextMeshProUGUI loadGameInfoText
    public Button clearDataButton

    [Header("Username Input Panel")]
    public TMP_InputField usernameInput
    public Button confirmUsernameButton
    public Button cancelUsernameButton
    public TextMeshProUGUI usernameErrorText
    public TextMeshProUGUI emailDisplayText


    private AccountManager accountManager
    private bool isProcessing = false


    private void Start()
  {
    accountManager = AccountManager.Instance
        if (accountManager == null)
    {
      Debug.LogError("AccountManager not found! Make sure it's in the scene.")
            return
        }

    SetupUI()
        StartSplashScreen()
    }

  private void SetupUI()
  {
    // Subscribe to events
    accountManager.OnAccountLoaded += OnAccountLoaded
        accountManager.OnAccountCreated += OnAccountCreated
        accountManager.OnAuthenticationFailed += OnAuthenticationFailed

        // Setup button listeners
        newGameButton.onClick.AddListener(OnNewGameClicked)
        loadGameButton.onClick.AddListener(OnLoadGameClicked)
        clearDataButton.onClick.AddListener(OnClearDataClicked)
        confirmUsernameButton.onClick.AddListener(OnConfirmUsernameClicked)
        cancelUsernameButton.onClick.AddListener(OnCancelUsernameClicked)

        // Setup input field
        usernameInput.onValueChanged.AddListener(OnUsernameChanged)

        // Initially hide all panels except loading
        ShowPanel(loadingPanel)
    }

  private void StartSplashScreen()
  {
    StartCoroutine(SplashScreenFlow())
    }

  private IEnumerator SplashScreenFlow()
  {
    // Step 1: Show loading screen
    ShowPanel(loadingPanel)
        UpdateLoadingProgress(0f, "Initializing...")


        yield return new WaitForSeconds(0.5f)

        // Step 2: Start authentication
        UpdateLoadingProgress(0.3f, "Authenticating with platform...")
        accountManager.StartAuthentication()

        // Wait for authentication to complete
        while (!accountManager.isAuthenticated && !isProcessing)
    {
      yield return null
        }

    if (isProcessing) yield break // Error occurred


        UpdateLoadingProgress(0.6f, "Checking account status...")
        yield return new WaitForSeconds(0.5f)

        // Step 3: Check if user has existing account
        if (accountManager.HasExistingAccount())
    {
      UpdateLoadingProgress(1f, "Account found!")
            yield return new WaitForSeconds(0.5f)
            ShowLoadGamePanel()
        }
    else
    {
      UpdateLoadingProgress(1f, "New user detected!")
            yield return new WaitForSeconds(0.5f)
            ShowNewGamePanel()
        }
  }

  private void UpdateLoadingProgress(float progress, string message)
  {
    if (loadingProgress != null)
      loadingProgress.value = progress


        if (loadingText != null)
      loadingText.text = message
    }

  private void ShowNewGamePanel()
  {
    ShowPanel(newGamePanel)
        newGameInfoText.text = $"Welcome! You'll be playing on {accountManager.currentPlatform}.\nEmail: {accountManager.userEmail}"
    }

  private void ShowLoadGamePanel()
  {
    ShowPanel(loadGamePanel)
        loadGameInfoText.text = $"Welcome back!\nPlatform: {accountManager.currentPlatform}\nEmail: {accountManager.userEmail}"
    }

  private void ShowUsernameInputPanel()
  {
    ShowPanel(usernameInputPanel)
        emailDisplayText.text = $"Email: {accountManager.userEmail}"
        usernameInput.text = ""
        usernameErrorText.text = ""
        usernameInput.Select()
    }

  private void OnNewGameClicked()
  {
    if (isProcessing) return
    ShowUsernameInputPanel()
    }

  private void OnLoadGameClicked()
  {
    if (isProcessing) return
    isProcessing = true
        accountManager.LoadExistingAccount()
    }

  private void OnClearDataClicked()
  {
    if (isProcessing) return

    // Show confirmation dialog (you can enhance this)
    Debug.Log("Clearing all data...")
        accountManager.ClearAccount()
        ShowNewGamePanel()
    }

  private void OnConfirmUsernameClicked()
  {
    if (isProcessing) return

    string username = usernameInput.text.Trim()


        if (string.IsNullOrEmpty(username))
    {
      usernameErrorText.text = "Username cannot be empty"
            return
        }

    if (username.Length < 3)
    {
      usernameErrorText.text = "Username must be at least 3 characters"
            return
        }

    if (username.Length > 20)
    {
      usernameErrorText.text = "Username must be 20 characters or less"
            return
        }

    // Username is valid, create account
    isProcessing = true
        accountManager.CreateNewAccount(username)
    }

  private void OnCancelUsernameClicked()
  {
    ShowNewGamePanel()
    }

  private void OnUsernameChanged(string value)
  {
    // Clear error when user starts typing
    if (!string.IsNullOrEmpty(usernameErrorText.text))
    {
      usernameErrorText.text = ""
        }
  }

  private void OnAccountLoaded(AccountManager.UserAccount account)
  {
    isProcessing = false
        Debug.Log($"Account loaded successfully: {account.username}")

        // Here you would transition to your main menu or game
        // For now, we'll just show a success message
        ShowPanel(loadGamePanel)
        loadGameInfoText.text = $"Welcome back, {account.username}!\nPlatform: {account.platform}\nEmail: {account.email}"
    }

  private void OnAccountCreated(AccountManager.UserAccount account)
  {
    isProcessing = false
        Debug.Log($"Account created successfully: {account.username}")

        // Here you would transition to your main menu or game
        // For now, we'll just show a success message
        ShowPanel(newGamePanel)
        newGameInfoText.text = $"Welcome, {account.username}!\nYour account has been created.\nPlatform: {account.platform}\nEmail: {account.email}"
    }

  private void OnAuthenticationFailed(string error)
  {
    isProcessing = true
        Debug.LogError($"Authentication failed: {error}")


        ShowPanel(authenticationPanel)
        authStatusText.text = $"Authentication failed: {error}\nPlease check your internet connection and try again."
    }

  private void ShowPanel(GameObject panelToShow)
  {
    // Hide all panels
    if (loadingPanel != null) loadingPanel.SetActive(false)
        if (authenticationPanel != null) authenticationPanel.SetActive(false)
        if (newGamePanel != null) newGamePanel.SetActive(false)
        if (loadGamePanel != null) loadGamePanel.SetActive(false)
        if (usernameInputPanel != null) usernameInputPanel.SetActive(false)

        // Show the requested panel
        if (panelToShow != null) panelToShow.SetActive(true)
    }

  private void OnDestroy()
  {
    if (accountManager != null)
    {
      accountManager.OnAccountLoaded -= OnAccountLoaded
            accountManager.OnAccountCreated -= OnAccountCreated
            accountManager.OnAuthenticationFailed -= OnAuthenticationFailed
        }
  }
}