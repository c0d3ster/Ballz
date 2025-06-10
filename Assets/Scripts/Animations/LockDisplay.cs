using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class LockDisplay : MonoBehaviour
{
    [Header("Lock Settings")]
    public Texture2D lockTexture;
    public Color lockColor = Color.white;
    public float rotationSpeed = 30f;
    public float pulseSpeed = 1f;
    public float pulseAmount = 0.2f;

    private Material material;
    private float baseScale;

    void Start()
    {
        // Get the material instance
        material = GetComponent<MeshRenderer>().material;
        
        // Set the lock texture
        if (lockTexture != null)
        {
            material.mainTexture = lockTexture;
        }
        
        // Set the color
        material.color = lockColor;
        
        // Store the base scale
        baseScale = transform.localScale.x;
    }

    void Update()
    {
        // Rotate the lock
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Create a pulsing effect
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        float newScale = baseScale * (1f + pulse);
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }
} 