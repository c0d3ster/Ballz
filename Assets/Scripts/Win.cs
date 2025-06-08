using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Win : MonoBehaviour
{
    public virtual void Start()
    {
        Time.timeScale = 0;
        SceneLoader.IncrementLevel();
    }

    //creates two buttons for main menu and try again
    public virtual void OnGUI()
    {
        // Calculate button dimensions relative to screen size
        float buttonWidth = Screen.width * 0.25f;
        float buttonHeight = Screen.height * 0.15f; // 15% of screen height
        float verticalPosition = Screen.height * 0.7f; // 70% down the screen
        // Main Menu button - left position
        if (GUI.Button(new Rect(Screen.width * 0.08f, verticalPosition, buttonWidth, buttonHeight), "Main Menu")) // 8% from left
        {
            Time.timeScale = 1;
            if (SceneLoader.currentScene == "Ball Collector 1")
            {
                SceneLoader.ReloadScene();
            }
            else
            {
                SceneLoader.ChangeScene("Active Main Menu");
            }
        }
        // Try For Better Time button - center position
        if (GUI.Button(new Rect(Screen.width * 0.375f, verticalPosition, buttonWidth, buttonHeight), "Try For A Better Time")) // 37.5% from left
        {
            Time.timeScale = 1;
            SceneLoader.ReloadScene();
        }
        // Next Level button - right position
        if (GUI.Button(new Rect(Screen.width * 0.67f, verticalPosition, buttonWidth, buttonHeight), "Next Level")) // 67% from left
        {
            Time.timeScale = 1;
            SceneLoader.NextLevel();
        }
    }

    public virtual void Update()
    {
    }

}