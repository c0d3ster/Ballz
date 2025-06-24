# Ad-Based Life Recovery System

This document explains the ad-based life recovery system that allows players to watch ads to gain additional lives when they run out.

## Overview

The ad system integrates with Unity Ads to provide players with a way to continue playing when they run out of lives. Players can watch a rewarded ad to receive 2 additional lives.

## Quick Setup Guide

### Step 1: Enable Unity Services
1. Open Unity and go to **Window > General > Services**
2. Sign in with your Unity account (or create one at unity.com)
3. Click **"Enable Services"** for your project

### Step 2: Enable Unity Ads
1. In the Services window, find **"Ads"** in the left sidebar
2. Click on it and then click **"Enable"**
3. This activates Unity Ads for your project

### Step 3: Get Your Project ID
1. After enabling Ads, note your **Project ID** (this is your game ID)
2. It will look something like: `12345678-1234-1234-1234-123456789012`

### Step 4: Configure AdManager
1. Find the AdManager GameObject in your scene (created automatically)
2. Set the following values:
   - **Android Game ID**: Your Project ID from Step 3
   - **iOS Game ID**: Your Project ID from Step 3
   - **Test Mode**: `true` (for testing)
   - **Rewarded Ad Unit ID**: `"Rewarded_Android"` (test ID)

### Step 5: Test Immediately
- **Editor Testing**: Use context menu on AdManager:
  - Right-click AdManager component → "Test Ad Completion"
  - Right-click AdManager component → "Test Ad Failure"
- **Build Testing**: Build and run with `testMode = true`

## Components

### AdManager
- **Location**: `Assets/Scripts/Managers/AdManager.cs`
- **Purpose**: Handles Unity Ads integration and ad lifecycle
- **Features**:
  - Automatic initialization of Unity Ads
  - Loading and showing rewarded ads
  - Event system for ad completion/failure
  - Test methods for editor development

### LivesManager Integration
- **New Method**: `AddLivesViaAd(int livesToAdd = 2)`
- **Purpose**: Adds lives when ad is completed
- **Behavior**: Respects max lives limit

### Scene Integration

#### Win Scene
- **Modified**: `Assets/Scripts/Scenes/Win.cs`
- **Changes**:
  - Checks if player is out of lives
  - Shows warning message when out of lives
  - Changes "Next Level" button to "Watch Ad to Continue"
  - Disables progression until lives are available

#### Game Over Scene
- **Modified**: `Assets/Scripts/Scenes/GAME_OVER.cs`
- **Changes**:
  - Shows ad button when out of lives
  - Provides "Watch Ad for 2 Lives" option
  - Handles ad loading states

#### SceneLoader
- **Modified**: `Assets/Scripts/Loaders/SceneLoader.cs`
- **Changes**:
  - `NextLevel()` method checks for lives before progression
  - Redirects to Game Over if out of lives

#### Portal Level Start
- **Modified**: `Assets/Scripts/Scenes/PORTAL_LEVEL_START.cs`
- **Changes**:
  - Checks lives before allowing level start from main menu
  - Redirects to Game Over if out of lives

## Setup Instructions

### 1. Unity Ads Configuration
1. Open Unity Services window (Window > General > Services)
2. Enable Unity Ads service
3. Configure your game ID for Android/iOS platforms
4. Set up rewarded ad units in the Unity Dashboard

### 2. AdManager Configuration
1. The AdManager is automatically created during game bootstrap
2. Configure the following in the AdManager component:
   - `androidGameId`: Your Android game ID
   - `iosGameId`: Your iOS game ID
   - `testMode`: Set to true for testing, false for production
   - `rewardedAdUnitId`: Your rewarded ad unit ID

### 3. Testing
- Use the context menu options on AdManager in the editor:
  - "Test Ad Completion" - Simulates successful ad completion
  - "Test Ad Failure" - Simulates ad failure

## User Flow

### When Player Runs Out of Lives

1. **Level Completion (Win Scene)**:
   - Player completes a level but has no lives
   - Win screen shows "You're out of lives! Watch an ad to continue."
   - "Next Level" button becomes "Watch Ad to Continue"
   - Player must watch ad to proceed

2. **Game Over**:
   - Player dies and has no lives
   - Game Over screen shows ad option
   - "Watch Ad for 2 Lives" button appears
   - Player can watch ad to get lives and continue

3. **Level Start from Main Menu**:
   - Player tries to start a level without lives
   - System redirects to Game Over screen
   - Player must watch ad to get lives before starting

### Ad Completion
- Player watches the full ad
- System automatically adds 2 lives
- Player can now continue with gameplay
- Lives are saved and persist across sessions

## Error Handling

### Ad Not Available
- If ads fail to load or are not available:
  - Button shows "Ad Not Available"
  - Player must wait for lives to regenerate naturally
  - System gracefully handles ad failures

### Ad Skipped/Failed
- If player skips ad or ad fails:
  - No lives are awarded
  - Player must try again or wait for natural regeneration
  - System logs appropriate error messages

## Integration Points

### Bootstrap Process
The AdManager is automatically created during game bootstrap in `GameManager.cs`:
```csharp
// Create AdManager if it doesn't exist
if (FindFirstObjectByType<AdManager>() == null)
{
    GameObject am = new GameObject("AdManager");
    am.transform.SetParent(null);
    AdManager adManager = am.AddComponent<AdManager>();
    DontDestroyOnLoad(am);
}
```

### Event System
The ad system uses C# events for communication:
```csharp
adManager.OnAdCompleted += OnAdCompleted;
adManager.OnAdFailed += OnAdFailed;
```

## Best Practices

1. **Always check for lives before progression**
2. **Provide clear feedback when ads are not available**
3. **Handle ad failures gracefully**
4. **Test thoroughly in editor before building**
5. **Monitor ad performance and user engagement**

## Troubleshooting

### Common Issues

1. **Ads not loading**:
   - Check Unity Services configuration
   - Verify game ID is correct
   - Ensure ad units are properly set up

2. **Ad completion not triggering**:
   - Check event subscriptions
   - Verify ad unit ID matches dashboard
   - Test with editor methods

3. **Lives not being added**:
   - Check LivesManager integration
   - Verify event handlers are properly connected
   - Check for max lives limit

### Debug Logging
The system provides comprehensive debug logging:
- `[AdManager]` - Ad-related messages
- `[Win]` - Win scene ad handling
- `[GAME_OVER]` - Game over scene ad handling
- `[SceneLoader]` - Level progression checks
- `[PORTAL_LEVEL_START]` - Portal level start checks 