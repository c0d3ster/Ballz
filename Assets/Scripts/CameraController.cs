using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class CameraController : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;
    public Vector3 lower;
    public virtual void Start()
    {
        this.lower.z = -5; //shows less of behind the player object
        this.lower.y = 2.5f; //gets camera closer to player object
        this.transform.position = this.transform.position + this.player.transform.position; //adjusts camera for non-zero starting position on player
        this.offset = (this.transform.position - this.player.transform.position) - this.lower; //creates the initial offset distance from player
    }

    public virtual void LateUpdate()
    {
        this.transform.position = this.player.transform.position + this.offset;
    }

}