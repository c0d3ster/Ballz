using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Optionz : MonoBehaviour
{
  //stores difficulty multiplier (1.2 = easy, 1 = medium, .8 = hard)
  public static double diff = 1.0; // Default to medium difficulty
                                   // Control options - can have multiple enabled
  public static bool useTarget;
  public static bool useAccelerometer; // Mobile only
  public static bool useJoystick;
  public static bool useKeyboard; // Desktop only

  // PlayerPrefs keys for settings persistence
  private const string DIFFICULTY_KEY = "Optionz_Difficulty";
  private const string USE_TARGET_KEY = "Optionz_UseTarget";
  private const string USE_ACCELEROMETER_KEY = "Optionz_UseAccelerometer";
  private const string USE_JOYSTICK_KEY = "Optionz_UseJoystick";
  private const string USE_KEYBOARD_KEY = "Optionz_UseKeyboard";

  public static void ChangeDifficulty()
  {
    if (Optionz.diff == 1.0)
    {
      Optionz.diff = 0.8;
    }
    else if (Optionz.diff == 0.8)
    {
      Optionz.diff = 1.2;
    }
    else if (Optionz.diff == 1.2)
    {
      Optionz.diff = 1.0;
    }
    else
    {
      Optionz.diff = 1.0; // Reset to medium if unknown state
    }

    // Save difficulty setting
    SaveDifficulty();
  }

  public static string DisplayDifficulty()
  {
    if (Optionz.diff == 1.0)
    {
      return "Medium";
    }
    else if (Optionz.diff == 0.8)
    {
      return "Hard";
    }
    else if (Optionz.diff == 1.2)
    {
      return "Easy";
    }
    return "Unknown";
  }

  public static void SetDifficulty(string difficulty)
  {
    switch (difficulty.ToLower())
    {
      case "easy":
        Optionz.diff = 1.2;
        break;
      case "medium":
        Optionz.diff = 1.0;
        break;
      case "hard":
        Optionz.diff = 0.8;
        break;
      default:
        Debug.LogWarning("Invalid difficulty: " + difficulty);
        Optionz.diff = 1.0; // Default to medium
        break;
    }

    // Save difficulty setting
    SaveDifficulty();
  }

  // Save control settings to AccountManager
  public static void SaveControlSettings()
  {
    AccountManager accountManager = AccountManager.Instance;
    if (accountManager != null)
    {
      accountManager.UpdateSettings((float)diff, useTarget, useAccelerometer, useJoystick, useKeyboard);
      Debug.Log("[Optionz] Control settings saved to AccountManager");
    }
    else
    {
      Debug.LogWarning("[Optionz] AccountManager not found, settings not saved");
    }
  }

  // Save difficulty setting to AccountManager
  private static void SaveDifficulty()
  {
    AccountManager accountManager = AccountManager.Instance;
    if (accountManager != null)
    {
      accountManager.UpdateSettings((float)diff, useTarget, useAccelerometer, useJoystick, useKeyboard);
      Debug.Log("[Optionz] Difficulty setting saved to AccountManager: " + DisplayDifficulty());
    }
    else
    {
      Debug.LogWarning("[Optionz] AccountManager not found, difficulty not saved");
    }
  }

  // Load all settings from AccountManager
  public static void LoadSettings()
  {
    AccountManager accountManager = AccountManager.Instance;
    if (accountManager != null)
    {
      // Load settings from AccountManager
      diff = accountManager.GetDifficulty();
      useTarget = accountManager.GetUseTarget();
      useAccelerometer = accountManager.GetUseAccelerometer();
      useJoystick = accountManager.GetUseJoystick();
      useKeyboard = accountManager.GetUseKeyboard();

      Debug.Log("[Optionz] Settings loaded from AccountManager - Difficulty: " + DisplayDifficulty() +
                ", Target: " + useTarget + ", Accelerometer: " + useAccelerometer +
                ", Joystick: " + useJoystick + ", Keyboard: " + useKeyboard);
    }
    else
    {
      // Fallback to default values if AccountManager not available
      Debug.LogWarning("[Optionz] AccountManager not found, using default settings");
      diff = 1.0; // Default to medium
      useTarget = true;
      useAccelerometer = true;
      useJoystick = true;
      useKeyboard = true;
    }
  }

  static Optionz()
  {
    // Load saved settings instead of using hardcoded defaults
    LoadSettings();
  }
}