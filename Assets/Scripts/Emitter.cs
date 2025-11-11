using UnityEngine;

public class Emitter : MonoBehaviour, IActivatable
{
	public Vector2Int Position;
	public int Strength;
	public Receiver Trigger;
	public Button Pusher;

	public void Start()
	{
		GameManager.master.Map.Emitters.Add(this);
		Deactivate();
		Trigger.Activatables.Add(this);
		if(Pusher != null) Pusher.Activatables.Add(this);
	}

	public void Activate()
	{
		GameManager.master.CurrentLevel.GenerateNewWave(Position, Strength);
	}

	public void Deactivate()
	{
	}
}
