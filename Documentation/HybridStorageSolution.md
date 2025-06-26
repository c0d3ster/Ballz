# Hybrid Storage Solution

This document outlines a hybrid approach that combines Unity Cloud Save with a simple username system and local leaderboard, providing minimal complexity while supporting username validation and conflict checking.

## Overview

The Hybrid Storage Solution provides:
- Unity Cloud Save for persistent storage
- Simple username system with global conflict checking
- Local leaderboard with username integration
- No external database required
- Easy setup and maintenance

## Architecture

```
Unity Cloud Save (Account Data + Username Registry)
    ↓
SimpleUserManager (Username Validation + Local Leaderboard)
    ↓
Game Managers (Lives, Progress, Settings)
```

## Components

### 1. SimpleUserManager
- **Location**: `Assets/Scripts/Managers/SimpleUserManager.cs`
- **Purpose**: Handles username management and local leaderboard
- **Features**:
  - Username validation and conflict checking
  - Local leaderboard with username integration
  - Unity Cloud Save integration for username registry
  - Fallback to local storage

### 2. UsernameManager
- **Location**: `Assets/Scripts/UI/UsernameManager.cs`
- **Purpose**: UI for username creation and management
- **Features**:
  - Username input and validation
  - Real-time conflict checking
  - Error handling and user feedback

### 3. SimpleLeaderboard
- **Location**: `Assets/Scripts/UI/SimpleLeaderboard.cs`
- **Purpose**: Local leaderboard with username integration
- **Features**:
  - Display top scores with usernames
  - Add new scores
  - Sort and filter functionality

## Setup Instructions

### Step 1: Install Unity Cloud Save

1. Open Package Manager (Window > Package Manager)
2. Add package by name: `com.unity.services.cloudsave`
3. Enable Unity Services in Project Settings
4. Create a new project in Unity Dashboard
5. Link your project to the Unity Services project

### Step 2: Create SimpleUserManager

1. Create an empty GameObject in your scene
2. Name it "SimpleUserManager"
3. Add the `SimpleUserManager` script
4. Configure settings in the inspector

### Step 3: Set up Username UI

1. Create a Canvas for username management
2. Add the `UsernameManager` script to a UI GameObject
3. Create UI elements:
   - Username input field
   - Confirm button
   - Error text display
   - Loading indicator

### Step 4: Set up Leaderboard UI

1. Create a Canvas for the leaderboard
2. Add the `SimpleLeaderboard` script
3. Create UI elements:
   - Score list
   - Username display
   - Add score functionality

## Usage

### Creating a Username

```csharp
// Get the SimpleUserManager instance
SimpleUserManager userManager = SimpleUserManager.Instance;

// Check if username is available
bool isAvailable = await userManager.IsUsernameAvailable("PlayerName");

if (isAvailable)
{
    // Create the username
    await userManager.CreateUsername("PlayerName");
}
```

### Adding Scores to Leaderboard

```csharp
// Get the leaderboard instance
SimpleLeaderboard leaderboard = SimpleLeaderboard.Instance;

// Add a new score
leaderboard.AddScore("PlayerName", 1000, "Collect");

// Get top scores
List<LeaderboardEntry> topScores = leaderboard.GetTopScores(10);
```

### Accessing User Data

```csharp
// Get current username
string username = SimpleUserManager.Instance.CurrentUsername;

// Check if user has username
bool hasUsername = SimpleUserManager.Instance.HasUsername;

// Get user's best score
int bestScore = SimpleLeaderboard.Instance.GetUserBestScore(username);
```

## Data Structure

### Username Registry (Cloud Save)
```json
{
  "usernames": {
    "PlayerName1": "email1@example.com",
    "PlayerName2": "email2@example.com"
  }
}
```

### Local Leaderboard Data
```json
{
  "scores": [
    {
      "username": "PlayerName1",
      "score": 1000,
      "gameMode": "Collect",
      "timestamp": "2024-01-01T12:00:00Z"
    }
  ]
}
```

## Benefits

1. **Minimal Complexity**: No external database or complex authentication
2. **Username Support**: Players can choose custom usernames
3. **Conflict Prevention**: Global username registry prevents duplicates
4. **Local Leaderboard**: Fast, responsive leaderboard without network calls
5. **Cloud Persistence**: Data survives app uninstall/reinstall
6. **Offline Support**: Works without internet connection
7. **Easy Migration**: Can upgrade to Firebase later if needed

## Limitations

1. **Local Leaderboard**: Not truly global across all players
2. **No Cross-Device**: Username tied to device/account
3. **Limited Social**: No friend system or achievements
4. **Username Conflicts**: First-come-first-served for usernames

## Migration Path

This solution can be easily upgraded to more complex systems:

1. **Add Firebase**: Replace Unity Cloud Save with Firebase
2. **Global Leaderboard**: Replace local leaderboard with cloud database
3. **Friend System**: Add social features and friend lists
4. **Achievements**: Add achievement system with cloud tracking

## Testing

### Username Testing
1. Test username creation with valid names
2. Test username conflict detection
3. Test username validation rules
4. Test offline username creation

### Leaderboard Testing
1. Test adding new scores
2. Test leaderboard sorting
3. Test username display
4. Test data persistence

### Cloud Save Testing
1. Test data sync across devices
2. Test offline functionality
3. Test data recovery after uninstall
4. Test conflict resolution

## Troubleshooting

### Common Issues

1. **Username not saving**: Check Unity Cloud Save configuration
2. **Leaderboard not updating**: Verify local storage permissions
3. **Conflict detection not working**: Check internet connection
4. **Data not syncing**: Verify Unity Services project setup

### Debug Logs

The system includes comprehensive logging:
- Username creation attempts
- Conflict detection results
- Cloud save operations
- Leaderboard updates

Check the Console for detailed information.

## Next Steps

1. **Implement UI**: Create username input and leaderboard displays
2. **Add Validation**: Implement username rules and restrictions
3. **Test Integration**: Verify with existing game systems
4. **Add Features**: Consider adding achievements or friend system 