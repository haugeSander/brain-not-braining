# Brain not Braining

**A consciousness-awakening puzzle game where a laboratory mouse gradually unlocks different brain functions to escape.**

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.2.6f2-black?style=flat&logo=unity" alt="Unity Version">
  <img src="https://img.shields.io/badge/Template-3D%20URP-blue" alt="Template">
  <img src="https://img.shields.io/badge/Status-Early%20Beta-orange" alt="Status">
  <img src="https://img.shields.io/badge/License-Educational-green" alt="License">
</p>

---

## 📖 Table of Contents
- [About the Game](#-about-the-game)
- [Key Features](#-key-features)
- [Gameplay & Progression](#-gameplay--progression)
- [Development Team](#-development-team)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [Controls](#-controls)
- [Technical Details](#-technical-details)
- [Development Progress](#-development-progress)
- [Contributing](#-contributing)
- [License](#-license)

---

## About the Game

Brain not Braining explores consciousness through progressive gameplay mechanics. Players experience the gradual assembly of awareness as a laboratory mouse awakening different brain regions, each introducing new cognitive abilities and sensory perceptions.

**Game Type:** 3D Puzzle Platformer  
**Playtime:** 20-30 minutes  
**Theme:** Neuroscience meets existential awakening  
**Target Platform:** PC (Windows/Mac/Linux)

### The Core Concept

The game uses a unique metaphor: cognitive and sensory awakening drives both narrative and gameplay. As you unlock each brain region, you literally gain new ways to perceive and interact with the world. What starts in complete darkness with only reflexes gradually evolves into full consciousness with vision, movement, spatial awareness, and reasoning.

---

## Key Features

- **Progressive Sensory Evolution**: Experience the world transforming from void → grayscale → partial color → full vibrant reality
- **Dynamic Audio Layering**: Soundscape evolves from heartbeat only → footsteps → ambient sounds → rich environmental audio
- **Neuroscience-Inspired Puzzles**: Each level corresponds to an actual brain region with thematically appropriate mechanics
- **Adaptive Difficulty**: Puzzles increase in complexity as more cognitive functions unlock
- **Immersive Visual Effects**: Edge detection, color grading, particle systems, and shader effects simulate emerging perception
- **Accessibility Options**: Adjustable graphics settings, volume controls, and gameplay assists

---

## Gameplay & Progression

### Level Overview

| Level | Brain Region | Core Mechanic | Description |
|-------|-------------|---------------|-------------|
| **0** | Brainstem | Reflexes | Timed button sequences to stabilize heart rate and breathing (500 bpm target) |
| **1** | Motor Cortex | Movement (WASD) | Navigate a pitch-black maze using only collision feedback (red flashes) |
| **2** | Somatosensory Cortex | Touch & Spatial Awareness | Edge-detected vision with temperature cues through particles and pain indicators |
| **3** | Visual Cortex (Stage 1) | Black & White Vision | Navigate using monochrome sight and collect "carrots" to improve eyesight |
| **4** | Visual Cortex (Stage 2) | Color & Depth Perception | Full color vision unlocked, with depth perception through shadows |
| **5** | Prefrontal Cortex | Memory & Reasoning | Navigate a grid by memorizing tile patterns before they disappear |
| **6** | Full Brain Integration | All Abilities Combined | Escape the laboratory using all unlocked cognitive functions |

### Evolution Systems

**Visual Progression:**
```
Darkness/Void → Black & White → Grayscale → Partial Color → Full Vibrant Color
```

**Audio Progression:**
```
Heartbeat Only → Footsteps → Ambient Sounds → Environment Sounds → Rich Layered Soundtrack
```

**Mechanic Progression:**
```
No Input → Button Prompts → Movement → Spatial Awareness → Vision → Reasoning
```

### Special Mechanics

- **Puzzle 3**: Press **E** to send a sonar to see edges in the area. This uses Sobel edge detection.
- **Puzzle 4**: Hold **E** to toggle between edge-detected and normal vision while avoiding an enemy that follows when seen (uses NavMesh). Puzzles goal is to collect carrots to progressively unlock proper eyesight.
- **Puzzle 6**: Memorize a 3-second path pattern before tiles disappear
- **Puzzle 7**: Complete challenges within a strict time limit

---

## Development Team

| Member | Role | Key Contributions |
|--------|------|-------------------|
| **Sander Hauge** | Lead Designer & Programmer | Puzzles 0, 3, 4; Movement system; UI/UX (menus, settings); Level progression; Graphics management; Intro cutscene; Ending cutscene; Debug tools |
| **Giulio Francesco Zemignani** | Lead Programmer | Puzzle 8; Mouse model & animation; Minimap improvements; Bug fixes & testing |
| **Christopher Auer** | QA & Programmer | Puzzles 1, 6, 7; Materials & visual polish; Demo video creation; Testing |

**Course:** EVI: Entretenimiento y Videojuegos  
**Institution:** Universitat Politècnica de València (UPV)  
**Semester:** Autumn 2025

---

## Getting Started

### Prerequisites

- **Unity Hub** (latest version)
- **Unity Editor 6000.2.6f2** or later
- **Git** with **Git LFS** support
- Operating System: Windows 10/11, macOS 10.15+, or Linux

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/haugeSander/brain-not-braining.git
   cd brain-not-braining
   ```

2. **Initialize Git LFS:**
   ```bash
   git lfs install
   git lfs pull
   ```

3. **Open in Unity:**
   - Launch Unity Hub
   - Click "Open" and select the project folder
   - Wait for Unity to import all assets (first import may take several minutes)

4. **Load the Main Menu:**
   - Navigate to `Assets/Scenes/Core/MainMenu.unity`
   - Press the Play button to test

### First Time Setup

The game will automatically create necessary folders and configuration files on first launch. If you encounter missing references, reimport all assets via `Assets > Reimport All`.

---

## Controls

### Keyboard & Mouse

| Action | Key/Button | Description |
|--------|-----------|-------------|
| Movement | WASD | Move the mouse character |
| Jump | Space | Jump |
| Ability | E | Start Echolocaition and Toggle vision modes (Puzzle 3 and 4) |
| Interact | F | Toggle vision modes (Puzzle 3 and 4) |
| Pause | Escape | Open pause menu |
| Graphics Debug | G | Open graphics comparison screen |
| Cheat Mode | F | Enable debugging shortcuts (if cheats enabled). Final puzzle uses T instead |
| Skip Cutscene | S | Skip intro and transition cutscenes |

### Puzzle-Specific Controls

- **Puzzle 0**: Click timed circles, press Space for breathing control
- **Puzzle 6**: WASD to navigate tiles, memorize the lit path

---

## Project Structure

```
Assets/
├── Audio/                      # Sound effects and music
│   ├── Music/                  # Background music tracks
│   ├── SFX/                    # Sound effects
│   └── Mixers/                 # Audio mixer configurations
├── Materials/                  # Materials organized by level
│   ├── Level0_Brainstem/
│   ├── Level1_MotorCortex/
│   └── ...
├── Models/                     # 3D models and animations
│   └── Mouse/                  # Mouse character model & animations
├── Particles/                  # Visual effect systems
│   ├── BrainActivation/        # Brain unlocking effects
│   └── Environmental/          # Temperature, pain indicators
├── Prefabs/                    # Reusable game objects
│   ├── Player/                 # Player controller prefabs
│   ├── Puzzles/                # Puzzle element prefabs
│   └── UI/                     # UI element prefabs
├── Scenes/                     # Game scenes
│   ├── Core/                   # MainMenu, LevelFinished, IntroScene
│   └── Levels/                 # Individual level scenes (0-8)
├── Scripts/                    # C# scripts
│   ├── Core/                   # Managers (GameManager, AudioManager, etc.)
│   ├── Gameplay/               # Puzzle logic and game mechanics
│   ├── Player/                 # Player movement and controls
│   ├── UI/                     # Menu systems and HUD
│   └── Utilities/              # Helper scripts and extensions
├── Shaders/                    # Custom shaders
│   ├── EdgeDetection/          # Sobel edge detection
│   └── ColorGrading/           # Visual progression effects
├── Settings/                   # URP and rendering settings
│   ├── URP-Settings.asset
│   └── Quality/                # Quality presets
└── Textures/                   # Texture assets
    ├── UI/                     # Interface textures
    └── Environment/            # World textures
```

---

## Technical Details

### Unity Configuration

- **Unity Version:** 6000.2.6f2
- **Rendering Pipeline:** Universal Render Pipeline (URP)
- **Project Template:** 3D URP
- **Asset Serialization:** Force Text (for version control)
- **API Compatibility:** .NET Standard 2.1

### Key Systems

1. **Visual Evolution System**
   - Progressive color desaturation/saturation
   - Edge detection (Sobel filter) for somatosensory puzzles
   - Dynamic depth-of-field and color grading
   - Particle effects for brain activation

2. **Audio System**
   - Volume mixers for Music, SFX, and Master channels
   - Layered ambient soundscapes
   - Dynamic audio transitions between brain states

3. **AI & Navigation**
   - NavMesh-based enemy AI (Puzzle 3)
   - Pathfinding for chase sequences
   - Minimap with camera-relative positioning

4. **Settings Management**
   - Graphics quality presets
   - Volume control (Music, SFX, Master)
   - Gameplay assists (cheat mode for debugging)
   - Resume from specific levels

5. **Debug Tools**
   - **G Key**: Graphics comparison screen
   - **F Key**: Cheat mode (when enabled in main menu)
     - Puzzle 0: Add 100+ HR instantly
     - Puzzle 3: Teleport player to goal
     - Puzzle 4: Add collectibles instantly

### Post-Processing Stack

- Bloom
- Color Adjustments (Saturation, Contrast)
- Vignette
- Depth of Field
- Film Grain (for low-perception states)

---

## 📊 Development Progress

### Current Status: **Early Beta**

The core gameplay loop is complete with all 8 puzzles implemented and functional. Current focus is on polishing, difficulty tuning, and bug fixing.

### Completed Features

#### Core Systems
- Main menu with play, Play from specifc level, settings, credits, exit
- Settings menu (audio, graphics, gameplay)
- Pause menu with resume/restart/quit
- Level progression and scene management
- Save/load system for level unlocking
- Introductory cutscene with skip functionality
- Brain activation cutscenes between levels

#### Puzzle Implementations
- **Puzzle 0** (Brainstem): Heart rate and breathing mini-games
- **Puzzle 1** (Motor Cortex): Dark maze navigation
- **Puzzle 2** (Somatosensory): Edge-detected spatial awareness
- **Puzzle 3** (Visual Stage 1): Vision toggle with enemy AI and collectible-based sight improvement
- **Puzzle 4** (Visual Stage 2): Pushing of colored spheres into a hole
- **Puzzle 5** (Prefrontal): Memory and reasoning puzzle
- **Puzzle 6** (Final Challenge): Time-limited escape

#### Visual & Audio
- Progressive visual evolution system
- Layered audio system with mixers
- Mouse character model with animations
- Particle effects for brain activation and environmental cues
- Edge detection shader
- Color grading system
- Minimap implementation

#### Quality of Life
- Tutorial prompts for each puzzle
- Debug tools (graphics preview, cheat codes)
- Improved brain unlocking visuals
- Settings persistence

---

## Contributing

This is an educational project for the EVI course at UPV. While we're not accepting external contributions, feedback and suggestions are welcome!

### Reporting Issues

If you encounter bugs during testing:
1. Note the puzzle/level where it occurred
2. Describe steps to reproduce
3. Include screenshots/videos if applicable
4. Report to the team lead (Sander)

---

## License

This project is developed for educational purposes as part of the **EVI: Entretenimiento y Videojuegos** course at **Universitat Politècnica de València (UPV)**.

**Academic Year:** 2025-2026  
**Semester:** Autumn 2025

All rights reserved. Not for commercial use or redistribution without permission from the development team and UPV.

---

## Acknowledgments

- UPV Faculty for guidance and support
- Friends and playtesters for valuable feedback
- Unity Technologies for the game engine and documentation
- The neuroscience community for inspiration on brain function visualization

---

<p align="center">
  <i>"At first there was nothing, then consciousness began."</i>
</p>

<p align="center">
  Made with 🧠 by the Brain not Braining Team
</p>