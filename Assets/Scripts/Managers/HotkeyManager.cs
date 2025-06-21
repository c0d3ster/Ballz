using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class HotkeyManager : MonoBehaviour
{
  // Singleton instance
  private static HotkeyManager instance;
  public static HotkeyManager Instance
  {
    get { return instance; }
  }

  // Hotkey definitions
  [Header("Hotkey Settings")]
  [SerializeField] private KeyCode resetKey = KeyCode.R;
  [SerializeField] private KeyCode completeLevelKey = KeyCode.C;
  [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
  [SerializeField] private KeyCode jumpKey = KeyCode.Space;

  // Events for other systems to subscribe to
  public static event Action OnResetPressed;
  public static event Action OnCompleteLevelPressed;
  public static event Action OnPausePressed;
  public static event Action OnJumpPressed;
  public static event Action OnJumpReleased;

  // Track key states to prevent multiple triggers
  private bool resetKeyPressed = false;
  private bool completeLevelKeyPressed = false;
  private bool pauseKeyPressed = false;
  private bool jumpKeyPressed = false;

  private void Awake()
  {
    if (instance != null)
    {
      Destroy(instance.gameObject);
    }
    instance = this;
    DontDestroyOnLoad(gameObject);
  }

  private void Update()
  {
    HandleHotkeys();
  }

  private void HandleHotkeys()
  {
    // Handle Reset key (R)
    HandleKeyPress(resetKey, ref resetKeyPressed, OnResetPressed, "Reset");

    // Handle Complete Level key (C) - Editor only
#if UNITY_EDITOR
    HandleKeyPress(completeLevelKey, ref completeLevelKeyPressed, OnCompleteLevelPressed, "Complete Level");
#endif

    // Handle Pause key (Escape)
    HandleKeyPress(pauseKey, ref pauseKeyPressed, OnPausePressed, "Pause");

    // Handle Jump key (Space) - both press and release
    HandleJumpKey();
  }

  private void HandleKeyPress(KeyCode key, ref bool keyPressed, Action eventAction, string actionName)
  {
    if (Input.GetKeyDown(key) && !keyPressed)
    {
      keyPressed = true;
      Debug.Log($"[HotkeyManager] {actionName} key pressed");
      eventAction?.Invoke();
    }
    else if (Input.GetKeyUp(key))
    {
      keyPressed = false;
    }
  }

  private void HandleJumpKey()
  {
    if (Input.GetKeyDown(jumpKey) && !jumpKeyPressed)
    {
      jumpKeyPressed = true;
      Debug.Log("[HotkeyManager] Jump key pressed");
      OnJumpPressed?.Invoke();
    }
    else if (Input.GetKeyUp(jumpKey))
    {
      jumpKeyPressed = false;
      Debug.Log("[HotkeyManager] Jump key released");
      OnJumpReleased?.Invoke();
    }
  }

  // Public methods for other systems to check key states
  public bool IsResetKeyPressed() => Input.GetKey(resetKey);
  public bool IsCompleteLevelKeyPressed() => Input.GetKey(completeLevelKey);
  public bool IsPauseKeyPressed() => Input.GetKey(pauseKey);
  public bool IsJumpKeyPressed() => Input.GetKey(jumpKey);

  // Public methods for other systems to check key down states
  public bool IsResetKeyDown() => Input.GetKeyDown(resetKey);
  public bool IsCompleteLevelKeyDown() => Input.GetKeyDown(completeLevelKey);
  public bool IsPauseKeyDown() => Input.GetKeyDown(pauseKey);
  public bool IsJumpKeyDown() => Input.GetKeyDown(jumpKey);

  // Public methods for other systems to check key up states
  public bool IsResetKeyUp() => Input.GetKeyUp(resetKey);
  public bool IsCompleteLevelKeyUp() => Input.GetKeyUp(completeLevelKey);
  public bool IsPauseKeyUp() => Input.GetKeyUp(pauseKey);
  public bool IsJumpKeyUp() => Input.GetKeyUp(jumpKey);

  // Method to manually trigger reset (for UI buttons, etc.)
  public void TriggerReset()
  {
    Debug.Log("[HotkeyManager] Reset triggered manually");
    OnResetPressed?.Invoke();
  }

  // Method to manually trigger pause (for UI buttons, etc.)
  public void TriggerPause()
  {
    Debug.Log("[HotkeyManager] Pause triggered manually");
    OnPausePressed?.Invoke();
  }

  // Method to manually trigger jump (for UI buttons, etc.)
  public void TriggerJump()
  {
    Debug.Log("[HotkeyManager] Jump triggered manually");
    OnJumpPressed?.Invoke();
  }

  // Method to manually trigger jump release (for UI buttons, etc.)
  public void TriggerJumpRelease()
  {
    Debug.Log("[HotkeyManager] Jump release triggered manually");
    OnJumpReleased?.Invoke();
  }
}