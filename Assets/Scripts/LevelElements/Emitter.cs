using UnityEngine;

public class Emitter : IActivatable
{
	public const int STEPS_BETWEEN_WAVES = 6;
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
			currentSteps++;
			if(currentSteps>=STEPS_BETWEEN_WAVES)
			{
				currentSteps = 0;
				HasEmittedThisTimestep = true;
				GameManager.master.CurrentLevel.GenerateNewWave(
					Position, 
					Strength);
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

	public bool Push(MapDirection _direction)
	{
		// if (!CanPush(_direction)) return false;
		Position = Map.CoordAfterMovement(
				Position,
				_direction);
		return true;
	}
}
