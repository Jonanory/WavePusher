using UnityEngine;

public class Emitter : IActivatable
{
	const int STEPS_BETWEEN_WAVES = 6;
	public int currentSteps;
	public bool HasEmittedThisTimestep = true;
	public Vector2Int Position;
	public int Strength;
	public bool IsActive = false;

	public void Activate()
	{
		IsActive = true;
		currentSteps = 0;
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
}
