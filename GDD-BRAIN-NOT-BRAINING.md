EVI \- GDD

Sander Hauge, Giulio Zemignani, Christopher Auer

October 2025

![][image1]  
**Brain not Braining**

## 

**[1 Presentation	2](#1-presentation)**

[**2 Planning	3**](#2-planning)

[3.1 Market Positioning	6](#3.1-market-positioning)

[**4 General	8**](#4-general)

[**5 Gameplay	9**](#5-gameplay)

[5.1 Unlocks	9](#heading=h.vj76m8iblp5n)

[5.2 Levels	9](#5.1-levels-&-unlocks)

[Level Group 0 – Brainstem Awakening	10](#level-group-0-–-brainstem-awakening)

[Level Group 1 – Motor Cortex	10](#level-group-1-–-motor-cortex)

[Level Group 2 – Somatosensory Cortex	10](#level-group-2-–-somatosensory-cortex)

[Level Group 3 – Visual Cortex	11](#level-group-3-–-visual-cortex)

[Level Group 4 – Prefrontal Cortex	11](#level-group-4-–-prefrontal-cortex)

[**6 Story / Lore	12**](#6-story-/-lore)

[Prologue	12](#prologue)

[Narrative Arc	12](#narrative-arc)

[Epilogue	12](#epilogue)

[**8 Characters	13**](#8-characters)

[9.1 Core Actions	14](#9.1-core-actions)

[**10 Interaction	16**](#10-interaction)

[**11\. Other Considerations	16**](#11.-other-considerations)

## 

## **1 Presentation** {#1-presentation}

**Title:** *Brain not braining*

**Concept:** A short, atmospheric puzzle game where the player gradually awakens consciousness by unlocking different parts of a mouse brain. Each newly activated brain function introduces a distinct mechanic and puzzle type, reflecting the growing complexity of awareness.

**Progression**: The journey begins in absolute darkness: no movement, no sense of space or sound. As the player completes challenges, fragments of perception emerge: movement, touch, sight, and finally reasoning. The experience evolves from pure instinct to deliberate thought, mirroring the assembly of a conscious mind.

**Concept sketches:**

A *light prism* captures the feeling of the game. At first, there is only a dim, colorless glow, or total absence of light. Passing through challenges (the prism), light separates into color, symbolizing the awakening of new senses and ideas. The transformation from a single dull beam into a vibrant spectrum represents the shift from nothingness to consciousness.

The *visual progression* of the game follows this same principle. In the top row of the appended images, the world’s atmosphere shifts from empty and monochrome to richly detailed and colored. In the bottom row, corresponding regions of the brain activate one by one, illuminating the path from primitive reflex to full cognition.

![][image2]![][image3]![][image4]

![][image5]![][image6]![][image7]

## **2 Planning** {#2-planning}

**Team:** Group of 3 developers (learning-focused project). 

| Name | Role | Primary Responsibilities |
| :---- | :---- | :---- |
| Sander Hauge | Lead designer / Programmer | Core systems, visual evolution & integration |
| Giulio Zemignani | Lead programmer | Scripting & sound layering |
| Christopher Auer | QA / Programmer | Gameplay logic, UI, testing & polish |

**Scope:**  
8 puzzles, 3 unlocks

**Timeline:**

* **Week 1–2:** Planning, scoping and learning

* **Week 3-4:** Start implementation, Prototype core movement \+ block pushing

* **Week 5-7:** Implement unlock system \+ visuals/audio feedback

* **Week 8-10:** Puzzle design \+ iteration

* **Week 11:** Polish (UI, audio layers)

* **Week 12:** Testing \+ delivery

Gantt chart found below, changes could be made to it while development. Each member of the group is represented by a color and in the chart we can see which macro task is assigned to which participant. We add one bonus colour representing the group for coordinated effort on a specific task.

![][image8]

**3 Competitive Analysis**

*Brain not Braining* draws conceptual energy from games that explore perception, limitation, and gradual discovery. Titles like *Limbo* and *Inside* demonstrated how atmosphere and subtle feedback can replace dialogue and still communicate deep narrative. Their focus on minimalism and environmental storytelling directly shaped our intention to let lighting, sound, and pacing tell the story of awakening, rather than exposition or cutscenes.

![][image9]![][image10]

We also looked to *The Unfinished Swan* and *Blind VR* for how they handle sensory absence. Both build player curiosity by forcing the mind to fill in blanks before revealing the full picture. That gradual clarity aligns with our structure of unlocking senses one by one. However, where those titles frame the experience around sight alone, *Brain not Braining* extends the metaphor to multiple cognitive functions, movement, touch, and reasoning, treating each as a stage in the formation of consciousness.

![][image11]![][image12]

*OSU\!* plays a smaller but notable role in our inspiration. Its rhythm-based timing and visual feedback loops influenced how we think about sensory engagement and reward. The sense of flow it achieves, tight coupling between perception, reaction, and feedback, inspired the design of our early “reflex” puzzles, where rhythm and timing are stand-ins for instinctive neural responses.

![][image13]

In essence, *Brain not Braining* shares the minimalist DNA of these works but narrows the scope to a single, scientific-poetic premise: a mouse brain reassembling itself. Where others lean on story or sensory gimmicks, we use mechanics as anatomy, each system a lobe, each puzzle a neuron firing. The differentiation lies in this unity of concept and design: everything the player does is literally an act of becoming conscious.

| Reference | Relation / Influence |
| :---- | :---- |
| **Limbo** (Playdead, 2010\) | Minimalist art, emotional progression without dialogue |
| **Blind VR** (Tiny Bull Studios, 2018\) | Use of sensory limitation as gameplay mechanic |
| **OSU\!** (Dean Herbert, 2007\) | Rhythm-based pattern recognition and reaction flow |

### **3.1 Market Positioning** {#3.1-market-positioning}

*Brain not Braining* is positioned for players who appreciate small, concept-driven puzzle experiences. The typical audience overlaps with fans of titles like *The Unfinished Swan*, and *INSIDE*: people who enjoy discovering systems by intuition and atmosphere rather than instruction. These players are comfortable with slower pacing and seek emotional or intellectual reward rather than competition.

The game fits neatly into the **indie experimental puzzle** space but takes a distinctive path within it. Most games in this niche explore perception, abstraction, or emotion; *Brain not Braining* focuses on *cognition itself*. Every new mechanic isn’t just a gameplay variation: it represents a neural function being restored. The design makes the player *feel* the act of thinking, turning problem solving into both narrative and mechanic at once.

This approach gives the project an uncommon identity. Despite research and comparison across the genre, we haven’t found a game that maps real brain regions directly to player abilities or progression systems. Others have explored memory, blindness, or emotion, but not consciousness as a mechanical structure. That absence in the market allows *Brain not Braining* to stand out immediately, even in small-scale showcases or festivals that favor strong conceptual cohesion over production scale.

The result is a game that markets itself through its premise: short, minimal, and conceptually tight. It’s easy to explain in a single line: *a puzzle game about a mouse brain waking up*. Yet it's rich enough to sustain curiosity. The simplicity of its presentation keeps production feasible for a student team, while its philosophical framing makes it appealing to audiences and juries looking for originality and meaning in interactive design.

## 

## **4 General** {#4-general}

**Genre:** Puzzle game

**Platform:** PC (Windows/Mac)

**Target Audience:** Players of indie puzzle games (15+) and neuroscience students

**Target mood:** Experimental puzzle flow, and progressive mood, from tame to vivid

**PEGI:** 3+ (no violence, abstract visuals)

**What makes it new/different?**

*Brain not Braining* connects cognitive science to gameplay. Each ability the player unlocks corresponds to a real brain function (motor control, vision, reasoning). Instead of treating mechanics as isolated systems, the game treats them as *parts of a mind*. This mechanical metaphor, using gameplay to mirror consciousness, is what gives the project its identity. The narrative of awakening is told entirely through the evolution of play and perception.

**Why is it exciting?**

The experience blends the satisfaction of puzzle solving with the gradual sensory reward of becoming aware. Every solved puzzle doesn’t just advance progress, it *feels* like thinking, sensing, or understanding for the first time. That moment-to-moment feedback, paired with the visual growth of the brain HUD, creates a clear emotional arc within a small, focused project.

## **5 Gameplay** {#5-gameplay}

### **5.1 Levels & Unlocks** {#5.1-levels-&-unlocks}

Overview:

---

| LVL | Group | Brain Function | Description | Puzzles |
| :---- | :---- | :---- | :---- | :---- |
| **0** | **Brainstem Awakening** | Reflex | Stimulus-response; initiate heartbeat and breathing | 1 |
| **1** | **Motor Cortex** | Movement | Learn directional control through trial and error | 1 |
| **2** | **Somatosensory Cortex** | Touch, temperature, pain | Navigate mazes using sensory cues | 1 |
| **3\.**  | **Visual Cortex** | Sight | Unlock visibility, block pushing, color puzzles | 3 |
| **4** | **Prefrontal Cortex** | Reasoning | Spatial memory, pattern recognition, logic | 2 |

---

### **Level Group 0 – Brainstem Awakening** {#level-group-0-–-brainstem-awakening}

Basic survival reflexes — a sequence of early life moments (1 puzzle).

* **Puzzle 1:** Start by pushing buttons in quick succession  to achieve heart rate and breathing. Simple stimulus–response patterns (if light appears, move towards it).

**Narrative:**  
“First, I must breathe... I must survive...”

This early life moments are necessary to step further in the process of gaining consciousness

![][image14]

Sketches for the initial puzzle

---

### **Level Group 1 – Motor Cortex** {#level-group-1-–-motor-cortex}

![][image15]

Concept art for puzzle 2

Learn movement (1 puzzle).

* **Puzzle 2:** Forward-only movement using **W**, with sound/text/lighting feedback upon collision. Then introduce the full movement (**A**, **S**, **D**).

---

### **Level Group 2 – Somatosensory Cortex** {#level-group-2-–-somatosensory-cortex}

![][image16]

Concept art of object detection with vibrations

Touch and spatial awareness (1 puzzle)

* **Puzzle 3:** Navigate mazes using touch cues, feeling walls before collision by projecting a shockwave on the screen.

---

### **Level Group 3 – Visual Cortex** {#level-group-3-–-visual-cortex}

![][image17]![][image18]

![][image19]

Concept art of puzzle 4-5-6 showing the transition from a 2D black\&white vision to 3D color vision

Starting to see the environment — first black and white, then color (3 small puzzles)

* **Puzzle 4:** Introduce block pushing and turning switches  
* **Puzzle 5:** Color is introduced after in for example, match two colored boxes,   
* **Puzzle 6:** Depth perception in the final visual puzzle.

---

### **Level Group 4 – Prefrontal Cortex** {#level-group-4-–-prefrontal-cortex}

![][image20]![][image21]

Concept art of puzzle 7 and 8

Spatial memory and navigation: Introduce sequences and multi-step puzzles (2 puzzles).

* **Puzzle 7:** Remember and repeat a path sequence  
* **Puzzle 8:** Navigate using landmarks after brief viewing, build a mental map; revisit previous areas with new knowledge

**Narrative:**  
 “I remember this place... I can learn...”

---

**Final Puzzle:**  
Combines all systems: survival instincts, movement, touch, vision, and spatial memory.

![][image22]

Concept art of the ending

**Ending Concept:** Instead of just seeing the cage, the mouse realizes it can remember and plan — showing true consciousness. The mouse can move in a 3D world and needs to escape the lab in an action platform video game style.

## 

## **6 Story / Lore** {#6-story-/-lore}

#### **Prologue** {#prologue}

Before consciousness, there is only darkness. The player begins with no movement, no sound, no visible world: a state of complete void. The first puzzle introduces a faint pulse, symbolizing the spark of life and the first primitive neural firing.

![][image23]![][image24]

#### **Narrative Arc** {#narrative-arc}

Through successive puzzles, the player reconstructs the mouse’s brain step by step, mirroring the early development of awareness. Each unlock corresponds to the activation of a new brain region: reflexes, movement, perception, and reasoning. As the player solves puzzles, the world grows more defined, light sharpens, sounds layer in, and space itself becomes clearer. The narrative remains environmental, told through sensory evolution rather than text or dialogue.

The structure loosely references **Jean Pierre Flourens’ (1794–1867)** research on animal brain function. Flourens’ experiments revealed that different brain regions control distinct abilities, movement, perception, coordination, and that damage to the brainstem leads to loss of life. *Brain not Braining* reinterprets this historically grim material through a symbolic lens: instead of destruction, the player rebuilds what Flourens studied by breaking. It becomes a quiet reversal of his process, an act of reconstruction and empathy through play.

#### **Epilogue** {#epilogue}

By the end, the mouse achieves full self-awareness. The final scene reveals its environment: a laboratory cage, instruments, faint human silhouettes. The camera slowly pans back as the brain diagram on the HUD glows completely white. There’s no explicit escape, only realization. The true “freedom” is consciousness itself, not the physical act of leaving the cage.

## 

## **8 Characters** {#8-characters}

**Protagonist: The Mouse Consciousness**

The player embodies the mind of a lab mouse regaining awareness. Initially represented as a single spark of light within a black void, this entity slowly takes form through progress. With each brain function restored, its representation evolves, from a single lone neuron to a faint mouse silhouette. Its “personality” isn’t expressed through words but through the tone of play: cautious at first, curious later, and finally determined. The protagonist isn’t heroic in a traditional sense; it’s a metaphor for life striving toward understanding.

**Antagonists: The Environment and the Scientists**

There are no direct enemies. The world itself provides resistance: puzzles act as cognitive barriers, and darkness behaves like ignorance. Environmental hazards, locked doors, fading pathways, sound-based mazes, represent the struggle of forming coherent thought. In the final sequence, distant shadows or silhouettes of scientists appear as abstract observers. They are not villains, but reminders of the mouse’s origin and purpose, the human gaze that created the experiment.

**Supporting Entities (optional inclusion)**

You could also mention *The Brain* itself as a subtle guiding force, perhaps small neuron nodes that light up as if beckoning the player forward. These serve as silent companions or mentors, reflecting internal motivation rather than external instruction.

**9 Game Elements**

### **9.1 Core Actions** {#9.1-core-actions}

* Move

* Push blocks

* Activate switches (step on or interact)

* Sequence / order-based puzzles

**Objects:**

|  Blocks (pushable) | ![][image25] |
| :---- | :---- |
|  Pressure plates / switches | ![][image26]![][image27] |
|  Doors / gates | ![][image28]![][image29] |
|  Connectable nodes (neurons)	 | ![][image30] |

**Environment:**  
First the world is completely black, signifying nothingness before life. Following this, there will be a menu-like experience revolving around neurons, and basic life functions. Unlocking more, the world will go from minimalistic and abstract geometry, to a vivid and complex world. Though this is ever-evolving, as while consciousness evolves the world evolves with it.

**Ending idea:**  
The mouse is part of a study on consciousness. As it awakens, the environment becomes more advanced, starting black and white, then gaining color but limited render distance. Finally, the player realizes the mouse is in a cage, observed by humans.

**Menu & Settings**:

![][image31]**![][image32]**

**HUD example**:

![][image33]

## 

## **10 Interaction** {#10-interaction}

**Camera:**  
Top-down or isometric, fixed view.

**HUD:**  
Minimal. Possibly show brain outline filling with light as progression.

**Controls:**

* **Arrow keys / WASD:** Move

* **Space / Enter:** Interact

* **Escape:** Pause menu

* **Mouse Right Click**: connecting dots

## **11\. Other Considerations** {#11.-other-considerations}

The game seems like an engaging educational title about mouse brain anatomy. The idea grew out of Sander’s master’s thesis, which uses morphing between rodent species to make anatomical differences more visible. Because the brain is complex and often abstract in textbooks, building a playable experience felt like a useful way to deepen understanding through interaction.

When comparing brains across species, neuroscientists search for homologous regions (areas that share similar cytoarchitecture, connectivity, and function) rather than assuming the same anatomical layout. Primary sensory areas (for example, primary somatosensory cortex S1 or primary visual cortex V1) can often be recognized across mammals, but their precise location, extent, and internal organization vary by species and typically require careful anatomical and functional mapping to identify correctly.

Beyond its educational goals, the project has narrative potential: the stepwise awakening of perception and cognition provides an intuitive, memorable frame for exploration. That combination: clear, research-informed mechanics plus a simple emotional arc, makes the game suitable as an introductory tool for students of neuroscience and biology.

**Other notes:**

* **Engine:** Unity  
* **Estimated Duration:** 20–30 minutes  
* **Team Objective:** Educational and creative exploration of consciousness  
* **Potential Future Expansion:** Optional post-project prototype, the “escape scene” expanded into a short 3D segment