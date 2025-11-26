using UnityEngine;

public interface IActivatable
{
	public InGameButton ButtonActivator {get;set;}
	public Receiver ReceiverActivator {get;set;}
	public void Activate();
	public void Deactivate();
}
