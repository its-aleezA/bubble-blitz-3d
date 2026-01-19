# 🎯 **Bubble Blitz 3D**  

A physics-based bubble shooter game built in Unity with **dynamic color matching, grid-based gameplay, and glowing visual effects**. Features two challenging levels, real-time scoring, and responsive controls optimized for an engaging player experience.

---

## 🎮 **Features**

**Core Gameplay**:
- ✅ **Dynamic Color Matching** – Match 3+ bubbles of the same color to pop them
- ✅ **Grid-Based Layout** – Organized bubble rows with randomized colors
- ✅ **Two Challenging Levels** – Level 2 introduces more colors and complexity
- ✅ **Real-Time Timer** – Beat the clock before time runs out
- ✅ **Physics-Based Shooting** – Realistic bubble trajectory and collisions

**Visual Polish**:
- ✅ **Glowing Bubble Materials** – Custom transparent shaders with emission effects
- ✅ **Aim Prediction Line** – Visual trajectory guide for precise shots
- ✅ **Particle Effects** – Satisfying pop animations and visual feedback
- ✅ **Clean UI Design** – Real-time score, timer, and level display

**Technical Implementation**:
- ✅ **Object-Oriented C# Scripts** – Modular and well-documented code
- ✅ **Efficient Collision Detection** – Optimized bubble matching algorithms
- ✅ **Scene Management** – Smooth transitions between levels and menus
- ✅ **Input Handling** – Keyboard and mouse controls for accessibility

---

## 🚀 **Quick Start**

### **1. Prerequisites**
- Unity 2021.3+ (3D Core template)
- Basic C# knowledge (for customization)
- Blender 3.0+ (for custom bubble models)

### **2. Installation**
```bash
# Clone the repository
git clone https://github.com/its-aleezA/bubble-blitz-3d.git

# Open in Unity Hub
Open Unity Hub → Add Project → Select the cloned folder
```

### **3. Basic Setup**
1. **Open the `Level1` scene** (`Assets/Scenes/Level1.unity`)
2. **Configure GameManager**:
   - Drag UI elements to their slots
   - Set bubble colors in the Inspector
   - Assign the bubble prefab
3. **Press Play** to test immediately

### **4. Controls**
- **A/D or Left/Right Arrow** – Rotate shooter
- **Spacebar** – Shoot bubble
- **Mouse Right-Click** – Alternative rotation
- **ESC** – Pause menu

---

## 📊 **Game Mechanics**

### **Scoring System**
| Action | Points | Description |
|--------|--------|-------------|
| 3-Bubble Match | 300 | Base match |
| 4-Bubble Match | 500 | Bonus chain |
| 5+ Bubble Match | 800 | Combo bonus |
| Level Completion | 1000 | Time bonus |

### **Level Design**
| Level | Bubbles | Colors | Time Limit | Difficulty |
|-------|---------|--------|------------|------------|
| Level 1 | 40 | 3 | 90s | Beginner |
| Level 2 | 56 | 5 | 75s | Advanced |

---

## 🗂️ **Project Structure**

```
BubbleBlitz3D/
├── Assets/
│   ├── Scripts/
│   │   ├── GameManager.cs          # Main game controller
│   │   ├── Bubble.cs              # Bubble behavior & matching
│   │   ├── ShooterController.cs   # Player controls & shooting
│   │   ├── CameraController.cs    # Smooth camera follow
│   │   └── UIManager.cs           # UI updates & animations
│   ├── Materials/
│   │   ├── BubbleRed.mat          # Red bubble with emission
│   │   ├── BubbleBlue.mat         # Blue bubble with emission
│   │   └── ... (5 colors total)
│   ├── Prefabs/
│   │   ├── Bubble.prefab          # Main bubble prefab
│   │   └── BubbleExplosion.prefab # Particle effect
│   ├── Scenes/
│   │   ├── MainMenu.unity         # Start screen
│   │   ├── Level1.unity           # First level
│   │   └── Level2.unity           # Second level
│   └── UI/
│       ├── Fonts/
│       ├── Sprites/
│       └── Prefabs/
├── Documentation/
│   ├── GameDesignDocument.md
│   └── TechnicalImplementation.md
└── README.md
```

