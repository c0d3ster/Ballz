using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Rotator : MonoBehaviour
{
  public virtual void Update()
  {
    this.transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
  }

}