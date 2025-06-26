# Cross-Platform User Management

This document outlines a comprehensive cross-platform user management system that supports true account sharing across iOS, Android, Desktop, and WebGL platforms.

## Overview

The Cross-Platform User Management system provides:
- True cross-platform account sharing
- Automatic device linking and synchronization
- Global username registry with conflict detection
- Unified data management across all platforms
- Seamless user experience across devices

## Architecture

```
CrossPlatformUserManager (Core Logic)
    ↓
Platform-Specific Authentication
    ↓
Unity Cloud Save (Global Data + Username Registry)
    ↓
Local Storage (Offline Backup)
```

## Components

### 1. CrossPlatformUserManager
- **Location**: `Assets/Scripts/Managers/CrossPlatformUserManager.cs`
- **Purpose**: Handles cross-platform account management and synchronization
- **Features**:
  - Cross-platform ID generation and management
  - Device linking and synchronization
  - Global username registry with conflict detection
  - Automatic data sync across platforms
  - Offline support with local backup

### 2. PlatformAuthManager
- **Location**: `Assets/Scripts/Managers/PlatformAuthManager.cs`
- **Purpose**: Handles platform-specific authentication
- **Features**:
  - Google Play Games Services (Android)
  - Game Center (iOS)
  - Steam (Desktop)
  - WebGL authentication
  - Fallback to anonymous authentication

### 3. CrossPlatformUI
- **Location**: `Assets/Scripts/UI/CrossPlatformUI.cs`
- **Purpose**: UI for cross-platform account management
- **Features**:
  - Account linking interface
  - Device management
  - Username creation and management
  - Sync status display

## Platform Support

### Android
- Google Play Games Services integration
- Automatic email retrieval
- Achievement and leaderboard support
- Cloud save integration

### iOS
- Game Center integration
- iCloud backup support
- Achievement and leaderboard support
- Automatic device linking

### Desktop
- Steam integration (optional)
- Local account management
- Cross-platform sync via cloud
- Offline mode support

### WebGL
- Browser-based authentication
- Local storage with cloud sync
- Cross-platform account linking
- Progressive web app support

## Setup Instructions

### Step 1: Platform Configuration

#### Android Setup
1. Configure Google Play Games Services
2. Add Google Play Games plugin
3. Set up OAuth credentials
4. Configure cloud save

#### iOS Setup
1. Enable Game Center in Unity
2. Configure iCloud capabilities
3. Set up App Store Connect
4. Configure cloud save

#### Desktop Setup
1. Optional Steam integration
2. Configure local storage
3. Set up cloud sync
4. Configure offline mode

#### WebGL Setup
1. Configure web authentication
2. Set up local storage
3. Configure cloud sync
4. Test cross-platform linking

### Step 2: Create CrossPlatformUserManager

1. Create an empty GameObject in your scene
2. Name it "CrossPlatformUserManager"
3. Add the `CrossPlatformUserManager` script
4. Configure platform settings in the inspector

### Step 3: Set up Platform Authentication

1. Add the `PlatformAuthManager` script
2. Configure platform-specific settings
3. Set up authentication callbacks
4. Test platform detection

### Step 4: Create Cross-Platform UI

1. Create a Canvas for account management
2. Add the `CrossPlatformUI` script
3. Create UI elements:
   - Account linking interface
   - Device management panel
   - Username creation form
   - Sync status display

## Usage

### Initializing Cross-Platform System

```csharp
// Get the CrossPlatformUserManager instance
CrossPlatformUserManager userManager = CrossPlatformUserManager.Instance;

// Initialize the system
await userManager.Initialize();

// Check if user has cross-platform account
bool hasAccount = userManager.HasCrossPlatformAccount();
```

### Creating Cross-Platform Account

```csharp
// Create a new cross-platform account
await userManager.CreateCrossPlatformAccount("PlayerName", "user@email.com");

// Link current device to account
await userManager.LinkCurrentDevice();

// Verify account creation
bool isLinked = userManager.IsCurrentDeviceLinked();
```

