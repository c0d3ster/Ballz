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
  }

  static Optionz()
  {
    Optionz.diff = 1f;
    Optionz.useTarget = true;
    Optionz.useAccelerometer = true;
    Optionz.useJoystick = true;
    Optionz.useKeyboard = true;
  }

}