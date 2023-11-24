# Project Car (PC)

Project car is a project that is a game in the genre of "racing". The game has a main menu, an in-game store and 3 levels. The interface is intuitive and does not need special consideration.

1. A training level that tells about the main aspects of the game.
2. A difficult level, including: jumping from trampolines and tremendous speed.
3. The last level, which includes: twisted elements of the track and multiple obstacles.

For an amateur gaming experience(60 frames per second), at least:
- AMD Ryzen 5 3500U | Intel Core i5-8265U
- 4 GB of RAM
- AMD Radeon HD 7850 | NVIDIA GeForce GTX 750

The build project is archived: `./BuildArchive/Project car.zip `  
Unity version: `2020.3.28f1`

## Gameplay

<p align="center">
  <img src="./ForReadMe/Gifs/gameplay.gif" alt="animated" />
</p>

The gameplay of the game is built around multiple passing levels, thereby incrementing the completed lap counter. Falls from the track increment the drop counter.

- For every 10 completed laps, the player receives 10 coins (in-game currency in the form of crowns) and resets both counters.
- For each fall, the player receives a penalty in the amount of the number of drops in the square (the penalty is imposed on the in-game currency), 10 drops also resets both counters.
- There are various objects on the levels that facilitate /hinder the passage of the level.

Controls:
- `W`, `A`, `S`, `D` - vehicle movement control.
- `SPACE` - handbrake.
- `ESC` - pause time.

## Store

<p align="center">
  <img src="./ForReadMe/Gifs/store.gif" alt="animated" />
</p>

There are coins on the levels.
Coins are an in-game currency for buying car colors. Each selected coin is +1$.

- Any color costs $35.
- The spoiler on the car can be equipped/removed for free.

From the store, you can get to the menu and back to the level from which you entered the store.

## Menu

<p align="center">
  <img src="./ForReadMe/Gifs/menu.gif" alt="animated" />
</p>

You can control the car in the menu.

- Using the `PLAY` button, you can select a level.
- The `STORE` button opens the store.
- `EXIT` - exit.

## Assets

- [Low-gauge modular roads FREE](https://assetstore.unity.com/packages/3d/environments/roadways/modular-lowpoly-track-roads-free-205188)
- [ARCADE: FREE racing car](https://assetstore.unity.com/packages/3d/vehicles/land/arcade-free-racing-car-161085 )
- [Simple Street props](https://assetstore.unity.com/packages/3d/props/simple-street-props-194706 )
