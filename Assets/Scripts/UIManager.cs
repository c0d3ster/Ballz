using UnityEngine;
using System.Collections;

[System.Serializable]
public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    // Reference to your touch controller canvas
    public Canvas touchControllerCanvas;
    public virtual void Awake()
    {
        // Singleton pattern
        if (UIManager.instance == null)
        {
            UIManager.instance = this;
            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
            // Make sure canvas persists too
            if (this.touchControllerCanvas)
            {
                UnityEngine.Object.DontDestroyOnLoad(this.touchControllerCanvas.gameObject);
            }
        }
        else
        {
            // If an instance already exists, destroy this one
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

    // Function to toggle touch controller visibility
    public static void ShowTouchController(bool show)
    {
        if (UIManager.instance && UIManager.instance.touchControllerCanvas)
        {
            UIManager.instance.touchControllerCanvas.enabled = show;
        }
    }

}