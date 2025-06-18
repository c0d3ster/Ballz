using UnityEngine;
using UnityEngine.UI;

public class LivesDisplay : MonoBehaviour
{
  [Header("UI References")]
  [SerializeField] private Transform livesContainer;

  [Header("Display Settings")]
  [SerializeField] private float iconSpacing = 25f;
  [SerializeField] private float iconSize = 25f;
  [SerializeField] private Color availableLifeColor = Color.white;
  [SerializeField] private Color depletedLifeColor = new Color(1f, 1f, 1f, 0.2f); // Very translucent
  [SerializeField] private float outlineWidth = 2f;
  [SerializeField] private Color outlineColor = Color.black;

  private Image[] lifeIcons;
  private LivesManager livesManager;
  private UIManager uiManager;
  private Material playerMaterial;

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

    // Find the player to get their material
    FindPlayerMaterial();

    InitializeLivesDisplay();
    livesManager.OnLivesChanged += UpdateLivesDisplay;

    // Update the display with the current lives count
    UpdateLivesDisplay(livesManager.CurrentLives);

    Debug.Log($"[LivesDisplay] Initialized with {livesManager.CurrentLives} lives");
  }

  private void OnDestroy()
  {
    if (livesManager != null)
    {
      livesManager.OnLivesChanged -= UpdateLivesDisplay;
    }
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

      // Add Image component
      Image image = iconObj.AddComponent<Image>();

      // Create a better-looking circle with outline effect
      image.sprite = CreateCircleWithOutlineSprite();

      // Don't set color here - let UpdateLivesDisplay handle it
      image.color = Color.white; // Default white, will be tinted by UpdateLivesDisplay

      image.preserveAspect = true;

      // Set position - positioned below CountText (which is at y: -20)
      RectTransform rectTransform = iconObj.GetComponent<RectTransform>();
      if (rectTransform != null)
      {
        // Position horizontally with spacing, vertically stacked
        float xPosition = i * (iconSize + iconSpacing);
        float yPosition = 0; // Start from top of container
        rectTransform.anchoredPosition = new Vector2(xPosition, yPosition);
        rectTransform.sizeDelta = new Vector2(iconSize, iconSize);
      }

      lifeIcons[i] = image;
    }

    Debug.Log($"[LivesDisplay] Created {livesManager.MaxLives} life icons");
  }

  private Sprite CreateCircleWithOutlineSprite()
  {
    // Create a circle with outline effect
    int size = 64;
    Texture2D texture = new Texture2D(size, size);

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
          // Outline - black
          texture.SetPixel(x, y, outlineColor);
        }
        else
        {
          // Outside - transparent
          texture.SetPixel(x, y, Color.clear);
        }
      }
    }

    texture.Apply();

    // Create sprite from texture
    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
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
}