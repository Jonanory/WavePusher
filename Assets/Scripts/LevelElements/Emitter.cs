using UnityEngine;

public class Emitter : IActivatable
{
	public const int STEPS_BETWEEN_WAVES = 6;
	public const int START_STRENGTH = 12;
	public int Offset = 0;
	public int currentSteps;
	public bool HasEmittedThisTimestep = true;
	public Vector2Int Position;
	public int Strength;
	public bool IsActive = false;

	InGameButton _buttonActivator;
	public InGameButton ButtonActivator
	{
		get { return _buttonActivator; }
		set { _buttonActivator = value; }
	}
	Receiver _receiverActivator;
	public Receiver ReceiverActivator
	{
		get { return _receiverActivator; }
		set { _receiverActivator = value; }
	}

	public void Activate()
	{
		IsActive = true;
		currentSteps = STEPS_BETWEEN_WAVES;
	}

	public void Deactivate()
	{
		IsActive = false;
	}

	public bool TryGenerate()
	{
		if(IsActive && !HasEmittedThisTimestep)
		{
			if(GameManager.master.CurrentLevel.Time % STEPS_BETWEEN_WAVES == Offset)
			{
				HasEmittedThisTimestep = true;
				GameManager.master.CurrentLevel.GenerateNewWave(
					Position, 
					START_STRENGTH);
			}
			return true;
		}
		return false;
	}

	public void InitiateTimestep()
	{
		HasEmittedThisTimestep = false;
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

	public int StepsTilEmit()
	{
		return (GameManager.master.CurrentLevel.Time + STEPS_BETWEEN_WAVES - Offset) % STEPS_BETWEEN_WAVES;
	}

	public bool Push(MapDirection _direction)
	{
		// if (!CanPush(_direction)) return false;
		Position = Map.CoordAfterMovement(
				Position,
				_direction);
		return true;
	}
}
