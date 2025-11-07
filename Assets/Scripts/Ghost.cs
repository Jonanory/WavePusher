using UnityEngine;

public class Ghost
{
	const int STEPS_BETWEEN_WAVES = 7;
	int currentSteps;
	public Vector2Int Position;

	public Ghost(Vector2Int _position)
	{
		currentSteps = STEPS_BETWEEN_WAVES;
		Position = _position;
	}

	public void TimeStamp()
	{
		currentSteps--;
		if(currentSteps == 0)
		{
			currentSteps = STEPS_BETWEEN_WAVES;
			GameManager.master.CurrentLevel.GenerateWave(Position);
		}
	}
}