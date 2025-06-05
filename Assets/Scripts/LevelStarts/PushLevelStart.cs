using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class PushLevelStart : MonoBehaviour
{
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            SceneLoader.ChangeScene("Ball Pusher " + SceneLoader.pushCounter);
        }
    }

}