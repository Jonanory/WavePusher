using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameState
{
	public int Time;
	public Vector2Int playerPos;
	public List<Vector2Int> boxPositions;
	public List<EmitterState> emitters;
	public List<DoorState> doors;
	public List<ReceiverState> receivers;
	public List<ButtonState> buttons;
	public List<WaveState> waves;
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
	public int offset;
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
