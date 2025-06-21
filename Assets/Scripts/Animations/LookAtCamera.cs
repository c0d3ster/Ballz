using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
  [Header("Look At Camera Settings")]
  [SerializeField] private bool flipToFaceCamera = true;
  [SerializeField] private int updateFrequency = 8; // Update every N frames to prevent flickering

  private void Update()
  {
    if (Camera.main != null && Time.frameCount % updateFrequency == 0)
    {
      transform.LookAt(Camera.main.transform);

      if (flipToFaceCamera)
      {
        transform.Rotate(0, 180, 0); // Flip to face camera properly
      }
    }
  }
}