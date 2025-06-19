using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LivesDisplay : MonoBehaviour
{
  [Header("UI References")]
  [SerializeField] private Transform livesContainer;

  [Header("Display Settings")]
  [SerializeField] private float iconSpacing = 30f;
  [SerializeField] private float iconSize = 25f;
  [SerializeField] private Color availableLifeColor = Color.white;
  [SerializeField] private Color depletedLifeColor = new Color(1f, 1f, 1f, 0.2f); // Very translucent
  [SerializeField] private float outlineWidth = 5f;
  [SerializeField] private Color outlineColor = Color.black;

  [Header("Countdown Timer")]
  [SerializeField] private TextMeshProUGUI countdownText;

  private Image[] lifeIcons;
  private LivesManager livesManager;
  private UIManager uiManager;
  private Material playerMaterial;
  private float lastCountdownUpdate;
  private bool wasWaitingForLife = false;
  private TextMeshProUGUI countText;

  private void Start()
  {
    livesManager = LivesManager.Instance;
    if (livesManager == null)
    {
      Debug.LogError("LivesManager not found!");
      return;
    }

    uiManager = UIManager.Instance;
    if (uiManager == null)
    {
      Debug.LogError("UIManager not found!");
      return;
    }

    // Get references from UIManager if not set
    if (livesContainer == null)
    {
      // Find the lives container in the UIManager's canvas
      Transform canvasTransform = uiManager.touchControllerCanvas.transform;
      livesContainer = canvasTransform.Find("LivesContainer");
    }

    InitializeLivesDisplay();
    CreateCountdownText();
    livesManager.OnLivesChanged += UpdateLivesDisplay;

    // Subscribe to scene changes to find player material
    UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

    Debug.Log($"[LivesDisplay] Initialized with {livesManager.CurrentLives} lives");
  }

  private void Update()
  {
    UpdateCountdownTimer();
  }

  private void OnDestroy()
  {
    if (livesManager != null)
    {
      livesManager.OnLivesChanged -= UpdateLivesDisplay;
    }
    UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
  }

  private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
  {
    // Only process non-additive scene loads
    if (mode != UnityEngine.SceneManagement.LoadSceneMode.Additive)
    {
      FindPlayerMaterial();
      UpdateLivesDisplay(livesManager.CurrentLives);
    }
  }

  private System.Collections.IEnumerator FindPlayerMaterialDelayed()
  {
    // Wait a frame to ensure scene is fully loaded
    yield return null;

    FindPlayerMaterial();

    // Update the display with the current lives count and new material
    UpdateLivesDisplay(livesManager.CurrentLives);
  }

  private void FindPlayerMaterial()
  {
    // Try to find the player in the current scene
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
      Renderer playerRenderer = player.GetComponent<Renderer>();
      if (playerRenderer != null && playerRenderer.material != null)
      {
        playerMaterial = playerRenderer.material;
        Debug.Log($"[LivesDisplay] Found player material: {playerMaterial.name}");
      }
    }

    // If no player found, we'll use a simple colored circle
    if (playerMaterial == null)
    {
      Debug.Log("[LivesDisplay] No player material found, using fallback circle for lives display");
    }
  }

  private void InitializeLivesDisplay()
  {
    if (livesContainer == null)
    {
      Debug.LogError("Lives container not found!");
      return;
    }

    Debug.Log($"[LivesDisplay] Creating {livesManager.MaxLives} life icons in container: {livesContainer.name}");

    // Clear existing icons
    foreach (Transform child in livesContainer)
    {
      Destroy(child.gameObject);
    }

    // Create life icons
    lifeIcons = new Image[livesManager.MaxLives];
    for (int i = 0; i < livesManager.MaxLives; i++)
    {
      // Create life icon GameObject
      GameObject iconObj = new GameObject($"LifeIcon_{i}");
      iconObj.transform.SetParent(livesContainer);

      Debug.Log($"[LivesDisplay] Created LifeIcon_{i} as child of {livesContainer.name}");

      // Add Image component
      Image image = iconObj.AddComponent<Image>();

      // Create a better-looking circle with outline effect
      image.sprite = CreateCircleWithOutlineSprite();

      // Don't set color here - let UpdateLivesDisplay handle it
      image.color = Color.white; // Default white, will be tinted by UpdateLivesDisplay

      image.preserveAspect = true;

      // Set position - positioned horizontally aligned with gear
      RectTransform rectTransform = iconObj.GetComponent<RectTransform>();
      if (rectTransform != null)
      {
        float xPosition = i * (iconSize + iconSpacing);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(xPosition, 0);
        rectTransform.sizeDelta = new Vector2(iconSize, iconSize);
      }

      lifeIcons[i] = image;
    }

    Debug.Log($"[LivesDisplay] Created {livesManager.MaxLives} life icons");
  }

  private Sprite CreateCircleWithOutlineSprite()
  {
    // Create a circle with outline effect - higher resolution for smooth edges
    int size = 128; // Increased from 64 for better quality
    Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
    texture.filterMode = FilterMode.Bilinear; // Changed from Point to Bilinear for smooth edges
    texture.wrapMode = TextureWrapMode.Clamp;

    Vector2 center = new Vector2(size / 2f, size / 2f);
    float radius = (size / 2f) - outlineWidth;
    float outerRadius = size / 2f;

    for (int x = 0; x < size; x++)
    {
      for (int y = 0; y < size; y++)
      {
        float distance = Vector2.Distance(new Vector2(x, y), center);

        if (distance <= radius)
        {
          // Inner circle - white (will be tinted by color)
          texture.SetPixel(x, y, Color.white);
        }
        else if (distance <= outerRadius)
        {
          // Outline - black with anti-aliasing
          float alpha = 1f - Mathf.Clamp01((distance - radius) / outlineWidth);
          Color outlineColorWithAlpha = new Color(outlineColor.r, outlineColor.g, outlineColor.b, alpha);
          texture.SetPixel(x, y, outlineColorWithAlpha);
        }
        else
        {
          // Outside - transparent
          texture.SetPixel(x, y, Color.clear);
        }
      }
    }

    texture.Apply();

    // Create sprite from texture with smooth settings
    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
    return sprite;
  }

  private void UpdateLivesDisplay(int currentLives)
  {
    if (lifeIcons == null) return;

    Debug.Log($"[LivesDisplay] Updating display: {currentLives} lives remaining");

    for (int i = 0; i < lifeIcons.Length; i++)
    {
      if (lifeIcons[i] != null)
      {
        bool isAvailable = i < currentLives;

        // Set color based on whether this life is available
        Color targetColor;

        if (isAvailable)
        {
          if (playerMaterial != null)
          {
            // Use player material color with lighting effect
            Color playerColor = playerMaterial.color;
            targetColor = new Color(
              Mathf.Min(1f, playerColor.r * 1.3f),
              Mathf.Min(1f, playerColor.g * 1.3f),
              Mathf.Min(1f, playerColor.b * 1.3f),
              playerColor.a
            );
          }
          else
          {
            targetColor = availableLifeColor;
          }
        }
        else
        {
          // Depleted life - very translucent
          targetColor = depletedLifeColor;
        }

        lifeIcons[i].color = targetColor;
      }
    }
  }

  // Call this when player skin/material changes
  public void UpdatePlayerMaterial(Material newMaterial)
  {
    playerMaterial = newMaterial;
    Debug.Log($"[LivesDisplay] Player material updated: {newMaterial?.name ?? "null"}");
    UpdateLivesDisplay(livesManager.CurrentLives);
  }

  // Call this to refresh the lives display (useful when lives regenerate)
  public void RefreshLives()
  {
    if (livesManager != null)
    {
      FindPlayerMaterial();
      UpdateLivesDisplay(livesManager.CurrentLives);
      Debug.Log($"[LivesDisplay] Lives refreshed: {livesManager.CurrentLives} lives");
    }
  }

  private void CreateCountdownText()
  {
    Canvas canvas = UIManager.Instance?.touchControllerCanvas;
    if (canvas == null)
    {
      Debug.LogError("[LivesDisplay] No canvas found for countdown text!");
      return;
    }

    GameObject countdownObj = new GameObject("CountdownText");
    countdownObj.transform.SetParent(canvas.transform, false);

    var tmp = countdownObj.AddComponent<TMPro.TextMeshProUGUI>();
    tmp.text = "Next life: 00:00";
    tmp.fontSize = 24;
    tmp.color = Color.white;
    tmp.alignment = TMPro.TextAlignmentOptions.TopLeft;
    tmp.textWrappingMode = TMPro.TextWrappingModes.NoWrap;

    RectTransform rectTransform = countdownObj.GetComponent<RectTransform>();
    rectTransform.anchorMin = new Vector2(0, 1);
    rectTransform.anchorMax = new Vector2(0, 1);
    rectTransform.pivot = new Vector2(0, 1);
    rectTransform.anchoredPosition = new Vector2(40, -100);
    rectTransform.sizeDelta = new Vector2(250, 60);
    rectTransform.localScale = Vector3.one;

    countdownText = tmp;
  }

  private void UpdateCountdownTimer()
  {
    if (countdownText == null || livesManager == null) return;

    // Only show countdown on main menu
    bool isMainMenu = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Active Main Menu";

    // Hide countdown if not on main menu or if at max lives
    bool shouldShowCountdown = isMainMenu && livesManager.CurrentLives < livesManager.MaxLives;
    countdownText.gameObject.SetActive(shouldShowCountdown);

    if (shouldShowCountdown)
    {
      float timeUntilNextLife = livesManager.TimeUntilNextLife;

      if (timeUntilNextLife > 0)
      {
        int minutes = Mathf.FloorToInt(timeUntilNextLife / 60f);
        int seconds = Mathf.FloorToInt(timeUntilNextLife % 60f);
        countdownText.text = $"Next life: {minutes:00}:{seconds:00}";
        wasWaitingForLife = true;
      }
      else
      {
        // If time is 0, we should be at max lives, so hide the text
        countdownText.gameObject.SetActive(false);

        // If we were waiting for a life and now it's ready, refresh the display
        if (wasWaitingForLife)
        {
          RefreshLives();
          wasWaitingForLife = false;
        }
      }
    }
  }
}