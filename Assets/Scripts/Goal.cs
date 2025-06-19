using UnityEngine.UI;
using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Goal : MonoBehaviour
{
  public virtual void Start()
  {
    // Goal no longer needs to manage count - CountManager handles this
    Debug.Log("[Goal] Goal initialized - count management handled by CountManager");
  }

  public virtual void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.CompareTag("Pick Up"))
    {
      // Use CountManager to handle pickup collection
      if (CountManager.Instance != null)
      {
        CountManager.Instance.CollectPickup(other.gameObject);
      }
    }
  }
}