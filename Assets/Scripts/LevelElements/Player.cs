using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		LevelState levelState = new LevelState(1);
		GameState gameState = HistoryManager.master.CaptureState();
		Vector2Int newPosition = Map.CoordAfterMovement(Position, _direction);
		Vector2Int newPushablePosition = Map.CoordAfterMovement(newPosition, _direction);
		
		if(GameManager.master.Map.CoordIsFullyBlocked(newPosition)) return;
		if(!GameManager.master.Map.PathClear(Position,_direction)) return;

		if (GameManager.master.CurrentLevel.Doors.ContainsKey(newPosition))
		{
			if(!GameManager.master.CurrentLevel.Doors[newPosition].Open && !DoorIsOpenAt(levelState, newPosition, _direction)) return;
		}

		if (GameManager.master.CurrentLevel.GetBox(newPosition) != null)
		{
			if(GameManager.master.Map.CoordIsFullyBlocked(newPushablePosition) ||
					!GameManager.master.Map.PathClear(newPosition,_direction) ||
					GameManager.master.CurrentLevel.Boxes.ContainsKey(newPushablePosition) ||
					GameManager.master.CurrentLevel.Emitters.ContainsKey(newPushablePosition)) return;

			if (GameManager.master.CurrentLevel.Doors.ContainsKey(newPushablePosition))
				if(!DoorIsOpenAt(levelState, newPushablePosition, _direction, true, false)) return;

			if (!GameManager.master.CurrentLevel.PushBox(newPosition, _direction))
				return;
		}
		else if (GameManager.master.CurrentLevel.GetEmitter(newPosition) != null)
		{
			if(GameManager.master.Map.CoordIsFullyBlocked(newPushablePosition) ||
					!GameManager.master.Map.PathClear(newPosition,_direction) ||
					GameManager.master.CurrentLevel.Boxes.ContainsKey(newPushablePosition) ||
					GameManager.master.CurrentLevel.Emitters.ContainsKey(newPushablePosition)) return;

			if (GameManager.master.CurrentLevel.Doors.ContainsKey(newPushablePosition))
				if(!DoorIsOpenAt(levelState, newPushablePosition, _direction, false, true)) return;
			if (!GameManager.master.CurrentLevel.PushEmitter(newPosition, _direction))
				return;
		}

		UndoManager.master.PushState(gameState);
		GameManager.master.CurrentLevel.TimeStep();
		Position = newPosition;
		GameManager.master.CurrentLevel.CheckButtons();
		GameManager.master.CurrentLevel.Refresh();
		GameManager.master.Camera.SetTarget(Map.CoordToWorldPoint(Position));
		GameManager.master.CurrentLevel.CheckWin();
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

	bool DoorIsOpenAt(
			LevelState _levelState,
			Vector2Int _position,
			MapDirection _direction,
			bool _pushBox = false,
			bool _pushEmitter = false)
	{
		/* Check that the door will be open by simulating the step 
		 * taken and checking the door's button/receiver */
		_levelState.MovePlayer(_direction, _pushBox, _pushEmitter);
		Door door = GameManager.master.CurrentLevel.Doors[_position];
		if(door.ButtonActivator != null)
		{
			if(!_levelState.ItemAtPosition(door.ButtonActivator.Position)) return false;
		}
		else if(door.ReceiverActivator != null)
		{
			_levelState.MakeWaves();
			if(!_levelState.WaveAtPosition(door.ReceiverActivator.Position)) return false;
		}
		else
		{
			return false;
		}
		return true;
	}
}