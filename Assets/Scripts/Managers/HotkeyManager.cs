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
  [SerializeField] private KeyCode resetPickupsKey = KeyCode.P;

  // Events for other systems to subscribe to
  public static event Action OnResetPressed;
  public static event Action OnResetConfirmed;
  public static event Action OnCompleteLevelPressed;
  public static event Action OnPausePressed;
  public static event Action OnJumpPressed;
  public static event Action OnJumpReleased;
  public static event Action OnResetPickupsPressed;

  // Track key states to prevent multiple triggers (only for jump)
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
    // Handle Reset key (R) - show confirmation dialog
    HandleResetKey();

    // Handle Complete Level key (C) - Editor only
#if UNITY_EDITOR
    HandleKeyPress(completeLevelKey, OnCompleteLevelPressed, "Complete Level");
#endif

    // Handle Pause key (Escape)
    HandleKeyPress(pauseKey, OnPausePressed, "Pause");

    // Handle Jump key (Space) - both press and release
    HandleJumpKey();

    // Handle Reset Pickups key (P)
    HandleKeyPress(resetPickupsKey, OnResetPickupsPressed, "Reset Pickups");
  }

  private void HandleResetKey()
  {
    if (Input.GetKeyDown(resetKey))
    {
      Debug.Log("[HotkeyManager] Reset key pressed - showing confirmation dialog");
      OnResetPressed?.Invoke();

      // Show the reset confirmation dialog
      UnityEngine.SceneManagement.SceneManager.LoadScene("RESET_CONFIRMATION", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
  }

  private void HandleKeyPress(KeyCode key, Action eventAction, string actionName)
  {
    if (Input.GetKeyDown(key))
    {
      Debug.Log($"[HotkeyManager] {actionName} key pressed");
      eventAction?.Invoke();
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

  // Public methods for other systems to check jump key states (for UI buttons, etc.)
  public bool IsJumpKeyPressed() => Input.GetKey(jumpKey);
  public bool IsJumpKeyDown() => Input.GetKeyDown(jumpKey);
  public bool IsJumpKeyUp() => Input.GetKeyUp(jumpKey);

  // Method to manually trigger reset (for UI buttons, etc.)
  public void TriggerReset()
  {
    Debug.Log("[HotkeyManager] Reset triggered manually");
    OnResetPressed?.Invoke();

    // Show the reset confirmation dialog
    UnityEngine.SceneManagement.SceneManager.LoadScene("RESET_CONFIRMATION", UnityEngine.SceneManagement.LoadSceneMode.Additive);
  }

  // Method to trigger the actual reset after confirmation
  public void TriggerResetConfirmed()
  {
    Debug.Log("[HotkeyManager] Reset confirmed - executing reset");
    OnResetConfirmed?.Invoke();
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

  // Method to manually trigger reset pickups (for UI buttons, etc.)
  public void TriggerResetPickups()
  {
    Debug.Log("[HotkeyManager] Reset Pickups triggered manually");
    OnResetPickupsPressed?.Invoke();
  }
}