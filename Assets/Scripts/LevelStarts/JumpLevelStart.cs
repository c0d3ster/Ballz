using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class JumpLevelStart : MonoBehaviour
{
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // make delay for next level for expansion animation
            other.gameObject.SetActive(false);
            if (SceneLoader.currentScene == "Active Main Menu")
            {
                SceneLoader.ChangeScene("Ball Jumper " + LevelProgressManager.Instance.GetHighestLevelNumber("Jump"));
            }
            else
            {
                SceneLoader.Win();
            }
        }
    }

}