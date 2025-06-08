using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    private UIManager uiManager;
    private SceneLoader sceneLoader;
    private const string UI_PREFAB_PATH = "Prefabs/Managers/UIManager";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("GameManager");
            instance = go.AddComponent<GameManager>();
            DontDestroyOnLoad(go);
            
            // Add required components
            instance.sceneLoader = go.AddComponent<SceneLoader>();
            go.AddComponent<Optionz>();
            
            // Setup UI
            instance.InitializeUI();

            // Subscribe to scene changes
            SceneManager.sceneLoaded += instance.OnSceneLoaded;
            
            // Check initial scene
            instance.UpdateUIVisibility();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateUIVisibility();
    }

    private void UpdateUIVisibility()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        bool shouldHideUI = currentScene == "Splash Screen" || 
                          currentScene == "GAME OVER" || 
                          currentScene == "WIN";
        
        UIManager.ShowTouchController(!shouldHideUI);
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    void InitializeUI()
    {
        if (FindFirstObjectByType<UIManager>() == null)
        {
            // Load from Resources folder
            GameObject prefab = Resources.Load<GameObject>(UI_PREFAB_PATH);
            if (prefab == null)
            {
                Debug.LogError($"UIManager prefab not found at Resources/{UI_PREFAB_PATH}");
                return;
            }
            
            GameObject uiObj = Instantiate(prefab);
            uiManager = uiObj.GetComponent<UIManager>();
            if (uiManager != null)
            {
                uiObj.name = "UIManager";
                DontDestroyOnLoad(uiObj);
            }
            else
            {
                Debug.LogError("UIManager component not found on instantiated prefab!");
            }
        }
    }
} 