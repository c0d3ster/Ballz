using UnityEngine;
using System.Collections;
using Enums;

[System.Serializable]
public partial class JumpLevelStart : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // make delay for next level for expansion animation
            other.gameObject.SetActive(false);
            if (SceneLoader.currentScene == "Active Main Menu")
            {
                SceneLoader.ChangeScene("Ball Jumper " +  LevelProgressManager.Instance.GetHighestLevelNumber(GameMode.Jump));
            }
            else
            {
                SceneLoader.Win();
            }
        }
    }

}