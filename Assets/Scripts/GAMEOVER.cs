using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class GAMEOVER : MonoBehaviour
{
    //creates two buttons for main menu and try again
    public virtual void OnGUI()
    {
        // Calculate button dimensions relative to screen size
        float buttonWidth = Screen.width * 0.35f;
        float buttonHeight = Screen.height * 0.15f; // 10% of screen height
        float verticalPosition = Screen.height * 0.7f; // 70% down the screen
        // Try Again button - positioned on the right
        if (GUI.Button(new Rect(Screen.width * 0.55f, verticalPosition, buttonWidth, buttonHeight), "Try Again")) // 60% from left
        {
            SceneLoader.LoadLastScene();
        }
        // Main Menu button - positioned on the left
        if (GUI.Button(new Rect(Screen.width * 0.1f, verticalPosition, buttonWidth, buttonHeight), "Main Menu")) // 15% from left
        {
            SceneLoader.ChangeScene("Active Main Menu");
        }
    }

    public virtual void Update()
    {
    }

}