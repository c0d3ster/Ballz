using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class PushLevelStart : MonoBehaviour
{
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.SetActive(false);
            SceneLoader.ChangeScene("Ball Pusher " + LevelProgressManager.Instance.GetHighestLevelNumber("Push"));
        }
    }
}