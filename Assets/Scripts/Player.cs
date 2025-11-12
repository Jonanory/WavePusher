using UnityEngine;

public class Player
{
	public Vector2Int Position;
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	public void TryMove(MapDirection _direction)
	{
		Vector2Int newPosition = Map.CoordAfterMovement(Position, _direction);

		if (GameManager.master.CurrentLevel.GetBox(newPosition) != null)
		{
			if (GameManager.master.CurrentLevel.PushBox(newPosition, _direction))
			{
				GameManager.master.CurrentLevel.TimeStep();
				Position = newPosition;
				GameManager.master.CurrentLevel.Refresh();
			}
			return;
		}

		if (GameManager.master.Map.CoordIsBlocked(newPosition)) return;
		GameManager.master.CurrentLevel.TimeStep();
		Position = newPosition;
		GameManager.master.CurrentLevel.Refresh();
	}
}
