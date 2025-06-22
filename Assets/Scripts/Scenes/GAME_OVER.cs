using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class GAME_OVER : MonoBehaviour
{
  private LivesManager livesManager;
  private float timeUntilNextLife;
  private bool hasProcessedDeath = false;

  public virtual void Start()
  {
    livesManager = LivesManager.Instance;
    if (livesManager != null)
    {
      timeUntilNextLife = livesManager.TimeUntilNextLife;
      ProcessPlayerDeath();
    }
    else
    {
      Debug.LogError("[GAME_OVER] LivesManager not found!");
    }
  }

  private void ProcessPlayerDeath()
  {
    if (hasProcessedDeath)
    {
      return;
    }
    hasProcessedDeath = true;

    // Only process death if we're not on the main menu
    if (string.IsNullOrEmpty(SceneLoader.Instance.currentScene) || SceneLoader.Instance.currentScene == "Active Main Menu")
    {
      return;
    }

    // Always lose a life when we reach game over (unless we're on main menu)
    if (livesManager.HasLives())
    {
      livesManager.LoseLife();
      // Don't automatically reload - let player choose what to do
    }

    // Stay on game over screen regardless of lives remaining
  }

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

    // Create lives info text style
    GUIStyle livesStyle = new GUIStyle(GUI.skin.label);
    livesStyle.fontSize = (int)(Screen.height * 0.05f);
    livesStyle.alignment = TextAnchor.MiddleCenter;
    livesStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Lighter gray for better visibility
    livesStyle.hover = livesStyle.normal;
    livesStyle.active = livesStyle.normal;

    // Draw GAME OVER text
    GUI.Label(new Rect(0, Screen.height * 0.25f, Screen.width, Screen.height * 0.2f), "GAME OVER", gameOverStyle);

    // Draw lives info if out of lives
    if (livesManager != null && !livesManager.HasLives())
    {
      string livesText = $"Out of lives! Next life in: {Mathf.FloorToInt(timeUntilNextLife / 60f)}m {Mathf.FloorToInt(timeUntilNextLife % 60f)}s";
      GUI.Label(new Rect(0, Screen.height * 0.55f, Screen.width, Screen.height * 0.1f), livesText, livesStyle);
    }

    // Try Again button - positioned on the right
    if (GUI.Button(new Rect(Screen.width * 0.55f, verticalPosition, buttonWidth, buttonHeight), "Try Again", buttonStyle)) // 60% from left
    {
      SceneLoader.Instance.LoadLastScene();
    }
    // Main Menu button - positioned on the left
    if (GUI.Button(new Rect(Screen.width * 0.1f, verticalPosition, buttonWidth, buttonHeight), "Main Menu", buttonStyle)) // 15% from left
    {
      SceneLoader.Instance.ChangeScene("Active Main Menu");
    }
  }

  public virtual void Update()
  {
    // Update time until next life
    if (livesManager != null)
    {
      timeUntilNextLife = livesManager.TimeUntilNextLife;

      // Don't automatically return to last scene - let player choose
    }
  }
}