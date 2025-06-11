using UnityEngine;

public class EnergyPulse : MonoBehaviour
{
  [Header("Pulse Settings")]
  public float pulseSpeed = 2f;
  public float pulseAmount = 0.3f;
  public float minAlpha = 0.3f;
  public float maxAlpha = 0.8f;
  public float emissionIntensity = 1f;

  private Material material;
  private Color baseColor;
  private Color baseEmissionColor;

  void Start()
  {
    // Get the material instance
    material = GetComponent<MeshRenderer>().material;

    // Store the original colors
    baseColor = material.color;
    baseEmissionColor = material.GetColor("_EmissionColor");
  }

  void Update()
  {
    // Create a pulsing effect
    float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

    // Update color alpha with min and max limits
    Color color = baseColor;
    color.a = Mathf.Clamp(minAlpha + pulse, minAlpha, maxAlpha);
    material.color = color;

    // Update emission intensity
    Color emissionColor = baseEmissionColor;
    emissionColor.a = emissionIntensity + pulse;
    material.SetColor("_EmissionColor", emissionColor);
  }
}