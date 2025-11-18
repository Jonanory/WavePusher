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
	WALL = 2,
	EXIT = 3
}

public class Map : MonoBehaviour
{
	public static float Scale = 0.5f;
	public Tilemap AreaMap;
	public Vector2Int Exit;

	void Start()
	{
		Display();
	}

	public void ClearAll()
	{
		AreaMap.ClearAllTiles();
	}

	public void Display()
	{
		AreaMap.SetTile(
			new Vector3Int(
				Exit.x,
				Exit.y,
				(int)MapLayer.EXIT),
			TileManager.GetTile(CellType.EXIT));
	}

	void DrawFloors(List<Vector2Int> _floorPositions)
	{
		foreach (Vector2Int floorPos in _floorPositions)
		{
			Tile tile;
			if (Map.Mod(floorPos.x, 3) == Map.Mod(floorPos.y + Map.Mod(floorPos.y,6) / 2, 3) )
			{
				tile = TileManager.GetTile(CellType.FLOOR_B);
			}
			else
			{
				tile = TileManager.GetTile(CellType.FLOOR);
			}
			GameManager.master.Map.AreaMap.SetTile(
				new Vector3Int(
					floorPos.x,
					floorPos.y,
					(int)MapLayer.FLOOR),
			tile);
		}
	}

	void DrawWalls(List<Vector2Int> _wallPositions)
	{
		foreach(Vector2Int wallPos in _wallPositions)
			GameManager.master.Map.AreaMap.SetTile(
				new Vector3Int(
					wallPos.x,
					wallPos.y,
					(int)MapLayer.WALL),
			TileManager.master.WallTile);
	}

	void DrawHoles(List<Vector2Int> _holePositions)
	{
		foreach(Vector2Int holePos in _holePositions)
			GameManager.master.Map.AreaMap.SetTile(
				new Vector3Int(
					holePos.x,
					holePos.y,
					(int)MapLayer.HOLE),
			TileManager.GetTile(CellType.HOLE));
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
		foreach (Emitter emitter in GameManager.master.CurrentLevel.Emitters.Values)
			if (emitter.Position == _coord) return true;
		foreach (Receiver receiver in GameManager.master.CurrentLevel.Receivers.Values)
			if (receiver.Position == _coord) return true;
		if (GameManager.master.CurrentLevel.Boxes.ContainsKey(_coord)) return true;
		foreach (Door door in GameManager.master.CurrentLevel.Doors.Values)
			if (!door.Open && door.Position == _coord) return true;
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