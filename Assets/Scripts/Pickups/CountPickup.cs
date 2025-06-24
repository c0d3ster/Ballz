using UnityEngine;

public class CountPickup : BasePickup
{
  [Header("Count Pickup Settings")]
  [SerializeField] private bool destroyOnPickup = true;

  protected override void Start()
  {
    // Subscribe to hotkey events for reset
    HotkeyManager.OnResetConfirmed += ResetPickup;
    HotkeyManager.OnResetPickupsPressed += ResetPickup;

    // Call base Start() which will call OnPickupStart()
    base.Start();
  }

  private void OnDestroy()
  {
    // Unsubscribe from hotkey events
    HotkeyManager.OnResetConfirmed -= ResetPickup;
    HotkeyManager.OnResetPickupsPressed -= ResetPickup;
  }

  protected override void OnPickupStart()
  {
    // Ensure this pickup is tagged correctly for CountManager
    if (!gameObject.CompareTag("Pick Up"))
    {
      Debug.LogWarning($"[CountPickup] {gameObject.name} should be tagged 'Pick Up' for CountManager integration. Setting tag now.");
      gameObject.tag = "Pick Up";
    }
  }

  protected override void OnPickupCollected(Collider other)
  {
    if (CountManager.Instance != null)
    {
      // Use CountManager for level completion tracking
      CountManager.Instance.CollectPickup(gameObject);
    }
    else
    {
      Debug.LogWarning($"[CountPickup] CountManager not found for {gameObject.name}");
    }

    if (destroyOnPickup)
    {
      SetActive(false);
    }
  }

  private void ResetPickup()
  {
    // Reactivate the pickup
    SetActive(true);
    Debug.Log($"[CountPickup] {gameObject.name} reset");
  }
}