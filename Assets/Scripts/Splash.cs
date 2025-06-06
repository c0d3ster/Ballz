using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Splash : MonoBehaviour
{
    public virtual void Start()
    {
        this.StartCoroutine("Load");
    }

    public virtual IEnumerator Load()
    {
        yield return new WaitForSeconds(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Active Main Menu");
    }

}