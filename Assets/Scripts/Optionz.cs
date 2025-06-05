using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Optionz : MonoBehaviour
{
    //stores difficulty multiplier (1.2 = easy, 1 = medium, .8 = hard)
    public static double diff; // Initialize with default value
    // Control options - can have multiple enabled
    public static bool useTarget;
    public static bool useAccelerometer; // Mobile only
    public static bool useJoystick;
    public static bool useKeyboard; // Desktop only
    public static void ChangeDifficulty()
    {
        if (!Optionz.diff)
        {
            Optionz.diff = 1f; // Safety check in case static var is reset
        }
        switch (Optionz.diff)
        {
            case 1:
                Optionz.diff = 0.8f;
                break;
            case 0.8:
                Optionz.diff = 1.2f;
                break;
            case 1.2:
                Optionz.diff = 1;
                break;
            default:
                Optionz.diff = 1f; // Reset to medium if unknown state
                break;
        }
    }

    public static string DisplayDifficulty()
    {
        string diffString = null;
        switch (Optionz.diff)
        {
            case 1:
                diffString = "Medium";
                break;
            case 0.8:
                diffString = "Hard";
                break;
            case 1.2:
                diffString = "Easy";
                break;
            default:
                diffString = "Unknown";
                break;
        }
        return diffString;
    }

    public static void SetDifficulty(string difficulty)
    {
        switch (difficulty.ToLower())
        {
            case "easy":
                Optionz.diff = 1.2f;
            case "medium":
                Optionz.diff = 1f;
            case "hard":
                Optionz.diff = 0.8f;
            default:
                Debug.LogWarning("Invalid difficulty: " + difficulty);
                Optionz.diff = 1f; // Default to medium
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