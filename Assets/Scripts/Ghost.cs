using UnityEngine;

public class Ghost
{
	const int STEPS_BETWEEN_WAVES = 7;
	public int currentSteps;
	public Vector2Int Position;

	public Ghost(Vector2Int _position, int _currentSteps = STEPS_BETWEEN_WAVES)
	{
		currentSteps = _currentSteps;
		Position = _position;
	}

	public void TimeStep()
	{
		currentSteps--;
		if(currentSteps == 0)
		{
			currentSteps = STEPS_BETWEEN_WAVES;
			GameManager.master.CurrentLevel.GenerateNewWave(Position);
		}
	}
}