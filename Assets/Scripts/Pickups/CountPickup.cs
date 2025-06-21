using UnityEngine;

public class CountPickup : BasePickup
{
  [Header("Count Pickup Settings")]
  [SerializeField] private bool destroyOnPickup = true;
  [SerializeField] private bool useCountManager = true;

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
    if (useCountManager && !gameObject.CompareTag("Pick Up"))
    {
      Debug.LogWarning($"[CountPickup] {gameObject.name} should be tagged 'Pick Up' for CountManager integration. Setting tag now.");
      gameObject.tag = "Pick Up";
    }
  }

  public override void HandlePickup(Collider other)
  {
    if (!isActive) return;

    // Check if the other object matches our pickup tag
    if (other.CompareTag(pickupTag))
    {
      OnPickupCollected(other);
    }
  }

  protected override void OnPickupCollected(Collider other)
  {
    Debug.Log($"[CountPickup] {gameObject.name} collected by {other.name} (tag: {other.tag})");

    if (useCountManager && CountManager.Instance != null)
    {
      // Use CountManager for level completion tracking
      CountManager.Instance.CollectPickup(gameObject);
    }
    else
    {
      // Direct pickup without CountManager
      OnDirectPickup(other);
    }

    if (destroyOnPickup)
    {
      SetActive(false);
    }
  }

  protected virtual void OnDirectPickup(Collider other)
  {
    // Override in derived classes for custom pickup behavior
    Debug.Log($"[CountPickup] Direct pickup of {gameObject.name}");
  }

  private void ResetPickup()
  {
    // Reactivate the pickup
    SetActive(true);
    Debug.Log($"[CountPickup] {gameObject.name} reset");
  }
}