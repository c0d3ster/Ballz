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

  // Save control settings to PlayerPrefs
  public static void SaveControlSettings()
  {
    PlayerPrefs.SetInt(USE_TARGET_KEY, useTarget ? 1 : 0);
    PlayerPrefs.SetInt(USE_ACCELEROMETER_KEY, useAccelerometer ? 1 : 0);
    PlayerPrefs.SetInt(USE_JOYSTICK_KEY, useJoystick ? 1 : 0);
    PlayerPrefs.SetInt(USE_KEYBOARD_KEY, useKeyboard ? 1 : 0);
    PlayerPrefs.Save();
    Debug.Log("[Optionz] Control settings saved");
  }

  // Save difficulty setting to PlayerPrefs
  private static void SaveDifficulty()
  {
    PlayerPrefs.SetFloat(DIFFICULTY_KEY, (float)diff);
    PlayerPrefs.Save();
    Debug.Log("[Optionz] Difficulty setting saved: " + DisplayDifficulty());
  }

  // Load all settings from PlayerPrefs
  public static void LoadSettings()
  {
    bool needsSave = false; // Track if we need to save defaults

    // Load difficulty
    if (PlayerPrefs.HasKey(DIFFICULTY_KEY))
    {
      diff = PlayerPrefs.GetFloat(DIFFICULTY_KEY);
    }
    else
    {
      diff = 1.0; // Default to medium
      needsSave = true;
    }

    // Load control settings
    if (PlayerPrefs.HasKey(USE_TARGET_KEY))
    {
      useTarget = PlayerPrefs.GetInt(USE_TARGET_KEY) == 1;
    }
    else
    {
      useTarget = true; // Default to true
      needsSave = true;
    }

    if (PlayerPrefs.HasKey(USE_ACCELEROMETER_KEY))
    {
      useAccelerometer = PlayerPrefs.GetInt(USE_ACCELEROMETER_KEY) == 1;
    }
    else
    {
      useAccelerometer = true; // Default to true
      needsSave = true;
    }

    if (PlayerPrefs.HasKey(USE_JOYSTICK_KEY))
    {
      useJoystick = PlayerPrefs.GetInt(USE_JOYSTICK_KEY) == 1;
    }
    else
    {
      useJoystick = true; // Default to true
      needsSave = true;
    }

    if (PlayerPrefs.HasKey(USE_KEYBOARD_KEY))
    {
      useKeyboard = PlayerPrefs.GetInt(USE_KEYBOARD_KEY) == 1;
    }
    else
    {
      useKeyboard = true; // Default to true
      needsSave = true;
    }

    // Save defaults if this is the first time loading settings
    if (needsSave)
    {
      SaveControlSettings();
      SaveDifficulty();
      Debug.Log("[Optionz] First time loading - saved default settings");
    }

    Debug.Log("[Optionz] Settings loaded - Difficulty: " + DisplayDifficulty() +
              ", Target: " + useTarget + ", Accelerometer: " + useAccelerometer +
              ", Joystick: " + useJoystick + ", Keyboard: " + useKeyboard);
  }

  static Optionz()
  {
    // Load saved settings instead of using hardcoded defaults
    LoadSettings();
  }
}