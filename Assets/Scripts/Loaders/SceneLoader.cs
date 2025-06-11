using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using Enums;

[System.Serializable]
public partial class SceneLoader : MonoBehaviour
{
    // stores scene names for reference
    public static string lastScene;
    public static string currentScene;
    public static bool isPaused;

    // Scenes that don't have gameplay interaction
    private static readonly string[] nonInteractiveScenes = new string[] 
    {
        "Splash Screen",
        "GAME OVER",
        "WIN"
    };

    public static bool IsCurrentSceneNonInteractive
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

    public static bool IsNonInteractiveScene(string sceneName)
    {
        return System.Array.Exists(nonInteractiveScenes, scene => scene == sceneName);
    }

    //keeps object between scenes to manage all scene movements
    public virtual void Awake()
    {
        Time.timeScale = 1;
        SceneLoader.isPaused = false;
        
        // Set initial scene immediately in Awake
        SceneLoader.currentScene = SceneManager.GetActiveScene().name;
        SceneLoader.lastScene = SceneLoader.currentScene;
        Debug.Log("Initial scene set in Awake to: " + SceneLoader.currentScene);
        
        // Register scene load callback
        SceneManager.sceneLoaded += this.OnSceneLoaded;
    }

    public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[Scene] OnSceneLoaded - Old current: '{currentScene}', New scene: '{scene.name}', Mode: {mode}");
        
        // Only update currentScene if this is not an additive load
        if (mode != LoadSceneMode.Additive)
        {
            SceneLoader.currentScene = scene.name;
            Debug.Log($"[Scene] Updated current scene to: '{currentScene}', Last: '{lastScene}'");
        }
        else
        {
            Debug.Log($"[Scene] Additive load - keeping current scene as: '{currentScene}'");
        }
    }

    public virtual void OnDestroy()
    {
        // Clean up event listener
        SceneManager.sceneLoaded -= this.OnSceneLoaded;
    }

    public static void ChangeScene(string sceneName)
    {
        Debug.Log($"[Scene] ChangeScene called - Current: '{currentScene}', Last: '{lastScene}', Changing to: '{sceneName}'");
        SceneLoader.lastScene = SceneLoader.currentScene;
        SceneLoader.currentScene = sceneName;
        Debug.Log($"[Scene] After update - Current: '{currentScene}', Last: '{lastScene}'");
        SceneManager.LoadScene(SceneLoader.currentScene);
    }

    public static void ReloadScene()
    {
        Debug.Log($"[Scene] ReloadScene called - Current: '{currentScene}', Last: '{lastScene}'");
        // Don't update lastScene when reloading
        SceneManager.LoadScene(SceneLoader.currentScene);
    }

    public static void LoadLastScene()
    {
        Debug.Log($"[Scene] LoadLastScene called - Current: '{currentScene}', Last: '{lastScene}'");
        if (lastScene != null)
        {
            SceneLoader.ChangeScene(lastScene);
        }
        else
        {
            SceneLoader.ChangeScene("Active Main Menu");
        }
    }

    public static void GameOver()
    {
        SceneLoader.ChangeScene("GAME OVER");
    }

    public static void Win()
    {
        // Complete the current level and save progress
        GameMode? gameMode = DetermineGameMode(currentScene);
        Debug.Log($"[SceneLoader] Win - Current scene: {currentScene}, Game mode: {gameMode}, Game mode value: {gameMode.Value}");
        if (gameMode.HasValue)
        {
            LevelProgressManager.Instance.CompleteLevel(gameMode.Value);
        }
        
        // Load the win scene
        SceneManager.LoadScene("WIN", LoadSceneMode.Additive);
    }

    public static void Pause()
    {
        SceneManager.LoadScene("PAUSE", LoadSceneMode.Additive);
    }

    public static void LevelSelect()
    {
        SceneManager.LoadScene("LEVEL SELECT", LoadSceneMode.Additive);
    }

    public static int GetLevelNumberFromCurrentScene()
    {
        // Try to find a number at the end of the string
        if (!string.IsNullOrEmpty(SceneLoader.currentScene))
        {
            int i = SceneLoader.currentScene.Length - 1;
            while (i >= 0)
            {
                if (char.IsDigit(SceneLoader.currentScene[i]))
                {
                    return int.Parse(SceneLoader.currentScene[i].ToString());
                }
                i--;
            }
        }
        return 0; // Default return if no number found
    }

    private static string GetGameModeSuffix(GameMode gameMode)
    {
        var field = gameMode.GetType().GetField(gameMode.ToString());
        var attribute = (SceneSuffixAttribute)field.GetCustomAttributes(typeof(SceneSuffixAttribute), false)[0];
        return attribute.Suffix;
    }

    public static GameMode? DetermineGameMode(string sceneName)
    {
        if (sceneName.StartsWith("Ball "))
        {
            string modePart = sceneName.Substring(5).Split(' ')[0];
            
            // Remove the suffix to match enum values
            if (modePart == "Pusher")
                modePart = "Push";
            else if (modePart.EndsWith("or"))
                modePart = modePart.Substring(0, modePart.Length - 2);
            else if (modePart.EndsWith("r"))
                modePart = modePart.Substring(0, modePart.Length - 1);
            else if (modePart.EndsWith("er"))
                modePart = modePart.Substring(0, modePart.Length - 2);
                
            if (System.Enum.TryParse<GameMode>(modePart, true, out GameMode gameMode))
            {
                return gameMode;
            }
        }
        return null;
    }

    private static bool SceneExists(string sceneName)
    {
        return UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/{sceneName}.unity") != -1;
    }

    public static void NextLevel()
    {
        string currentScene = SceneLoader.currentScene;
        GameMode? gameMode = DetermineGameMode(currentScene);

        // If we found a valid game mode, load the next level
        if (gameMode.HasValue)
        {
            string nextScene = $"Ball {gameMode}{GetGameModeSuffix(gameMode.Value)} {GetLevelNumberFromCurrentScene() + 1}";
            if (SceneExists(nextScene))
            {
                SceneLoader.ChangeScene(nextScene);
            }
            else
            {
                // If next level doesn't exist, go to main menu
                SceneLoader.ChangeScene("Active Main Menu");
            }
        }
        else
        {
            // If we're not on a valid level, go to main menu
            SceneLoader.ChangeScene("Active Main Menu");
        }
    }
}