### Managing Devices

```csharp
// Get all linked devices
List<DeviceInfo> devices = userManager.GetLinkedDevices();

// Unlink a device
await userManager.UnlinkDevice(deviceId);

// Check if device is linked
bool isLinked = userManager.IsDeviceLinked(deviceId);
```

### Synchronizing Data

```csharp
// Sync data across all devices
await userManager.SyncData();

// Get sync status
SyncStatus status = userManager.GetSyncStatus();

// Force sync
await userManager.ForceSync();
```

### Username Management

```csharp
// Check username availability globally
bool isAvailable = await userManager.IsUsernameAvailableGlobally("PlayerName");

// Create username with global conflict checking
await userManager.CreateUsername("PlayerName");

// Update username
await userManager.UpdateUsername("NewPlayerName");
```

## Data Structure

### Cross-Platform Account Data
```json
{
  "crossPlatformId": "unique-id-12345",
  "username": "PlayerName",
  "email": "user@email.com",
  "createdDate": "2024-01-01T12:00:00Z",
  "devices": [
    {
      "deviceId": "android-device-1",
      "platform": "Android",
      "linkedDate": "2024-01-01T12:00:00Z",
      "lastSync": "2024-01-01T12:00:00Z"
    }
  ],
  "gameData": {
    "lives": 5,
    "levels": {
      "collect": 10,
      "balance": 5,
      "dodge": 8
    },
    "settings": {
      "difficulty": 1.0,
      "useTarget": true
    }
  }
}
```

### Global Username Registry
```json
{
  "usernames": {
    "PlayerName1": {
      "crossPlatformId": "unique-id-1",
      "createdDate": "2024-01-01T12:00:00Z",
      "lastActive": "2024-01-01T12:00:00Z"
    }
  }
}
```

## Benefits

1. **True Cross-Platform**: Seamless experience across all platforms
2. **Device Linking**: Link multiple devices to one account
3. **Global Usernames**: Unique usernames across all platforms
4. **Automatic Sync**: Data syncs automatically across devices
5. **Offline Support**: Works without internet connection
6. **Platform Integration**: Leverages native platform features
7. **Scalable**: Can handle millions of users across platforms

## Limitations

1. **Complex Setup**: Requires platform-specific configuration
2. **Platform Dependencies**: Relies on platform services
3. **Network Required**: Full sync requires internet connection
4. **Platform Restrictions**: Subject to platform policies and limitations

## Migration Path

This system can be upgraded to more advanced features:

1. **Social Features**: Add friend system and social interactions
2. **Achievements**: Cross-platform achievement system
3. **Leaderboards**: Global leaderboards across all platforms
4. **Analytics**: Cross-platform analytics and insights

## Testing

### Cross-Platform Testing
1. Test account creation on each platform
2. Test device linking across platforms
3. Test data sync between devices
4. Test username conflicts across platforms

### Platform-Specific Testing
1. Test Android Google Play Games integration
2. Test iOS Game Center integration
3. Test Desktop Steam integration
4. Test WebGL browser compatibility

### Sync Testing
1. Test automatic data sync
2. Test manual sync functionality
3. Test conflict resolution
4. Test offline mode

## Troubleshooting

### Common Issues

1. **Platform authentication failing**: Check platform configuration
2. **Device not linking**: Verify network connection and permissions
3. **Data not syncing**: Check cloud save configuration
4. **Username conflicts**: Verify global registry access

### Debug Logs

The system includes comprehensive logging:
- Platform detection and authentication
- Device linking operations
- Data sync operations
- Username management
- Error handling

Check the Console for detailed information.

## Next Steps

1. **Platform Setup**: Configure each platform's authentication
2. **UI Implementation**: Create cross-platform account management UI
3. **Testing**: Test across all target platforms
4. **Optimization**: Optimize sync performance and reliability 