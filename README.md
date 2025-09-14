Little Adventurer Andi - C# Scripts

The last guardian awakens. The first battle for a lost world begins.
About This Repository
This repository contains the complete C# source code for my solo portfolio project, Little Adventurer Andi, a polished isometric action-adventure game built in Unity 6.

Please Note: This repository includes the scripts only. Due to asset licensing restrictions, the full Unity project, including models, textures, and other art assets, is not included.

Play the full game on Itch.io: [https://randomdx.itch.io/little-adventurer]

View the full Game Design Document (GDD): [https://drive.google.com/file/d/12KuPnio-Ay9EVjDKkjmQ9xV9EuT6NE1Y/view?usp=sharing]

Key Features & Skills Demonstrated
This project was a comprehensive exercise in game development, showcasing my ability to design, program, and polish a complete gameplay loop from the ground up.


Custom Character Controller:

A responsive isometric character controller built using Unity's CharacterController.

Features a skill-based, three-hit sword combo system with animation events.

Includes a dodge-roll mechanic for defensive maneuvering.

Modular Interaction System (PlayerInteractor.cs, Interactable.cs):

A single, reusable system that drives all player interactions with the world.

Powers complex events like the timed two-button gate puzzle and the discovery of lore tablets.


Enemy AI & Combat:

A simple but effective state machine (Character.cs with isPlayer flag) controls the behavior of three unique enemy types (Melee Golem, Ark Shooter, and Giant Golem).

Designed a DamageCaster system to handle weapon collisions and damage application.

UI & Game Management (GameManager.cs, DialogueManager.cs):

Complete UI flow with a main menu, dynamic dialogue system with a typewriter effect, and in-game HUD.

A central GameManager handles all game states (playing, paused, game over) and manages cursor visibility for a seamless player experience.

Controls
Movement: WASD Keys

Attack: Left Mouse Button

Dodge Roll: Space Bar

Interact: 'E' Key

Pause: Escape Key

Credits
Game Design, Programming, Sound Design, and Level Design: Mihir Kumar

Art & Visual Assets: Ryan. This project is for portfolio and demonstration purposes only.
