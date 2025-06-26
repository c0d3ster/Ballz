using UnityEngine
using UnityEngine.UI
using TMPro
using System.Collections

public class AccountLoader : MonoBehaviour
{
  [Header("Loading UI")]
  public Slider loadingBar
    public TextMeshProUGUI loadingText
    public TextMeshProUGUI statusText

    [Header("Account UI")]
    public GameObject accountPanel
    public TextMeshProUGUI emailText
    public TMP_InputField usernameInput
    public Button confirmButton
    public Button skipButton
    public TextMeshProUGUI errorText

    [Header("Settings")]
    public float minLoadTime = 2f
    public string mainMenuScene = "Active Main Menu"


    private AccountManager accountManager
    private bool isLoading = false
    private bool hasAccount = false


    private void Start()
  {
    // Initialize AccountManager if it doesn't exist
    if (AccountManager.Instance == null)
    {
      GameObject accountManagerObj = new GameObject("AccountManager")
            accountManagerObj.AddComponent<AccountManager>()
        }

    accountManager = AccountManager.Instance
        SetupUI()
        StartLoading()
    }

  private void SetupUI()
  {
    // Hide account panel initially
    if (accountPanel != null)
      accountPanel.SetActive(false)

        // Setup button listeners
        if (confirmButton != null)
      confirmButton.onClick.AddListener(OnConfirmUsername)


        if (skipButton != null)
      skipButton.onClick.AddListener(OnSkipAccount)

        // Setup input field
        if (usernameInput != null)
    {
      usernameInput.onValueChanged.AddListener(OnUsernameChanged)
            usernameInput.onSubmit.AddListener(OnConfirmUsername)
        }

    // Subscribe to account events
    accountManager.OnAccountLoaded += OnAccountLoaded
        accountManager.OnAccountCreated += OnAccountCreated
        accountManager.OnAuthenticationFailed += OnAuthenticationFailed
    }

  private void StartLoading()
  {
    isLoading = true
        StartCoroutine(LoadingSequence())
    }

  private IEnumerator LoadingSequence()
  {
    float startTime = Time.time
        float progress = 0f

        // Step 1: Initialize (0-20%)
        UpdateLoadingProgress(0f, "Initializing...")
        yield return new WaitForSeconds(0.3f)

        // Step 2: Start authentication (20-50%)
        UpdateLoadingProgress(0.2f, "Authenticating with platform...")
        accountManager.StartAuthentication()

        // Wait for authentication to complete
        while (!accountManager.isAuthenticated && !isLoading)
    {
      yield return null
        }

    if (!isLoading) yield break // Error occurred


        UpdateLoadingProgress(0.5f, "Checking account status...")
        yield return new WaitForSeconds(0.3f)

        // Step 3: Check for existing account (50-80%)
        hasAccount = accountManager.HasExistingAccount()


        if (hasAccount)
    {
      UpdateLoadingProgress(0.8f, "Loading existing account...")
            accountManager.LoadExistingAccount()
        }
    else
    {
      UpdateLoadingProgress(0.8f, "New user detected...")
        }

    // Step 4: Finalize (80-100%)
    UpdateLoadingProgress(1f, "Ready!")

        // Ensure minimum load time
        float elapsedTime = Time.time - startTime
        if (elapsedTime < minLoadTime)
    {
      yield return new WaitForSeconds(minLoadTime - elapsedTime)
        }

    // Check if we need to show account creation
    if (!hasAccount && accountManager.isAuthenticated)
    {
      ShowAccountCreation()
        }
    else
    {
      LoadMainMenu()
        }
  }

  private void UpdateLoadingProgress(float progress, string message)
  {
    if (loadingBar != null)
      loadingBar.value = progress


        if (loadingText != null)
      loadingText.text = $"Loading... {(progress * 100):F0}%"


        if (statusText != null)
      statusText.text = message
    }

  private void ShowAccountCreation()
  {
    isLoading = false


        if (accountPanel != null)
      accountPanel.SetActive(true)


        if (emailText != null)
      emailText.text = $"Email: {accountManager.userEmail}"


        if (usernameInput != null)
    {
      usernameInput.text = ""
            usernameInput.Select()
        }

    if (errorText != null)
      errorText.text = ""
    }

  private void OnUsernameChanged(string value)
  {
    // Clear error when user starts typing
    if (errorText != null && !string.IsNullOrEmpty(errorText.text))
    {
      errorText.text = ""
        }
  }

  private void OnConfirmUsername()
  {
    if (usernameInput == null) return

    string username = usernameInput.text.Trim()


        if (string.IsNullOrEmpty(username))
    {
      if (errorText != null)
        errorText.text = "Username cannot be empty"
            return
        }

    if (username.Length < 3)
    {
      if (errorText != null)
        errorText.text = "Username must be at least 3 characters"
            return
        }

    if (username.Length > 20)
    {
      if (errorText != null)
        errorText.text = "Username must be 20 characters or less"
            return
        }

    // Create account
    accountManager.CreateNewAccount(username)
    }

  private void OnSkipAccount()
  {
    Debug.Log("User skipped account creation")
        LoadMainMenu()
    }

  private void OnAccountLoaded(AccountManager.UserAccount account)
  {
    Debug.Log($"Account loaded: {account.username}")
        LoadMainMenu()
    }

  private void OnAccountCreated(AccountManager.UserAccount account)
  {
    Debug.Log($"Account created: {account.username}")
        LoadMainMenu()
    }

  private void OnAuthenticationFailed(string error)
  {
    Debug.LogError($"Authentication failed: {error}")


        if (statusText != null)
      statusText.text = "Authentication failed. Continuing without account..."

        // Continue to main menu even if authentication fails
        StartCoroutine(ContinueAfterError())
    }

  private IEnumerator ContinueAfterError()
  {
    yield return new WaitForSeconds(1f)
        LoadMainMenu()
    }

  private void LoadMainMenu()
  {
    isLoading = false
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuScene)
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