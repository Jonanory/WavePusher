using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameState
{
	public Vector2Int playerPos;
	public int playerRecharge;
	public List<Vector2Int> boxPositions;
	
	public List<GhostState> ghosts;
	public List<EmitterState> emitters;
	
	public int timeStep;
	
	// Optional if you don't recompute:
	public List<DoorState> doors;
	public List<ReceiverState> receivers;
	public List<ButtonState> buttons;

	public List<WaveState> waves;
	// etc...
}

[System.Serializable]
public struct GhostState
{
	public Vector2Int position;
	public int ticksUntilNextWave;
}

[System.Serializable]
public struct DoorState
{
	public Vector2Int position;
	public bool isOpen;
}

[System.Serializable]
public struct ReceiverState
{
	public Vector2Int position;
	public bool isActive;
	public int scoreNeeded;
}


[System.Serializable]
public struct EmitterState
{
	public Vector2Int position;
	public int ticksUntilNextWave;
	public bool isActive;
}


[System.Serializable]
public struct ButtonState
{
	public Vector2Int position;
	public bool isActive;
}

[System.Serializable]
public struct WaveElementState
{
	public Vector2Int position;
	public int strength;
	public List<MapDirection> flowDirections;
}

[System.Serializable]
public struct WaveState
{
	public Vector2Int center;
	public List<WaveElementState> elements;
}
