# AccountManager Integration Guide

This guide explains how to integrate the new AccountManager with existing managers that currently use PlayerPrefs for data persistence.

## Overview

The AccountManager now handles all data persistence through Unity Cloud Save with PlayerPrefs as a local backup. Other managers (LivesManager, LevelProgressManager) should:
- Keep their existing logic and functionality
- Use AccountManager for data persistence instead of PlayerPrefs
- Call AccountManager methods when data changes occur

## Architecture

```
AccountManager (Cloud Save + PlayerPrefs backup)
    ↓
Other Managers (LivesManager, LevelProgressManager, etc.)
    ↓
Game Systems (UI, Gameplay, etc.)
```

## Integration Approach

### Keep Existing Manager Logic

**LivesManager** should continue to:
- Handle lives logic (losing lives, regeneration, etc.)
- Manage UI updates and events
- Control game flow based on lives

**LevelProgressManager** should continue to:
- Track level progress
- Handle level completion logic
- Manage level selection and unlocking

### Use AccountManager for Data Persistence

Instead of using PlayerPrefs directly, managers should:
- Load initial data from AccountManager
- Save data changes through AccountManager
- Subscribe to AccountManager events for data updates

## Integration Steps

### Step 1: Update Managers to Use AccountManager

#### LivesManager Integration

**Current LivesManager uses:**
- `PlayerPrefs.SetInt("PlayerLives", value)`
- `PlayerPrefs.SetString("LastLifeLostTime", value)`

**Updated approach:**
```csharp
public class LivesManager : MonoBehaviour
{
    private AccountManager accountManager
    
    private void Start()
    {
        accountManager = AccountManager.Instance
        LoadLivesFromAccount()
    }
    
    private void LoadLivesFromAccount()
    {
        if (accountManager != null && accountManager.GetGameData() != null)
        {
            var gameData = accountManager.GetGameData()
            CurrentLives = gameData.currentLives
            lastLifeLostTime = new DateTime(gameData.lastLifeLostTicks)
        }
        else
        {
            // Fallback to default values
            CurrentLives = maxLives
            lastLifeLostTime = DateTime.Now
        }
        
        // Update UI and trigger events
        OnLivesChanged?.Invoke(CurrentLives)
    }
    
    public void LoseLife()
    {
        CurrentLives = Mathf.Max(0, CurrentLives - 1)
        
        // Save through AccountManager
        if (accountManager != null)
        {
            accountManager.UpdateLives(CurrentLives)
        }
        
        OnLivesChanged?.Invoke(CurrentLives)
    }
    
    public void RegenerateLife()
    {
        if (CurrentLives < maxLives)
        {
            CurrentLives++
            
            // Save through AccountManager
            if (accountManager != null)
            {
                accountManager.UpdateLives(CurrentLives)
            }
            
            OnLivesChanged?.Invoke(CurrentLives)
        }
    }
}
```

#### LevelProgressManager Integration

**Current LevelProgressManager uses:**
- `PlayerPrefs.SetInt("CollectLevel", value)`
- `PlayerPrefs.SetInt("BalanceLevel", value)`

**Updated approach:**
```csharp
public class LevelProgressManager : MonoBehaviour
{
    private AccountManager accountManager
    
    private void Start()
    {
        accountManager = AccountManager.Instance
        LoadProgressFromAccount()
    }
    
    private void LoadProgressFromAccount()
    {
        if (accountManager != null && accountManager.GetGameData() != null)
        {
            var gameData = accountManager.GetGameData()
            collectLevel = gameData.collectLevel
            balanceLevel = gameData.balanceLevel
            dodgeLevel = gameData.dodgeLevel
            jumpLevel = gameData.jumpLevel
            pushLevel = gameData.pushLevel
        }
        else
        {
            // Fallback to default values
            collectLevel = 1
            balanceLevel = 1
            dodgeLevel = 1
            jumpLevel = 1
            pushLevel = 1
        }
        
        // Update UI and trigger events
        OnProgressLoaded?.Invoke()
    }
    
    public void CompleteLevel(string gameMode, int level)
    {
        // Update local progress
        switch (gameMode.ToLower())
        {
            case "collect":
                if (level >= collectLevel)
                    collectLevel = level + 1
                break
            case "balance":
                if (level >= balanceLevel)
                    balanceLevel = level + 1
                break
            // ... other game modes
        }
        
        // Save through AccountManager
        if (accountManager != null)
        {
            accountManager.UpdateLevel(gameMode, GetCurrentLevel(gameMode))
        }
        
        OnLevelCompleted?.Invoke(gameMode, level)
    }
    
    private int GetCurrentLevel(string gameMode)
    {
        switch (gameMode.ToLower())
        {
            case "collect": return collectLevel
            case "balance": return balanceLevel
            case "dodge": return dodgeLevel
            case "jump": return jumpLevel
            case "push": return pushLevel
            default: return 1
        }
    }
}
```