---

## 🛠️ **Customization Guide**

### **Adding New Bubble Colors**
1. Create a new material in `Assets/Materials/`
2. Set properties:
   - Shader: Standard
   - Rendering Mode: Transparent
   - Emission: Enabled (set intensity)
3. Add to `GameManager.colors` array
4. Update `Bubble.cs` color indexing

### **Modifying Level Design**
```csharp
// In GameManager.cs, modify:
int rows = 6;  // Increase row count
int cols = 10; // Increase column count
float spacing = 1.0f; // Adjust bubble spacing
```

### **Creating Power-Ups**
1. Create new prefab with special effects
2. Add `PowerUp.cs` script:
   ```csharp
   public class PowerUp : MonoBehaviour
   {
       public enum Type { Rainbow, Bomb, MultiShot }
       public Type powerUpType;
       // ... implementation
   }
   ```
3. Integrate with `GameManager` spawning logic

---

## 📈 **Performance Optimization**

### **Best Practices Implemented**:
- **Object Pooling** for bubble instantiation
- **Cached References** to avoid repeated `GetComponent<>()` calls
- **Coroutines** for delayed actions instead of `Invoke()`
- **Layer-based Collision Matrix** to optimize physics
- **Material Instancing** to reduce draw calls

### **For Large Grids**:
```csharp
// Enable for 100+ bubbles
void OptimizeForLargeGrids()
{
    // Use static batching
    StaticBatchingUtility.Combine(gameObject);
    
    // Reduce physics updates
    Physics.autoSimulation = false;
    Physics.Simulate(Time.fixedDeltaTime);
}
```

---

## 🐛 **Troubleshooting**

| Issue | Solution |
|-------|----------|
| **Bubbles not matching colors** | Check `Bubble.colorIndex` assignment in `GameManager` |
| **Shooter not rotating** | Verify Input Manager settings (Edit → Project Settings → Input) |
| **Materials not glowing** | Enable Emission in Scene view (Effects → Emission) |
| **Game Over triggers early** | Check `ballsRemaining` initialization in `GameManager.Start()` |
| **UI buttons not working** | Verify OnClick() events are connected to `GameManager` functions |

---

## 🎨 **Visual Customization**

### **Shader Effects**:
The game uses custom Standard shader modifications:
```shader
// Bubble glow effect
_EmissionColor = BaseColor * GlowIntensity;
_Mode = 3; // Transparent
_Glossiness = 0.8; // Shiny surface
```

### **Particle Systems**:
- **Match Explosion**: Radial burst with color matching
- **Bubble Trail**: Gradient trail following shot bubbles
- **UI Effects**: Score pop-ups and level transitions

---

## 📚 **Learning Outcomes**

This project demonstrates:
- **Unity Physics System** – Rigidbody dynamics and collision detection
- **Material Programming** – Custom shaders and visual effects
- **Game Architecture** – MVC pattern with GameManager coordination
- **UI/UX Design** – Intuitive interfaces and feedback systems
- **Algorithm Design** – Cluster detection for bubble matching

---

## 📸 Demo

![Demo GIF](demo.gif)

---

## 👥 **Contributors**

- [Aleeza Rizwan](https://github.com/its-aleezA)

---

## 📜 **License**

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

**Academic Use**: This project was developed for **Computer Graphics (EC-301)** at the National University of Sciences and Technology. Please cite appropriately if used for educational purposes.

---

> [!Tip]
> For the highest score, aim for chain reactions by targeting bubbles that connect multiple same-color groups!

---

**Happy Bubble Popping!** 🎮✨

*For issues or feature requests, please open an issue on the GitHub repository.*
