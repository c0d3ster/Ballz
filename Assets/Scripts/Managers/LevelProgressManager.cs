using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelProgressManager : MonoBehaviour
{
    // Level progression counters
    private int pushLevel = 1;
    private int collectLevel = 1;
    private int balanceLevel = 1;
    private int dodgeLevel = 1;
    private int jumpLevel = 1;

    // Arrays for locked state visuals (including portals)
    public GameObject[] balanceLockedItems;
    public GameObject[] dodgeLockedItems;
    public GameObject[] jumpLockedItems;
    public GameObject[] pushLockedItems;

    // References to lock objects
    public GameObject balanceLock;
    public GameObject dodgeLock;
    public GameObject jumpLock;
    public GameObject pushLock;

    // Singleton instance
    private static LevelProgressManager instance;
    public static LevelProgressManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Active Main Menu")
        {
            UpdateGameModeVisibility();
        }
    }

    private void Start()
    {
        LoadProgress();
        UpdateGameModeVisibility();
    }

    private void Update()
    {
        // Press R to reset progress (only in editor)
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetProgress();
        }
        #endif
    }

    public void UpdateGameModeVisibility()
    {
        // Balance is unlocked after completing Collect level 1
        bool balanceUnlocked = collectLevel > 1;
        SetGameModeState(balanceLockedItems, balanceLock, balanceUnlocked);

        // Dodge is unlocked after completing Balance level 1
        bool dodgeUnlocked = balanceLevel > 1;
        SetGameModeState(dodgeLockedItems, dodgeLock, dodgeUnlocked);

        // Jump is unlocked after completing Dodge level 1
        bool jumpUnlocked = dodgeLevel > 1;
        SetGameModeState(jumpLockedItems, jumpLock, jumpUnlocked);

        // Push is unlocked after completing Jump level 1
        bool pushUnlocked = jumpLevel > 1;
        SetGameModeState(pushLockedItems, pushLock, pushUnlocked);
    }

    private void SetGameModeState(GameObject[] lockedItems, GameObject lockObject, bool isUnlocked)
    {
        // Show/hide the lock object
        if (lockObject != null)
        {
            lockObject.SetActive(!isUnlocked);
        }

        // Handle all other locked items
        if (lockedItems != null)
        {
            foreach (GameObject item in lockedItems)
            {
                if (item != null)
                {
                    item.SetActive(isUnlocked);
                }
            }
        }
    }

    // Level progression methods
    public void CompleteLevel(string gameMode)
    {
        switch (gameMode.ToLower())
        {
            case "collect":
                if (SceneLoader.currentScene == "Ball Collector " + collectLevel)
                {
                    collectLevel++;
                    UpdateGameModeVisibility();
                }
                break;
            case "balance":
                if (SceneLoader.currentScene == "Ball Balancer " + balanceLevel)
                {
                    balanceLevel++;
                    UpdateGameModeVisibility();
                }
                break;
            case "dodge":
                if (SceneLoader.currentScene == "Ball Dodger " + dodgeLevel)
                {
                    dodgeLevel++;
                    UpdateGameModeVisibility();
                }
                break;
            case "jump":
                if (SceneLoader.currentScene == "Ball Jumper " + jumpLevel)
                {
                    jumpLevel++;
                    UpdateGameModeVisibility();
                }
                break;
            case "push":
                if (SceneLoader.currentScene == "Ball Pusher " + pushLevel)
                {
                    pushLevel++;
                    UpdateGameModeVisibility();
                }
                break;
        }
        SaveProgress();
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("CollectLevel", collectLevel);
        PlayerPrefs.SetInt("BalanceLevel", balanceLevel);
        PlayerPrefs.SetInt("DodgeLevel", dodgeLevel);
        PlayerPrefs.SetInt("JumpLevel", jumpLevel);
        PlayerPrefs.SetInt("PushLevel", pushLevel);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        collectLevel = PlayerPrefs.GetInt("CollectLevel", 1);
        balanceLevel = PlayerPrefs.GetInt("BalanceLevel", 1);
        dodgeLevel = PlayerPrefs.GetInt("DodgeLevel", 1);
        jumpLevel = PlayerPrefs.GetInt("JumpLevel", 1);
        pushLevel = PlayerPrefs.GetInt("PushLevel", 1);
    }

    public void ResetProgress()
    {
        // Reset all level counters to 1
        pushLevel = 1;
        collectLevel = 1;
        balanceLevel = 1;
        dodgeLevel = 1;
        jumpLevel = 1;

        // Clear saved progress
        PlayerPrefs.DeleteKey("CollectLevel");
        PlayerPrefs.DeleteKey("BalanceLevel");
        PlayerPrefs.DeleteKey("DodgeLevel");
        PlayerPrefs.DeleteKey("JumpLevel");
        PlayerPrefs.DeleteKey("PushLevel");
        PlayerPrefs.Save();

        // Update visibility
        UpdateGameModeVisibility();
    }

    [ContextMenu("Clear All Game Data")]
    public void ClearAllGameData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        // Reload the current scene to apply changes
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public int GetHighestLevelNumber(string gameMode)
    {
        switch (gameMode.ToLower())
        {
            case "collect":
                return collectLevel;
            case "balance":
                return balanceLevel;
            case "dodge":
                return dodgeLevel;
            case "jump":
                return jumpLevel;
            case "push":
                return pushLevel;
            default:
                return 1;
        }
    }
}