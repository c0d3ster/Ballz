using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Spinner : MonoBehaviour
{
  public float speed;
  public virtual void Update()
  {
    this.transform.Rotate(new Vector3(0, this.speed, 0) * Time.deltaTime);
  }

  public virtual void OnTriggerEnter(Collider other) //loads option screen if touched
  {
    if (other.gameObject.CompareTag("Player") && this.gameObject.CompareTag("Optionz"))
    {
      SceneLoader.Pause();
    }
  }

}