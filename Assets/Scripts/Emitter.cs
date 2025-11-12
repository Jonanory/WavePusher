using UnityEngine;

public class Emitter : IActivatable
{
	public Vector2Int Position;
	public Vector2Int TriggerPosition;
	public int Strength;

	public void Activate()
	{
		GameManager.master.CurrentLevel.GenerateNewWave(Position, Strength);
	}

	public void Deactivate()
	{
	}
}
