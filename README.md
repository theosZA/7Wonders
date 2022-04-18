# 7 Wonders

This is an implementation of the [7 Wonders](https://boardgamegeek.com/boardgame/68448/7-wonders) boardgame using the [Godot](https://godotengine.org/) game engine. The code is spread across several projects detailed below.

## 7WondersGame

A _7 Wonders_ game implemented in the Godot game engine. It has been tested with Godot version 3.4.4. The Godot scripts are implemented in C#. While these scripts can be edited in the Godot editor, I suggest using VS Code (with the C# and Godot extensions) instead.

⚠️ Currently the game does not support a human player. All seats are played by AI players. Press `SPACE` to advance the game turn by turn.

## 7WondersConsoleGame

A console (text-based) implementation of a _7 Wonders_ game. It allows a human player to play against a number of AI players. The text-based nature limits the ease of use of the game and it really only exists to aid in testing the other components.

## PerformanceTester

A small console application that determines how fast a _7 Wonders_ game can be played using AI players.

## 7WondersGameLogic

A library for the underlying game logic of a _7 Wonders_ game independent of the actual implementation. (Note that there are still console-specific output methods in the library.)

The `Game` class is the entry point into the library for wanting to run a game. The `PlayerAgent` interface has to be implemented for any players (human or AI) to have them play the game.

Below is sample code showing how, given a collection of `PlayerAgent` objects, to play through a game.
```
void PlayOneGame(IReadOnlyCollection<PlayerAgent> playerAgents)
{
  // The paths to use below will depend on the working folder.
  var availableTableaus = new StartingTableauCollection("..\\Cities.xml");
  var allCards = new CardCollection("..\\Cards.xml");

  var game = new Game(playerAgents, availableTableaus, allCards);
  while (!game.IsGameOver)
  {
    game.PlayTurn();
  }
}
```

## 7WondersRobots

A library containing `PlayerAgent` implementations for AI-driven players. The suggested AI to use is a `CityPlayer` created by the `RobotPlayerFactory.CreatePlayer` method. This AI will use logic specific to their city board.

## 7WondersEvolver

A Windows application that can be used for generating new AI players using an evolutionary process. Individual player DNA codes can be copied into the `Robots.xml` file for use by the AIs.

## Utilities

A library with utility classes and methods that are not specific to _7 Wonders_ in any way.
