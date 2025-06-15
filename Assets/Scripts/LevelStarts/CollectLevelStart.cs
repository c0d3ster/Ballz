using UnityEngine;
using System.Collections;
using Enums;

[System.Serializable]
public partial class CollectLevelStart : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    Debug.Log("[CollectLevelStart] OnTriggerEnter called highest level: " + LevelProgressManager.Instance.GetHighestLevelNumber(GameMode.Collect));
    if (other.CompareTag("Player"))
    {
      other.gameObject.SetActive(false);
      int level = LevelProgressManager.Instance.GetHighestLevelNumber(GameMode.Collect);
      string scene = $"Ball Collector {level}";
      if (!SceneLoader.SceneExists(scene))
      {
        level--;
      }
      SceneLoader.ChangeScene($"Ball Collector {level}");
    }
  }

}