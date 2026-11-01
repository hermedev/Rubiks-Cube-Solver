# 3D Rubik's Cube Solver (Unity 6 + URP)

[![Play Live Demo](https://img.shields.io/badge/Play-Live%20Demo-brightgreen?style=for-the-badge&logo=googlechrome)](https://hermedev.github.io/Rubiks-Cube-Solver-play/)
[![Playable Build Repo](https://img.shields.io/badge/Build%20Repo-Rubiks--Cube--Solver--play-orange?style=for-the-badge&logo=github)](https://github.com/hermedev/Rubiks-Cube-Solver-play)

An interactive 3D Rubik's Cube simulator and automated solver built with **Unity 6** using the **Universal Render Pipeline (URP)** and powered by the **Kociemba Two-Phase Algorithm**.

🎮 **Play Online Now:** [https://hermedev.github.io/Rubiks-Cube-Solver-play/](https://hermedev.github.io/Rubiks-Cube-Solver-play/)

---

## 🚀 Features

- **Interactive 3D Rubik's Cube**: Smooth 3x3 cube simulation composed of modular piece prefabs with realistic Lit materials.
- **Kociemba Two-Phase Solver**: Fast algorithmic solver computing optimal move sequences to solve any scrambled state.
- **Dynamic Face Rotation & Animation**: Smooth rotation of faces around dynamic pivots with configurable animation speeds.
- **Shuffle & Move Execution**: Automatic cube scrambling and step-by-step or automated move sequence execution.
- **Color Detection & 2D Mapping**: Raycast-based facelet reader (`ReadCube`) synchronized with an interactive 2D flat-map UI (`CubeMap`).
- **Intuitive Controls**: Drag to orbit the cube, click faces to rotate slices, or use the UI panel to shuffle and solve.
- **Universal Render Pipeline (URP)**: Optimized multi-platform graphics with modern lighting and shaders.
- **WebGL Ready**: Configured for web deployment (itch.io, GitHub Pages, or standalone web servers).

---

## 🛠️ Requirements & Environment

- **Unity Version**: Unity 6 (`6000.0` / `6000.6.3f1` or later recommended)
- **Render Pipeline**: Universal Render Pipeline (URP v17.x)
- **Platforms**: WebGL, PC / Mac Standalone, Mobile

---

## 📁 Project Structure

```text
Assets/
├── Materials/
│   ├── CubeColors/          # URP Lit materials for each cube face (Red, Blue, Green, Yellow, Orange, White)
│   └── Table/               # Background table textures and URP Lit material
├── Prefabs/
│   ├── Piece.prefab         # Individual cube piece (cubie) prefab
│   └── RayStart.prefab      # Raycast emitter prefab used to detect facelet colors
├── Scenes/
│   └── Rubics Cube.unity    # Main gameplay and solver scene
├── Scripts/
│   ├── CubeInput.cs         # Input interfaces for user interaction
│   ├── CubeInputReader.cs   # Keyboard and mouse drag input handling
│   ├── CubeManager.cs       # Central coordinator for cube state, shuffle, and solver logic
│   ├── CubeMap.cs           # Manages 2D representation & UI color sync
│   ├── CubeMoveExecutor.cs  # Queues and executes move notations (U, D, L, R, F, B, etc.)
│   ├── CubeShuffle.cs       # Scrambles the cube with random valid rotations
│   ├── CubeState.cs         # Tracks facelet states, face arrays, and active sides
│   ├── CubeUIController.cs  # Manages UI buttons, solver status, and playback controls
│   ├── KociembaSolver.cs    # Implementation of Kociemba's two-phase Rubik's cube solver
│   ├── PivotRotation.cs     # Controls face rotation parenting, angle lerping, and completion events
│   ├── ReadCube.cs          # Uses raycasts to read colors off cube piece colliders
│   ├── RotateCube.cs        # Handles whole-cube rotation and camera orbiting
│   └── SelectFace.cs        # Raycast picking for face selection and slice rotations
└── Settings/
    ├── UniversalRenderer.asset             # Forward URP Renderer configuration
    └── UniversalRenderPipelineAsset.asset   # Active URP pipeline asset
```

---

## 🎮 How to Run Locally

### In the Unity Editor:
1. Open the project in **Unity 6**.
2. In the Project window, navigate to `Assets/Scenes/`.
3. Open **`Rubics Cube.unity`**.
4. Press the **Play** button in the Unity Editor toolbar.

### Local Web Testing:
- To test the WebGL build locally, use **File > Build and Run** (`Ctrl + B`). Unity will start its internal local web server with all required HTTP headers preconfigured.

---

## 🕹️ Controls & Interaction

| Action | Control |
| :--- | :--- |
| **Orbit Cube** | Left Click + Drag on background |
| **Rotate Face / Slice** | Click or Drag directly on a facelet / slice |
| **Shuffle** | Click the **Shuffle** button in the UI |
| **Solve** | Click the **Solve** button to calculate and animate the solution sequence |

---

## 🌐 WebGL Build & GitHub Pages Deployment

When hosting the WebGL build on static sites like **GitHub Pages**, follow these steps to avoid `Uncaught SyntaxError: illegal character U+001F` / missing `Content-Encoding: gzip` errors:

### 1. WebGL Publishing Settings in Unity
1. Go to **Edit > Project Settings > Player**.
2. Select the **WebGL** tab (HTML5 icon).
3. Expand **Publishing Settings**:
   - Set **Compression Format** to **Disabled** *(recommended for GitHub Pages)*, **OR**
   - Enable **Decompression Fallback** if keeping Gzip.
4. Go to **File > Build Settings**, select **WebGL**, and click **Build**.

### 2. Deploying to GitHub Pages
1. Push your build output files (`index.html`, `Build/`, `TemplateData/`) to your repository (e.g., inside a `docs/` folder or on a `gh-pages` branch).
2. **Add an empty `.nojekyll` file** in the same directory as `index.html`. This ensures GitHub's Jekyll processor doesn't filter out Unity's build files.
3. In your GitHub repository:
   - Go to **Settings > Pages**.
   - Under **Build and deployment > Source**, select **Deploy from a branch**.
   - Select your branch and directory (`/root` or `/docs`), then click **Save**.

---

## 📜 License

This project is licensed under the MIT License - feel free to use, modify, and distribute it for personal and educational projects.
