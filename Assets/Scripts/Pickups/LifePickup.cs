using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class LifePickup : BasePickup
{
  [Header("Respawn Settings")]
  [SerializeField] private int respawnTimeMinutes = 5;

  [Header("Text Settings")]
  [SerializeField] private Color textColor = Color.white;
  [SerializeField] private Color outlineColor = Color.black;

  private Renderer sphereRenderer;
  private Material translucentMaterial;
  private TextMeshPro countdownText;
  private GameObject pickupSphere;
  private GameObject respawnTimer;

  // Respawn tracking
  private DateTime lastPickupTime;
  private bool isRespawning = false;
  private const string PICKUP_TIME_KEY = "LifePickup_LastPickupTime_";

  protected override void Start()
  {
    // Subscribe to hotkey events
    HotkeyManager.OnResetConfirmed += ResetPickup;
    HotkeyManager.OnResetPickupsPressed += ResetPickup;

    // Call base Start() which will call OnPickupStart()
    base.Start();
  }

  protected override void OnPickupStart()
  {
    // Initialize the pickup
    InitializePickup();
  }

  private void InitializePickup()
  {
    // Find child objects by name
    pickupSphere = transform.Find("Pickup Sphere")?.gameObject;
    respawnTimer = transform.Find("Respawn Timer")?.gameObject;

    // Validate prefab structure
    if (pickupSphere == null || respawnTimer == null)
    {
      Debug.LogError($"LifePickup: Missing child objects on {gameObject.name}! Make sure you have 'Pickup Sphere' and 'Respawn Timer' children.");
      return;
    }

    // Get the sphere renderer
    sphereRenderer = pickupSphere.GetComponent<Renderer>();
    if (sphereRenderer == null)
    {
      Debug.LogError($"LifePickup: No Renderer found on the Pickup Sphere for {gameObject.name}!");
      return;
    }

    // Get countdown text component
    countdownText = respawnTimer.GetComponent<TextMeshPro>();
    if (countdownText == null)
    {
      Debug.LogError($"LifePickup: No TextMeshPro found on the Respawn Timer for {gameObject.name}!");
      return;
    }

    // Find player material and create translucent copy
    FindPlayerMaterialAndCreateTranslucent();

    // Load last pickup time
    LoadLastPickupTime();

    // Check if we should be respawning
    CheckRespawnStatus();
  }

  private void OnDestroy()
  {
    // Unsubscribe from hotkey events
    HotkeyManager.OnResetConfirmed -= ResetPickup;
    HotkeyManager.OnResetPickupsPressed -= ResetPickup;

    // Clean up the created material
    if (translucentMaterial != null)
    {
      DestroyImmediate(translucentMaterial);
    }
  }

  private void LoadLastPickupTime()
  {
    string key = PICKUP_TIME_KEY + gameObject.name;
    string lastPickupTicksString = PlayerPrefs.GetString(key, "0");

    if (long.TryParse(lastPickupTicksString, out long ticks))
    {
      lastPickupTime = new DateTime(ticks, DateTimeKind.Utc);
    }
    else
    {
      lastPickupTime = DateTime.UtcNow.AddMinutes(-respawnTimeMinutes - 1); // Allow immediate pickup
    }
  }

  private void SaveLastPickupTime()
  {
    string key = PICKUP_TIME_KEY + gameObject.name;
    PlayerPrefs.SetString(key, lastPickupTime.Ticks.ToString());
    PlayerPrefs.Save();
  }

  private void CheckRespawnStatus()
  {
    TimeSpan timeSincePickup = DateTime.UtcNow - lastPickupTime;
    double respawnTimeSeconds = respawnTimeMinutes * 60.0;

    if (timeSincePickup.TotalSeconds < respawnTimeSeconds)
    {
      // Still respawning
      isRespawning = true;
      SetPickupActive(false);
    }
    else
    {
      // Ready to be picked up
      isRespawning = false;
      SetPickupActive(true);
    }
  }

  private void SetPickupActive(bool active)
  {
    // Enable/disable the Pickup Sphere (includes +1 text)
    if (pickupSphere != null)
    {
      pickupSphere.SetActive(active);
    }

    // Show/hide Respawn Timer
    if (respawnTimer != null)
    {
      respawnTimer.SetActive(!active);
    }
  }

  private void Update()
  {
    if (isRespawning)
    {
      UpdateCountdown();
    }
  }

  private void UpdateCountdown()
  {
    if (countdownText == null) return;

    TimeSpan timeSincePickup = DateTime.UtcNow - lastPickupTime;
    double respawnTimeSeconds = respawnTimeMinutes * 60.0;
    double timeRemaining = respawnTimeSeconds - timeSincePickup.TotalSeconds;

    if (timeRemaining <= 0)
    {
      // Respawn complete
      isRespawning = false;
      SetPickupActive(true);
    }
    else
    {
      // Update countdown text
      double remainingMinutes = timeRemaining / 60.0;
      if (remainingMinutes >= 60)
      {
        // Show hours:minutes:seconds format
        int hours = Mathf.FloorToInt((float)timeRemaining / 3600f);
        int minutes = Mathf.FloorToInt((float)timeRemaining % 3600f / 60f);
        int seconds = Mathf.FloorToInt((float)timeRemaining % 60f);
        countdownText.text = $"{hours}:{minutes:00}:{seconds:00}";
      }
      else
      {
        // Show minutes:seconds format
        int minutes = Mathf.FloorToInt((float)timeRemaining / 60f);
        int seconds = Mathf.FloorToInt((float)timeRemaining % 60f);
        countdownText.text = $"{minutes:00}:{seconds:00}";
      }
    }
  }

  private void FindPlayerMaterialAndCreateTranslucent()
  {
    // Find the player in the current scene
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player == null)
    {
      Debug.LogWarning($"[LifePickup] No player found with tag 'Player' for {gameObject.name}");
      return;
    }

    Renderer playerRenderer = player.GetComponent<Renderer>();
    if (playerRenderer == null)
    {
      Debug.LogWarning($"[LifePickup] No Renderer found on player {player.name} for {gameObject.name}");
      return;
    }

    if (playerRenderer.material != null)
    {
      Color playerColor = playerRenderer.material.color;
      CreateTranslucentMaterial(playerColor);
    }
    else
    {
      Debug.LogWarning($"[LifePickup] Player material is null for {gameObject.name}");
    }
  }

  private void CreateTranslucentMaterial(Color playerColor)
  {
    // Create a new translucent material
    translucentMaterial = new Material(Shader.Find("Standard"));
    translucentMaterial.SetFloat("_Mode", 3); // Transparent mode
    translucentMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
    translucentMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
    translucentMaterial.SetInt("_ZWrite", 0);
    translucentMaterial.DisableKeyword("_ALPHATEST_ON");
    translucentMaterial.EnableKeyword("_ALPHABLEND_ON");
    translucentMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    translucentMaterial.renderQueue = 3000;

    // Set the color to match the player but with 75% opacity
    translucentMaterial.color = new Color(playerColor.r, playerColor.g, playerColor.b, playerColor.a * 0.75f);

    // Apply the translucent material
    sphereRenderer.material = translucentMaterial;
  }

  public override void HandlePickup(Collider other)
  {
    // Don't process pickup if we're respawning
    if (isRespawning)
    {
      return;
    }

    // Only log if it's actually a relevant collision
    if (other.CompareTag(base.PickupTag))
    {
      int currentLives = LivesManager.Instance != null ? LivesManager.Instance.CurrentLives : 0;
      int maxLives = LivesManager.Instance != null ? LivesManager.Instance.MaxLives : 0;
      Debug.Log($"{base.PickupTag} collided with life pickup. Lives: {currentLives}/{maxLives}");
    }

    // Don't process pickup if player is at max lives
    if (LivesManager.Instance != null && LivesManager.Instance.CurrentLives >= LivesManager.Instance.MaxLives)
    {
      return;
    }

    // Call base implementation if all checks pass
    base.HandlePickup(other);
  }

  protected override void OnPickupCollected(Collider other)
  {
    // Add a life to the player
    LivesManager livesManager = LivesManager.Instance;

    // Fallback: find LivesManager in scene if static reference is null (happens during script recompilation)
    if (livesManager == null)
    {
      livesManager = FindFirstObjectByType<LivesManager>();
      if (livesManager != null)
      {
        Debug.Log($"[LifePickup] Found LivesManager instance in {gameObject.name}");
      }
    }

    if (livesManager != null)
    {
      livesManager.AddLife();
    }
    else
    {
      Debug.LogWarning($"[LifePickup] LivesManager.Instance is null and no LivesManager found in scene for {gameObject.name}. Life pickup ignored.");
    }

    // Set pickup time and start respawn
    lastPickupTime = DateTime.UtcNow;
    SaveLastPickupTime();
    isRespawning = true;
    SetPickupActive(false);
  }

  [ContextMenu("Reset Pickup")]
  private void ResetPickup()
  {
    // Clear the saved pickup time
    string key = PICKUP_TIME_KEY + gameObject.name;
    PlayerPrefs.DeleteKey(key);
    PlayerPrefs.Save();

    // Reset the pickup state
    lastPickupTime = DateTime.UtcNow.AddMinutes(-respawnTimeMinutes - 1); // Allow immediate pickup
    isRespawning = false;
    SetPickupActive(true);
  }
}