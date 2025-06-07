using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public GameObject uiManagerPrefab;  // Assign the UIManager prefab in inspector
    public GameObject sceneLoaderPrefab; // Assign the SceneLoader prefab in inspector

    private UIManager uiManager;
    private SceneLoader sceneLoader;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeManagers()
    {
        // Check if UIManager exists, if not create it
        if (FindObjectOfType<UIManager>() == null && uiManagerPrefab != null)
        {
            GameObject uiObj = Instantiate(uiManagerPrefab);
            uiManager = uiObj.GetComponent<UIManager>();
        }

        // Check if SceneLoader exists, if not create it
        if (FindObjectOfType<SceneLoader>() == null && sceneLoaderPrefab != null)
        {
            GameObject sceneObj = Instantiate(sceneLoaderPrefab);
            sceneLoader = sceneObj.GetComponent<SceneLoader>();
        }
    }

    // Optional: Method to check if we're in a gameplay scene
    public bool IsGameplayScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        return !currentScene.Contains("MENU") && 
               !currentScene.Contains("WIN") && 
               !currentScene.Contains("GAME OVER");
    }
} 