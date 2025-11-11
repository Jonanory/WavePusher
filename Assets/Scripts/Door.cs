using UnityEngine;

public class Door: MonoBehaviour, IActivatable, IBlockable
{
	public bool Open = false;
	public Vector2Int Position;

	public Receiver Opener;
	public Button Pusher;

	bool _Blocking;
	public bool IsBlocking{
		get { return _Blocking; }
	}

	public void Start()
	{
		GameManager.master.CurrentLevel.Doors.Add(this);
		Deactivate();
		if(Opener != null) Opener.Activatables.Add(this);
		if(Pusher != null) Pusher.Activatables.Add(this);
	}

	public void Activate()
	{
		Open = true;
		_Blocking = false;
	}

	public void Deactivate()
	{
		Open = false;
		_Blocking = true;
	}
}