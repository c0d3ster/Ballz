# BALLZ

> Roll, Collect, Conquer - The Ultimate Ball Adventure!

## Description
BALLZ is an engaging Unity-based game where you control a dynamic ball through various challenging levels. Master five unique game modes, each with its own set of challenges and mechanics. From collecting cubes to pushing pucks, each mode offers a distinct gameplay experience that tests different aspects of your skills.

## Game Modes
1. **Collect Mode**
   - Navigate through levels as a ball
   - Gather cubes while avoiding obstacles
   - Progress through increasingly complex collection challenges

2. **Balance Mode**
   - Master the art of precision movement
   - Keep your ball balanced on narrow platforms
   - Navigate through challenging paths without falling

3. **Push Mode**
   - Control your ball to push a puck
   - Score goals by getting the puck into the target
   - Master momentum and physics-based challenges

4. **Jump Mode**
   - Time your jumps perfectly
   - Navigate through vertical challenges
   - Reach new heights and overcome obstacles

5. **Dodge Mode**
   - Avoid obstacles and hazards
   - React quickly to moving threats
   - Test your reflexes and timing

## Setup/Initialization
1. Clone the repository
2. Open the project in Unity 2022.3 LTS or later
3. Open the main scene from `Assets/_Scenes/Active Main Menu.unity`
4. Press Play to start the game

## Mechanics
- **Ball Control**: Smooth and responsive ball movement
- **Mode-Specific Goals**: Each mode has unique objectives
- **Level Progression**: Unlock new levels in each game mode
- **Score System**: Track your achievements in each mode
- **Physics-Based**: Realistic ball physics and interactions

## Code Organization
The project follows a structured organization pattern:

```
Assets/
├── Scripts/
│   ├── Animations/    # Animation controllers and related scripts
│   ├── Buttons/       # UI button functionality
│   ├── Controllers/   # Game control systems
│   ├── Enums/         # Game enumerations
│   ├── LevelStarts/   # Level initialization logic
│   ├── Loaders/       # Scene and asset loading
│   ├── Managers/      # Game management systems
│   ├── Pickups/       # Pickup system scripts
│   └── Scenes/        # Scene-specific scripts
│
├── _Scenes/           # Main scenes directory
│   ├── Active Main Menu.unity    # Main menu scene
│   ├── Splash Screen.unity       # Game splash screen
│   ├── Collector Series/         # Collect mode levels
│   ├── Balancer Series/          # Balance mode levels
│   ├── Pusher Series/            # Push mode levels
│   ├── Jumper Series/            # Jump mode levels
│   ├── Dodger Series/            # Dodge mode levels
│   ├── Overlay Scenes/           # Scenes that load on top of main scenes (named in UPPERCASE)
│   └── Test Scenes/              # Development and testing scenes
│
├── Resources/         # Runtime loaded assets
│   └── Prefabs/       # Prefabs including Life Pickup
├── Materials/         # Material assets
├── Textures/          # Texture assets
├── Sprites/           # 2D sprite assets
├── Plugins/           # Third-party plugins and integrations
│
└── External/          # Third-party asset packages
    ├── Water FX Pack/     # Water effects package
    ├── Standard Assets/   # Unity standard assets
    ├── Free_Rocks/        # Rock assets
    └── meadow_assets/     # Environment assets
```

## Documentation
For detailed documentation on specific systems and features, see the [Documentation](./Documentation/) folder:

- **[Architecture](./Documentation/Architecture.md)** - Game architecture, manager system, and component communication
- **[Pickup System](./Documentation/PickupSystem.md)** - Unified pickup system supporting Collect mode, Push mode, and Life pickups with respawn
- **[Timer System](./Documentation/TimerSystem.md)** - Centralized timer system documentation
- *More documentation will be added here as the project grows*

## Development
- Built with Unity
- Uses Angular commit conventions for version control
- Follows clean code principles and SOLID design patterns

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes using Angular commit conventions
4. Push to the branch
5. Create a Pull Request

## License
MIT