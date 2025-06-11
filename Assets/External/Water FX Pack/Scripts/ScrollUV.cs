using UnityEngine;

public class ScrollUV : MonoBehaviour
{
  [SerializeField] private float scrollSpeed_X = 0.5f;
  [SerializeField] private float scrollSpeed_Y = 0.5f;

  private Material material;
  private Vector2 offset;

  private void Start()
  {
    // Get the material instance to avoid modifying the shared material
    material = GetComponent<Renderer>().material;
  }

  private void Update()
  {
    offset.x = Time.time * scrollSpeed_X;
    offset.y = Time.time * scrollSpeed_Y;
    material.mainTextureOffset = offset;
  }
}