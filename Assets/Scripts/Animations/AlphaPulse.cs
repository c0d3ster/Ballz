using UnityEngine;

public class AlphaPulse : MonoBehaviour
{
  [Header("Alpha Pulse Settings")]
  [SerializeField] private float pulseSpeed = 2f;
  [SerializeField] private float minAlpha = 0.3f;
  [SerializeField] private float maxAlpha = 0.8f;

  private Material material;
  private float pulseTime;

  private void Start()
  {
    pulseTime = 0f;
  }

  private void Update()
  {
    // Get the current material being rendered (handles material replacement)
    material = GetComponent<Renderer>().material;
    if (material == null) return;

    // Update pulse time (same cycle approach as Bob)
    pulseTime += Time.deltaTime * pulseSpeed;

    // Calculate alpha using the same (1 + sin) / 2 approach as Bob
    // This gives a range from 0 to 1, then map to our alpha range
    float t = (1f + Mathf.Sin(pulseTime)) * 0.5f;
    // Flip the t value so it starts at maxAlpha and goes to minAlpha
    float alpha = Mathf.Lerp(maxAlpha, minAlpha, t);

    // Apply the alpha change
    Color currentColor = material.color;
    currentColor.a = alpha;
    material.color = currentColor;
  }

  // Public methods to adjust pulse settings at runtime
  public void SetPulseSpeed(float newSpeed)
  {
    pulseSpeed = newSpeed;
  }

  public void SetAlphaRange(float min, float max)
  {
    minAlpha = min;
    maxAlpha = max;
  }
}