using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public enum MapDirection
{
	UP = 0,
	UP_RIGHT = 1,
	DOWN_RIGHT = 2,
	DOWN = 3,
	DOWN_LEFT = 4,
	UP_LEFT = 5
}

public enum MapLayer
{
	HOLE = -1,
	FLOOR = 0,
	EXTRA = 1,
	WALL = 2
}

public class Map : MonoBehaviour
{
	public static float Scale = 0.5f;
	public Tilemap AreaMap;
	public Dictionary<Vector2Int, Emitter> Emitters = new Dictionary<Vector2Int, Emitter>();
	public Dictionary<Vector2Int, Receiver> Receivers = new Dictionary<Vector2Int, Receiver>();
	public Dictionary<Vector2Int, Button> Buttons = new Dictionary<Vector2Int, Button>();

	public Dictionary<Vector2Int, IBlockable> Blockables = new Dictionary<Vector2Int, IBlockable>();

	void Start()
	{
		Display();
	}

	public void ClearAll()
	{
		AreaMap.ClearAllTiles();
		Emitters = new Dictionary<Vector2Int, Emitter>();
		Receivers = new Dictionary<Vector2Int, Receiver>();
		Buttons = new Dictionary<Vector2Int, Button>();
		Blockables = new Dictionary<Vector2Int, IBlockable>();
	}

	public void Display()
	{
		foreach (Button button in Buttons.Values)
		{
			AreaMap.SetTile(
				new Vector3Int(
					button.Position.x,
					button.Position.y,
					(int)MapLayer.EXTRA),
				TileManager.GetTile(CellType.BUTTON));
		}

		foreach (Emitter emitter in Emitters.Values)
			AreaMap.SetTile(
				new Vector3Int(
					emitter.Position.x,
					emitter.Position.y,
					(int)MapLayer.EXTRA),
				TileManager.GetTile(CellType.EMITTER));

		foreach (Receiver receiver in Receivers.Values)
			AreaMap.SetTile(
				new Vector3Int(
					receiver.Position.x,
					receiver.Position.y,
					(int)MapLayer.EXTRA),
				TileManager.GetTile(CellType.RECEIVER));
	}

	public static MapDirection RotateClockwise(MapDirection _direction, int _numberOfSteps = 1)
	{
		int newDirection = ((int)_direction + _numberOfSteps) % 6;
		return (MapDirection)newDirection;
	}

	public static MapDirection RotateCounterClockwise(MapDirection _direction, int _numberOfSteps = 1)
	{
		int newDirection = (int)_direction - _numberOfSteps;
		while (newDirection < 0) newDirection += 6;
		return (MapDirection)newDirection;
	}

	public static Vector2Int CoordAfterMovement(Vector2Int _startPoint, MapDirection _direction)
	{
		switch (_direction)
		{
			case MapDirection.UP:
				return new Vector2Int(_startPoint.x + 1, _startPoint.y);
			case MapDirection.UP_LEFT:
				return new Vector2Int(_startPoint.x + Mod(_startPoint.y, 2), _startPoint.y - 1);
			case MapDirection.UP_RIGHT:
				return new Vector2Int(_startPoint.x + Mod(_startPoint.y, 2), _startPoint.y + 1);
			case MapDirection.DOWN:
				return new Vector2Int(_startPoint.x - 1, _startPoint.y);
			case MapDirection.DOWN_LEFT:
				return new Vector2Int(_startPoint.x - 1 + Mod(_startPoint.y, 2), _startPoint.y - 1);
			case MapDirection.DOWN_RIGHT:
				return new Vector2Int(_startPoint.x - 1 + Mod(_startPoint.y, 2), _startPoint.y + 1);
			default:
				return _startPoint;
		}
	}

	public static int Mod(int _value, int _base)
	{
		if (_value >= 0) return _value % _base;
		int valueHold = _value;
		while (valueHold < 0) valueHold += _base;
		return valueHold;
	}

	public bool CoordIsBlocked(Vector2Int _coord)
	{
		if (AreaMap.HasTile(
			new Vector3Int(
				_coord.x,
				_coord.y,
				(int)MapLayer.WALL))) return true;
		if (AreaMap.HasTile(
			new Vector3Int(
				_coord.x,
				_coord.y,
				(int)MapLayer.HOLE))) return true;
		if (GameManager.master.CurrentLevel.Boxes.ContainsKey(_coord)) return true;
		foreach (Door door in GameManager.master.CurrentLevel.Doors.Values)
			if (!door.Open && door.Position == _coord) return true;
		if (Blockables.ContainsKey(_coord)) return Blockables[_coord].IsBlocking;
		return false;
	}

	public bool CoordIsFlowable(Vector2Int _coord)
	{
		if (AreaMap.HasTile(
			new Vector3Int(
				_coord.x,
				_coord.y,
				(int)MapLayer.WALL))) return false;
		return true;
	}

	public static Vector2 CoordToWorldPoint(Vector2Int _coord)
	{
		return new Vector2(
			_coord.y * 1.5f * Scale,
			(_coord.x + Mod(_coord.y, 2) / 2f) * Mathf.Sqrt(3) * Scale
		);
	}

	public static Vector2Int WorldPointToCoord(Vector2 worldPoint)
	{
		int y = Mathf.RoundToInt(worldPoint.x / 1.5f);
		float rowParity = Mod(y, 2) / 2f;
		int x = Mathf.RoundToInt(worldPoint.y / Mathf.Sqrt(3f) - rowParity);
		return new Vector2Int(x, y);
	}
}