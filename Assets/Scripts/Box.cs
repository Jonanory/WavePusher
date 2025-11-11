using UnityEngine;

public class Box : MonoBehaviour
{
	public Vector2Int Position;

	public void Start()
	{
		GameManager.master.CurrentLevel.Boxes.Add(Position, this);
	}

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
