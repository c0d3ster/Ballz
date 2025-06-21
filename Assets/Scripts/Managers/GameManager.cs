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

      // Initialize SceneLoader first (dependency injection)
      GameObject sceneLoaderObj = new GameObject("SceneLoader");
      sceneLoaderObj.transform.SetParent(null);
      SceneLoader sceneLoader = sceneLoaderObj.AddComponent<SceneLoader>();
      DontDestroyOnLoad(sceneLoaderObj);

      // Create LevelProgressManager if it doesn't exist
      if (FindFirstObjectByType<LevelProgressManager>() == null)
      {
        GameObject lpm = new GameObject("LevelProgressManager");
        lpm.transform.SetParent(null);
        lpm.AddComponent<LevelProgressManager>();
        DontDestroyOnLoad(lpm);
      }

      // Create LivesManager if it doesn't exist
      if (FindFirstObjectByType<LivesManager>() == null)
      {
        GameObject lm = new GameObject("LivesManager");
        lm.transform.SetParent(null);
        LivesManager livesManager = lm.AddComponent<LivesManager>();
        DontDestroyOnLoad(lm);
      }

      // Create CountManager if it doesn't exist
      if (FindFirstObjectByType<CountManager>() == null)
      {
        GameObject cm = new GameObject("CountManager");
        cm.transform.SetParent(null);
        CountManager countManager = cm.AddComponent<CountManager>();
        DontDestroyOnLoad(cm);
      }

      // Create TimerManager if it doesn't exist
      if (FindFirstObjectByType<TimerManager>() == null)
      {

        GameObject tm = new GameObject("TimerManager");
        tm.transform.SetParent(null);
        TimerManager timerManager = tm.AddComponent<TimerManager>();
        DontDestroyOnLoad(tm);
      }

      // Create HotkeyManager if it doesn't exist
      if (FindFirstObjectByType<HotkeyManager>() == null)
      {
        GameObject hm = new GameObject("HotkeyManager");
        hm.transform.SetParent(null);
        HotkeyManager hotkeyManager = hm.AddComponent<HotkeyManager>();
        DontDestroyOnLoad(hm);
      }

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