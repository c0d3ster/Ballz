using UnityEngine
using System
using System.Collections.Generic
using System.Linq

public class SimpleLeaderboardManager : MonoBehaviour
{
  public static SimpleLeaderboardManager Instance { get; private set; }

  [Header("Leaderboard Settings")]
  [SerializeField]
  private int maxLeaderboardEntries = 100
  [SerializeField] private bool enableLocalLeaderboard = true

  // Events
  public static event Action OnLeaderboardUpdated
  public static event Action<string> OnScoreSubmitted

  // Leaderboard data structure
  [Serializable]
  public class LeaderboardEntry
  {
    public string username
    public string userId
    public int score
    public long timestamp
    public string gameMode
  }

  [Serializable]
  public class LeaderboardData
  {
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>()
    public long lastUpdateTime = 0
  }

  private LeaderboardData leaderboardData = new LeaderboardData()
  private const string LEADERBOARD_KEY = "SimpleLeaderboard_Data"

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this
      DontDestroyOnLoad(gameObject)
      Debug.Log("[SimpleLeaderboardManager] Instance created")
    }
    else
    {
      Debug.Log("[SimpleLeaderboardManager] Duplicate instance found, destroying")
      Destroy(gameObject)
    }
  }

  private void Start()
  {
    if (Instance == this)
    {
      LoadLeaderboard()
    }
  }

  // Submit a score to the leaderboard
  public void SubmitScore(int score, string gameMode = "General")
  {
    if (SimpleUserManager.Instance == null)
    {
      Debug.LogWarning("[SimpleLeaderboardManager] SimpleUserManager not available")
      return
    }

    if (!SimpleUserManager.Instance.HasUsername)
    {
      Debug.LogWarning("[SimpleLeaderboardManager] No username set, cannot submit score")
      return
    }

    var entry = new LeaderboardEntry
    {
      username = SimpleUserManager.Instance.GetUsername(),
      userId = SimpleUserManager.Instance.GetUserId(),
      score = score,
      timestamp = DateTime.UtcNow.Ticks,
      gameMode = gameMode
    }

    AddEntryToLeaderboard(entry)
    OnScoreSubmitted?.Invoke(entry.username)
  }

  // Submit score for specific game modes
  public void SubmitCollectScore(int level)
  {
    SubmitScore(level, "Collect")
  }

  public void SubmitBalanceScore(int level)
  {
    SubmitScore(level, "Balance")
  }

  public void SubmitDodgeScore(int level)
  {
    SubmitScore(level, "Dodge")
  }

  public void SubmitJumpScore(int level)
  {
    SubmitScore(level, "Jump")
  }

  public void SubmitPushScore(int level)
  {
    SubmitScore(level, "Push")
  }

  private void AddEntryToLeaderboard(LeaderboardEntry entry)
  {
    // Add new entry
    leaderboardData.entries.Add(entry)

    // Sort by score (highest first)
    leaderboardData.entries = leaderboardData.entries
      .OrderByDescending(e => e.score)
      .ThenBy(e => e.timestamp) // Earlier entries first for same score
      .ToList()

    // Keep only top entries
    if (leaderboardData.entries.Count > maxLeaderboardEntries)
    {
      leaderboardData.entries = leaderboardData.entries.Take(maxLeaderboardEntries).ToList()
    }

    // Update timestamp
    leaderboardData.lastUpdateTime = DateTime.UtcNow.Ticks

    // Save leaderboard
    SaveLeaderboard()

    Debug.Log($"[SimpleLeaderboardManager] Score submitted: {entry.username} - {entry.score} ({entry.gameMode})")
    OnLeaderboardUpdated?.Invoke()
  }

  // Get leaderboard entries
  public List<LeaderboardEntry> GetLeaderboard(string gameMode = "General", int count = 10)
  {
    var entries = leaderboardData.entries
      .Where(e => e.gameMode == gameMode)
      .OrderByDescending(e => e.score)
      .ThenBy(e => e.timestamp)
      .Take(count)
      .ToList()

    return entries
  }

  // Get all leaderboard entries
  public List<LeaderboardEntry> GetAllLeaderboard(int count = 50)
  {
    return leaderboardData.entries.Take(count).ToList()
  }

  // Get user's best score
  public int GetUserBestScore(string gameMode = "General")
  {
    if (SimpleUserManager.Instance == null) return 0

    var userEntries = leaderboardData.entries
      .Where(e => e.userId == SimpleUserManager.Instance.GetUserId() && e.gameMode == gameMode)
      .OrderByDescending(e => e.score)
      .ToList()

    return userEntries.Count > 0 ? userEntries[0].score : 0
  }

  // Get user's rank
  public int GetUserRank(string gameMode = "General")
  {
    if (SimpleUserManager.Instance == null) return -1

    var sortedEntries = leaderboardData.entries
      .Where(e => e.gameMode == gameMode)
      .OrderByDescending(e => e.score)
      .ThenBy(e => e.timestamp)
      .ToList()

    for (int i = 0; i < sortedEntries.Count; i++)
    {
      if (sortedEntries[i].userId == SimpleUserManager.Instance.GetUserId())
      {
        return i + 1 // Rank is 1-based
      }
    }

    return -1 // Not found
  }

  // Get top score
  public int GetTopScore(string gameMode = "General")
  {
    var entries = GetLeaderboard(gameMode, 1)
    return entries.Count > 0 ? entries[0].score : 0
  }

  // Get leaderboard statistics
  public LeaderboardStats GetLeaderboardStats(string gameMode = "General")
  {
    var entries = leaderboardData.entries.Where(e => e.gameMode == gameMode).ToList()


    if (entries.Count == 0)
    {
      return new LeaderboardStats
      {
        totalEntries = 0,
        averageScore = 0,
        topScore = 0,
        recentActivity = false
      }
    }

    return new LeaderboardStats
    {
      totalEntries = entries.Count,
      averageScore = (int)entries.Average(e => e.score),
      topScore = entries.Max(e => e.score),
      recentActivity = (DateTime.UtcNow.Ticks - entries.Max(e => e.timestamp)) < TimeSpan.TicksPerDay * 7 // Activity in last 7 days
    }
  }

  [Serializable]
  public class LeaderboardStats
  {
    public int totalEntries
    public int averageScore
    public int topScore
    public bool recentActivity
  }

  // Save/Load methods
  private void SaveLeaderboard()
  {
    if (!enableLocalLeaderboard) return

    try
    {
      string jsonData = JsonUtility.ToJson(leaderboardData)
      PlayerPrefs.SetString(LEADERBOARD_KEY, jsonData)
      PlayerPrefs.Save()
      Debug.Log("[SimpleLeaderboardManager] Leaderboard saved")
    }
    catch (Exception e)
    {
      Debug.LogError($"[SimpleLeaderboardManager] Save failed: {e.Message}")
    }
  }

  private void LoadLeaderboard()
  {
    if (!enableLocalLeaderboard) return

    try
    {
      if (PlayerPrefs.HasKey(LEADERBOARD_KEY))
      {
        string jsonData = PlayerPrefs.GetString(LEADERBOARD_KEY)
        leaderboardData = JsonUtility.FromJson<LeaderboardData>(jsonData)
        Debug.Log($"[SimpleLeaderboardManager] Leaderboard loaded with {leaderboardData.entries.Count} entries")
      }
      else
      {
        Debug.Log("[SimpleLeaderboardManager] No leaderboard data found, starting fresh")
      }
    }
    catch (Exception e)
    {
      Debug.LogError($"[SimpleLeaderboardManager] Load failed: {e.Message}")
      leaderboardData = new LeaderboardData()
    }
  }

  // Clear leaderboard
  public void ClearLeaderboard()
  {
    leaderboardData.entries.Clear()
    leaderboardData.lastUpdateTime = 0
    SaveLeaderboard()
    Debug.Log("[SimpleLeaderboardManager] Leaderboard cleared")
    OnLeaderboardUpdated?.Invoke()
  }

  // Get leaderboard info
  public int GetTotalEntries()
  {
    return leaderboardData.entries.Count
  }

  public long GetLastUpdateTime()
  {
    return leaderboardData.lastUpdateTime
  }

  public bool HasRecentActivity()
  {
    if (leaderboardData.entries.Count == 0) return false
    long lastEntryTime = leaderboardData.entries.Max(e => e.timestamp)
    return (DateTime.UtcNow.Ticks - lastEntryTime) < TimeSpan.TicksPerDay * 7 // Activity in last 7 days
  }

#if UNITY_EDITOR
  [ContextMenu("Test Submit Score")]
  private void TestSubmitScore()
  {
    SubmitScore(UnityEngine.Random.Range(1, 100), "Test")
  }

  [ContextMenu("Test Submit All Game Mode Scores")]
  private void TestSubmitAllScores()
  {
    SubmitCollectScore(UnityEngine.Random.Range(1, 50))
    SubmitBalanceScore(UnityEngine.Random.Range(1, 50))
    SubmitDodgeScore(UnityEngine.Random.Range(1, 50))
    SubmitJumpScore(UnityEngine.Random.Range(1, 50))
    SubmitPushScore(UnityEngine.Random.Range(1, 50))
  }

  [ContextMenu("Clear Leaderboard")]
  private void TestClearLeaderboard()
  {
    ClearLeaderboard()
  }
#endif
}