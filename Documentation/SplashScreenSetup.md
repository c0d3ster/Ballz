# Splash Screen Setup Guide

This guide explains how to integrate the AccountLoader into your existing Splash Screen scene to replace the simple 1-second delay with account authentication and loading.

## Overview

The AccountLoader will:
- Replace the simple 1-second delay in your existing Splash.cs
- Add a loading bar and status text
- Handle platform authentication
- Show username input for new users
- Load existing accounts automatically

## Current Setup

Your existing Splash Screen scene has:
- A Splash.cs script that waits 1 second and loads "Active Main Menu"
- Basic scene setup with camera and objects

## Integration Steps

### Step 1: Update the Splash Script

The Splash.cs script has been updated to automatically add the AccountLoader component. The script now:
- Checks if AccountLoader exists
- Adds AccountLoader if not present
- Falls back to original behavior if account system is disabled

**No changes needed in the scene** - the script will handle everything automatically.

### Step 2: Add UI Elements (Optional)

If you want to show loading progress and account creation UI, add these elements to your Splash Screen scene:

#### Required UI Elements for AccountLoader:

1. **Loading Bar (Slider)**
   - Add a UI Slider for progress indication
   - Position at bottom of screen
   - Assign to `loadingBar` field in AccountLoader

2. **Loading Text (TextMeshProUGUI)**
   - Add text for percentage display
   - Position near loading bar
   - Assign to `loadingText` field in AccountLoader

3. **Status Text (TextMeshProUGUI)**
   - Add text for status messages
   - Position above loading bar
   - Assign to `statusText` field in AccountLoader

4. **Account Panel (GameObject)**
   - Create a panel for username input
   - Initially disabled
   - Assign to `accountPanel` field in AccountLoader

5. **Username Input (TMP_InputField)**
   - Add input field for username
   - Place inside account panel
   - Assign to `usernameInput` field in AccountLoader

6. **Confirm Button (Button)**
   - Add button to confirm username
   - Place inside account panel
   - Assign to `confirmButton` field in AccountLoader

7. **Skip Button (Button)**
   - Add button to skip account creation
   - Place inside account panel
   - Assign to `skipButton` field in AccountLoader

8. **Error Text (TextMeshProUGUI)**
   - Add text for validation errors
   - Place inside account panel
   - Assign to `errorText` field in AccountLoader

9. **Email Display Text (TextMeshProUGUI)**
   - Add text to show user's email
   - Place inside account panel
   - Assign to `emailText` field in AccountLoader

### Step 3: UI Layout Example

```
Canvas
├── Loading Bar (Slider)
├── Loading Text (TextMeshProUGUI)
├── Status Text (TextMeshProUGUI)
└── Account Panel (GameObject) - Initially Disabled
    ├── Email Display Text (TextMeshProUGUI)
    ├── Username Input (TMP_InputField)
    ├── Confirm Button (Button)
    ├── Skip Button (Button)
    └── Error Text (TextMeshProUGUI)
```

### Step 4: Configure AccountLoader

1. Select the GameObject with the Splash script
2. In the Inspector, find the AccountLoader component
3. Assign all UI references to their respective fields
4. Adjust settings:
   - `minLoadTime`: Minimum loading time (default: 2 seconds)
   - `mainMenuScene`: Scene to load after account setup (default: "Active Main Menu")

### Step 5: Test the Integration

1. **Test New User Flow:**
   - Run the game
   - Should see loading progress
   - Should show username input for new users
   - Should create account and load main menu

2. **Test Existing User Flow:**
   - Run the game after creating an account
   - Should automatically load existing account
   - Should skip to main menu

3. **Test Error Handling:**
   - Disconnect internet
   - Should show error message
   - Should continue to main menu

## Minimal Setup (No UI)

If you don't want to add UI elements, the AccountLoader will still work:

1. The Splash script automatically adds AccountLoader
2. AccountLoader will handle authentication in background
3. New users will skip account creation
4. Existing users will load automatically
5. Scene will transition to main menu after minimum load time

## Troubleshooting

### Common Issues:

1. **AccountLoader not found:**
   - Ensure Splash script is on a GameObject in the scene
   - Check that `useAccountSystem` is enabled in Splash script

2. **UI references missing:**
   - AccountLoader will work without UI elements
   - Add UI elements for better user experience

3. **Scene not loading:**
   - Check that `mainMenuScene` field matches your scene name
   - Ensure "Active Main Menu" scene is in Build Settings

4. **Authentication failing:**
   - Check console for error messages
   - AccountLoader will continue to main menu even if authentication fails

### Debug Information:

The AccountLoader provides debug logs for:
- Platform detection
- Authentication status
- Account creation/loading
- Error messages

Check the Console window for detailed information.

## Next Steps

1. **Add UI Elements:** Enhance user experience with loading bar and account creation UI
2. **Test on Device:** Verify platform authentication works on actual devices
3. **Integrate with Managers:** Update LivesManager and LevelProgressManager to use AccountManager
4. **Add Platform Integration:** Replace simulated emails with real Google Play Games Services/Game Center

## File Structure

```
Assets/
├── Scripts/
│   ├── Loaders/
│   │   └── AccountLoader.cs          # Loading flow and UI
│   ├── Managers/
│   │   └── AccountManager.cs         # Data persistence and cloud save
│   └── Scenes/
│       └── Splash.cs                 # Updated to use AccountLoader
└── _Scenes/
    └── Splash Screen.unity           # Your existing scene
``` 