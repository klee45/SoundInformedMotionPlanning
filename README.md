### Running the project

The project is run on Unity version 2019.2.11f1
This is NOT required to run the demonstration and test demo code which are contained in the builds folder
In order to run the builds, you just need to go into one of the build folders and run the executable.
The editor is required if you want to modify hyperparameters or generate your own maps.

### Source code

All code is contained within the Assets/Scripts folder. All of it was written by me except for the min-heap implementation (although I did modify it), and the matrix library.
I have placed these scripts in a subfolder labeled "Imported Libraries". That is, all libraries and imported code is placed in Assets/Scripts/Imported Libraries.

### Generating maps and editing parameters

If you wish to modify the project parameters, you will need Unity 2019.2.11f1.
From there open the project and enter either the demo scene (the scene shown in my project presentation), or the test scene.
The main parameters that may be interest to modify are as follows:

#### Editor inspector objects:
- Robot (Either scene)
  - Sound Listener script
    - A* Steps - number of A* steps before switching to Monte Carlo Tree Search
    - Monte Carlo Step - number of steps of Monte Carlo movement before switching to A*
  - Primary Echo Source script
    - Strength - size / strength of the echo wave emitted for echolocation
- Monte Carlo Tree (TestingScene)
  - MC Tree script
    - UCT Tradeoff - Tradeoff variable between exploration and exploitation (higher is more exploration)
    - Time Budget - How long the robot is allowed to use the Monte Carlo Tree search for at each attempt

#### Scripts and other files:
Constants
 - Any of the kalman filter error values
   - These are the standard deviations for both observation and also state prediction
 - MonteCarloTree
   - Forgetting factor - how much less valueable later information is in the tree search
 - Values
   - Learning Rate - Speed of map learning for the echolocation

Map.txt

This file contains the map that will be generated by the TerrainContainer script.
The values are space delimited and are one of the following:
  - 0 - Floor tile
  - 1 - Wall tile
  - 2 - Secondary sound source and floor tile
  - p - Player and floor tile
  - r - Robot and floor tile

The map will be generated based on this file. Note that only one player and robot each can be placed.
