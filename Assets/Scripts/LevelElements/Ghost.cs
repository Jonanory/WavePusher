using UnityEngine;

public class Ghost
{
	const int STEPS_BETWEEN_WAVES = 6;
	public int currentSteps;
	public Vector2Int Position;

	public Ghost(Vector2Int _position, int _currentSteps = 0)
	{
		currentSteps = _currentSteps;
		Position = _position;
	}

	public void TimeStep()
	{
		currentSteps++;
		if(currentSteps >= STEPS_BETWEEN_WAVES)
		{
			currentSteps = 0;
			GameManager.master.CurrentLevel.GenerateNewWave(Position,6);
		}
	}
}