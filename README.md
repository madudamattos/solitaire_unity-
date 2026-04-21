# ♠ Solitaire Professional Base

> A professional-grade Solitaire implementation for Unity 2D, built on clean architecture principles — fully decoupled, testable, and extensible.

---

## 📐 Architecture Overview

This project is structured around a strict **separation of concerns**, divided into three main layers:

| Layer | Technology | Responsibility |
|---|---|---|
| **Model** | Pure C# | Data management — Card values, suits, pile state |
| **View** | MonoBehaviour | Visual rendering — sprites, positions, DOTween animations |
| **Presenter** | Mediator class | Bridges Model and View — receives input, validates, dispatches commands |

The core principle: **the game logic never depends on the rendering layer.** Models are pure C# classes with zero Unity dependencies, making them fully unit-testable.

---

## 🏗️ Design Patterns

### Command Pattern
Every valid move creates a `MoveCommand` object. Commands are stored in a `Stack<ICommand>`, enabling a full **Undo history** at no architectural cost.

### State Pattern
A lightweight state machine governs the game lifecycle across three states:

```
Setup → PlayerTurn → Victory
```

### Observer Pattern
`System.Action` and `System.EventHandler` delegates allow the Model layer to notify Presenters of state changes **without holding a direct reference** to them — keeping the dependency graph clean.

### Manual Dependency Injection
All references are passed via constructors or `Initialize()` methods. There are **no** `GameObject.Find` calls, no `FindObjectOfType`, and no global Singletons.

---

## ⚙️ Tech Stack

| Concern | Solution |
|---|---|
| Engine | Unity 2D Core (URP) |
| Animation | DOTween |
| Async Logic | Native Coroutines / C# `async`/`await` |
| Asset Configuration | ScriptableObjects + Inspector references |

> **Why DOTween?** It eliminates the boilerplate of manual coroutines per movement and provides a clean, chainable API for sequencing card animations.

---

## 📁 Project Structure

```
Assets/
└── _Project/
    ├── Scripts/
    │   ├── Core/           # Interfaces and Enums (ISuit, Rank, GameState)
    │   ├── Models/         # Card.cs, Deck.cs, Pile.cs  — Pure C#, no MonoBehaviour
    │   ├── Views/          # CardView.cs, PileView.cs   — MonoBehaviours, visuals only
    │   ├── Presenters/     # CardPresenter.cs, GamePresenter.cs
    │   └── Logic/          # MoveValidator.cs, CommandManager.cs
    ├── Art/
    │   ├── Sprites/        # Card faces, backs, and backgrounds
    │   └── UI/             # Buttons, icons, HUD elements
    └── Prefabs/
        ├── BaseCard
        └── BasePile
```

---

## 🔄 Execution Flow

The diagram below shows a full round-trip from player input to screen update:

```
[Player drags CardView]
        │
        ▼
[CardView] ──notifies──▶ [CardPresenter]
                                │
                         asks MoveValidator
                                │
                    ┌───────────┴───────────┐
                 Invalid                  Valid
                    │                      │
              reject drag         create MoveCommand
                                           │
                                    update Model data
                                           │
                                  call view.MoveTo()
                                           │
                                  [DOTween animation]
```

### Undo Flow

```
[Player clicks Undo]
        │
        ▼
[CommandManager] ──pops── Stack<ICommand>
        │
  ICommand.Undo()
        │
  revert Model data
        │
  View follows update
```

---

## 🚀 Getting Started

### Prerequisites

- Unity **2022.3 LTS** or later (URP template)
- [DOTween](http://dotween.demigiant.com/) — import via Package Manager or Asset Store

### Installation

```bash
# Clone the repository
git clone https://github.com/your-username/solitaire-professional-base.git

# Open the project in Unity Hub
# Import DOTween and run its setup panel (Tools > DOTween Utility Panel > Setup)
```

### Running the Game

1. Open the scene at `Assets/_Project/Scenes/GameScene.unity`
2. Press **Play** in the Unity Editor

---

## 🧪 Testing

Because all Models are pure C# with no Unity dependencies, they can be tested with any standard C# test runner (NUnit via Unity Test Framework):

```
Assets/
└── Tests/
    ├── EditMode/    # Model and Validator unit tests (no Unity runtime needed)
    └── PlayMode/    # Integration tests with the full game loop
```

Run tests via **Window > General > Test Runner** in the Unity Editor.

---

## 🗺️ Roadmap

- [ ] Full Klondike Solitaire ruleset
- [ ] Drag-and-drop input with multi-card stack dragging
- [ ] Animated card deal sequence on game start
- [ ] Win condition detection and victory screen
- [ ] Persistent high score via `PlayerPrefs` or ScriptableObject
- [ ] Sound effects layer (decoupled via an `AudioPresenter`)

---

## 🤝 Contributing

Contributions are welcome. Please follow the existing architectural conventions — new features should respect the Model/View/Presenter boundaries and avoid introducing cross-layer dependencies.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/my-feature`)
3. Commit your changes (`git commit -m 'feat: add my feature'`)
4. Push to the branch (`git push origin feature/my-feature`)
5. Open a Pull Request

---

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

---

<div align="center">
  <sub>Built with clean architecture principles · Unity 2D · DOTween</sub>
</div>
