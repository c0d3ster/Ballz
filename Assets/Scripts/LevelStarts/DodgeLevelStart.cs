using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class DodgeLevelStart : MonoBehaviour
{
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // make delay for next level for ball up into portal/fade animation
            other.gameObject.SetActive(false);
            if (SceneLoader.currentScene == "Active Main Menu")
            {
                SceneLoader.ChangeScene("Ball Dodger " + SceneLoader.dodgeCounter);
            }
            else
            {
                //SceneLoader.LevelSelect();
                SceneLoader.Win();
            }
        }
    }

}