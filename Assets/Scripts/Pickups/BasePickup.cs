using UnityEngine;

public abstract class BasePickup : MonoBehaviour
{
  [Header("Base Pickup Settings")]
  [SerializeField] protected bool isActive = true;
  [SerializeField] protected string pickupTag = "Player";

  // Public property to access pickupTag
  public string PickupTag => pickupTag;

  protected virtual void Start()
  {
    // Find collider on this object or its children
    var collider = GetComponent<Collider>();
    if (collider == null)
    {
      // Try to find collider on children
      collider = GetComponentInChildren<Collider>();
      if (collider == null)
      {
        Debug.LogError($"[BasePickup] {gameObject.name} has no Collider component on itself or children!");
        return;
      }
    }

    // Enable the collider if it's disabled
    if (!collider.enabled)
    {
      collider.enabled = true;
    }

    // Make sure the collider is a trigger
    if (!collider.isTrigger)
    {
      collider.isTrigger = true;
    }

    // Add trigger handler to the collider we found
    if (collider.GetComponent<PickupTrigger>() == null)
    {
      collider.gameObject.AddComponent<PickupTrigger>();
    }

    // Also add trigger handlers to all other colliders on this object and children
    var allColliders = GetComponentsInChildren<Collider>();
    foreach (var col in allColliders)
    {
      if (col.GetComponent<PickupTrigger>() == null)
      {
        col.gameObject.AddComponent<PickupTrigger>();
      }
    }

    OnPickupStart();
  }

  protected virtual void OnPickupStart()
  {
    // Override in derived classes for initialization
  }

  public virtual void HandlePickup(Collider other)
  {
    if (!isActive)
    {
      return;
    }

    if (other.CompareTag(pickupTag))
    {
      OnPickupCollected(other);
    }
  }

  protected abstract void OnPickupCollected(Collider other);

  public virtual void SetActive(bool active)
  {
    isActive = active;
    gameObject.SetActive(active);
  }

  public virtual bool IsActive => isActive;
}

// Generic trigger handler that can work with any BasePickup
public class PickupTrigger : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    // Find the BasePickup component on this object or its parent
    var pickup = GetComponent<BasePickup>();
    if (pickup == null)
    {
      pickup = GetComponentInParent<BasePickup>();
    }

    if (pickup != null)
    {
      // Only process if it's the configured pickup tag
      if (other.CompareTag(pickup.PickupTag))
      {
        pickup.HandlePickup(other);
      }
    }
    else
    {
      Debug.LogWarning($"[PickupTrigger] No BasePickup found on {gameObject.name} or its parents");
    }
  }
}