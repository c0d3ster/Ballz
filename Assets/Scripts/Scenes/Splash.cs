using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public partial class Splash : MonoBehaviour
{
  [Header("Account Integration")]
  public bool useAccountSystem = true;

  [Header("Settings")]
  private AccountLoader accountLoader;
  private AccountManager accountManager;
  private bool isProcessing = false;
  private int instanceId;

  // UI Elements (created programmatically)
  private Canvas uiCanvas;
  private TextMeshProUGUI loadingText;
  private Slider loadingBar;
  private Button newGameButton;
  private TMP_InputField usernameInput;
  private Button startGameButton;
  private TextMeshProUGUI errorText;
  private TextMeshProUGUI emailText;
  private TextMeshProUGUI usernameLabel;
  private TextMeshProUGUI emailLabel;

  // Loading system
  private List<LoadingStep> loadingSteps = new List<LoadingStep>();
  private int currentStepIndex = 0;
  private bool isLoading = true;

  [System.Serializable]
  private class LoadingStep
  {
    public string name;
    public string description;
    public float duration;
    public System.Action onComplete;

    public LoadingStep(string name, string description, float duration, System.Action onComplete = null)
    {
      this.name = name;
      this.description = description;
      this.duration = duration;
      this.onComplete = onComplete;
    }
  }

  private void Awake()
  {
    instanceId = GetInstanceID();
    Debug.Log($"[Splash] Instance created with ID: {instanceId}");
  }

  public virtual void Start()
  {
    Debug.Log($"[Splash] Start() called on instance {instanceId}");

    if (useAccountSystem)
    {
      // Get AccountLoader reference (always created by GameManager)
      accountLoader = FindFirstObjectByType<AccountLoader>();

      // Get AccountManager reference
      accountManager = AccountManager.Instance;

      // Create UI programmatically
      CreateUI();

      // Start loading sequence
      StartLoadingSequence();
    }
    else
    {
      // Fallback to original behavior
      this.StartCoroutine("Load");
    }
  }

  private void StartLoadingSequence()
  {
    Debug.Log($"[Splash] StartLoadingSequence() called on instance {instanceId}");

    // Define loading steps
    loadingSteps.Clear();
    loadingSteps.Add(new LoadingStep("Initializing", "Initializing game systems...", 0.5f));
    loadingSteps.Add(new LoadingStep("Checking Account", "Checking for existing account...", 0.8f));
    loadingSteps.Add(new LoadingStep("Loading Data", "Loading game data...", 0.7f));

    currentStepIndex = 0;
    StartCoroutine(ProcessLoadingSteps());
  }

  private IEnumerator ProcessLoadingSteps()
  {
    Debug.Log($"[Splash] ProcessLoadingSteps() started on instance {instanceId}");

    while (currentStepIndex < loadingSteps.Count && isLoading)
    {
      LoadingStep step = loadingSteps[currentStepIndex];

      // Update UI
      UpdateLoadingProgress((float)currentStepIndex / loadingSteps.Count, step.description);

      // Execute step action if any
      if (step.onComplete != null)
        step.onComplete();

      // Wait for step duration
      yield return new WaitForSeconds(step.duration);

      currentStepIndex++;
    }

    // Loading complete
    if (isLoading)
    {
      UpdateLoadingProgress(1f, "Ready!");
      yield return new WaitForSeconds(0.5f);

      // Check if we have an account
      if (accountManager != null && accountManager.HasExistingAccount())
      {
        // Account exists, transition to main menu
        Debug.Log($"[Splash] Existing account found on instance {instanceId}, transitioning to main menu");
        SceneLoader.Instance.LoadMainMenu();
      }
      else
      {
        // No account, show new game option
        Debug.Log($"[Splash] No existing account found on instance {instanceId}, showing new game option");
        ShowNewGameOption();
      }
    }
  }

  private void CreateUI()
  {
    Debug.Log($"[Splash] CreateUI() called on instance {instanceId}");

    // Create Canvas
    GameObject canvasObj = new GameObject("SplashUI");
    uiCanvas = canvasObj.AddComponent<Canvas>();
    uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
    uiCanvas.sortingOrder = 100; // Ensure it's on top
    canvasObj.AddComponent<CanvasScaler>();
    canvasObj.AddComponent<GraphicRaycaster>();

    // Get screen dimensions
    float screenHeight = Screen.height;
    float bottomThirdY = -screenHeight / 3f; // Bottom third of screen
    float buttonY = bottomThirdY + 60; // Consistent button level
    float newGameButtonY = buttonY - 70; // New Game button lower

    // Create loading text (above loading bar)
    GameObject loadingObj = CreateText("LoadingText", "Loading...", new Vector2(0, -screenHeight / 2f + 50));
    loadingText = loadingObj.GetComponent<TextMeshProUGUI>();
    loadingText.fontSize = 20;

    // Create loading bar (bottom of screen)
    GameObject loadingBarObj = CreateLoadingBar("LoadingBar");
    loadingBar = loadingBarObj.GetComponent<Slider>();

    // Create new game button (lower position)
    GameObject newGameObj = CreateButton("NewGameButton", "New Game", new Vector2(0, newGameButtonY));
    newGameButton = newGameObj.GetComponent<Button>();
    newGameButton.onClick.AddListener(OnNewGameClicked);
    newGameObj.SetActive(false); // Hidden initially

    // Create username input (higher position)
    GameObject inputObj = CreateInputField("UsernameInput", "Enter username...", new Vector2(50, buttonY));
    usernameInput = inputObj.GetComponent<TMP_InputField>();
    usernameInput.onValueChanged.AddListener(OnUsernameChanged);
    usernameInput.onSubmit.AddListener((string value) => OnStartGameClicked());
    inputObj.SetActive(false); // Hidden initially

    // Create start game button (same position as new game button)
    GameObject startGameObj = CreateButton("StartGameButton", "Start Game", new Vector2(0, newGameButtonY));
    startGameButton = startGameObj.GetComponent<Button>();
    startGameButton.onClick.AddListener(OnStartGameClicked);
    startGameObj.SetActive(false); // Hidden initially

    // Create error text (below buttons)
    GameObject errorObj = CreateText("ErrorText", "", new Vector2(0, newGameButtonY - 40));
    errorText = errorObj.GetComponent<TextMeshProUGUI>();
    errorText.color = Color.red;
    errorObj.SetActive(false); // Hidden initially

    // Create email text (above username input) - LEFT ALIGNED
    GameObject emailObj = CreateText("EmailText", "", new Vector2(209, buttonY + 50));
    emailText = emailObj.GetComponent<TextMeshProUGUI>();
    emailText.alignment = TextAlignmentOptions.Left;
    emailText.color = Color.gray;
    emailObj.SetActive(false); // Hidden initially

    // Create email label (to the left of email text)
    GameObject emailLabelObj = CreateText("EmailLabel", "Email:", new Vector2(-420, buttonY + 50));
    emailLabel = emailLabelObj.GetComponent<TextMeshProUGUI>();
    emailLabel.alignment = TextAlignmentOptions.Right;
    emailLabel.color = new Color(0.8f, 0.8f, 0.8f, 1f);
    emailLabel.fontSize = 24;
    emailLabelObj.SetActive(false); // Hidden initially

    // Create username label (to the left of username input)
    GameObject usernameLabelObj = CreateText("UsernameLabel", "Username:", new Vector2(-420, buttonY));
    usernameLabel = usernameLabelObj.GetComponent<TextMeshProUGUI>();
    usernameLabel.alignment = TextAlignmentOptions.Right;
    usernameLabel.color = new Color(0.8f, 0.8f, 0.8f, 1f);
    usernameLabel.fontSize = 24;
    usernameLabelObj.SetActive(false); // Hidden initially
  }

  private GameObject CreateLoadingBar(string name)
  {
    GameObject obj = new GameObject(name);
    obj.transform.SetParent(uiCanvas.transform, false);

    // Background - full width gray bar at bottom
    GameObject background = new GameObject("Background");
    background.transform.SetParent(obj.transform, false);
    Image bgImage = background.AddComponent<Image>();
    bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

    // Fill area - full width loading bar
    GameObject fillArea = new GameObject("Fill Area");
    fillArea.transform.SetParent(obj.transform, false);
    RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();

    // Fill - blue progress bar
    GameObject fill = new GameObject("Fill");
    fill.transform.SetParent(fillArea.transform, false);
    Image fillImage = fill.AddComponent<Image>();
    fillImage.color = new Color(0.2f, 0.6f, 1f, 1f);

    Slider slider = obj.AddComponent<Slider>();
    slider.fillRect = fill.GetComponent<RectTransform>();
    slider.value = 0f;

    RectTransform rect = obj.GetComponent<RectTransform>();
    rect.anchorMin = new Vector2(0, 0); // Anchor to bottom-left
    rect.anchorMax = new Vector2(1, 0); // Anchor to bottom-right
    rect.offsetMin = new Vector2(0, 0); // No offset from bottom
    rect.offsetMax = new Vector2(0, 8); // 8 pixels tall
    rect.anchoredPosition = Vector2.zero;

    // Background - full width gray bar
    RectTransform bgRect = background.GetComponent<RectTransform>();
    bgRect.anchorMin = new Vector2(0, 0);
    bgRect.anchorMax = new Vector2(1, 0);
    bgRect.offsetMin = new Vector2(0, 0);
    bgRect.offsetMax = new Vector2(0, 8);
    bgRect.anchoredPosition = Vector2.zero;

    // Fill area - full width for the loading bar
    fillAreaRect.anchorMin = new Vector2(0, 0); // Full width
    fillAreaRect.anchorMax = new Vector2(1, 0); // Full width
    fillAreaRect.offsetMin = new Vector2(0, 0);
    fillAreaRect.offsetMax = new Vector2(0, 8);
    fillAreaRect.anchoredPosition = Vector2.zero;

    // Fill - blue progress bar (same height as background, fills from left to right)
    RectTransform fillRect = fill.GetComponent<RectTransform>();
    fillRect.anchorMin = new Vector2(0, 0);
    fillRect.anchorMax = new Vector2(1, 0);
    fillRect.offsetMin = new Vector2(0, 0);
    fillRect.offsetMax = new Vector2(0, 8);
    fillRect.anchoredPosition = Vector2.zero;

    return obj;
  }

  private void UpdateLoadingProgress(float progress, string message)
  {
    if (loadingBar != null)
      loadingBar.value = progress;

    if (loadingText != null)
      loadingText.text = message;
  }

  private void ShowNewGameOption()
  {
    Debug.Log($"[Splash] ShowNewGameOption() called on instance {instanceId}");

    isLoading = false;

    // Hide loading elements
    if (loadingText != null) loadingText.gameObject.SetActive(false);
    if (loadingBar != null) loadingBar.gameObject.SetActive(false);

    // Show new game button
    if (newGameButton != null)
    {
      newGameButton.gameObject.SetActive(true);
      Debug.Log($"[Splash] New game button activated on instance {instanceId}");
    }
  }

  private GameObject CreateText(string name, string text, Vector2 position)
  {
    GameObject obj = new GameObject(name);
    obj.transform.SetParent(uiCanvas.transform, false);

    TextMeshProUGUI textComponent = obj.AddComponent<TextMeshProUGUI>();
    textComponent.text = text;
    textComponent.fontSize = 18;
    textComponent.alignment = TextAlignmentOptions.Center;
    textComponent.color = Color.white;
    textComponent.fontStyle = FontStyles.Normal;
    textComponent.enableAutoSizing = false;

    // Use default TMP font asset and material with null checks
    if (TMP_Settings.defaultFontAsset != null)
    {
      textComponent.font = TMP_Settings.defaultFontAsset;
      textComponent.fontSharedMaterial = TMP_Settings.defaultFontAsset.material;
    }
    else
    {
      Debug.LogWarning("TMP_Settings.defaultFontAsset is null. Text may not render properly.");
    }

    RectTransform rect = obj.GetComponent<RectTransform>();
    rect.anchoredPosition = position;
    rect.sizeDelta = new Vector2(600, 80);

    return obj;
  }

  private GameObject CreateButton(string name, string text, Vector2 position)
  {
    GameObject obj = new GameObject(name);
    obj.transform.SetParent(uiCanvas.transform, false);

    Image image = obj.AddComponent<Image>();
    image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

    Button button = obj.AddComponent<Button>();
    button.targetGraphic = image;

    // Create text child with proper settings
    GameObject textObj = new GameObject(name + "Text");
    textObj.transform.SetParent(obj.transform, false);

    TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
    textComponent.text = text;
    textComponent.fontSize = 18; // Normal button font size
    textComponent.alignment = TextAlignmentOptions.Center;
    textComponent.color = Color.white;
    textComponent.fontStyle = FontStyles.Normal;
    textComponent.enableAutoSizing = false;

    // Use default TMP font asset and material with null checks
    if (TMP_Settings.defaultFontAsset != null)
    {
      textComponent.font = TMP_Settings.defaultFontAsset;
      textComponent.fontSharedMaterial = TMP_Settings.defaultFontAsset.material;
    }
    else
    {
      Debug.LogWarning("TMP_Settings.defaultFontAsset is null. Text may not render properly.");
    }

    RectTransform textRect = textObj.GetComponent<RectTransform>();
    textRect.anchoredPosition = Vector2.zero;
    textRect.sizeDelta = new Vector2(280, 50); // Match button width with some padding

    RectTransform rect = obj.GetComponent<RectTransform>();
    rect.anchoredPosition = position;
    rect.sizeDelta = new Vector2(200, 50); // Smaller, more reasonable button size

    return obj;
  }

  private GameObject CreateInputField(string name, string placeholder, Vector2 position)
  {
    GameObject obj = new GameObject(name);
    obj.transform.SetParent(uiCanvas.transform, false);

    Image image = obj.AddComponent<Image>();
    image.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

    TMP_InputField inputField = obj.AddComponent<TMP_InputField>();

    // Create placeholder text
    GameObject placeholderObj = CreateText(name + "Placeholder", placeholder, Vector2.zero);
    placeholderObj.transform.SetParent(obj.transform, false);

    RectTransform placeholderRect = placeholderObj.GetComponent<RectTransform>();
    placeholderRect.anchorMin = Vector2.zero;
    placeholderRect.anchorMax = Vector2.one;
    placeholderRect.offsetMin = new Vector2(10, 5);
    placeholderRect.offsetMax = new Vector2(-10, -5);

    placeholderObj.GetComponent<TextMeshProUGUI>().color = Color.gray;
    placeholderObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
    placeholderObj.GetComponent<TextMeshProUGUI>().fontSize = 18; // Set placeholder font size
    inputField.placeholder = placeholderObj.GetComponent<TextMeshProUGUI>();

    // Create text component
    GameObject textObj = CreateText(name + "Text", "", Vector2.zero);
    textObj.transform.SetParent(obj.transform, false);

    RectTransform textRect = textObj.GetComponent<RectTransform>();
    textRect.anchorMin = Vector2.zero;
    textRect.anchorMax = Vector2.one;
    textRect.offsetMin = new Vector2(10, 5);
    textRect.offsetMax = new Vector2(-10, -5);

    textObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
    textObj.GetComponent<TextMeshProUGUI>().fontSize = 18; // Set input text font size
    inputField.textComponent = textObj.GetComponent<TextMeshProUGUI>();

    // Set character limit to 20
    inputField.characterLimit = 20;

    RectTransform rect = obj.GetComponent<RectTransform>();
    rect.anchoredPosition = position;
    rect.sizeDelta = new Vector2(300, 50);

    return obj;
  }

  private void OnNewGameClicked()
  {
    Debug.Log("OnNewGameClicked called");
    if (isProcessing) return;

    Debug.Log("Showing username input UI");
    // Hide new game button and show username input
    if (newGameButton != null) newGameButton.gameObject.SetActive(false);
    if (usernameInput != null) usernameInput.gameObject.SetActive(true);
    if (startGameButton != null) startGameButton.gameObject.SetActive(true);
    if (emailText != null) emailText.gameObject.SetActive(true);
    if (usernameLabel != null) usernameLabel.gameObject.SetActive(true);
    if (emailLabel != null) emailLabel.gameObject.SetActive(true);

    // Disable hotkeys while form is active
    if (HotkeyManager.Instance != null)
    {
      HotkeyManager.Instance.enabled = false;
    }

    // Show user's email
    if (emailText != null && accountManager != null)
      emailText.text = accountManager.userEmail;

    // Clear any previous errors
    if (errorText != null)
    {
      errorText.text = "";
      errorText.gameObject.SetActive(false);
    }

    // Focus on username input
    if (usernameInput != null)
    {
      usernameInput.text = "";
      usernameInput.Select();
    }
  }

  private void OnUsernameChanged(string value)
  {
    // Clear error when user starts typing
    if (errorText != null && !string.IsNullOrEmpty(errorText.text))
    {
      errorText.text = "";
      errorText.gameObject.SetActive(false);
    }
  }

  private void OnStartGameClicked()
  {
    if (isProcessing) return;

    if (usernameInput == null) return;

    string username = usernameInput.text.Trim();

    // Validate username
    if (string.IsNullOrEmpty(username))
    {
      if (errorText != null)
      {
        errorText.text = "Username cannot be empty";
        errorText.gameObject.SetActive(true);
      }
      return;
    }

    if (username.Length < 3)
    {
      if (errorText != null)
      {
        errorText.text = "Username must be at least 3 characters";
        errorText.gameObject.SetActive(true);
      }
      return;
    }

    if (username.Length > 20)
    {
      if (errorText != null)
      {
        errorText.text = "Username must be 20 characters or less";
        errorText.gameObject.SetActive(true);
      }
      return;
    }

    // Create account
    isProcessing = true;
    if (accountManager != null)
    {
      accountManager.CreateNewAccount(username);

      // Subscribe to account creation event
      accountManager.OnAccountCreated += OnAccountCreated;
    }
  }

  private void OnAccountCreated(AccountManager.UserAccount account)
  {
    isProcessing = false;
    Debug.Log($"Account created: {account.username}");

    // Re-enable hotkeys
    if (HotkeyManager.Instance != null)
    {
      HotkeyManager.Instance.enabled = true;
    }

    // Unsubscribe from event
    if (accountManager != null)
      accountManager.OnAccountCreated -= OnAccountCreated;

    // Transition to main menu
    SceneLoader.Instance.LoadMainMenu();
  }

  public virtual IEnumerator Load()
  {
    yield return new WaitForSeconds(1);
    SceneLoader.Instance.LoadMainMenu();
  }

  private void OnDestroy()
  {
    if (accountManager != null)
      accountManager.OnAccountCreated -= OnAccountCreated;

    // Destroy the Canvas to prevent it from affecting other scenes
    if (uiCanvas != null)
    {
      DestroyImmediate(uiCanvas.gameObject);
    }
  }
}