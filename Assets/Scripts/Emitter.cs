using UnityEngine;

public class Emitter : MonoBehaviour, IActivatable
{
	public Vector2Int Position;
	public int Strength;
	public Receiver Trigger;

	public void Start()
	{
		Deactivate();
		Trigger.Activatables.Add(this);

		GameManager.master.CurrentLevel.ElementsMap.SetTile(
			(Vector3Int)Position,
			GameManager.master.CurrentLevel.EmitterTile);
	}

	public void Activate()
	{
		GameManager.master.CurrentLevel.GenerateWave(Position, Strength);
	}

	public void Deactivate()
	{
	}
}
