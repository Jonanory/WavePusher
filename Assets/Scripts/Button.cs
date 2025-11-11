using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Button : MonoBehaviour
{
	public Vector2Int Position;
	public bool IsActivated = false;
	public List<IActivatable> Activatables = new List<IActivatable>();

	void Start()
	{
		GameManager.master.Map.Buttons.Add(this);
	}

	public void CheckCondition()
	{
		if (!IsActivated && (GameManager.master.Player.Position == Position || GameManager.master.CurrentLevel.Boxes.ContainsKey(Position)))
		{
			IsActivated = true;
			foreach (IActivatable activatable in Activatables)
			{
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
