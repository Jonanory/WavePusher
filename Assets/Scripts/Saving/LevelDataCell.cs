using UnityEngine;

public enum CellType
{
	PLAYER = 0,
	WALL,
	FLOOR,
	FLOOR_B,
	RECEIVER,
	BUTTON,
	BOX,
	EMITTER,
	DOOR,
	WAVE,
	HOLE,
	GHOST
}
[System.Serializable]
public struct LevelDataCell
{
	public CellType Type;
	public Vector2Int Position;
	public Vector2Int SecondaryPosition;
	public int Data;

	public LevelDataCell(
		CellType _type,
		Vector2Int _position,
		Vector2Int? _secondaryPosition,
		int? _data)
	{
		Type = _type;
		Position = _position;
		if (_secondaryPosition.HasValue) SecondaryPosition = _secondaryPosition.Value;
		else SecondaryPosition = new Vector2Int(0, 0);
		if(_data.HasValue) Data = _data.Value;
		else Data = 0;
	}
}

[System.Serializable]
public struct LevelDataLink {
	public LevelDataCell input;
	public LevelDataCell output;
}
