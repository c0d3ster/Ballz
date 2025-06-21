using UnityEngine;
using TMPro;

public class LifePickup : MonoBehaviour
{
  [Header("Text Settings")]
  [SerializeField] private Color textColor = Color.white;
  [SerializeField] private Color outlineColor = Color.black;

  private Renderer sphereRenderer;
  private Material originalMaterial;
  private Material translucentMaterial;

  private void Start()
  {
    // Get the sphere renderer
    sphereRenderer = GetComponent<Renderer>();
    if (sphereRenderer == null)
    {
      Debug.LogError("LifePickup: No Renderer found on the sphere!");
      return;
    }

    // Find player material and create translucent copy
    FindPlayerMaterialAndCreateTranslucent();

    Debug.Log("LifePickup initialized successfully");
  }

  private void FindPlayerMaterialAndCreateTranslucent()
  {
    Debug.Log("[LifePickup] Searching for player material...");

    // Try to find the player in the current scene
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
      Debug.Log($"[LifePickup] Found player: {player.name}");
      Renderer playerRenderer = player.GetComponent<Renderer>();
      if (playerRenderer != null && playerRenderer.material != null)
      {
        originalMaterial = playerRenderer.material;
        Debug.Log($"[LifePickup] Found player material: {originalMaterial.name}, color: {originalMaterial.color}");
        CreateTranslucentMaterial();
      }
      else
      {
        Debug.LogWarning("[LifePickup] Player found but no Renderer or material!");
        CreateDefaultTranslucentMaterial();
      }
    }
    else
    {
      Debug.LogWarning("[LifePickup] No player found with 'Player' tag!");
      CreateDefaultTranslucentMaterial();
    }
  }

  private void CreateDefaultTranslucentMaterial()
  {
    // Create a default translucent material if no player material found
    translucentMaterial = new Material(Shader.Find("Standard"));
    translucentMaterial.SetFloat("_Mode", 3); // Transparent mode
    translucentMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
    translucentMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
    translucentMaterial.SetInt("_ZWrite", 0);
    translucentMaterial.DisableKeyword("_ALPHATEST_ON");
    translucentMaterial.EnableKeyword("_ALPHABLEND_ON");
    translucentMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    translucentMaterial.renderQueue = 3000;

    // Set default translucent green color
    Color defaultColor = new Color(0.2f, 1f, 0.2f, 0.3f);
    translucentMaterial.color = defaultColor;

    // Apply the translucent material
    sphereRenderer.material = translucentMaterial;
    Debug.Log("[LifePickup] Applied default translucent material");
  }

  private void CreateTranslucentMaterial()
  {
    // Create a copy of the original material
    translucentMaterial = new Material(originalMaterial);

    // Set the shader to a transparent one
    translucentMaterial.shader = Shader.Find("Standard");
    translucentMaterial.SetFloat("_Mode", 3); // Transparent mode
    translucentMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
    translucentMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
    translucentMaterial.SetInt("_ZWrite", 0);
    translucentMaterial.DisableKeyword("_ALPHATEST_ON");
    translucentMaterial.EnableKeyword("_ALPHABLEND_ON");
    translucentMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    translucentMaterial.renderQueue = 3000;

    // Set the alpha to 3/4 of the player's opacity
    Color playerColor = originalMaterial.color;
    translucentMaterial.color = new Color(playerColor.r, playerColor.g, playerColor.b, playerColor.a * 0.75f);

    // Apply the translucent material
    sphereRenderer.material = translucentMaterial;
  }

  private void Update()
  {
    // Make the ball face the camera (only update every few frames to prevent flickering)
    if (Camera.main != null && Time.frameCount % 8 == 0)
    {
      transform.LookAt(Camera.main.transform);
      transform.Rotate(0, 180, 0); // Flip to face camera properly
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      // Add a life to the player
      LivesManager livesManager = LivesManager.Instance;
      if (livesManager != null)
      {
        livesManager.AddLife();
        Debug.Log($"LifePickup: Added life! Current lives: {livesManager.CurrentLives}");
      }
      else
      {
        Debug.LogError("LifePickup: LivesManager not found!");
      }

      // Destroy the pickup
      Destroy(gameObject);
    }
  }

  private void OnDestroy()
  {
    // Clean up the created material
    if (translucentMaterial != null)
    {
      DestroyImmediate(translucentMaterial);
    }
  }
}