# Centralized Timer System

This document explains the new centralized timer system that replaces individual TimerText GameObjects in each scene.

## Overview

The new timer system consists of:
- **TimerManager**: Centralized manager that handles all timer functionality
- **SceneTimerConfig**: Configuration class for storing time limits per scene
- **TimeBonus**: Script for adding time bonuses to the timer

## Benefits

1. **No more individual TimerText GameObjects**: All timer functionality is now centralized
2. **Easy configuration**: Time limits are stored in code and easily configurable
3. **Automatic scene detection**: Timer automatically initializes based on the current scene
4. **Pause/resume support**: Timer automatically pauses when game is paused
5. **Difficulty integration**: Timer respects the existing difficulty system
6. **Extensible**: Easy to add new scenes and time bonuses

## How It Works

### Automatic Initialization
The `TimerManager` is automatically created by the `GameManager` during bootstrap and persists across scenes. When a new scene loads, it automatically checks if that scene has a configured time limit and initializes the timer accordingly.

### Scene Configuration
Time limits for scenes are configured in `SceneTimerConfig.cs`. To add a new scene:

```csharp
// In SceneTimerConfig.cs
public static readonly Dictionary<string, float> SceneTimeLimits = new Dictionary<string, float>
{
    // Add your new scene here
    { 'Your New Scene Name', 30f },
    // ... existing scenes
}
```

### Timer Display
The timer display is automatically created and positioned in the UI canvas. It shows "Time Remaining: XX.XX" and automatically hides when no timer is active for the current scene.

## Usage

### Adding Time Limits to New Scenes

1. Open `Assets/Scripts/Config/SceneTimerConfig.cs`
2. Add your scene name and time limit to the `SceneTimeLimits` dictionary
3. The timer will automatically work for that scene

### Programmatic Control

You can control the timer from other scripts:

```csharp
// Pause the timer
TimerManager.Instance.PauseTimer()

// Resume the timer
TimerManager.Instance.ResumeTimer()

// Add time
TimerManager.Instance.AddTime(5f)

// Reset timer to original time
TimerManager.Instance.ResetTimer()

// Check if timer is active
if (TimerManager.Instance.IsTimerActive)
{
    // Do something
}

// Get current time
float currentTime = TimerManager.Instance.CurrentTime
```

### Events

The TimerManager provides events you can subscribe to:

```csharp
// Subscribe to time changes
TimerManager.Instance.OnTimeChanged += (time) => {
    Debug.Log($"Time remaining: {time}")
}

// Subscribe to time up event
TimerManager.Instance.OnTimeUp += () => {
    Debug.Log("Time is up!")
}
```

## Migration from Old System

### Removing Old TimerText GameObjects

1. In each scene, find the "Timer Text" GameObject
2. Delete the GameObject (it contains the old Timer script)
3. The new system will automatically handle timer display

### Updating Time Limits

1. Check the current time limits in your scenes
2. Update the values in `SceneTimerConfig.cs` to match
3. Test to ensure the new system works correctly

## Configuration Options

### TimerManager Settings

In the `TimerManager` component, you can configure:
- **Timer Format**: How the time is displayed (default: "Time Remaining: {0:F2}")
- **Font Size**: Size of the timer text
- **Text Color**: Color of the timer text
- **Timer Position**: Position of the timer on screen

### TimeBonus Settings

In the `TimeBonus` component, you can configure:
- **Time To Add**: Amount of time to add when collected
- **Destroy On Collect**: Whether to hide the bonus object after collection
- **Player Tag**: Tag of the object that can collect the bonus

## Troubleshooting

### Timer Not Showing
- Check that the scene name is exactly matched in `SceneTimerConfig`
- Ensure `UIManager` is properly initialized
- Check console for error messages

### Timer Not Counting Down
- Verify the scene has a time limit configured
- Check that `IsTimerActive` is true
- Ensure the game is not paused

### Time Bonus Not Working
- Verify the GameObject has a Collider set to trigger
- Check that the player has the correct tag
- Ensure `TimerManager.Instance` is not null

## Adding New Features

### Custom Timer Formats
You can modify the timer format in the `TimerManager` to show different information:

```csharp
// Show minutes and seconds
timerFormat = "Time: {0:mm\\:ss}"

// Show only seconds
timerFormat = "Time: {0:F0}s"
```

### Multiple Timer Types
You can extend the system to support different timer types by adding new properties to `TimerManager` and modifying the configuration system.

### Persistent Timer
To make the timer persist across scenes, you can modify the `OnSceneLoaded` method to not reset the timer for certain scene transitions. 