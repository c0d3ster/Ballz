# Splash Screen Setup Guide

This guide explains how to integrate the AccountLoader and updated Splash.cs into your existing Splash Screen scene to replace the simple 1-second delay with account authentication and loading.

## Overview

The updated system provides:
- **AccountLoader**: Handles loading existing accounts and shows loading progress
- **Splash.cs**: Handles new user creation flow (New Game button, username input)
- **AccountManager**: Handles data persistence and platform authentication

## Architecture

```
Splash.cs (Scene-specific logic + New User UI)
    ↓
AccountLoader (Loading flow + Progress UI)
    ↓
AccountManager (Data persistence + Cloud Save)
```

## Flow Description

1. **App Launch**: Splash.cs starts → AccountLoader checks for existing account
2. **Existing Account**: AccountLoader loads account → transitions to main menu
3. **New User**: Splash.cs shows "New Game" button → username input → account creation → main menu

## Setup Instructions

### Step 1: Update the Splash Script

The Splash.cs script has been updated to:
- Automatically add AccountLoader component
- Handle new user creation flow
- Manage username input and validation
- Call AccountManager for account creation

**No changes needed in the scene** - the script will handle everything automatically.

### Step 2: Add UI Elements (Optional)

If you want to show the new user creation flow, add these elements to your Splash Screen scene:

#### Required UI Elements for Splash.cs:

1. **New Game Panel (GameObject)**
   - Panel containing the "New Game" button
   - Initially disabled
   - Assign to `newGamePanel` field in Splash script

2. **New Game Button (Button)**
   - Button to start new game flow
   - Place inside new game panel
   - Assign to `newGameButton` field in Splash script

3. **Username Panel (GameObject)**
   - Panel containing username input
   - Initially disabled
   - Assign to `usernamePanel` field in Splash script

4. **Username Input (TMP_InputField)**
   - Input field for username
   - Place inside username panel
   - Assign to `usernameInput` field in Splash script

5. **Start Game Button (Button)**
   - Button to confirm username and start game
   - Place inside username panel
   - Assign to `startGameButton` field in Splash script

6. **Error Text (TextMeshProUGUI)**
   - Text for validation errors
   - Place inside username panel
   - Assign to `errorText` field in Splash script

7. **Email Text (TextMeshProUGUI)**
   - Text to show user's email
   - Place inside username panel
   - Assign to `emailText` field in Splash script

#### Required UI Elements for AccountLoader:

1. **Loading Bar (Slider)**
   - Progress bar for loading indication
   - Position at bottom of screen
   - Assign to `loadingBar` field in AccountLoader

2. **Loading Text (TextMeshProUGUI)**
   - Text for percentage display
   - Position near loading bar
   - Assign to `loadingText` field in AccountLoader

3. **Status Text (TextMeshProUGUI)**
   - Text for status messages
   - Position above loading bar
   - Assign to `statusText` field in AccountLoader

### Step 3: UI Layout Example

```
Canvas
├── Loading Bar (Slider) - AccountLoader
├── Loading Text (TextMeshProUGUI) - AccountLoader
├── Status Text (TextMeshProUGUI) - AccountLoader
├── New Game Panel (GameObject) - Splash.cs
│   └── New Game Button (Button)
└── Username Panel (GameObject) - Splash.cs
    ├── Email Text (TextMeshProUGUI)
    ├── Username Input (TMP_InputField)
    ├── Start Game Button (Button)
    └── Error Text (TextMeshProUGUI)
```

### Step 4: Configure Components

1. **Splash Script Configuration:**
   - Select the GameObject with the Splash script
   - Assign UI references to their respective fields
   - Set `mainMenuScene` to "Active Main Menu"

2. **AccountLoader Configuration:**
   - The AccountLoader component will be added automatically
   - Assign loading UI references
   - Adjust `minLoadTime` if needed (default: 2 seconds)

## Flow Details

### Existing User Flow
1. Splash.cs starts
2. AccountLoader checks for existing account
3. AccountLoader shows loading progress
4. AccountLoader loads existing account
5. AccountLoader transitions to main menu

### New User Flow
1. Splash.cs starts
2. AccountLoader checks for existing account
3. AccountLoader shows "No existing account found"
4. Splash.cs shows "New Game" button
5. User clicks "New Game" → Splash.cs shows username input
6. User enters username → Splash.cs validates and creates account
7. Splash.cs transitions to main menu

## Testing

### Test Existing User Flow
1. Create an account first
2. Restart the game
3. Should automatically load account and go to main menu

### Test New User Flow
1. Clear all data (or use fresh install)
2. Run the game
3. Should show "New Game" button
4. Click "New Game" → should show username input
5. Enter valid username → should create account and go to main menu

### Test Error Handling
1. Try invalid usernames (empty, too short, too long)
2. Should show appropriate error messages
3. Should allow retry with valid username

## Minimal Setup (No UI)

If you don't want to add UI elements:

1. **Existing Users**: Will automatically load and transition to main menu
2. **New Users**: Will skip account creation and go directly to main menu
3. **Loading Progress**: Will show in console logs only

## Troubleshooting

### Common Issues

1. **AccountLoader not found:**
   - Ensure Splash script is on a GameObject in the scene
   - Check that `useAccountSystem` is enabled in Splash script

2. **UI references missing:**
   - System will work without UI elements
   - Add UI elements for better user experience

3. **Scene not loading:**
   - Check that `mainMenuScene` field matches your scene name
   - Ensure "Active Main Menu" scene is in Build Settings

4. **Account creation failing:**
   - Check console for error messages
   - Verify AccountManager is properly initialized

### Debug Information

Both components provide debug logs for:
- Account checking and loading
- Username validation
- Account creation
- Error messages

Check the Console window for detailed information.

## File Structure

```
Assets/
├── Scripts/
│   ├── Loaders/
│   │   └── AccountLoader.cs          # Loading existing accounts
│   ├── Managers/
│   │   └── AccountManager.cs         # Data persistence and cloud save
│   └── Scenes/
│       └── Splash.cs                 # New user creation flow
└── _Scenes/
    └── Splash Screen.unity           # Your existing scene
```

## Next Steps

1. **Add UI Elements**: Enhance user experience with loading bar and username input
2. **Test on Device**: Verify platform authentication works on actual devices
3. **Integrate with Managers**: Update LivesManager and LevelProgressManager to use AccountManager
4. **Add Platform Integration**: Replace simulated emails with real Google Play Games Services/Game Center 