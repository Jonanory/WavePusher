using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Button
{
	public Vector2Int Position;
	public bool IsActivated = false;
	public List<IActivatable> Activatables = new List<IActivatable>();

	public void CheckCondition()
	{
		Debug.Log("---------------------------");
		Debug.Log(GameManager.master.Player.Position);
		Debug.Log(Position);
		if (!IsActivated && (GameManager.master.Player.Position == Position || GameManager.master.CurrentLevel.Boxes.ContainsKey(Position)))
		{
			IsActivated = true;
			foreach (IActivatable activatable in Activatables)
			{
				Debug.Log("a");
				activatable.Activate();
			}
		}
		else if (IsActivated && (GameManager.master.Player.Position != Position && !GameManager.master.CurrentLevel.Boxes.ContainsKey(Position)))
		{
			IsActivated = false;
			foreach (IActivatable activatable in Activatables)
			{
				activatable.Deactivate();
			}
		}
	}
}
