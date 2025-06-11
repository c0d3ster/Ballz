using UnityEngine;
using System.Collections;
using Enums;

[System.Serializable]
public partial class PushLevelStart : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Pick Up"))
    {
      SceneLoader.ChangeScene("Ball Pusher " + LevelProgressManager.Instance.GetHighestLevelNumber(GameMode.Push));
    }
  }
}