using UnityEngine;
using System.Collections;
using Enums;

[System.Serializable]
public partial class BalanceLevelStart : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.CompareTag("Player"))
    {
      // make delay for next level for flatten ball animation
      other.gameObject.SetActive(false);
      if (SceneLoader.currentScene == "Active Main Menu")
      {
        int level = LevelProgressManager.Instance.GetHighestLevelNumber(GameMode.Balance);
        string scene = $"Ball Balancer {level}";
        if (!SceneLoader.SceneExists(scene))
        {
          level--;
        }
        SceneLoader.ChangeScene($"Ball Balancer {level}");
      }
      else
      {
        SceneLoader.Win();
      }
    }
  }
}