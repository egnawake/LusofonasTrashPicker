# Lusófona's Trash Picker

Trash picking robot game. Second project for the Inteligência Artificial course
at Universidade Lusófona.

## Authors and tasks

João Inácio (22202654)

- Trash Picker game model
- Game loop (player input and model updating)
- AI setup with naive Bayes classifier
- Graphics and animation
- Methodology section

Rodrigo Pires (22103008)

- Introduction section
- Results and discussion section
- Data gathering
- Conclusions section

## Introduction

The objective of this project is to identify and understand the positives and negatives of specifically using Naive Bayes Classifier. It is expected that the Ai will perform in an adequate matter sense it will use human gameplay for it's learning data.
The game itself will be a simple trash colecting game, where the player can always choose between moving on the board, trying to pick up trash or doing something randomly. The game ends when the number of turns reaches 0 at which point the game will tell you how much you scored.
The article we chose is [Mimicking human strategies in fighting games using a Data Driven Finite State Machine](https://ieeexplore.ieee.org/abstract/document/6030356) where the goal was to achieve human like strategies using both NBC and FSM in a fighting game. What this project will attempt to is to show the strengths of just using NBC alongside a FSM.

## Methodology

The project follows an MVC architecture. It is built on the .NET framework and the Unity engine for rendering the game and receiving player input.

*UML diagram for the game engine*

![Game engine UML diagram](uml-game.png)

*UML diagram for the controller*

![Controller UML diagram](uml-controller.png)

*UML diagram for the view*

![View UML diagram](uml-view.png)

The objective of the game is to maximize the score obtained by collecting trash. The game is turn-based and there is a turn limit. The game world is a grid of cells. The player can only see the cell they are on and a single cell to the north, east, south and west. One of seven different actions can be performed on a turn: moving north, east, south or west, moving in a random direction, collecting trash and skipping a turn. Collecting trash will increase the player's score by 10 points, using the collect trash action on a cell without any trash will deduce 1 point and moving against the map's borders will deduce 5 points.

An AI that can play the game was built. It uses a machine learning technique - a naive Bayes classifier - to learn from human games. It will link what the human decides to do on a turn to the state of the cells that are visible on that same turn and replicate that behaviour.

Certain game parameters are modifiable on the Unity editor: the dimensions of the game grid, the turn limit and the probability of a cell having trash on it when a map is generated.

## Results and discussion

During the project the agent was trained with the following data: All human scores were above 80; The highest score was of 90
Base parameters (6 by 6) grid; 20 turns; 0.4 rate of trash spawn
- almost never went lower then 80
- hit the highest score multiple times
- could not beat the highest score
-seemed to mostly behave appropriatly

Diferent board size (10 by 3) grid; 20 turns; 0.4 rate of trash spawn
- performed below previous average score
- tactic used was no longer as effective as previously

Normal board more turns (6 by 6) grid; 26 turns pt.1
- anomaly was detected where agent would get stuck in the corner
- average score was slighty higher at 100

pt.2
- gave agent 2 more human games to attempt to fiz anomaly
- anomaly was fixed but a new one showed up, similar to previous one (stuck in a different place on the board)
- agente still managed to beat previous highscore (120)

pt.3
- gave agent 4 more human games
- human scored 120
- previous anomaly fixed and another one was created, again similar to the last
- agent average score now substancialy lower (around 60)

pt.4
- gave agent 5 more human games
- human scored 130
- agent average increased to 70
- anomaly fixed, yet once more it came back

More trash (6 by 6) grid; 20 turns; 0.7 rate of trash
- human highscore of 100
- human average of 90
- lowest agent score 39
- highest agent score 100  


## Conclusions

The conclusions found in reference to our results were the following:
- This method seems to be quite adept in the same environment of it's data but only there if the parameters are the same or similar
- This method seems to be really good at learning one specific thing and incredibly difficult to make the agent un-learn something
- This method is limited in it's learning capabilities since it heavely relies on the data it is given

## References

- Saini, S., Chung, P. W. H. & Dawson, C. W. (2011). "Mimicking human strategies in fighting games using a Data Driven Finite State Machine". *2011 6th IEEE Joint International Information Technology and Artificial Intelligence Conference*. Chongqing, China, 2011, pp. 389-393, doi: 10.1109/ITAIC.2011.6030356.
