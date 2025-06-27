using UnityEngine;
using TMPro;

public class CountDisplay : MonoBehaviour
{
  [Header("UI References")]
  [SerializeField] private TextMeshProUGUI countText;

  [Header("Display Settings")]
  [SerializeField] private string countFormat = "Count: {0}/{1}";
  [SerializeField] private int fontSize = 36;
  [SerializeField] private Color textColor = Color.white;

  private int currentCount = 0;
  private int totalCount = 0;
  private bool isInitialized = false;

  private void Start()
  {
    InitializeCountDisplay();
  }

  private void InitializeCountDisplay()
  {
    if (isInitialized) return;

    Canvas canvas = UIManager.Instance?.gameUICanvas;
    if (canvas == null)
    {
      Debug.LogError("[CountDisplay] No canvas found for count display!");
      return;
    }

    // Create count text GameObject
    GameObject countObj = new GameObject("CountText");
    countObj.transform.SetParent(canvas.transform, false);
    countObj.layer = canvas.gameObject.layer;

    // Add TextMeshProUGUI component
    countText = countObj.AddComponent<TextMeshProUGUI>();
    countText.text = countFormat.Replace("{0}", "0").Replace("{1}", "0");
    countText.fontSize = fontSize;
    countText.color = textColor;
    countText.alignment = TextAlignmentOptions.Center;

    // Position the text
    RectTransform rectTransform = countObj.GetComponent<RectTransform>();
    rectTransform.anchoredPosition = new Vector2(0, 35);
    rectTransform.sizeDelta = new Vector2(300, 50);

    isInitialized = true;
  }

  private void CreateCountText()
  {
    Canvas canvas = UIManager.Instance?.gameUICanvas;
    if (canvas == null)
    {
      Debug.LogError("[CountDisplay] No canvas found!");
      return;
    }

    GameObject countTextObj = new GameObject("CountText");
    countTextObj.transform.SetParent(canvas.transform, false);
    countTextObj.layer = canvas.gameObject.layer;

    TextMeshProUGUI tmp = countTextObj.AddComponent<TextMeshProUGUI>();
    tmp.text = "Count: 0/0";
    tmp.fontSize = fontSize;
    tmp.color = textColor;
    tmp.alignment = TextAlignmentOptions.TopLeft;
    tmp.textWrappingMode = TextWrappingModes.NoWrap;

    RectTransform rectTransform = countTextObj.GetComponent<RectTransform>();
    rectTransform.anchorMin = new Vector2(0, 1);
    rectTransform.anchorMax = new Vector2(0, 1);
    rectTransform.pivot = new Vector2(0, 1);
    rectTransform.anchoredPosition = new Vector2(42, -120);
    rectTransform.sizeDelta = new Vector2(200, 75);
    rectTransform.localScale = Vector3.one;

    countText = tmp;

    // Scale the new element if on mobile
    UIManager.Instance?.ScaleNewUIElement(countTextObj.transform);
  }

  public void UpdateCount(int count, int total)
  {
    if (!isInitialized)
    {
      InitializeCountDisplay();
    }

    currentCount = count;
    totalCount = total;

    if (countText != null)
    {
      // Hide the count text if total is 0 (nothing to count) or if on main menu
      bool isMainMenu = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Active Main Menu";
      bool shouldShow = total > 0 && !isMainMenu;
      countText.gameObject.SetActive(shouldShow);

      if (shouldShow)
      {
        countText.text = string.Format(countFormat, count, total);
      }
    }
  }

  public void SetCount(int count)
  {
    UpdateCount(count, totalCount);
  }

  public void SetTotal(int total)
  {
    UpdateCount(currentCount, total);
  }

  public void ClearCount()
  {
    UpdateCount(0, 0);
  }

  public void OnSceneLoaded()
  {
    ClearCount();
    Debug.Log("[CountDisplay] Scene loaded, cleared count display");
  }

  // Configuration methods for CountManager
  public void SetFormat(string format)
  {
    countFormat = format;
  }

  public void SetFontSize(int size)
  {
    fontSize = size;
    if (countText != null)
    {
      countText.fontSize = size;
    }
  }

  public void SetTextColor(Color color)
  {
    textColor = color;
    if (countText != null)
    {
      countText.color = color;
    }
  }
}