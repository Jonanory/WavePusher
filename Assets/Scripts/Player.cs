using UnityEngine;

public class Player
{
	public const int RECHARGE_LENGTH = 6;
	public int RechargeAmount;
	public Vector2Int Position;
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	public void TimeStep()
	{
		RechargeAmount++;
		if(RechargeAmount > RECHARGE_LENGTH ) RechargeAmount = RECHARGE_LENGTH;
	}

	public void TryMove(MapDirection _direction)
	{
		GameState gameState = HistoryManager.master.CaptureState();
		Vector2Int newPosition = Map.CoordAfterMovement(Position, _direction);

		if (GameManager.master.CurrentLevel.GetBox(newPosition) != null)
		{
			if (GameManager.master.CurrentLevel.PushBox(newPosition, _direction))
			{
				UndoManager.master.PushState(gameState);
				GameManager.master.CurrentLevel.TimeStep();
				Position = newPosition;
				GameManager.master.CurrentLevel.CheckButtons();
				GameManager.master.CurrentLevel.Refresh();
				GameManager.master.Camera.SetTarget(Map.CoordToWorldPoint(Position));
			}
			return;
		}

		if (GameManager.master.Map.CoordIsBlocked(newPosition)) return;
		UndoManager.master.PushState(gameState);
		GameManager.master.CurrentLevel.TimeStep();
		Position = newPosition;
		GameManager.master.CurrentLevel.CheckButtons();
		GameManager.master.CurrentLevel.Refresh();
		GameManager.master.Camera.SetTarget(Map.CoordToWorldPoint(Position));
	}

	public void TryGenerateWave()
	{
		if(RechargeAmount >= RECHARGE_LENGTH)
		{
			RechargeAmount = 0;
			GameState gameState = HistoryManager.master.CaptureState();
			UndoManager.master.PushState(gameState);
			GameManager.master.CurrentLevel.TimeStep();
			GameManager.master.CurrentLevel.GenerateNewWave(Position);
			GameManager.master.CurrentLevel.RecalculateScores();
			GameManager.master.CurrentLevel.Refresh();
		}
	}
}
