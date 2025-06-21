using UnityEngine;
using System.Collections;
using Enums;

[System.Serializable]
public class PORTAL_LEVEL_START : MonoBehaviour
{
  [SerializeField]
  private GameMode gameMode;

  private void OnTriggerEnter(Collider other)
  {
    // Check if the collider is the player or a pickup based on game mode
    bool isValidCollision = gameMode == GameMode.Push ?
      other.CompareTag("Pick Up") :
      other.CompareTag("Player");

    if (isValidCollision)
    {
      other.gameObject.SetActive(false);
      if (SceneLoader.Instance.currentScene == "Active Main Menu")
      {
        int level = LevelProgressManager.Instance.GetHighestLevelNumber(gameMode);
        string baseName = $"Ball {gameMode}{SceneLoader.Instance.GetGameModeSuffix(gameMode)}";
        string scene = $"{baseName} {level}";
        if (!SceneLoader.Instance.SceneExists(scene))
        {
          level--;
          scene = $"{baseName} {level}";
        }
        SceneLoader.Instance.ChangeScene(scene);
      }
      else
      {
        LevelProgressManager.Instance.CompleteCurrentLevel();
      }
    }
  }
}