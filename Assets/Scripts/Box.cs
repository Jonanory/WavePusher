using UnityEngine;

public class Box : MonoBehaviour
{
	public Vector2Int Position;

	public void Start()
	{
		GameManager.master.CurrentLevel.Boxes.Add(Position, this);
		GameManager.master.CurrentLevel.ElementsMap.SetTile(
			(Vector3Int)Position,
			GameManager.master.CurrentLevel.BoxTile);
	}

	public bool CanPush(MapDirection _direction)
	{
		Vector2Int positionAfterPush = Map.CoordAfterMovement(
				Position,
				_direction);
		return !GameManager.master.CurrentLevel.Map.CoordIsBlocked(
			positionAfterPush
		);
	}

	public bool Push(MapDirection _direction)
	{
		if (!CanPush(_direction)) return false;
		GameManager.master.CurrentLevel.ElementsMap.SetTile(
			(Vector3Int)Position,
			null);
		Position = Map.CoordAfterMovement(
				Position,
				_direction);
		GameManager.master.CurrentLevel.ElementsMap.SetTile(
			(Vector3Int)Position,
			GameManager.master.CurrentLevel.BoxTile);
		return true;
	}
}
