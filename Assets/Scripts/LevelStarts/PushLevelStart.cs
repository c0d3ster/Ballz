using UnityEngine;
using System.Collections;
using Enums;

[System.Serializable]
public partial class PushLevelStart : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Pick Up"))
    {
      int level = LevelProgressManager.Instance.GetHighestLevelNumber(GameMode.Push);
      string scene = $"Ball Pusher {level}";
      if (!SceneLoader.SceneExists(scene))
      {
        level--;
      }
      SceneLoader.ChangeScene($"Ball Pusher {level}");
    }
  }
}