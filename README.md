# Civ2-2

This is a project remaking Sid Meiers Civilization II in Unity for modern platforms.

I reverse engineered the old map file format, so that maps generated from the original scenario maker can be loaded and displayed (to the pixel). 
Unit movement, including a modified A* pathfinding for AI, visbility, game turns, combat and terrain modernizations are already implemented.
All of the above can be played in multi-player, using a self-made network manager based on byte-wise TCP communication.
Due to copyright reasons only the source code of the unity scripts is released here, as the full project includes sprites and sounds that I do not own.

To-Do city and nation management, including science, money, advisors and most of the UI
To-Do AI and diplomacy 
