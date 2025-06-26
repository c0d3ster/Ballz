# Cloud Save Setup Guide

This guide explains how to set up persistent cloud storage for your Unity mobile game that survives app uninstalls and reinstalls.

## Overview

The cloud save system provides:
- **Automatic backup** to cloud storage (iCloud for iOS, Google Play Games for Android)
- **Local fallback** using PlayerPrefs when cloud is unavailable
- **Cross-platform compatibility** through Unity Cloud Save
- **Automatic saving** when app goes to background or loses focus
- **Manual save/load controls** for testing and user control

## Setup Steps

### 1. Unity Services Setup

1. **Enable Unity Gaming Services**:
   - Go to `Window > General > Services`
   - Sign in with your Unity account
   - Create a new project or link existing project

2. **Enable Cloud Save**:
   - In the Services window, find "Cloud Save"
   - Click "Enable" to activate the service
   - This will automatically handle platform-specific implementations

3. **Configure Authentication**:
   - In Services, find "Authentication"
   - Enable "Anonymous Authentication" (required for cloud save)

### 2. Package Installation

The following packages should be automatically installed when you enable the services:

- `com.unity.services.core`
- `com.unity.services.authentication`
- `com.unity.services.cloudsave`

If not, install them via Package Manager:
- `Window > Package Manager`
- Click the "+" button
- Add package by name

### 3. Project Configuration

1. **Update Project Settings**:
   - Go to `Edit > Project Settings > Services`
   - Ensure your project is linked to Unity Gaming Services
   - Verify Cloud Save is enabled

2. **Build Settings**:
   - For iOS: Enable "iCloud" capability in Xcode
   - For Android: No additional setup required (handled by Unity)

### 4. Code Integration

The `CloudSaveManager` is automatically created by `GameManager` during bootstrap. No additional setup required.

## How It Works

### Data Flow

1. **App Start**: CloudSaveManager initializes and attempts to connect to cloud services
2. **Data Load**: If cloud data exists, it loads from cloud; otherwise loads from local storage
3. **Data Save**: Saves to local storage immediately, then attempts cloud save
4. **Automatic Save**: Triggers on app pause, focus loss, or scene changes

### Data Structure

The system saves all game data in a single JSON structure:

```json
{
  "difficulty": 1.0,
  "useTarget": true,
  "useAccelerometer": true,
  "useJoystick": true,
  "useKeyboard": true,
  "collectLevel": 1,
  "balanceLevel": 1,
  "dodgeLevel": 1,
  "jumpLevel": 1,
  "pushLevel": 1,
  "currentLives": 5,
  "lastLifeLostTicks": 0,
  "lastSaveTime": 0,
  "lastLoadTime": 0
}
```

## Platform-Specific Behavior

### iOS
- Uses iCloud Key-Value Storage
- Automatic backup to iCloud
- Survives app uninstall/reinstall
- Limited to 1MB per key, 1024 keys total

### Android
- Uses Google Play Games Services
- Automatic sync across devices
- Survives app uninstall/reinstall
- Requires Google Play Games account

### Fallback
- If cloud services unavailable, uses local PlayerPrefs
- Data persists until app is uninstalled
- Automatic retry when cloud becomes available

## Testing

### Editor Testing
- Cloud save works in editor with Unity Gaming Services
- Use context menu options in CloudSaveManager for testing
- Check console logs for save/load status

### Device Testing
- Test on actual devices (cloud save doesn't work in editor for platform-specific features)
- Test with and without internet connection
- Test app uninstall/reinstall scenarios

## Troubleshooting

### Common Issues

1. **"Cloud save not available"**
   - Check internet connection
   - Verify Unity Services are properly configured
   - Ensure Authentication is enabled

2. **"Sign in failed"**
   - Check Unity Services project configuration
   - Verify project is linked correctly

3. **"Save failed"**
   - Check cloud storage limits (especially on iOS)
   - Verify data structure is serializable
   - Check console for specific error messages

### Debug Commands

Use these context menu options in the CloudSaveManager component:
- **Test Cloud Save**: Manually trigger a save
- **Test Cloud Load**: Manually trigger a load
- **Test Ad Completion/Failure**: Test ad integration

## UI Integration

The `CloudSaveUI` component provides:
- Visual status indicators
- Manual save/load buttons
- Last save time display
- Clear data option

### Adding to Scenes

1. Create a UI Canvas
2. Add the CloudSaveUI component to a GameObject
3. Assign UI references (buttons, text, icons)
4. Configure display settings

## Best Practices

1. **Always save locally first** - Provides immediate backup
2. **Handle offline scenarios** - Graceful fallback to local storage
3. **Validate data** - Check data integrity after loading
4. **User feedback** - Show save status to users
5. **Error handling** - Provide meaningful error messages

## Limitations

- **iOS**: 1MB per key limit, 1024 keys total
- **Android**: Requires Google Play Games account
- **Network dependency**: Requires internet for cloud operations
- **Platform differences**: Behavior may vary between iOS and Android

## Future Enhancements

Consider these improvements:
- **Data compression** for larger save files
- **Incremental saves** to reduce bandwidth
- **Conflict resolution** for multiple devices
- **Backup verification** to ensure data integrity
- **User preferences** for cloud save settings 