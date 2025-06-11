using UnityEngine;
using System.Collections;
using Enums;

[System.Serializable]
public partial class CollectLevelStart : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[CollectLevelStart] OnTriggerEnter called highest level: " + LevelProgressManager.Instance.GetHighestLevelNumber(GameMode.Collect));
        if (other.CompareTag("Player"))
        {
            other.gameObject.SetActive(false);
            SceneLoader.ChangeScene("Ball Collector " + LevelProgressManager.Instance.GetHighestLevelNumber(GameMode.Collect));
        }
    }

}