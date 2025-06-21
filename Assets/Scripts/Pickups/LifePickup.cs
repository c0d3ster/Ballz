using UnityEngine;
using TMPro;
using System;

public class LifePickup : BasePickup
{
  [Header("Respawn Settings")]
  [SerializeField] private float respawnTimeMinutes = 5f;

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

  protected override void OnPickupStart()
  {
    // Find child objects by name
    pickupSphere = transform.Find("Pickup Sphere")?.gameObject;
    respawnTimer = transform.Find("Respawn Timer")?.gameObject;

    // Validate prefab structure
    if (pickupSphere == null || respawnTimer == null)
    {
      Debug.LogError("LifePickup: Missing child objects! Make sure you have 'Pickup Sphere' and 'Respawn Timer' children.");
      return;
    }

    // Get the sphere renderer
    sphereRenderer = pickupSphere.GetComponent<Renderer>();
    if (sphereRenderer == null)
    {
      Debug.LogError("LifePickup: No Renderer found on the Pickup Sphere!");
      return;
    }

    // Get countdown text component
    countdownText = respawnTimer.GetComponent<TextMeshPro>();
    if (countdownText == null)
    {
      Debug.LogError("LifePickup: No TextMeshPro found on the Respawn Timer!");
      return;
    }

    // Find player material and create translucent copy
    FindPlayerMaterialAndCreateTranslucent();

    // Load last pickup time AFTER finding all components
    LoadLastPickupTime();

    // Check if we should be respawning AFTER everything is set up
    CheckRespawnStatus();

    Debug.Log("LifePickup initialized successfully");
  }

  private void LoadLastPickupTime()
  {
    string key = PICKUP_TIME_KEY + gameObject.name;
    string lastPickupTicksString = PlayerPrefs.GetString(key, "0");

    if (long.TryParse(lastPickupTicksString, out long ticks))
    {
      lastPickupTime = new DateTime(ticks, DateTimeKind.Utc);
      Debug.Log($"[LifePickup] Loaded last pickup time for {gameObject.name}: {lastPickupTime}");
    }
    else
    {
      lastPickupTime = DateTime.UtcNow.AddMinutes(-respawnTimeMinutes - 1); // Allow immediate pickup
      Debug.Log($"[LifePickup] No previous pickup time found for {gameObject.name}, allowing immediate pickup");
    }
  }

  private void SaveLastPickupTime()
  {
    string key = PICKUP_TIME_KEY + gameObject.name;
    PlayerPrefs.SetString(key, lastPickupTime.Ticks.ToString());
    PlayerPrefs.Save();
    Debug.Log($"[LifePickup] Saved pickup time for {gameObject.name}: {lastPickupTime}");
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
      Debug.Log($"[LifePickup] {gameObject.name} is still respawning. Time remaining: {respawnTimeSeconds - timeSincePickup.TotalSeconds:F1}s");
    }
    else
    {
      // Ready to be picked up
      isRespawning = false;
      SetPickupActive(true);
      Debug.Log($"[LifePickup] {gameObject.name} is ready to be picked up");
    }
  }

  private void SetPickupActive(bool active)
  {
    // Enable/disable the Pickup Sphere (includes +1 text)
    pickupSphere.SetActive(active);

    // Show/hide Respawn Timer
    respawnTimer.SetActive(!active);
  }

  private void Update()
  {
    if (isRespawning)
    {
      UpdateCountdown();
    }

    // Reset this pickup when R key is pressed (for testing)
    if (Input.GetKeyDown(KeyCode.R))
    {
      ResetPickup();
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
      Debug.Log($"[LifePickup] {gameObject.name} has respawned!");
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
    Renderer playerRenderer = player.GetComponent<Renderer>();

    if (playerRenderer?.material != null)
    {
      Color playerColor = playerRenderer.material.color;
      CreateTranslucentMaterial(playerColor);
    }
    // If no player material found, keep the prefab's default green material
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
    Debug.Log($"[LifePickup] Created translucent material with player color: {playerColor}");
  }

  private void OnDestroy()
  {
    // Clean up the created material
    if (translucentMaterial != null)
    {
      DestroyImmediate(translucentMaterial);
    }
  }

  protected override void OnPickupCollected(Collider other)
  {
    Debug.Log($"[LifePickup] HandlePickup called with {other?.name ?? "null"} (tag: {other?.tag ?? "null"})");

    if (!isRespawning)
    {
      // Check if player is already at max lives
      if (LivesManager.Instance.CurrentLives >= LivesManager.Instance.MaxLives)
      {
        Debug.Log($"[LifePickup] Pickup ignored - player already at max lives ({LivesManager.Instance.CurrentLives}/{LivesManager.Instance.MaxLives})");
        return;
      }

      // Add a life to the player
      LivesManager.Instance.AddLife();
      Debug.Log($"LifePickup: Added life! Current lives: {LivesManager.Instance.CurrentLives}");

      // Set pickup time and start respawn
      lastPickupTime = DateTime.UtcNow;
      SaveLastPickupTime();
      isRespawning = true;
      SetPickupActive(false);

      Debug.Log($"[LifePickup] {gameObject.name} picked up, starting {respawnTimeMinutes} minute respawn");
    }
    else
    {
      Debug.Log($"[LifePickup] Pickup ignored - currently respawning");
    }
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

    Debug.Log($"[LifePickup] {gameObject.name} reset - ready for pickup");
  }
}