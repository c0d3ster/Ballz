using UnityEngine.UI;
using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Goal : MonoBehaviour
{
  public GameObject[] totalBoxes;
  public int count;

  private CountDisplay countDisplay;

  public virtual void Start()
  {
    this.totalBoxes = GameObject.FindGameObjectsWithTag("Pick Up");
    this.count = 0;

    // Get CountDisplay reference
    if (UIManager.Instance != null)
    {
      countDisplay = UIManager.Instance.GetComponent<CountDisplay>();
    }

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
    Debug.Log($"[Goal] SetCountText called - count: {this.count}, total: {this.totalBoxes.Length}");

    if (countDisplay != null)
    {
      countDisplay.UpdateCount(this.count, this.totalBoxes.Length);
    }

    if (this.count >= this.totalBoxes.Length)
    {
      Debug.Log($"[Goal] Level completion triggered! count: {this.count}, total: {this.totalBoxes.Length}");
      SceneLoader.Instance.Win();
    }
  }
}