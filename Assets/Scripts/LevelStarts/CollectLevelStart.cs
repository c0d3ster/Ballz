using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class CollectLevelStart : MonoBehaviour
{
    public virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("[CollectLevelStart] OnTriggerEnter called highest level: " + LevelProgressManager.Instance.GetHighestLevelNumber("Collect"));
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.SetActive(false);
            SceneLoader.ChangeScene("Ball Collector " + LevelProgressManager.Instance.GetHighestLevelNumber("Collect"));
        }
    }

}