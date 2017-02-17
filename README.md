# EpochFight

Use ONLY Version 5.5.1f1 of Unity(https://unity3d.com/get-unity/update) to avoid version collision.

## Features (Scene WorkerExample):
### Task system
1. Attack.
2. Collect resources.
3. Go to.

### Unit logic
1. Worker: Collects resources and defends when attacked.
2. Enemy: Defends when attacked. Attacks enemy units or base when within a certain radius. Roams around randomly.

TODO: Implement waypoints for the enemy AI.

##### NavMesh navigation
All units use a NavMeshController and scenes are pre-baked into NavMeshes.

### Basic combat system
Only melee combat for now.

### Basic GUI system
Simple UI for both in-game and menu.

### Camera controls and input
Selectable objects with mouse. Movable camera with keyboard (WASD and arrow keys).
