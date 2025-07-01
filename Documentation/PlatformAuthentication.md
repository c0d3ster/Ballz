# Platform Authentication Setup

This document explains how to set up platform authentication for the Ballz game using Google Play Games Services (Android) and Game Center (iOS).

## Overview

The `PlatformAuthManager` provides a unified interface for platform authentication across different mobile platforms. It automatically detects the platform and uses the appropriate authentication method.

## Supported Platforms

- **Android**: Google Play Games Services
- **iOS**: Game Center
- **Fallback**: Manual username entry (for unsupported platforms or when authentication fails)

## Setup Instructions

### Android - Google Play Games Services

#### 1. Download the Official Plugin
The Google Play Games Services plugin is **NOT** available through Unity Package Manager. You need to download it manually:

1. Go to: https://github.com/playgameservices/play-games-plugin-for-unity
2. Download the latest release (e.g., `GooglePlayGamesPlugin-0.11.01.unitypackage`)
3. Import the `.unitypackage` file into your Unity project

#### 2. Configure Google Play Console
1. Go to [Google Play Console](https://play.google.com/console)
2. Create a new app or select existing app
3. Enable Google Play Games Services
4. Get your **OAuth 2.0 Client ID** and **Web Client ID**

#### 3. Configure the Plugin in Unity
1. In Unity, go to **Window** → **Google Play Games** → **Setup**
2. Enter your **OAuth 2.0 Client ID** and **Web Client ID**
3. Configure additional settings as needed

#### 4. Initialize in Your Game
Add this initialization code to your game startup (e.g., in `GameManager.cs`):

```csharp
using GooglePlayGames;
using GooglePlayGames.BasicApi;

void Start()
{
    // Configure Google Play Games
    PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .RequestEmail()
        .RequestServerAuthCode(false)
        .Build();
    
    PlayGamesPlatform.InitializeInstance(config);
    PlayGamesPlatform.Activate();
    
    // Set as the default Social platform
    Social.Active = PlayGamesPlatform.Activate();
}
```

### iOS - Game Center

#### 1. Enable Game Center in Xcode
1. Open your project in Xcode
2. Select your target
3. Go to **Signing & Capabilities**
4. Add **Game Center** capability

#### 2. Configure in Unity
1. In Unity, go to **Player Settings** → **iOS**
2. Enable **Game Center** in the **Other Settings** section

## Usage

The `PlatformAuthManager` is automatically initialized by the `GameManager`. It will:

1. **Detect Platform**: Automatically determine if running on Android or iOS
2. **Attempt Authentication**: Try to authenticate with the platform's service
3. **Fallback**: If authentication fails, fall back to manual username entry
4. **Provide Email**: Return the user's email address for account creation

### Example Flow

```csharp
// The authentication happens automatically when the game starts
// You can listen for the authentication result:

PlatformAuthManager.Instance.OnAuthenticationSuccess += (email, platform) => {
    Debug.Log($"Authenticated with {platform}: {email}");
    // Proceed to username entry or account creation
};

PlatformAuthManager.Instance.OnAuthenticationFailed += (error) => {
    Debug.Log($"Authentication failed: {error}");
    // Show manual username entry UI
};
```

## Implementation Details

### Android Implementation
- Uses Unity's Social API with Google Play Games plugin
- Attempts to get user email (may require additional permissions)
- Falls back to manual entry if Google Play Games is not available

### iOS Implementation
- Uses Unity's built-in Game Center integration
- Attempts to get user email from Game Center
- Falls back to manual entry if Game Center is not available

### Fallback Authentication
- Provides a simple username entry interface
- Stores account data locally using PlayerPrefs
- No email verification required

## Troubleshooting

### Common Issues

1. **Google Play Games Not Working**
   - Ensure the plugin is properly imported
   - Check that OAuth credentials are correct
   - Verify the app is signed with the correct keystore

2. **Game Center Not Working**
   - Ensure Game Center capability is enabled in Xcode
   - Check that the user is signed into Game Center on the device
   - Verify the app is properly signed for iOS

3. **Email Not Available**
   - Some platforms may not provide email access
   - The system will fall back to manual username entry
   - Consider requesting additional permissions if needed

### Debug Logging

The `PlatformAuthManager` provides detailed debug logging. Check the Unity Console for messages starting with `[PlatformAuthManager]` to diagnose issues.

## Security Considerations

- Platform authentication provides verified user identity
- Email addresses are obtained from trusted platform services
- Fallback authentication has no verification (as intended for simplicity)
- Consider implementing additional security measures for production use

## Future Enhancements

- Add support for additional platforms (Steam, Epic, etc.)
- Implement email verification for fallback accounts
- Add account linking between platforms
- Implement cloud save integration with platform accounts 