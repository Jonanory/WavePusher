using UnityEngine;

public class Door: IActivatable, IBlockable
{
	public bool Open = false;
	public Vector2Int Position;

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