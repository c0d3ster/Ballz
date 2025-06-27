using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using Enums;

[System.Serializable]
public partial class SceneLoader : MonoBehaviour
{
  // Singleton instance
  private static SceneLoader instance;
  public static SceneLoader Instance { get { return instance; } }

  // stores scene names for reference
  public string lastScene;
  public string currentScene;
  public bool isPaused;

  // Scenes that don't have gameplay interaction
  private readonly string[] nonInteractiveScenes = new string[]
  {
        "Splash Screen",
        "GAME_OVER",
        "WIN",
        "RESET_CONFIRMATION"
  };

  public bool IsCurrentSceneNonInteractive
  {
    get
    {
      for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
      {
        var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
        if (IsNonInteractiveScene(scene.name))
        {
          return true;
        }
      }
      return false;
    }
  }

  public bool IsNonInteractiveScene(string sceneName)
  {
    return System.Array.Exists(nonInteractiveScenes, scene => scene == sceneName);
  }

  //keeps object between scenes to manage all scene movements
  public void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);

      Time.timeScale = 1;
      this.isPaused = false;

      // Set initial scene immediately in Awake
      this.currentScene = SceneManager.GetActiveScene().name;
      this.lastScene = this.currentScene;

      // Register scene load callback
      SceneManager.sceneLoaded += this.OnSceneLoaded;
    }
    else
    {
      Destroy(gameObject);
    }
  }


  public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    // Only update currentScene if this is not an overlay scene
    if (mode != LoadSceneMode.Additive)
    {
      this.currentScene = scene.name;
      Debug.Log($"[Scene] Updated current scene to: '{currentScene}', Last: '{lastScene}'");
    }
    else
    {
      Debug.Log($"[Scene] Overlay scene loaded - keeping current scene as: '{currentScene}'");
    }
  }

  public void OnDestroy()
  {
    if (instance == this)
    {
      // Clean up event listener
      SceneManager.sceneLoaded -= this.OnSceneLoaded;
      instance = null;
    }
  }

  public void ChangeScene(string sceneName)
  {
    Debug.Log($"[Scene] ChangeScene called - Current: '{currentScene}', Last: '{lastScene}', Changing to: '{sceneName}'");
    this.lastScene = this.currentScene;
    this.currentScene = sceneName;

    SceneManager.LoadScene(this.currentScene);
  }

  public void ReloadScene()
  {
    // Don't update lastScene when reloading
    SceneManager.LoadScene(this.currentScene);
  }

  public void LoadLastScene()
  {
    Debug.Log($"[Scene] LoadLastScene called - Current: '{currentScene}', Last: '{lastScene}'");
    if (lastScene != null)
    {
      this.ChangeScene(lastScene);
    }
    else
    {
      this.LoadMainMenu();
    }
  }

  public void LoadMainMenu()
  {
    this.LoadMainMenu();
  }

  public void LoadWinScene()
  {
    // Only responsible for loading the WIN scene
    SceneManager.LoadScene("WIN", LoadSceneMode.Additive);
  }

  public void Pause()
  {
    SceneManager.LoadScene("PAUSE", LoadSceneMode.Additive);
  }

  public void LevelSelect()
  {
    SceneManager.LoadScene("LEVEL SELECT", LoadSceneMode.Additive);
  }

  public int GetLevelNumberFromCurrentScene()
  {
    // Try to find a number at the end of the string
    if (!string.IsNullOrEmpty(this.currentScene))
    {
      int i = this.currentScene.Length - 1;
      while (i >= 0)
      {
        if (char.IsDigit(this.currentScene[i]))
        {
          return int.Parse(this.currentScene[i].ToString());
        }
        i--;
      }
    }
    return 0; // Default return if no number found
  }

  public string GetGameModeSuffix(GameMode gameMode)
  {
    var field = gameMode.GetType().GetField(gameMode.ToString());
    var attribute = (SceneSuffixAttribute)field.GetCustomAttributes(typeof(SceneSuffixAttribute), false)[0];
    return attribute.Suffix;
  }

  public GameMode? DetermineGameMode(string sceneName)
  {
    if (sceneName.StartsWith("Ball "))
    {
      string modePart = sceneName.Substring(5).Split(' ')[0];

      // Try each game mode
      foreach (GameMode mode in System.Enum.GetValues(typeof(GameMode)))
      {
        var field = mode.GetType().GetField(mode.ToString());
        var attribute = (SceneSuffixAttribute)field.GetCustomAttributes(typeof(SceneSuffixAttribute), false)[0];

        // Check if the mode part matches the game mode with its suffix
        if (modePart == mode.ToString() + attribute.Suffix)
        {
          return mode;
        }
      }
    }
    return null;
  }

  private string GetSeriesFolderName(string sceneName)
  {
    GameMode? gameMode = DetermineGameMode(sceneName);
    if (!gameMode.HasValue)
    {
      Debug.LogError($"Could not determine game mode from scene name: {sceneName}");
      return string.Empty;
    }
    return $"{gameMode}{GetGameModeSuffix(gameMode.Value)} Series";
  }

  public bool SceneExists(string sceneName)
  {
    string seriesFolder = GetSeriesFolderName(sceneName);
    if (string.IsNullOrEmpty(seriesFolder))
    {
      return false;
    }
    return UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath($"Assets/_Scenes/{seriesFolder}/{sceneName}.unity") != -1;
  }

  public void NextLevel()
  {
    // Check if player has lives before allowing progression
    if (LivesManager.Instance != null && !LivesManager.Instance.HasLives())
    {
      Debug.Log("[SceneLoader] Player is out of lives - cannot proceed to next level");
      // Load game over scene instead
      GameOver();
      return;
    }

    string currentScene = this.currentScene;
    GameMode? gameMode = DetermineGameMode(currentScene);

    // If we found a valid game mode, load the next level
    if (gameMode.HasValue)
    {
      string nextScene = $"Ball {gameMode}{GetGameModeSuffix(gameMode.Value)} {GetLevelNumberFromCurrentScene() + 1}";
      if (SceneExists(nextScene))
      {
        this.ChangeScene(nextScene);
      }
      else
      {
        // If next level doesn't exist, go to main menu
        this.LoadMainMenu();
      }
    }
    else
    {
      // If we're not on a valid level, go to main menu
      this.LoadMainMenu();
    }
  }

  public void GameOver()
  {
    this.ChangeScene("GAME_OVER");
  }
}