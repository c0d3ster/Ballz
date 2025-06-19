using UnityEngine;
using TMPro;

public class CountDisplay : MonoBehaviour
{
  [Header("UI References")]
  [SerializeField] private TextMeshProUGUI countText;

  [Header("Display Settings")]
  [SerializeField] private string countFormat = "Count: {0}/{1}";
  [SerializeField] private int fontSize = 24;
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

    if (countText == null)
    {
      CreateCountText();
    }

    if (countText != null)
    {
      countText.fontSize = fontSize;
      countText.color = textColor;
      countText.alignment = TextAlignmentOptions.TopLeft;
      countText.enableWordWrapping = false;
    }

    isInitialized = true;
    Debug.Log("[CountDisplay] Initialized count display");
  }

  private void CreateCountText()
  {
    Canvas canvas = UIManager.Instance?.touchControllerCanvas;
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
    tmp.enableWordWrapping = false;

    RectTransform rectTransform = countTextObj.GetComponent<RectTransform>();
    rectTransform.anchorMin = new Vector2(0, 1);
    rectTransform.anchorMax = new Vector2(0, 1);
    rectTransform.pivot = new Vector2(0, 1);
    rectTransform.anchoredPosition = new Vector2(50, -60);
    rectTransform.sizeDelta = new Vector2(200, 50);
    rectTransform.localScale = Vector3.one;

    countText = tmp;
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
      countText.text = string.Format(countFormat, count, total);
    }

    Debug.Log($"[CountDisplay] Updated count: {count}/{total}");
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

  // Method to copy settings from an existing text component (for quality matching)
  public void CopyTextSettings(TextMeshProUGUI sourceText)
  {
    if (countText != null && sourceText != null)
    {
      countText.fontSize = sourceText.fontSize;
      countText.color = sourceText.color;
      countText.alignment = sourceText.alignment;
      countText.enableWordWrapping = sourceText.enableWordWrapping;

      Debug.Log("[CountDisplay] Copied text settings from source text");
    }
  }
}