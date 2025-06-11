using UnityEngine.UI;
using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Goal : MonoBehaviour
{
  public GameObject[] totalBoxes;
  public int count;
  public Text countText;
  public virtual void Start()
  {
    this.totalBoxes = GameObject.FindGameObjectsWithTag("Pick Up");
    this.count = 0;
    this.SetCountText();
  }

  public virtual void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.CompareTag("Pick Up"))
    {
      other.gameObject.SetActive(false);
      this.count++;
      this.SetCountText();
    }
  }

  public virtual void SetCountText()
  {
    this.countText.text = (("Count: " + this.count.ToString()) + " out of ") + this.totalBoxes.Length;
    if (this.count >= this.totalBoxes.Length)
    {
      SceneLoader.Win();
    }
  }

}