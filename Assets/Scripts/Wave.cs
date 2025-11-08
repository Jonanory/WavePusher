using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Wave
{
	public const int BASE_WAVE_STRENGTH = 7;
	public int Radius;
	public Vector2Int Center;
	public Dictionary<Vector2Int,WaveElement> Elements = new Dictionary<Vector2Int,WaveElement>();

	public Wave(Vector2Int _center)
	{
		Center = _center;
		Radius = 1;
	}

	public void Init(int? _strength = null)
	{
		int strength = _strength.HasValue ? _strength.Value : BASE_WAVE_STRENGTH;
		NewElements = new Dictionary<Vector2Int, WaveElement>();
		foreach(MapDirection direction in Enum.GetValues(typeof(MapDirection)))
			TryCreateWaveElement(
				strength,
				Map.CoordAfterMovement(Center, direction),
				direction);
		Elements = NewElements;
	}

	public WaveElement ElementAtCoord(Vector2Int _coord)
	{
		if(Elements.ContainsKey(_coord)) return Elements[_coord];
		return null;
	}

	Dictionary<Vector2Int,WaveElement> NewElements = new Dictionary<Vector2Int,WaveElement>();
	HashSet<Vector2Int> ExistingElementLocations= new HashSet<Vector2Int>();
	HashSet<Vector2Int> BlockedElementLocations = new HashSet<Vector2Int>();
	HashSet<Vector2Int> NewElementLocations = new HashSet<Vector2Int>();
	public void TimeStamp()
	{
		Radius++;
		NewElements = new Dictionary<Vector2Int,WaveElement>();
		ExistingElementLocations = new HashSet<Vector2Int>();
		BlockedElementLocations = new HashSet<Vector2Int>();
		NewElementLocations = new HashSet<Vector2Int>();

		foreach (Vector2Int WaveElementPosition in Elements.Keys)
		{
			ExistingElementLocations.Add(WaveElementPosition);
		}

		foreach (KeyValuePair<Vector2Int,WaveElement> WaveElement in Elements)
		{
			foreach(MapDirection direction in WaveElement.Value.DirectionsToFlow)
			{
				TryCreateWaveElement(
					WaveElement.Value.Strength - 1, 
					Map.CoordAfterMovement(
						WaveElement.Key, 
						direction),
					direction);
			}
		}

		foreach(WaveElement element in NewElements.Values)
		{
			element.Simplify();
		}
		Elements = NewElements;
	}

	void TryCreateWaveElement(int _strength, Vector2Int _newPosition, MapDirection _flowFrom)
	{
		if (_strength <= 0) return;
		if (!ExistingElementLocations.Contains(_newPosition) &&
			!BlockedElementLocations.Contains(_newPosition))
		{
			WaveElement newElement;
			if(!NewElementLocations.Contains(_newPosition))
			{
				if(GameManager.master.CurrentLevel.Map.CoordIsBlocked(_newPosition))
				{
					BlockedElementLocations.Add(_newPosition);
					return;
				}
				newElement = new WaveElement(_strength);
				NewElements.Add(_newPosition, newElement);
				NewElementLocations.Add(_newPosition);
			}
			else
			{
				newElement = NewElements[_newPosition];
			}

			newElement.AddFlowDirection(_flowFrom);
			newElement.AddFlowDirection(Map.RotateClockwise(_flowFrom));
			newElement.AddFlowDirection(Map.RotateCounterClockwise(_flowFrom));
		}
	}
}

public class WaveElement
{
	public int Strength;
	public HashSet<MapDirection> DirectionsToFlow;

	public WaveElement(int _strength)
	{
		Strength = _strength;
		DirectionsToFlow = new HashSet<MapDirection>();
	}

	public float GetTransparency()
	{
		return Strength*1f/Wave.BASE_WAVE_STRENGTH;
	}

	public void AddFlowDirection(MapDirection _direction)
	{
		DirectionsToFlow.Add(_direction);
	}

	public void Simplify()
	{
		if (DirectionsToFlow.Contains(MapDirection.UP) &&
				DirectionsToFlow.Contains(MapDirection.DOWN))
		{
			DirectionsToFlow.Remove(MapDirection.UP);
			DirectionsToFlow.Remove(MapDirection.DOWN);
		}

		if (DirectionsToFlow.Contains(MapDirection.UP_LEFT) &&
				DirectionsToFlow.Contains(MapDirection.DOWN_RIGHT))
		{
			DirectionsToFlow.Remove(MapDirection.UP_LEFT);
			DirectionsToFlow.Remove(MapDirection.DOWN_RIGHT);
		}

		if(DirectionsToFlow.Contains(MapDirection.UP_RIGHT) &&
				DirectionsToFlow.Contains(MapDirection.DOWN_LEFT))
		{
			DirectionsToFlow.Remove(MapDirection.UP_RIGHT);
			DirectionsToFlow.Remove(MapDirection.DOWN_LEFT);
		}
	}
}