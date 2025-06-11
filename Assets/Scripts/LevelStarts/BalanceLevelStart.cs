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
                SceneLoader.ChangeScene("Ball Balancer " + SceneLoader.balanceCounter);
            }
            else
            {
                SceneLoader.Win();
            }
        }
    }

}