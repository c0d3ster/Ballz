using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AccountLoader : MonoBehaviour
{
  [Header("Loading UI")]
  public Slider loadingBar;
  public TextMeshProUGUI loadingText;
  public TextMeshProUGUI statusText;

  [Header("Settings")]
  public float minLoadTime = 2f;
  public string mainMenuScene = "Active Main Menu";

  private AccountManager accountManager;
  private bool isLoading = false;
  private bool hasExistingAccount = false;

  // Events
  public System.Action<bool> OnAccountCheckComplete; // bool = hasExistingAccount

  private void Start()
  {
    // Initialize AccountManager if it doesn't exist
    if (AccountManager.Instance == null)
    {
      GameObject accountManagerObj = new GameObject("AccountManager");
      accountManagerObj.AddComponent<AccountManager>();
    }

    accountManager = AccountManager.Instance;
    SetupUI();
    StartLoading();
  }

  private void SetupUI()
  {
    // Subscribe to account events
    accountManager.OnAccountLoaded += OnAccountLoaded;
    accountManager.OnAuthenticationFailed += OnAuthenticationFailed;
  }

  private void StartLoading()
  {
    isLoading = true;
    StartCoroutine(LoadingSequence());
  }

  private IEnumerator LoadingSequence()
  {
    float startTime = Time.time;

    // Step 1: Initialize (0-20%)
    UpdateLoadingProgress(0f, "Initializing...");
    yield return new WaitForSeconds(0.3f);

    // Step 2: Start authentication (20-50%)
    UpdateLoadingProgress(0.2f, "Authenticating with platform...");
    accountManager.StartAuthentication();

    // Wait for authentication to complete
    while (!accountManager.isAuthenticated && !isLoading)
    {
      yield return null;
    }

    if (!isLoading) yield break; // Error occurred

    UpdateLoadingProgress(0.5f, "Checking account status...");
    yield return new WaitForSeconds(0.3f);

    // Step 3: Check for existing account (50-80%)
    hasExistingAccount = accountManager.HasExistingAccount();

    if (hasExistingAccount)
    {
      UpdateLoadingProgress(0.8f, "Loading existing account...");
      accountManager.LoadExistingAccount();
    }
    else
    {
      UpdateLoadingProgress(0.8f, "No existing account found...");
    }

    // Step 4: Finalize (80-100%)
    UpdateLoadingProgress(1f, "Ready!");

    // Ensure minimum load time
    float elapsedTime = Time.time - startTime;
    if (elapsedTime < minLoadTime)
    {
      yield return new WaitForSeconds(minLoadTime - elapsedTime);
    }

    // Notify completion
    OnAccountCheckComplete?.Invoke(hasExistingAccount);

    // If account exists, transition to main menu
    if (hasExistingAccount)
    {
      LoadMainMenu();
    }
  }

  private void UpdateLoadingProgress(float progress, string message)
  {
    if (loadingBar != null)
      loadingBar.value = progress;

    if (loadingText != null)
      loadingText.text = $"Loading... {(progress * 100):F0}%";

    if (statusText != null)
      statusText.text = message;
  }

  private void OnAccountLoaded(AccountManager.UserAccount account)
  {
    Debug.Log($"Account loaded: {account.username}");
    // Account loading is complete, will transition to main menu
  }

  private void OnAuthenticationFailed(string error)
  {
    Debug.LogError($"Authentication failed: {error}");

    if (statusText != null)
      statusText.text = "Authentication failed. Continuing without account...";

    // Continue without account
    isLoading = false;
    OnAccountCheckComplete?.Invoke(false);
  }

  private void LoadMainMenu()
  {
    isLoading = false;
    UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuScene);
  }

  private void OnDestroy()
  {
    if (accountManager != null)
    {
      accountManager.OnAccountLoaded -= OnAccountLoaded;
      accountManager.OnAuthenticationFailed -= OnAuthenticationFailed;
    }
  }
}