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
    float buttonHeight = Screen.height * 0.15f; // 15% of screen height
    float verticalPosition = Screen.height * 0.7f; // 70% down the screen

    // Create button style with larger text
    GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
    buttonStyle.fontSize = (int)(buttonWidth * 0.06f);

    // Create game over text style
    GUIStyle gameOverStyle = new GUIStyle(GUI.skin.label);
    gameOverStyle.fontSize = (int)(Screen.height * 0.15f);
    gameOverStyle.alignment = TextAnchor.MiddleCenter;
    gameOverStyle.normal.textColor = new Color(0.2f, 0.2f, 0.2f, 1f); // Charcoal gray
    gameOverStyle.hover = gameOverStyle.normal; // Disable hover effect
    gameOverStyle.active = gameOverStyle.normal; // Disable active effect

    // Draw GAME OVER text
    GUI.Label(new Rect(0, Screen.height * 0.30f, Screen.width, Screen.height * 0.2f), "GAME OVER", gameOverStyle);

    // Try Again button - positioned on the right
    if (GUI.Button(new Rect(Screen.width * 0.55f, verticalPosition, buttonWidth, buttonHeight), "Try Again", buttonStyle)) // 60% from left
    {
      SceneLoader.LoadLastScene();
    }
    // Main Menu button - positioned on the left
    if (GUI.Button(new Rect(Screen.width * 0.1f, verticalPosition, buttonWidth, buttonHeight), "Main Menu", buttonStyle)) // 15% from left
    {
      SceneLoader.ChangeScene("Active Main Menu");
    }
  }

  public virtual void Update()
  {
  }
}