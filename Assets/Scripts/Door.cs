using UnityEngine;

public class Door: IActivatable, IBlockable
{
	public bool Open = false;
	public Vector2Int Position;
	public Vector2Int TriggerPosition;


	bool _Blocking = true;
	public bool IsBlocking
	{
		get { return _Blocking; }
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