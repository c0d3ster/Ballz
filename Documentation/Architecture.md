# Game Architecture

This document explains the overall architecture of the BALLZ game, including the manager system, bootstrap process, and component communication.

## Overview

The game follows a manager-based architecture with centralized systems that handle different aspects of gameplay. All managers are singletons that persist across scenes and communicate through events.

The codebase distinguishes between **Managers** and **Loaders** based on their primary responsibilities:

- **Managers**: Handle ongoing game state, data persistence, and system coordination. They are long-lived, persist across scenes, and manage continuous game systems like lives, scores, progress, and UI elements.

- **Loaders**: Handle resource loading, initialization, and one-time setup operations. They may be long-lived but focus on loading/initialization tasks like scene transitions, asset loading, and external service connections.

### Core Managers

#### GameManager
- **Purpose**: Main orchestrator and bootstrap coordinator
- **Location**: `Assets/Scripts/Managers/GameManager.cs`
- **Initialization**: Created automatically via `[RuntimeInitializeOnLoadMethod]`
- **Responsibilities**:
  - Creates and initializes all other managers
  - Sets up the UI system
  - Ensures proper initialization order
  - Manages the bootstrap process

#### UIManager
- **Purpose**: Manages the persistent UI canvas and UI state
- **Location**: `Assets/Scripts/Managers/UIManager.cs`
- **Initialization**: Loaded from prefab by GameManager
- **Responsibilities**:
  - Persistent UI canvas management
  - Pause/resume functionality
  - UI visibility based on scene type
  - Touch controls and UI interactions

#### LivesManager
- **Purpose**: Handles player lives system and regeneration
- **Location**: `Assets/Scripts/Managers/LivesManager.cs`
- **Initialization**: Created by GameManager during bootstrap
- **Responsibilities**:
  - Life tracking and persistence
  - Life regeneration timing
  - LivesDisplay creation and management
  - Life loss/gain logic

#### CountManager
- **Purpose**: Manages collection-based game modes
- **Location**: `Assets/Scripts/Managers/CountManager.cs`
- **Initialization**: Created by GameManager during bootstrap
- **Responsibilities**:
  - Pickup counting and tracking
  - Level completion detection
  - CountDisplay creation and management
  - Collection event handling

#### TimerManager
- **Purpose**: Handles time-based game modes
- **Location**: `Assets/Scripts/Managers/TimerManager.cs`
- **Initialization**: Created by GameManager during bootstrap
- **Responsibilities**:
  - Timer display and countdown
  - Scene-based time limit configuration
  - Pause/resume integration
  - Time up event handling

#### LevelProgressManager
- **Purpose**: Manages level progression and unlocks
- **Location**: `Assets/Scripts/Managers/LevelProgressManager.cs`
- **Initialization**: Created by GameManager during bootstrap
- **Responsibilities**:
  - Level completion tracking
  - Progress persistence
  - Game mode unlocking logic
  - Level visibility management

### Core Loaders

#### SceneLoader
- **Purpose**: Handles all scene transitions and scene state
- **Location**: `Assets/Scripts/Loaders/SceneLoader.cs`
- **Initialization**: Created by GameManager during bootstrap
- **Responsibilities**:
  - Scene loading and unloading
  - Scene state tracking (current, last, paused)
  - Game mode detection from scene names
  - Overlay scene management (PAUSE, GAME OVER, WIN)
  - Level progression logic

## Bootstrap Process

The game uses Unity's `[RuntimeInitializeOnLoadMethod]` to automatically initialize before any scene loads:

```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
private static void Bootstrap()
{
    // 1. Create GameManager
    // 2. Create SceneLoader
    // 3. Create LevelProgressManager
    // 4. Create LivesManager
    // 5. Create CountManager
    // 6. Create TimerManager
    // 7. Initialize UI system
}
```

### Initialization Order
1. **GameManager** - Main coordinator
2. **SceneLoader** - Scene management (dependency for other systems)
3. **LevelProgressManager** - Level progression
4. **LivesManager** - Lives system
5. **CountManager** - Collection system
6. **TimerManager** - Timer system
7. **UIManager** - UI system (loaded from prefab)

