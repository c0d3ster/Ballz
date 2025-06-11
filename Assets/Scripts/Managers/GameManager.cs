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
    private const string UI_PREFAB_PATH = "Prefabs/Managers/UIManager";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("GameManager");
            go.transform.SetParent(null);
            instance = go.AddComponent<GameManager>();
            DontDestroyOnLoad(go);
            
            // Initialize SceneLoader
            go.AddComponent<SceneLoader>();
            
            // Setup UI
            instance.InitializeUI();
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
            uiObj.transform.SetParent(null);
            
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