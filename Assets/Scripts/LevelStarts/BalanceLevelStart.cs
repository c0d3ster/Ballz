using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class BalanceLevelStart : MonoBehaviour
{
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // make delay for next level for flatten ball animation
            other.gameObject.SetActive(false);
            if (SceneLoader.currentScene == "Active Main Menu")
            {
                SceneLoader.ChangeScene("Ball Balancer " + LevelProgressManager.Instance.GetHighestLevelNumber("Balance"));
            }
            else
            {
                SceneLoader.Win();
            }
        }
    }

}