## Event System

The game uses C# events for communication between managers:

### Key Events
- `LivesManager.OnLivesChanged` - When player lives change
- `CountManager.OnCountChanged` - When pickup count changes
- `CountManager.OnLevelComplete` - When level is completed
- `TimerManager.OnTimeChanged` - When timer value changes
- `TimerManager.OnTimeUp` - When timer reaches zero

### Event Usage Example
```csharp
// Subscribe to lives changes
LivesManager.Instance.OnLivesChanged += (lives) => {
    Debug.Log($"Lives changed to: {lives}");
};

// Subscribe to level completion
CountManager.Instance.OnLevelComplete += () => {
    SceneLoader.Instance.Win();
};
```

## Scene Management Architecture

### Scene Types
1. **Main Scenes**: Gameplay scenes (Ball Collector 1, Ball Balancer 1, etc.)
2. **Overlay Scenes**: UI scenes that load additively (PAUSE, GAME OVER, WIN)
3. **Non-Interactive Scenes**: Splash screen, main menu

### Scene Loading Modes
- **Single Mode**: Replaces current scene (main scenes)
- **Additive Mode**: Loads on top of current scene (overlay scenes)

### Scene State Tracking
```csharp
public string lastScene;    // Previous scene name
public string currentScene; // Current active scene
public bool isPaused;       // Whether game is paused
```

## UI Architecture

### Canvas Hierarchy
```
UIManager (Persistent)
└── UICanvas
    ├── TouchControllerOuter
    │   └── TouchControllerInner
    ├── SettingsButton
    ├── JumpButton
    └── [Dynamic UI Elements]
        ├── LivesContainer (created by LivesManager)
        ├── CountText (created by CountManager)
        └── TimerText (created by TimerManager)
```

### Dynamic UI Creation
Each manager creates its own UI elements:
- **LivesManager** → LivesContainer + LivesDisplay
- **CountManager** → CountDisplay
- **TimerManager** → TimerText

## Persistence System

### PlayerPrefs Usage
- **LivesManager**: Stores current lives and last life lost time
- **LevelProgressManager**: Stores level completion status for each game mode

### Data Keys
```csharp
// LivesManager
private const string LIVES_KEY = "PlayerLives";
private const string LAST_LIFE_LOST_KEY = "LastLifeLostTime";

// LevelProgressManager
private const string PUSH_LEVEL_KEY = "PushLevel";
private const string COLLECT_LEVEL_KEY = "CollectLevel";
// ... etc for each game mode
```

## Communication Patterns

### Manager-to-Manager Communication
1. **Direct References**: Managers can access other managers via singleton instances
2. **Events**: Managers communicate state changes through events
3. **Scene Events**: Scene loading triggers manager initialization

### Example Communication Flow
```
Scene Loads → SceneLoader.OnSceneLoaded → 
TimerManager.InitializeTimerForScene → 
TimerManager.UpdateTimerDisplay → 
UI shows/hides timer
```

## Error Handling

### Manager Initialization
- Each manager checks for existing instances to prevent duplicates
- Graceful fallbacks when dependencies are missing
- Debug logging for troubleshooting

### Scene Loading
- Validation of scene names and game modes
- Fallback to main menu for invalid scenes
- Error handling for missing scenes

## Performance Considerations

### Singleton Pattern
- All managers use singleton pattern for global access
- DontDestroyOnLoad ensures persistence across scenes
- Automatic cleanup of duplicate instances

### UI Optimization
- Dynamic UI creation only when needed
- Automatic show/hide based on scene requirements
- Efficient event subscription/unsubscription

## Extension Points

### Adding New Managers
1. Create manager class with singleton pattern
2. Add to GameManager bootstrap process
3. Implement scene loading integration
4. Add UI elements if needed

### Adding New Game Modes
1. Add to GameMode enum
2. Update SceneLoader game mode detection
3. Add to LevelProgressManager
4. Configure scene naming conventions

### Adding New UI Systems
1. Create UI manager or extend existing one
2. Add to UIManager canvas hierarchy
3. Implement show/hide logic based on scene
4. Add event integration if needed 