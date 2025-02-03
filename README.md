# 2D Collapse/Blast Mechanic Game

**Developed by Yiğit Durmuş**  

This is a case study project for Good Job Games. The game is a **2D puzzle game** with **collapse/explosion mechanics**, developed from scratch in **10 days** without using any templates. The project supports both **mobile (Android/iOS) and PC** platforms, ensuring a **smooth and responsive user experience**.

📺 [Gameplay Video](https://www.youtube.com/watch?v=VIDEO_ID)

## 📌 Features

### 🎮 Dynamic Board Generation  
- The board adapts dynamically to different screen sizes and resolutions.  
- Ensures consistent gameplay across devices by adjusting tile size, spacing, and margins.

![f33unn5](https://github.com/user-attachments/assets/86eda19d-0bd6-4dbf-be33-aed7e4d20e6c)
![7ky06ht](https://github.com/user-attachments/assets/716185a8-9efe-40f7-ad54-6cfe4acee213)

### 🛠️ Level Editor  
- JSON-based level data structure.  
- Allows designers to create and modify levels efficiently.  
- Features include:
  - Custom board dimensions.
  - Initial block placements and colors.
  - Special conditions (obstacles, power-ups, etc.).
  - 
![7baiicz](https://github.com/user-attachments/assets/a2fadf19-49c6-4e7b-8afa-b9f39754de85)
![image](https://github.com/user-attachments/assets/0ff74944-cf85-43dc-9dc2-1c572cd507d0)

### 🎨 Adjustable Animations & Assets  
- All animations and assets can be configured directly from the Unity Inspector.  
- Customizable parameters:
  - Block movement animations.
  - Explosion effects.
  - Shuffle animations.

![image](https://github.com/user-attachments/assets/4b9db9d5-e5c9-42f0-9d4c-39d26d6e5388)
![image](https://github.com/user-attachments/assets/f07e32a0-fad6-4f79-afcb-354a6de7e32a)


### 🔄 Non-Deadlock Shuffle System  
- Prevents unsolvable board states by ensuring at least one valid move is always available.  
- Key methods:
  - `IsBoardPlayable()`: Detects deadlocks.
  - `ShuffleBoard()`: Rearranges blocks.
  - `HandleDeadlock()`: Ensures playability.  

### ⚡ Seamless Gameplay  
- Players can make consecutive moves without delays.  
- Ensures a **fast and responsive** gaming experience.  

### 🔧 Flexible Architecture with Scriptable Objects  
- Modular and scalable structure.  
- Supports easy modifications for blocks, levels, and settings.  

## 📊 Algorithms Used

### 🔍 Flood Fill Algorithm (Matching Groups)  
- Used to find and clear connected tiles of the same color.  
- Optimized with:
  - **Stack-based implementation** (prevents stack overflows).  
  - **Pre-allocated memory** for better performance.  

### 🔀 Shuffle Algorithm (Ensuring No Deadlocks)  
- Fisher-Yates Shuffle ensures a fair and random board arrangement.  
- Guarantees that at least one valid move exists.  

### 📉 Column Reordering & Filling System  
- Ensures that blocks fall into empty spaces after matches.  
- Efficient spawning system prevents unnecessary block generation.  

## 🚀 Performance Optimizations

- **Object Pooling**: Reduces CPU overhead by reusing objects.  
- **Asynchronous Operations (UniTask)**: Ensures smooth animations and prevents lag.  
- **Optimized Rendering**: Uses dynamic batching and efficient shaders.  
- **Memory Management**: Avoids unnecessary GC events, optimizing RAM usage.
![image](https://github.com/user-attachments/assets/ec9447c7-1746-41d5-b49e-382ee5d8884c)


## 🛠️ Technical Details

- **Unity Version**: `2022.3f.13f1`  
- **Third-Party Packages**: `Prime Tween`, `Odin`, `UniTask`  
- **GitHub Repository**: [CaseStudyGJG](https://github.com/hidro0x/CaseStudyGJG)  

---

### 📥 Installation & Usage

1. Clone the repository:
   ```sh
   git clone https://github.com/hidro0x/CaseStudyGJG.git
