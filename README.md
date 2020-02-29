# CS 528 Project 2 - Marco Polo V2 - Electric Boogaloo

### Project Idea

The idea of the project is to create a virtual environment space that will need navigation from point A to point B in a map. The player will not be able to see the environment. Instead, they will be guided by audio. An audio clearly for assisting the player will be provided.

There will be several levels consisting of increasing difficulty. The current levels are as follows:
- Full open space
- Open space with single colliding wall
- Open space with multiple colliding walls
- Open space with objects that walk toward the player
- Open space with colliding walls and objects that walk toward the player

In levels that include colliding walls, the player will lose if they contact the wall. In levels that include objects walking toward the player, the player will lose if they contact the object. 

### Intended Environment

The intended virtual environment will be generated and purposed for the VIVE, as it will allow for motion control through physical walking in space, rather than the CAVE2, which allows for motion control through directional input.


### Concerns
- Spatial Constraints due to the nature of how much space is safely available to maneuver with the VIVE.
- People have differing levels of hearing - may consider having an audio source control for all sound to be able to calibrate
- Handling several sounds. The intention is to have audio that will assist the player, but if I want to include multiple objects that also negatively impact the player, they will conflict with the assisting audio.

### Potential Additions
- Adding areas outside of just point A to point B in a map
