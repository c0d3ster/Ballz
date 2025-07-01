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

      // Initialize SceneLoader first (foundation for scene management)
      if (FindObjectsByType<SceneLoader>(FindObjectsSortMode.None).Length == 0)
      {
        GameObject sceneLoaderObj = new GameObject("SceneLoader");
        sceneLoaderObj.transform.SetParent(null);
        SceneLoader sceneLoader = sceneLoaderObj.AddComponent<SceneLoader>();
        DontDestroyOnLoad(sceneLoaderObj);
      }

      // Initialize EventSystem for UI functionality
      if (FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None).Length == 0)
      {
        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.transform.SetParent(null);
        eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        DontDestroyOnLoad(eventSystemObj);
      }

      // Initialize AccountManager (for authentication and data persistence)
      if (FindObjectsByType<AccountManager>(FindObjectsSortMode.None).Length == 0)
      {
        GameObject am = new GameObject("AccountManager");
        am.transform.SetParent(null);
        AccountManager accountManager = am.AddComponent<AccountManager>();
        DontDestroyOnLoad(am);
      }

      // Initialize PlatformAuthManager (for platform-specific authentication)
      if (FindObjectsByType<PlatformAuthManager>(FindObjectsSortMode.None).Length == 0)
      {
        GameObject pam = new GameObject("PlatformAuthManager");
        pam.transform.SetParent(null);
        PlatformAuthManager platformAuthManager = pam.AddComponent<PlatformAuthManager>();
        DontDestroyOnLoad(pam);
      }

      // Initialize AccountLoader (for loading existing accounts)
      if (FindObjectsByType<AccountLoader>(FindObjectsSortMode.None).Length == 0)
      {
        GameObject al = new GameObject("AccountLoader");
        al.transform.SetParent(null);
        AccountLoader accountLoader = al.AddComponent<AccountLoader>();
        DontDestroyOnLoad(al);
      }

      // Create LevelProgressManager if it doesn't exist
      if (FindObjectsByType<LevelProgressManager>(FindObjectsSortMode.None).Length == 0)
      {
        GameObject lpm = new GameObject("LevelProgressManager");
        lpm.transform.SetParent(null);
        lpm.AddComponent<LevelProgressManager>();
        DontDestroyOnLoad(lpm);
      }

      // Create LivesManager if it doesn't exist
      if (FindObjectsByType<LivesManager>(FindObjectsSortMode.None).Length == 0)
      {
        GameObject lm = new GameObject("LivesManager");
        lm.transform.SetParent(null);
        LivesManager livesManager = lm.AddComponent<LivesManager>();
        DontDestroyOnLoad(lm);
      }

      // Create CountManager if it doesn't exist
      if (FindObjectsByType<CountManager>(FindObjectsSortMode.None).Length == 0)
      {
        GameObject cm = new GameObject("CountManager");
        cm.transform.SetParent(null);
        CountManager countManager = cm.AddComponent<CountManager>();
        DontDestroyOnLoad(cm);
      }

      // Create TimerManager if it doesn't exist
      if (FindObjectsByType<TimerManager>(FindObjectsSortMode.None).Length == 0)
      {
        GameObject tm = new GameObject("TimerManager");
        tm.transform.SetParent(null);
        TimerManager timerManager = tm.AddComponent<TimerManager>();
        DontDestroyOnLoad(tm);
      }

      // Create HotkeyManager if it doesn't exist
      if (FindObjectsByType<HotkeyManager>(FindObjectsSortMode.None).Length == 0)
      {
        GameObject hm = new GameObject("HotkeyManager");
        hm.transform.SetParent(null);
        HotkeyManager hotkeyManager = hm.AddComponent<HotkeyManager>();
        DontDestroyOnLoad(hm);
      }

      // Create AdManager if it doesn't exist
      if (FindObjectsByType<AdManager>(FindObjectsSortMode.None).Length == 0)
      {
        GameObject am = new GameObject("AdManager");
        am.transform.SetParent(null);
        AdManager adManager = am.AddComponent<AdManager>();
        DontDestroyOnLoad(am);
      }

      // Setup UI
      instance.InitializeUI();

      // Debug: Log instance counts
      instance.LogManagerInstanceCounts();

      Debug.Log("[GameManager] All managers initialized");
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

  void LogManagerInstanceCounts()
  {
    // Managers
    int gameManagerCount = FindObjectsByType<GameManager>(FindObjectsSortMode.None).Length;
    int accountManagerCount = FindObjectsByType<AccountManager>(FindObjectsSortMode.None).Length;
    int platformAuthManagerCount = FindObjectsByType<PlatformAuthManager>(FindObjectsSortMode.None).Length;
    int levelProgressManagerCount = FindObjectsByType<LevelProgressManager>(FindObjectsSortMode.None).Length;
    int livesManagerCount = FindObjectsByType<LivesManager>(FindObjectsSortMode.None).Length;
    int countManagerCount = FindObjectsByType<CountManager>(FindObjectsSortMode.None).Length;
    int timerManagerCount = FindObjectsByType<TimerManager>(FindObjectsSortMode.None).Length;
    int hotkeyManagerCount = FindObjectsByType<HotkeyManager>(FindObjectsSortMode.None).Length;
    int adManagerCount = FindObjectsByType<AdManager>(FindObjectsSortMode.None).Length;

    // Loaders
    int sceneLoaderCount = FindObjectsByType<SceneLoader>(FindObjectsSortMode.None).Length;
    int accountLoaderCount = FindObjectsByType<AccountLoader>(FindObjectsSortMode.None).Length;

    // UI Systems
    int eventSystemCount = FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None).Length;

    Debug.Log($"[GameManager] Managers - GameManager: {gameManagerCount}, AccountManager: {accountManagerCount}, PlatformAuthManager: {platformAuthManagerCount}, LevelProgressManager: {levelProgressManagerCount}, LivesManager: {livesManagerCount}, CountManager: {countManagerCount}, TimerManager: {timerManagerCount}, HotkeyManager: {hotkeyManagerCount}, AdManager: {adManagerCount}");
    Debug.Log($"[GameManager] Loaders - SceneLoader: {sceneLoaderCount}, AccountLoader: {accountLoaderCount}");
    Debug.Log($"[GameManager] UI Systems - EventSystem: {eventSystemCount}");
  }
}