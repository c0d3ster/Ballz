using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Splash : MonoBehaviour
{
  [Header("Account Integration")]
  public bool useAccountSystem = true;

  public virtual void Start()
  {
    if (useAccountSystem)
    {
      // Check if AccountLoader already exists
      if (FindObjectOfType<AccountLoader>() == null)
      {
        // Add AccountLoader to this GameObject
        gameObject.AddComponent<AccountLoader>();
      }
    }
    else
    {
      // Fallback to original behavior
      this.StartCoroutine("Load");
    }
  }

  public virtual IEnumerator Load()
  {
    yield return new WaitForSeconds(1);
    UnityEngine.SceneManagement.SceneManager.LoadScene("Active Main Menu");
  }
}