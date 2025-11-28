***

# 🛒 3D Market Idle Tycoon (Prototype)

A base prototype for a 3D Idle Tycoon game developed with **Unity 2022.3.37f1**. The project demonstrates a modular architecture, a custom save system, and core tycoon mechanics without external libraries.

## 🎮 Gameplay Overview
*   **Build & Expand:** Start with a small budget. Construct a **Meat Stall** and a **Cashier** to open the shop.
*   **Serve Customers:** AI agents navigate the shop, form smart queues, and purchase goods.
*   **Progression:** Earn money to upgrade **Service Speed** and hire more **Workers**.
*   **Dynamic Balance:** As you upgrade your shop, customer traffic increases automatically.
*   **Golden Tickets:** A luck-based inventory mechanic to earn bonus cash.

## ⚙️ Technical Highlights
*   **Architecture:** Event-driven architecture (EventBus) to decouple logic from UI.
*   **Save System:** Custom JSON-based system using `PlayerPrefs`. Saves buildings, stats, and currency.
*   **AI:** `NavMeshAgent` with custom queuing logic. Agents avoid obstacles and stand in formation.
*   **Visual Debug:** Extensive use of **Gizmos** to visualize queues and interaction points in the Editor.
*   **Adaptive UI:** Includes `SafeArea` support for mobile notches and responsive World Space UI.
*   **No 3rd Party Assets:** 100% native Unity code (No Zenject, DOTween, or Odin).

## 🛠 Design Choices & Justifications
### Why these solutions?
*   **uGUI vs UI Toolkit:** For this prototype, standard uGUI was chosen for rapid development and world-space UI compatibility. *For a release version, UI Toolkit would be preferred for performance.*
*   **Coroutines vs DOTween:** Animations (UI pulsing, progress bars) are handled via lightweight Coroutines (`PulseAnimation.cs`) to keep the project dependency-free.
*   **Input Manager:** Uses the standard Input class for simplicity. *New Input System is recommended for a production release.*
*   **Save System Scope:** The system persists economy, building states, and tutorial progress. **Active customers are not saved**; they respawn upon loading. This is an intentional optimization sufficient for an MVP.

## ⚠️ Technical Limitations & Trade-offs (For Prototype Scope)
While the current architecture ensures a stable and playable prototype, certain decisions were made to prioritize development speed and native Unity features over production-level scalability.

1.  **Object Instantiation (Allocation):**
    *   **Current State:** Customers are instantiated and destroyed at runtime.
    *   **Why:** Sufficient for low NPC counts in a prototype.
    *   **Production Solution:** Implementation of **Object Pooling** is mandatory for release to eliminate Garbage Collection spikes during high traffic.

2.  **Singleton Pattern:**
    *   **Current State:** Core systems (`CurrencyController`, `SaveManager`) use the Singleton pattern.
    *   **Why:** Provides easy global access without the overhead of setting up a DI Container.
    *   **Production Solution:** Transition to **Dependency Injection** (e.g., Zenject/VContainer) to improve modularity and unit testing capabilities.

3.  **Input Handling:**
    *   **Current State:** Legacy `Input` class is used.
    *   **Why:** Simple and sufficient for mouse/touch interactions in this specific genre.
    *   **Production Solution:** Migration to the **New Input System** to abstract controls and support multiple devices natively.

4.  **Save Security:**
    *   **Current State:** JSON data is stored in `PlayerPrefs` as plain text.
    *   **Why:** Great for debugging and validating data structure.
    *   **Production Solution:** Binary serialization or encryption should be applied to prevent cheating.

## 🚀 Future Improvements (Roadmap to Release)
If this project were to move to production, the following changes are recommended:
1.  **Dependency Injection:** Replace Singletons with a DI container (e.g., **Zenject/VContainer**).
2.  **Reactive Programming:** Implement **R3/UniRx** for more robust event handling.
3.  **Object Pooling:** Implement a pool for Customers to reduce Garbage Collection spikes.
4.  **Asset Management:** Integrate **Addressables** for easier content updates.

## 🕹 Controls
*   **Camera Move:** Drag mouse/finger on the ground.
*   **Camera Zoom:** Scroll wheel or Pinch.
*   **Interact:** Click/Tap on buildings, buttons, cars, or ATMs.

---
*Created for the Unity Developer Test Task.*
