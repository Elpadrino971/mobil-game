# 🤝 Contributing to Prison Island Manager

Thank you for your interest in contributing! This document provides guidelines for contributing to the project.

## 📋 Table of Contents
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Coding Standards](#coding-standards)
- [Pull Request Process](#pull-request-process)
- [Bug Reports](#bug-reports)
- [Feature Requests](#feature-requests)

## 🚀 Getting Started

1. **Fork the repository**
2. **Clone your fork**
   ```bash
   git clone https://github.com/YOUR_USERNAME/mobil-game.git
   cd mobil-game
   ```
3. **Open in Unity 2022.3 LTS**
4. **Create a new branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

## 🛠️ Development Setup

### Prerequisites
- Unity 2022.3 LTS or later
- Visual Studio 2022 or Rider
- Git

### First-Time Setup
1. Open Unity Hub
2. Add the project
3. Install required packages (TextMeshPro, NavMesh)
4. Create a test scene
5. Run the game to verify

## 📝 Coding Standards

### C# Style Guide

#### Naming Conventions
```csharp
// Classes: PascalCase
public class GameManager { }

// Methods: PascalCase
public void UpdateGameState() { }

// Private fields: camelCase with _
private float _gameSpeed;

// Public fields: PascalCase
public int CurrentDay;

// Constants: UPPER_SNAKE_CASE
private const float MAX_SECURITY = 100f;

// Namespaces: PascalCase
namespace PrisonIsland.Core { }
```

#### Code Organization
```csharp
using UnityEngine;
using System.Collections.Generic;

namespace PrisonIsland.Core
{
    /// <summary>
    /// Brief description of the class
    /// </summary>
    public class ExampleClass : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Settings")]
        [SerializeField] private float speed = 10f;
        #endregion

        #region Private Fields
        private bool isActive;
        #endregion

        #region Unity Lifecycle
        private void Awake() { }
        private void Start() { }
        private void Update() { }
        #endregion

        #region Public Methods
        public void DoSomething() { }
        #endregion

        #region Private Methods
        private void HelperMethod() { }
        #endregion
    }
}
```

### Best Practices

1. **Use XML Documentation**
   ```csharp
   /// <summary>
   /// Spawns a new prisoner at the specified position
   /// </summary>
   /// <param name="position">World position to spawn</param>
   /// <returns>The spawned prisoner instance</returns>
   public Prisoner SpawnPrisoner(Vector3 position)
   ```

2. **Null Checks**
   ```csharp
   if (GameManager.Instance != null)
   {
       GameManager.Instance.DoSomething();
   }
   ```

3. **Event Subscriptions**
   ```csharp
   private void OnEnable()
   {
       GameEvents.Instance.OnNewDay += HandleNewDay;
   }

   private void OnDisable()
   {
       GameEvents.Instance.OnNewDay -= HandleNewDay;
   }
   ```

4. **Singleton Pattern**
   ```csharp
   public static YourClass Instance { get; private set; }

   private void Awake()
   {
       if (Instance == null)
       {
           Instance = this;
           DontDestroyOnLoad(gameObject);
       }
       else
       {
           Destroy(gameObject);
       }
   }
   ```

## 🔄 Pull Request Process

### Before Submitting
1. ✅ Test your changes thoroughly
2. ✅ Update documentation if needed
3. ✅ Follow coding standards
4. ✅ No compilation errors
5. ✅ Add comments to complex code

### PR Template
```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Performance improvement
- [ ] Documentation update

## Testing
Describe how you tested your changes

## Screenshots (if applicable)
Add screenshots/GIFs

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-reviewed
- [ ] Commented complex code
- [ ] Updated documentation
- [ ] No new warnings
- [ ] Tested in Unity Editor
- [ ] Tested on mobile (if applicable)
```

### Review Process
1. Submit PR with descriptive title
2. Wait for code review
3. Address feedback
4. Get approval
5. Merge!

## 🐛 Bug Reports

### Template
```markdown
**Describe the bug**
Clear description

**To Reproduce**
1. Go to '...'
2. Click on '....'
3. See error

**Expected behavior**
What should happen

**Screenshots**
If applicable

**Environment:**
 - Unity Version: [e.g. 2022.3.15f1]
 - Platform: [e.g. Android, iOS, Editor]
 - Device: [e.g. Pixel 6]

**Additional context**
Any other info
```

## 💡 Feature Requests

### Template
```markdown
**Is your feature request related to a problem?**
Description of problem

**Describe the solution**
How it should work

**Describe alternatives**
Other solutions considered

**Additional context**
Mockups, examples, etc.
```

## 📂 Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # Core game systems
│   ├── Data/           # Data classes
│   ├── Managers/       # Specialized managers
│   ├── Buildings/      # Building behavior
│   ├── Prisoners/      # Prisoner AI
│   ├── UI/             # UI components
│   ├── VFX/            # Visual effects
│   └── Utilities/      # Helper functions
├── Prefabs/            # Prefab assets
├── Materials/          # Materials
├── Scenes/             # Game scenes
└── Models/             # 3D models
```

## 🎯 Areas to Contribute

### High Priority
- [ ] More building types
- [ ] Staff management system
- [ ] Weather effects
- [ ] More random events
- [ ] Performance optimizations

### Medium Priority
- [ ] More achievements
- [ ] Better UI/UX
- [ ] More audio
- [ ] Localization
- [ ] Better tutorials

### Low Priority
- [ ] Cosmetic improvements
- [ ] Extra features
- [ ] Documentation improvements

## ❓ Questions?

- Open an issue for questions
- Join discussions
- Check existing issues first

## 📜 License

By contributing, you agree that your contributions will be licensed under the same license as the project (MIT).

---

Thank you for contributing! 🎉
