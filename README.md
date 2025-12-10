# Brain not Braining

A Unity puzzle game about a laboratory mouse gradually awakening consciousness by unlocking different brain functions.

## Project Information

**Course:** EVI: Entretenimiento y Videojuegos
**Institution:** Universitat Politècnica de València (UPV)
**Semester:** Autumn 2025
**Unity Version:** 6000.2.6f2
**Template:** 3D URP (Universal Render Pipeline)

## Game Overview

Brain not Braining uses the metaphor of cognitive and sensory awakening as both narrative and gameplay mechanic. Players experience the gradual assembly of consciousness through progressive unlocking of brain regions, each introducing new abilities and mechanics.

**Estimated Playtime:** 20-30 minutes

### Progression System

The game consists of 5 levels across different brain regions:

- **Level 0: Brainstem** - Reflexes and button sequences
- **Level 1: Motor Cortex** - Movement controls (WASD)
- **Level 2: Somatosensory Cortex** - Touch and spatial awareness
- **Level 3: Visual Cortex** - Sight (black & white to color, depth perception)
- **Level 4: Prefrontal Cortex** - Reasoning and memory puzzles

### Core Mechanics

**Visual Evolution:** Darkness/void → Black & white → Grayscale → Partial color → Full vibrant color
**Audio Evolution:** Heartbeat only → Footsteps → Ambient sounds → Environment sounds → Rich layered soundtrack
**Mechanic Evolution:** No input → Button prompts → Movement → Spatial awareness → Vision → Reasoning

## Development Team

- **Sander Hauge** - Lead Designer/Programmer
- **Giulio Zemignani** - Lead Programmer
- **Christopher Auer** - QA/Programmer

## Getting Started

### Prerequisites

- Unity 6000.2.6f2 or later
- Git LFS (for large assets)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/haugeSander/brain-not-braining.git
   cd brain-not-braining
   ```

2. Ensure Git LFS is installed and initialized:
   ```bash
   git lfs install
   git lfs pull
   ```

3. Open the project in Unity Hub
4. Load the MainMenu scene from `Assets/Scenes/Core/MainMenu.unity`

## Project Structure

```
Assets/
├── Audio/          # Sound effects and music
├── Materials/      # Materials organized by level
├── Prefabs/        # Reusable game objects
├── Scenes/         # Game scenes
│   ├── Core/       # MainMenu, LevelFinished
│   └── Levels/     # Individual level scenes
├── Scripts/        # C# scripts
│   ├── Core/       # Managers and core systems
│   ├── Gameplay/   # Puzzle and game logic
│   ├── Player/     # Player controls
│   └── UI/         # User interface
└── Settings/       # URP and rendering settings
```

## License

This project is developed for educational purposes as part of the EVI course at UPV.
