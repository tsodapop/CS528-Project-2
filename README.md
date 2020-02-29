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

The player will begin at the top of a "tower" where the objective is to reach the bottom of the tower. Thus, they will start at the "highest" level. "Lower" levels will indicate increasing difficulty. In between each level, they will find themselves in a "safe" level so that they can open an options menu, or move to the next level.

When the player reaches the bottom of the tower and clears it, they will be shown the final level and given the option to add one of several options to the final level. This object will impair other players of the session, but will reset upon restarting the project.

### Intended Environment

The intended virtual environment will be generated and purposed for the VIVE, as it will allow for motion control through physical walking in space, rather than the CAVE2, which allows for motion control through directional input.

### Concerns
- Spatial Constraints due to the nature of how much space is safely available to maneuver with the VIVE.
- People have differing levels of hearing - may consider having an audio source control for all sound to be able to calibrate
- Handling several sounds. The intention is to have audio that will assist the player, but if I want to include multiple objects that also negatively impact the player, they will conflict with the assisting audio.
- Anxiety - lack of vision and negatively affecting objects may cause a player to feel anxious. Need to work around this by ensuring the negatively affecting objects are not too intimidating.

### Potential Additions
- Add story/plot to why the levels need to be traversed.
- Adding areas outside of just point A to point B in a map, such as bonus location.
- Providing levels that include visual assistance for comparison.
- Giving the player an object that they will have to bring around to the goal. The object should also not collide with anything.
- Giving the player an object that they can bring around through the space. The object can pause/remove negative objects.
- The ability for a player to revisit previously completed levels with complete visual assistance.

### Supporting Resources for Project Evidence
- [Realistic, Responsive Audio is the Next Frontier in VR Innovation](https://www.cbinsights.com/research/vr-audio-tech/)
- [Creating an Audio Story with Interactive Binaural Rendering in Virtual Reality](https://www.hindawi.com/journals/wcmc/2019/1463204/)
- [The effect of 3D Audio and Other Audio Techniques on Virtual Reality Experience](https://www.researchgate.net/publication/291516579_The_Effect_Of_3D_Audio_And_Other_Audio_Techniques_On_Virtual_Reality_Experience)
- [source Source Localization with Varying Amount of Visual Information in Virtual Reality](https://journals.plos.org/plosone/article?id=10.1371/journal.pone.0214603)


