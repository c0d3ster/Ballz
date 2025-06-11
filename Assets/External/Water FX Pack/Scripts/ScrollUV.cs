using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class ScrollUV : MonoBehaviour
{
  public float scrollSpeed_X;
  public float scrollSpeed_Y;
  public virtual void Update()
  {
    float offsetX = Time.time * this.scrollSpeed_X;
    float offsetY = Time.time * this.scrollSpeed_Y;
    this.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
  }

  public ScrollUV()
  {
    this.scrollSpeed_X = 0.5f;
    this.scrollSpeed_Y = 0.5f;
  }

}