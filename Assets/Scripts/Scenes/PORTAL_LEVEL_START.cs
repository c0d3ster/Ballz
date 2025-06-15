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
      if (SceneLoader.currentScene == "Active Main Menu")
      {
        int level = LevelProgressManager.Instance.GetHighestLevelNumber(gameMode);
        Debug.Log($"[Portal] Starting from main menu. Game mode: {gameMode}, Highest level: {level}");
        string baseName = $"Ball {gameMode}{SceneLoader.GetGameModeSuffix(gameMode)}";
        string scene = $"{baseName} {level}";
        Debug.Log($"[Portal] Checking scene: {scene} - Exists: {SceneLoader.SceneExists(scene)}");
        if (!SceneLoader.SceneExists(scene))
        {
          level--;
          scene = $"{baseName} {level}";
          Debug.Log($"[Portal] Scene doesn't exist, trying previous level: {scene} - Exists: {SceneLoader.SceneExists(scene)}");
        }
        SceneLoader.ChangeScene(scene);
      }
      else
      {
        SceneLoader.Win();
      }
    }
  }
}