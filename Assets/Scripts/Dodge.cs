using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Dodge : MonoBehaviour
{
    public Transform destination;
    private Vector3 originalPosition;
    public float moveSpeed;
    private float timer;
    public virtual void Start()
    {
        if (Optionz.diff != 0)
        {
            this.moveSpeed = (float) (this.moveSpeed / Optionz.diff);
        }
        this.originalPosition = this.transform.position; // grabs original position
    }

    public virtual void Update()
    {
        this.timer = this.timer + Time.deltaTime;
        this.transform.position = Vector3.Lerp(this.originalPosition, this.destination.position, Mathf.PingPong(this.timer * this.moveSpeed, 1f)); //moves object back and forth
    }

    public virtual void OnTriggerEnter(Collider other) //destroys player if it touches
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //add change color, loud sound, and explode then timer to load
            other.gameObject.SetActive(false);
            
            if (string.IsNullOrEmpty(SceneLoader.currentScene) || SceneLoader.currentScene == "Active Main Menu")
            {
                SceneLoader.ReloadScene();
            }
            else
            {
                SceneLoader.GameOver();
            }
        }
    }

    public Dodge()
    {
        this.moveSpeed = 0.5f;
    }

}