using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class CountManager : MonoBehaviour
{
  public static CountManager Instance { get; private set; }

  [Header("Count Settings")]
  [SerializeField] private string countFormat = "Count: {0}/{1}";
  [SerializeField] private int fontSize = 36;
  [SerializeField] private Color textColor = Color.white;

  public int CurrentCount { get; private set; }
  public int TotalCount { get; private set; }
  public bool IsLevelComplete => CurrentCount >= TotalCount && TotalCount > 0;

  public event Action<int, int> OnCountChanged;
  public event Action OnLevelComplete;

  private CountDisplay countDisplay;
  private bool isInitialized = false;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void Start()
  {
    if (Instance == this)
    {
      InitializeCountManager();
      SceneManager.sceneLoaded += OnSceneLoaded;
    }
  }

  private void OnDestroy()
  {
    if (Instance == this)
    {
      SceneManager.sceneLoaded -= OnSceneLoaded;
    }
  }

  private void InitializeCountManager()
  {
    if (isInitialized) return;

    // Create CountDisplay
    CreateCountDisplay();

    isInitialized = true;
  }

  private void CreateCountDisplay()
  {
    // Add CountDisplay component to this GameObject
    countDisplay = gameObject.AddComponent<CountDisplay>();

    // Configure the display
    if (countDisplay != null)
    {
      countDisplay.SetFormat(countFormat);
      countDisplay.SetFontSize(fontSize);
      countDisplay.SetTextColor(textColor);

      // Initialize with current values to trigger visibility logic
      countDisplay.UpdateCount(CurrentCount, TotalCount);
    }
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    // Only process non-additive scene loads
    if (mode != LoadSceneMode.Additive)
    {
      ResetCount();
      FindPickupsInScene();

      // Update count display to trigger visibility logic
      if (countDisplay != null)
      {
        countDisplay.UpdateCount(CurrentCount, TotalCount);
      }
    }
  }

  private void FindPickupsInScene()
  {
    // Find all pickups in the current scene
    GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pick Up");
    TotalCount = pickups.Length;
    CurrentCount = 0;

    // Update the display
    UpdateCountDisplay();
  }

  public void CollectPickup(GameObject pickup)
  {
    if (pickup == null) return;

    // Deactivate the pickup
    pickup.SetActive(false);

    // Increment count
    CurrentCount++;

    // Update display
    UpdateCountDisplay();

    // Check for level completion
    if (IsLevelComplete)
    {
      OnLevelComplete?.Invoke();
      SceneLoader.Instance.Win();
    }
  }

  private void UpdateCountDisplay()
  {
    if (countDisplay != null)
    {
      countDisplay.UpdateCount(CurrentCount, TotalCount);
    }

    // Notify other systems
    OnCountChanged?.Invoke(CurrentCount, TotalCount);
  }

  public void ResetCount()
  {
    CurrentCount = 0;
    TotalCount = 0;
    UpdateCountDisplay();
    Debug.Log("[CountManager] Count reset");
  }

  public void SetCount(int count)
  {
    CurrentCount = Mathf.Clamp(count, 0, TotalCount);
    UpdateCountDisplay();
  }

  public void SetTotal(int total)
  {
    TotalCount = Mathf.Max(0, total);
    CurrentCount = Mathf.Min(CurrentCount, TotalCount);
    UpdateCountDisplay();
  }

  public void ClearCount()
  {
    ResetCount();
  }

  [ContextMenu("Force Level Complete")]
  public void ForceLevelComplete()
  {
    if (TotalCount > 0)
    {
      CurrentCount = TotalCount;
      UpdateCountDisplay();
      OnLevelComplete?.Invoke();
      SceneLoader.Instance.Win();
    }
  }
}