### Step 2: Handle AccountManager Events

Managers should subscribe to AccountManager events to stay in sync:

```csharp
private void Start()
{
    accountManager = AccountManager.Instance
    
    if (accountManager != null)
    {
        // Subscribe to events
        accountManager.OnDataLoaded += OnAccountDataLoaded
        accountManager.OnDataSaved += OnAccountDataSaved
        accountManager.OnCloudSaveFailed += OnCloudSaveFailed
    }
    
    LoadDataFromAccount()
}

private void OnAccountDataLoaded()
{
    Debug.Log("Account data loaded, refreshing manager data")
    LoadDataFromAccount()
}

private void OnAccountDataSaved()
{
    Debug.Log("Account data saved successfully")
}

private void OnCloudSaveFailed()
{
    Debug.LogWarning("Cloud save failed, using local data")
}

private void OnDestroy()
{
    if (accountManager != null)
    {
        accountManager.OnDataLoaded -= OnAccountDataLoaded
        accountManager.OnDataSaved -= OnAccountDataSaved
        accountManager.OnCloudSaveFailed -= OnCloudSaveFailed
    }
}
```

### Step 3: Data Migration Strategy

For existing save data, implement a migration system:

```csharp
public class DataMigrationManager : MonoBehaviour
{
    public static void MigrateOldData()
    {
        // Check if migration is needed
        if (PlayerPrefs.HasKey("PlayerLives") && !PlayerPrefs.HasKey("MigrationComplete"))
        {
            var accountManager = AccountManager.Instance
            if (accountManager != null)
            {
                var gameData = accountManager.GetGameData()
                
                // Migrate lives data
                if (PlayerPrefs.HasKey("PlayerLives"))
                {
                    gameData.currentLives = PlayerPrefs.GetInt("PlayerLives", 5)
                }
                
                // Migrate level data
                if (PlayerPrefs.HasKey("CollectLevel"))
                {
                    gameData.collectLevel = PlayerPrefs.GetInt("CollectLevel", 1)
                }
                
                if (PlayerPrefs.HasKey("BalanceLevel"))
                {
                    gameData.balanceLevel = PlayerPrefs.GetInt("BalanceLevel", 1)
                }
                
                // Continue for other data...
                
                // Save migrated data
                accountManager.SaveGameProgress()
                
                // Mark migration as complete
                PlayerPrefs.SetInt("MigrationComplete", 1)
                PlayerPrefs.Save()
            }
        }
    }
}
```

## Usage Examples

### Saving Game Progress
```csharp
// Update level progress
AccountManager.Instance.UpdateLevel("collect", newLevel)

// Update lives
AccountManager.Instance.UpdateLives(currentLives)

// Update settings
AccountManager.Instance.UpdateSettings(difficulty, useTarget, useAccelerometer, useJoystick, useKeyboard)

// Save all data
AccountManager.Instance.SaveGameProgress()
```

### Loading Game Data
```csharp
var gameData = AccountManager.Instance.GetGameData()

// Access specific data
int currentLevel = gameData.collectLevel
int lives = gameData.currentLives
float difficulty = gameData.difficulty
bool useTarget = gameData.useTarget
```

### Event Handling
```csharp
// Subscribe to events
AccountManager.Instance.OnDataSaved += OnDataSaved
AccountManager.Instance.OnDataLoaded += OnDataLoaded
AccountManager.Instance.OnCloudSaveFailed += OnCloudSaveFailed

private void OnDataSaved()
{
    Debug.Log("Game data saved successfully")
}

private void OnDataLoaded()
{
    Debug.Log("Game data loaded successfully")
    // Refresh UI or game state
}

private void OnCloudSaveFailed()
{
    Debug.LogWarning("Cloud save failed, using local data")
}
```

## Benefits

1. **Centralized Data Management**: All game data in one place
2. **Cross-Device Sync**: Players can continue on different devices
3. **Offline Support**: Game works without internet
4. **Automatic Backup**: Local PlayerPrefs backup if cloud fails
5. **Simplified Code**: No need to manage PlayerPrefs keys manually
6. **Data Consistency**: All managers use the same data structure
7. **Preserved Logic**: Existing manager functionality remains intact

## Testing

1. **Test Cloud Save**: Verify data syncs across devices
2. **Test Offline Mode**: Disconnect internet and verify local backup works
3. **Test Migration**: Verify old PlayerPrefs data migrates correctly
4. **Test Error Handling**: Verify graceful fallback when cloud save fails
5. **Test Manager Integration**: Verify managers load and save data correctly

## Next Steps

1. Update existing managers to use AccountManager
2. Implement data migration for existing save data
3. Test cloud save functionality
4. Add additional data fields to GameData as needed
5. Consider adding data validation and corruption recovery 