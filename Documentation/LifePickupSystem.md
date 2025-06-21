# Life Pickup System

The Life Pickup System provides a +1 life pickup that players can collect to gain an additional life. This system integrates seamlessly with the existing LivesManager and uses the player's material for visual consistency.

## Overview

Life pickups are semi-transparent spheres that display "+1" text and provide visual feedback through rotation, bobbing, and pulsing effects. When collected, they automatically add a life to the player through the LivesManager system.

## Components

### LifePickup Script
- **Location**: `Assets/Scripts/Pickups/LifePickup.cs`
- **Purpose**: Handles the behavior and visual effects of life pickup objects
- **Key Features**:
  - Automatic player material detection and application
  - Rotating animation around Y-axis
  - Bobbing up and down motion
  - Pulsing transparency effect
  - +1 text display using TextMeshPro
  - Automatic life addition when collected
  - Integration with LivesManager

### Life Pickup Prefab
- **Location**: `Assets/Resources/Prefabs/Life Pickup.prefab`
- **Components**:
  - Sphere mesh (0.8 scale)
  - Trigger collider (0.6 radius) for player detection
  - Kinematic rigidbody (no gravity, no physics)
  - LifePickup script
  - Uses player material (applied at runtime)

## Usage

### Manual Placement
1. **Drag and Drop**: Drag the `Life Pickup.prefab` from `Assets/Resources/Prefabs/` into your scene
2. **Position**: Place it at the desired location in your level
3. **Automatic Setup**: The pickup will automatically:
   - Find and use the player's material
   - Start its animations
   - Be ready for collection

### Customization
Adjust these parameters in the LifePickup component inspector:

#### Animation Settings
- **Rotation Speed**: How fast the pickup rotates (default: 90°/s)
- **Bob Speed**: How fast the pickup bobs up and down (default: 2)
- **Bob Height**: How far the pickup bobs (default: 0.2)

#### Text Settings
- **Text Color**: Color of the +1 text (default: white)
- **Outline Color**: Color of the text outline (default: black)
- **Text Size**: Size of the +1 text (default: 0.5)

#### Visual Effects
- **Pulse Speed**: How fast the transparency pulses (default: 2)
- **Min Alpha**: Minimum transparency during pulse (default: 0.6)
- **Max Alpha**: Maximum transparency during pulse (default: 1.0)

## Integration

### Lives System Integration
The life pickup automatically integrates with the existing LivesManager:
- Calls `LivesManager.Instance.AddLife()` when collected
- Respects the maximum lives limit (won't add life if at max)
- Updates the lives display automatically
- Logs collection events for debugging

### Material System
- **Automatic Detection**: Finds the player object and uses its material
- **Visual Consistency**: Ensures pickups match the player's appearance
- **Transparency**: Applies pulsing transparency effect to the player material
- **Fallback**: Uses default material if player not found

## Technical Details

### Collection Detection
- **Trigger System**: Uses `OnTriggerEnter` with trigger collider
- **Player Tag**: Only responds to objects tagged as "Player"
- **Single Collection**: Prevents multiple collections with `isCollected` flag
- **Automatic Cleanup**: Deactivates pickup after collection

### Visual Effects
- **Rotation**: Continuous Y-axis rotation for visual appeal
- **Bobbing**: Sine wave motion in Y-axis for floating effect
- **Pulsing**: Alpha transparency animation for attention-grabbing effect
- **Text**: +1 text with outline, always facing camera direction

### Performance Considerations
- **Kinematic Rigidbody**: Efficient physics handling
- **Minimal Updates**: Only updates when active
- **Automatic Cleanup**: Removes from scene when collected
- **Material Sharing**: Uses existing player material (no new materials created)

## Testing and Debugging

### In Editor Testing
- **Reset Pickup**: Use the "Reset Pickup" context menu option to test collection multiple times
- **Console Logs**: Check console for debug messages during initialization and collection
- **Material Detection**: Verify player material is found and applied correctly

### In-Game Testing
- **Collection Verification**: Pickup disappears when collected
- **Lives Display**: Check lives display to confirm life was added
- **LivesManager Logs**: Verify LivesManager logs show life addition
- **Max Lives**: Test behavior when player is at maximum lives

### Debug Messages
The system provides comprehensive logging:
```
[LifePickup] Life pickup initialized
[LifePickup] Searching for player material...
[LifePickup] Found player: Player
[LifePickup] Found player material: PlayerMaterial, color: RGBA(1.000, 0.500, 0.200, 1.000)
[LifePickup] Life pickup collected!
[LifePickup] Life added to player
[LifePickup] Playing collection effect
```

## Best Practices

### Placement Guidelines
- **Visibility**: Place pickups in visible, accessible locations
- **Spacing**: Don't place too many pickups close together
- **Height**: Consider player jump height for placement
- **Theme**: Use pickups to reward exploration or difficult challenges

### Performance Guidelines
- **Limit Count**: Don't place excessive numbers of pickups
- **Cleanup**: Pickups automatically deactivate when collected
- **Material Efficiency**: Uses existing player material (no overhead)

### Design Guidelines
- **Consistency**: Pickups automatically match player appearance
- **Feedback**: Visual effects make pickups noticeable
- **Clarity**: +1 text clearly indicates the pickup's purpose

## Troubleshooting

### Common Issues

**Pickup not visible**
- Check if player material is being applied correctly
- Verify alpha values in the material
- Ensure pickup is not positioned underground

**Pickup not collectible**
- Verify trigger collider is enabled
- Check that player has "Player" tag
- Ensure pickup is active in hierarchy

**Life not added**
- Check if LivesManager exists in scene
- Verify player is not at maximum lives
- Check console for error messages

**Material not matching player**
- Ensure player has a Renderer component
- Verify player material is assigned
- Check console for material detection logs

### Debug Commands
- **Reset Pickup**: Right-click LifePickup component → Reset Pickup
- **Console Logs**: Check Unity console for detailed debug information
- **Inspector**: Verify all component settings in inspector

## Future Enhancements

Potential improvements for the life pickup system:
- **Particle Effects**: Add collection particle effects
- **Sound Effects**: Add pickup and collection sounds
- **Multiple Types**: Different pickup types (multiple lives, temporary effects)
- **Respawn System**: Pickups that respawn after time
- **Visual Feedback**: Enhanced effects when player is at max lives
- **Difficulty Scaling**: Pickup frequency based on game difficulty 