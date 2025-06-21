using UnityEngine;

public class Bob : MonoBehaviour
{
  [Header("Bob Settings")]
  [SerializeField] private float bobSpeed = 2f;
  [SerializeField] private float bobHeight = 1f;

  private Vector3 startPosition;
  private float bobTime;

  private void Start()
  {
    // Store the starting position where the object was placed (this is the lowest point)
    startPosition = transform.position;
    bobTime = 0f;
  }

  private void Update()
  {
    // Update bob time
    bobTime += Time.deltaTime * bobSpeed;

    // Calculate bob offset - goes up from start position (lowest point) and back down
    // Use (1 + sin) / 2 to get a range from 0 to 1, then multiply by bobHeight
    float bobOffset = (1f + Mathf.Sin(bobTime)) * 0.5f * bobHeight;

    // Apply the bob movement - start at lowest point, go up by bobHeight, then back down
    transform.position = startPosition + Vector3.up * bobOffset;
  }

  // Public methods to adjust bob settings at runtime
  public void SetBobSpeed(float newSpeed)
  {
    bobSpeed = newSpeed;
  }

  public void SetBobHeight(float newHeight)
  {
    bobHeight = newHeight;
  }
}