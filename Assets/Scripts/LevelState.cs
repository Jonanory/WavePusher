using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct LevelState
{
	public int TestNumber;
	public List<Vector2Int> Boxes;
	public Dictionary<Vector2Int, bool> Emitters;
	public Vector2Int Player;
	public Dictionary<Vector2Int, HashSet<MapDirection>> WaveElements;
	public bool HasAdvanced;

	public LevelState(int _test = 1)
	{
		TestNumber = _test;
		HasAdvanced = false;
		Boxes = new List<Vector2Int>();
		foreach(Vector2Int boxPos in GameManager.master.CurrentLevel.Boxes.Keys)
			Boxes.Add(boxPos);
		Player = GameManager.master.Player.Position;
		Emitters = new Dictionary<Vector2Int,bool>();
		foreach(Emitter emitter in GameManager.master.CurrentLevel.Emitters.Values)
		{
			Emitters.Add(
				emitter.Position,
				emitter.currentSteps+1 >= Emitter.STEPS_BETWEEN_WAVES
			);
		}
		WaveElements = new Dictionary<Vector2Int, HashSet<MapDirection>>();
		foreach(Wave wave in GameManager.master.CurrentLevel.Waves)
		{
			foreach(WaveElement element in wave.Elements.Values)
			{
				if(WaveElements.ContainsKey(element.Position))
				{
					foreach(MapDirection direction in element.DirectionsToFlow)
						WaveElements[element.Position].Add(direction);
				}
				else
				{
					WaveElements.Add(
						element.Position,
						element.DirectionsToFlow
					);
				}
			}
		}
	}

	public void MovePlayer(MapDirection _direction, bool _movingBox = false, bool _movingEmitter = false)
	{
		Vector2Int newPlayerPos = Map.CoordAfterMovement(Player, _direction);
		Vector2Int newPushablePos = Map.CoordAfterMovement(newPlayerPos, _direction);
		Player = newPlayerPos;
		if(_movingBox)
		{
			Boxes.Remove(newPlayerPos);
			Boxes.Add(newPushablePos);
		}
		else if (_movingEmitter)
		{
			bool isReady = Emitters.Remove(newPlayerPos);
			Emitters.Add(newPushablePos, isReady);
		}
	}

	public void MakeWaves()
	{
		if(HasAdvanced) return;
		Dictionary<Vector2Int, HashSet<MapDirection>> newWaves = new Dictionary<Vector2Int, HashSet<MapDirection>>();
		foreach(KeyValuePair<Vector2Int, HashSet<MapDirection>> element in WaveElements)
		{
			foreach(MapDirection direction in element.Value)
			{
				Vector2Int newPos = Map.CoordAfterMovement(element.Key, direction);
				if(!newWaves.ContainsKey(newPos)) newWaves.Add(newPos, null);
			}
		}
		foreach(KeyValuePair<Vector2Int, bool> emitter in Emitters)
		{
			if(emitter.Value)
			{
				for(int x=0;x<6;x++)
				{
					Vector2Int newPos = Map.CoordAfterMovement(emitter.Key, (MapDirection)x);
					if(!newWaves.ContainsKey(newPos)) newWaves.Add(newPos, null);
				}
			}
		}
		WaveElements = newWaves;
		HasAdvanced = true;
	}

	public void AddWave(Vector2Int _center)
	{
		for(int x=0;x<6;x++)
		{
			Vector2Int newPos = Map.CoordAfterMovement(_center, (MapDirection)x);
			if(!WaveElements.ContainsKey(newPos)) WaveElements.Add(newPos, null);
		}
	}

	public bool ItemAtPosition(Vector2Int _position)
	{
		return Player == _position ||
			Boxes.Contains(_position) ||
			Emitters.ContainsKey(_position);
	}

	public bool WaveAtPosition(Vector2Int _position)
	{
		return WaveElements.ContainsKey(_position);
	}
}