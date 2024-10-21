<h1>Chronovore</h1>
<p align="center">
<!--   <img width="100%"src=".png"> -->
  </br>
</p>

## 🔴 About Project
**Chronovore** is a 3D hack-and-slash action RPG where time is your only life force. As time constantly ticks down, players must fight through hordes of enemies to reclaim the precious seconds needed to survive. Every hit, kill, and combo adds time, but with each mistake, time slips away. The player wields the unique power to stop time for brief moments, gaining an edge in combat or evading death—but using this power comes at the cost of your remaining life. Set in a peaceful, yet monstrous world, Chronovore challenges you to manage your time and skillfully battle through a relentless struggle for survival.
<br>
The game itself is set on a mutated rainforest-like environment that drains off your life as time goes by. Entities on the forest are thrived with food as your life keeps on depleting, as you charge and attacked them hoping to life you gained your lifeforce back. 
<br>

## 📋 Project Info 
* Editor Version : Unity 2022.3.28f1

| **Name** | **Role** | **Development Time** |
|----------|----------|----------------------|
| Mario Valent Wibowo | Game Programmer | 3 days |
| Philips Sanjaya | Game Programmer | 3 days |
| Totti Adithana Sunarto | Game Programmer | 3 days |
| Justin Tjokro | Game Designer | 3 days |

```*note: this game is currently in development!!!```
<br>

## 📜 Scripts and Features

| Location |  Script       | Description                                                  |
|-----| ------- | ------------------------------------------------------------ |
|DataPersistence| `DataManager.cs` | Manages data storage and data distribution towards the interfaces. |
|DataPersistence| `DataHandler.cs` | Handles the save and loading system for the game. |
|Gameplay| `Inventory.cs` | Stores picture data and its detail for further uses during gameplay. |
|Manager| `UIManager.cs`  | Manages pausing and various UI element functions. |
|Underwater| `ScreenshotHandler.cs`  | Handles screenshot and also album creating. |
| | `etc`  | |
<br>

## 👤Contributions

- Mario Valent Wibowo (Lead Programmer)
<details>
  <summary> Philips Sanjaya (Programmer) </summary>
  
  **More Details**
  1. Post-Processing
     - using URP's post-processing to improve lighting, blooming, color grading, bluring.

![image](https://github.com/user-attachments/assets/1b8b7dcf-16c7-4bcd-9cc7-b240dd6360c8)

  2. Character Movement
    - Basic movement script using character controller that moves according to camera

     ![Walking GIf](https://github.com/user-attachments/assets/b016f30e-3fec-45f8-90ee-d776d820dab7)

  4. Further implimentation of cinemachines with the help of code
     - Using cinemachines component to give the camera logic alongside with a scripted targetting system
     
     ![Targetting GIF](https://github.com/user-attachments/assets/a5a05b9b-f637-4307-a732-af785739b193)

5. New input system
   - Using Unity's new input system as the first steps of migration from the old system to the new system.
     
  **What i learned as of currently**
  - I mostly learned new things about how input system work and how it is executed, like for example having the actionmap enable and disable function to actually make the actionmap to start functioning. Using the events like `performed, ispressed` really makes it much more simple and much more readable (for reading convention). Overall, i would be using the new input system when developing a game with a bigger scope as it is also organized in way that we dont need to create our own input script or find all the inputs used for the game.
  - Through the development, i improved my object placement skills as they are sensitive to changes on parents. Here in this project, we used an Animator that applies root motion in which this was the main parent of the object. As of the rotations, we seperated it into different types, the mesh rotations, and the player rotation. The player rotation itself is always aimed at the camera's forward direction as it is crutial for the User Experience to have the player move along with the camera rotating.
  - Through the animating process, i also learned to create animator parameters efficiently. As for an example in this project, i created the parameter by taking the `localVelocity` of the object and dividing that with the `base_walkingspeed`, this allows easy animating as they are reliant to the parameters 0 to 1 and also can be extended when adding others mechanics (ex : using run would just make the parameter above 1).
<br>

  - Initially the game used procedural map generation utilizing noise maps and animation curves as the base. The map generation idea was later disbanded as we found out that
    doing so alongside with the environment (grasses and trees) would make a lot of memmory buffers, even if we implimented DOTS or ECS. I learned alot mainly towards how mesh vertices and normals work,
    and how procedural generations work, through research i discovered other procedural methods in which where the WFC (Wave Function Collapse) or using structural generation that is supported by a* to create the paths.
<br>
</details>

- Totti Adithana (Project Manager)
- Justin Tjokro (Game Designer)
<br>

## 📂Files description

```
 ├── Capstonee              # In this Folder, containing all the Unity project files, to be opened by a Unity Editor
   ├── Assets                          #  In this Folder, it contains all our code, assets, scenes, etcwas not automatically created by Unity
      ├── ...
      ├── Script                       # In this folder, containing all the game codes
      ├── Scenes                       # In this folder, there are scenes. You can open these scenes to play the game via Unity
      ├── ...                      
   ├── ...
```
<br>

## 🕹️ Controls

The following controls are bound in-game, for gameplay and testing.

| Key Binding       | Function          |
| :---: | :---: |
| WASD | Player Movement |
| X | Target Locking |
| LMB | Use held item |
| Q | Equip / Unequip item |

<h3>Download Game</h3>
Currently the latest version is not downloadable yet as it is in further development and polishing

If you volunteer in trying our game, feel free to try and also please report us when encountering a major bug or problem
Thank you
