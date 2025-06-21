# HotkeyManager

The HotkeyManager centralizes all hotkey functionality throughout the Ballz application, providing a consistent interface for both keyboard input and UI interactions.

## Features

- **Centralized Hotkey Management**: All hotkeys are handled in one place
- **Event-Driven Architecture**: Systems subscribe to hotkey events instead of polling
- **UI Integration**: UI buttons can trigger the same functionality as keyboard hotkeys
- **Editor-Only Hotkeys**: Some hotkeys (like C for complete level) only work in the Unity Editor
- **Key State Tracking**: Prevents multiple triggers from the same key press

## Supported Hotkeys

| Key | Function | Editor Only |
|-----|----------|-------------|
| R | Reset (progress, lives, pickups) | No |
| C | Complete current level | Yes |
| Escape | Pause/Unpause | No |
| Space | Jump | No |

## Usage

### For Systems (Event Subscription)

Systems that need to respond to hotkeys should subscribe to the appropriate events:

```csharp
void Start()
{
    // Subscribe to hotkey events
    HotkeyManager.OnResetPressed += ResetProgress;
    HotkeyManager.OnPausePressed += TogglePause;
    HotkeyManager.OnJumpPressed += OnJumpPressed;
    HotkeyManager.OnJumpReleased += OnJumpReleased;
}

void OnDestroy()
{
    // Always unsubscribe to prevent memory leaks
    HotkeyManager.OnResetPressed -= ResetProgress;
    HotkeyManager.OnPausePressed -= TogglePause;
    HotkeyManager.OnJumpPressed -= OnJumpPressed;
    HotkeyManager.OnJumpReleased -= OnJumpReleased;
}
```

### For UI Buttons (Trigger Methods)

UI buttons should use the trigger methods to ensure consistent behavior:

```csharp
void OnPauseClick()
{
    // Use HotkeyManager to trigger pause - this ensures consistent behavior
    // whether pause is triggered by keyboard (Escape) or UI button
    if (HotkeyManager.Instance != null)
    {
        HotkeyManager.Instance.TriggerPause();
    }
}
```

### Available Trigger Methods

- `TriggerReset()` - Triggers reset functionality
- `TriggerPause()` - Triggers pause/unpause
- `TriggerJump()` - Triggers jump
- `TriggerJumpRelease()` - Triggers jump release

### Key State Checking

You can also check key states directly:

```csharp
// Check if keys are currently pressed
bool isResetPressed = HotkeyManager.Instance.IsResetKeyPressed();
bool isJumpPressed = HotkeyManager.Instance.IsJumpKeyPressed();

// Check for key down/up events
bool isResetDown = HotkeyManager.Instance.IsResetKeyDown();
bool isJumpUp = HotkeyManager.Instance.IsJumpKeyUp();
```

## Benefits

1. **Consistency**: Keyboard and UI interactions behave identically
2. **Maintainability**: All hotkey logic is in one place
3. **Extensibility**: Easy to add new hotkeys or modify existing ones
4. **Debugging**: Centralized logging for all hotkey events
5. **Performance**: Event-driven approach is more efficient than polling

## Integration Examples

### Before (Scattered Input Handling)
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.R))
    {
        ResetProgress();
    }
    
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        TogglePause();
    }
}
```

### After (Event-Driven)
```csharp
void Start()
{
    HotkeyManager.OnResetPressed += ResetProgress;
    HotkeyManager.OnPausePressed += TogglePause;
}
```

This approach eliminates the need for Update() methods just for input handling and makes the code more modular and testable. 