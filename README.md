# AI-for-Game-Technology

Done Prototype:
- Map generation 
- Determine player start and level end areas of map

TODO Prototype:
- Implement Player at start, ensure walls cannot be passed through
- Implement finish conditions and spawn player at spawn point
- Make Camera face right direction and focus on player (also make sure the camera doesn't show out of bounds area)
- Implement generation and behavior of enemies who damage player
- Remove dead ends from pathway (see Ideas for potential alternative)
- Some text explaining the game?


Ideas:
- Implement items in dead ends, letting the player make a choice of choosing to get an item and spending more time running from enemies, or running straight to the end

Map Generation:
- Most of the map generation is taken from https://pavcreations.com/procedural-generation-of-2d-maps-in-unity/
- Some simple comments are in the code to give basic idea of what each function does
- Variables width and height determine the map size -- preset to 200 and 100
- Filling percentage determines how many walls there are present -- preset to 50
- Hilbert reps and scale partially determine shape of pathway -- preset to 3 and 2
- Path girth determines how wide the hallways are -- preset to 8
- Negative path girth fills in areas that are not the main path slightly -- preset to 8
- Shift x and shift y determine how the map is shifted in relation to the hilbert curve -- randomly generated each run