using UnityEngine;
using System.Collections;
using Enums;

[System.Serializable]
public partial class DodgeLevelStart : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    // make delay for next level for ball up into portal/fade animation
    other.gameObject.SetActive(false);
    if (SceneLoader.currentScene == "Active Main Menu")
    {
      int level = LevelProgressManager.Instance.GetHighestLevelNumber(GameMode.Dodge);
      string scene = $"Ball Dodger {level}";
      if (!SceneLoader.SceneExists(scene))
      {
        level--;
      }
      SceneLoader.ChangeScene($"Ball Dodger {level}");
    }
    else
    {
      //SceneLoader.LevelSelect();
      SceneLoader.Win();
    }
  }
}