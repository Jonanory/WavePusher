using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InGameButton
{
	public Vector2Int Position;
	public bool IsActivated = false;
	public List<IActivatable> Activatables = new List<IActivatable>();

	public void CheckCondition(bool _ignorePlayer)
	{
		if (!IsActivated)
		{
			if(GameManager.master.CurrentLevel.Boxes.ContainsKey(Position))
				IsActivated = true;
			else if(GameManager.master.CurrentLevel.Emitters.ContainsKey(Position))
				IsActivated = true;
			else if(!_ignorePlayer && GameManager.master.Player.Position == Position)
				IsActivated = true;

			if(IsActivated)
			{
				foreach (IActivatable activatable in Activatables)
				{
					activatable.Activate();
				}
			}
		}
		else if (IsActivated && (GameManager.master.Player.Position != Position && !GameManager.master.CurrentLevel.Boxes.ContainsKey(Position) && !GameManager.master.CurrentLevel.Emitters.ContainsKey(Position)))
		{
			if(!GameManager.master.CurrentLevel.Boxes.ContainsKey(Position) && !GameManager.master.CurrentLevel.Emitters.ContainsKey(Position) && (_ignorePlayer || GameManager.master.Player.Position != Position))
				IsActivated = false;

			if(!IsActivated)
			{
				foreach (IActivatable activatable in Activatables)
				{
					activatable.Deactivate();
				}
			}
		}
	}
}
