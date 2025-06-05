using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class DodgeMenu : MonoBehaviour
{
    public Transform destination;
    private Vector3 originalPosition;
    public float moveSpeed;
    public virtual void Start()
    {
        this.originalPosition = this.transform.position; // grabs original position
    }

    public virtual void Update()
    {
        this.transform.position = Vector3.Lerp(this.originalPosition, this.destination.position, Mathf.PingPong(Time.time * this.moveSpeed, 1f)); //moves object back and forth
    }

    public virtual void OnTriggerEnter(Collider other) //destroys player if it touches
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //add change color, loud sound, and explode then timer to load
            other.gameObject.SetActive(false);
            SceneLoader.ChangeScene("Active Main Menu");
        }
    }

    public DodgeMenu()
    {
        this.moveSpeed = 0.5f;
    }

}