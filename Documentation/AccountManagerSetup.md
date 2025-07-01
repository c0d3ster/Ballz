# Account Manager Setup Guide

This guide explains how to set up the simple AccountManager system that uses platform authentication (Google Play Games Services for Android, Game Center for iOS) to automatically get user emails and manage basic accounts.

## Overview

The AccountManager system provides:
- Automatic platform detection (Android/iOS)
- Platform authentication to get user email
- New Game vs Load Game flow on splash screen
- Username creation for new accounts
- Basic account persistence using PlayerPrefs

## Components

### 1. AccountManager
- **Location**: `Assets/Scripts/Managers/AccountManager.cs`
- **Purpose**: Handles platform authentication, account creation, and data persistence
- **Features**:
  - Detects platform (Android/iOS/Editor)
  - Simulates getting user email from platform services
  - Manages account creation and loading
  - Saves account data to PlayerPrefs

### 2. SplashScreenUI
- **Location**: `Assets/Scripts/UI/SplashScreenUI.cs`
- **Purpose**: Handles the UI flow for new game vs load game
- **Features**:
  - Loading screen with progress
  - New Game panel with username input
  - Load Game panel for existing accounts
  - Error handling for authentication failures

## Setup Instructions

### Step 1: Create the AccountManager GameObject

1. Create an empty GameObject in your splash screen scene
2. Name it "AccountManager"
3. Add the `AccountManager` script to it
4. The script will automatically detect the platform and handle initialization

### Step 2: Set up the Splash Screen UI

1. Create a Canvas in your splash screen scene
2. Create the following UI panels as child objects:
   - `LoadingPanel`
   - `AuthenticationPanel`
   - `NewGamePanel`
   - `LoadGamePanel`
   - `UsernameInputPanel`

3. Add the `SplashScreenUI` script to the Canvas or a UI manager GameObject
4. Assign all the UI references in the inspector

### Step 3: UI Panel Structure

#### Loading Panel
- Progress bar (Slider)
- Loading text (TextMeshProUGUI)

#### New Game Panel
- Welcome text (TextMeshProUGUI)
- "New Game" button (Button)

#### Load Game Panel
- Welcome back text (TextMeshProUGUI)
- "Load Game" button (Button)
- "Clear Data" button (Button)

#### Username Input Panel
- Email display text (TextMeshProUGUI)
- Username input field (TMP_InputField)
- Confirm button (Button)
- Cancel button (Button)
- Error text (TextMeshProUGUI)

#### Authentication Panel
- Error status text (TextMeshProUGUI)

## Flow Description

1. **App Launch**: Splash screen shows loading panel
2. **Platform Authentication**: AccountManager detects platform and simulates getting user email
3. **Account Check**: System checks if user has existing account
4. **UI Flow**:
   - **New User**: Shows "New Game" panel → Username input → Account creation
   - **Existing User**: Shows "Load Game" panel → Account loading
5. **Transition**: After account setup, transition to main menu or game

## Platform Integration (Future Steps)

### Android - Google Play Games Services
To integrate with real Google Play Games Services:

1. Install Google Play Games plugin for Unity
2. Replace the simulated email in `AuthenticateWithPlatform()`:
```csharp
#if UNITY_ANDROID
    // Get real email from Google Play Games Services
    userEmail = PlayGamesClientConfiguration.GetUserEmail()
#endif
```

### iOS - Game Center
To integrate with real Game Center:

1. Enable Game Center in Unity iOS settings
2. Replace the simulated email in `AuthenticateWithPlatform()`:
```csharp
#if UNITY_IOS
    // Get real email from Game Center
    userEmail = GameCenterManager.GetUserEmail()
#endif
```

## Data Structure

The system saves the following data to PlayerPrefs:
- `UserEmail`: User's email from platform
- `Username`: User's chosen username
- `Platform`: Platform identifier (Android/iOS)
- `CreatedDate`: Account creation timestamp

## Customization

### Username Validation
Modify the username validation rules in `SplashScreenUI.OnConfirmUsernameClicked()`:
- Minimum length (currently 3 characters)
- Maximum length (currently 20 characters)
- Allowed characters
- Reserved words

### UI Styling
Customize the UI panels to match your game's visual style:
- Colors and fonts
- Animations and transitions
- Button styles and layouts

### Error Handling
Enhance error handling for:
- Network connectivity issues
- Platform service failures
- Data corruption

## Testing

### Editor Testing
- The system works in the Unity Editor with simulated emails
- Test both new user and existing user flows
- Verify data persistence across play sessions

### Device Testing
- Test on Android device with Google Play Games Services
- Test on iOS device with Game Center
- Verify platform detection and email retrieval

## Next Steps

1. **Real Platform Integration**: Replace simulated emails with actual platform authentication
2. **Enhanced UI**: Add animations, better styling, and confirmation dialogs
3. **Data Migration**: Plan for future data structure changes
4. **Cloud Save**: Consider adding cloud save for cross-device sync
5. **Analytics**: Add analytics tracking for user behavior

## Troubleshooting

### Common Issues

1. **AccountManager not found**: Ensure AccountManager GameObject exists in the scene
2. **UI references missing**: Check all UI element assignments in the inspector
3. **Platform detection wrong**: Verify build target settings
4. **Data not persisting**: Check PlayerPrefs permissions and storage

### Debug Logs
The system includes comprehensive debug logging. Check the Console for:
- Platform detection messages
- Authentication status
- Account creation/loading events
- Error messages 