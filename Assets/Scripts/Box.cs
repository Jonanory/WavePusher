using UnityEngine;

public class Box
{
	public Vector2Int Position;

	public bool CanPush(MapDirection _direction)
	{
		Vector2Int positionAfterPush = Map.CoordAfterMovement(
				Position,
				_direction);
		return !GameManager.master.Map.CoordIsBlocked(
			positionAfterPush
		);
	}

	public bool Push(MapDirection _direction)
	{
		if (!CanPush(_direction)) return false;
		Position = Map.CoordAfterMovement(
				Position,
				_direction);
		return true;
	}
}
