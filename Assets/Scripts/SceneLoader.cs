using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SceneLoader : MonoBehaviour
{
    // stores scene names for reference
    public static string lastScene;
    public static string currentScene;
    public static bool isPaused;
    //counters for each level sets progress
    public static int pushCounter;
    public static int collectCounter;
    public static int balanceCounter;
    public static int dodgeCounter;
    public static int jumpCounter;
    //keeps object between scenes to manage all scene movements
    public virtual void Awake()
    {
        Time.timeScale = 1;
        SceneLoader.isPaused = false;
        UnityEngine.Object.DontDestroyOnLoad(this.transform.gameObject);
        // Add safety check for difficulty
        if (!Options.diff)
        {
            Options.diff = 1f;
        }
        // Set initial scene immediately in Awake
        SceneLoader.currentScene = SceneManager.GetActiveScene().name;
        SceneLoader.lastScene = SceneLoader.currentScene;
        Debug.Log("Initial scene set in Awake to: " + SceneLoader.currentScene);
        // Register scene load callback
        SceneManager.sceneLoaded += this.OnSceneLoaded;
    }

    public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneLoader.currentScene = scene.name;
        Debug.Log("Scene loaded: " + SceneLoader.currentScene);
    }

    public virtual void OnDestroy()
    {
        // Clean up event listener
        SceneManager.sceneLoaded -= this.OnSceneLoaded;
    }

    public static void ChangeScene(string sceneName)
    {
        SceneLoader.lastScene = SceneLoader.currentScene;
        SceneLoader.currentScene = sceneName;
        Debug.Log("Changing scene to: " + SceneLoader.currentScene);
        SceneManager.LoadScene(SceneLoader.currentScene);
    }

    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneLoader.currentScene);
    }

    public static void LoadLastScene()
    {
        string last = SceneLoader.lastScene;
        SceneLoader.lastScene = SceneLoader.currentScene;
        SceneLoader.currentScene = last;
        SceneManager.LoadScene(SceneLoader.currentScene);
    }

    public static void GameOver()
    {
        SceneLoader.ChangeScene("GAME OVER");
    }

    public static void Win()
    {
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

    public static int GetLevelNumber()
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

    public static void NextLevel()
    {
        switch (SceneLoader.currentScene)
        {
            case "Ball Collector 1 (StudySoup)":
                SceneLoader.ReloadScene();
            default:
                if (SceneLoader.currentScene == ("Ball Balancer " + (SceneLoader.balanceCounter - 1)))
                {
                    SceneLoader.ChangeScene("Ball Balancer " + SceneLoader.balanceCounter);
                }
                if (SceneLoader.currentScene == ("Ball Collector " + (SceneLoader.collectCounter - 1)))
                {
                    SceneLoader.ChangeScene("Ball Collector " + SceneLoader.collectCounter);
                }
                if (SceneLoader.currentScene == ("Ball Dodger " + (SceneLoader.dodgeCounter - 1)))
                {
                    SceneLoader.ChangeScene("Ball Dodger " + SceneLoader.dodgeCounter);
                }
                if (SceneLoader.currentScene == ("Ball Jumper " + (SceneLoader.jumpCounter - 1)))
                {
                    SceneLoader.ChangeScene("Ball Jumper " + SceneLoader.jumpCounter);
                }
                if (SceneLoader.currentScene == ("Ball Pusher " + (SceneLoader.pushCounter - 1)))
                {
                    SceneLoader.ChangeScene("Ball Pusher " + SceneLoader.pushCounter);
                }
                SceneLoader.ChangeScene("Active Main Menu");
                break;
        }
    }

    // increments level only if it is the current highest level
    public static void IncrementLevel()
    {
        switch (SceneLoader.currentScene)
        {
            default:
                if (SceneLoader.currentScene == ("Ball Balancer " + SceneLoader.balanceCounter))
                {
                    SceneLoader.balanceCounter++;
                }
                if (SceneLoader.currentScene == ("Ball Collector " + SceneLoader.collectCounter))
                {
                    SceneLoader.collectCounter++;
                }
                if (SceneLoader.currentScene == ("Ball Dodger " + SceneLoader.dodgeCounter))
                {
                    SceneLoader.dodgeCounter++;
                }
                if (SceneLoader.currentScene == ("Ball Jumper " + SceneLoader.jumpCounter))
                {
                    SceneLoader.jumpCounter++;
                }
                if (SceneLoader.currentScene == ("Ball Pusher " + SceneLoader.pushCounter))
                {
                    SceneLoader.pushCounter++;
                }
                break;
        }
    }

    static SceneLoader()
    {
        SceneLoader.pushCounter = 1;
        SceneLoader.collectCounter = 1;
        SceneLoader.balanceCounter = 1;
        SceneLoader.dodgeCounter = 1;
        SceneLoader.jumpCounter = 1;
    }

}