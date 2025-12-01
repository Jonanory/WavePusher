using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Receiver
{
	public int ScoreNeeded = 1;
	public Vector2Int Position;
	public bool IsActivated = false;
	public List<IActivatable> Activatables = new List<IActivatable>();

	public void TimeStep()
	{
		if (!IsActivated && GameManager.master.CurrentLevel.ScoreAtCoord(Position) >= ScoreNeeded)
		{
			IsActivated = true;
			foreach (IActivatable activatable in Activatables)
			{
				activatable.Activate();
			}
		}
		else if (IsActivated && GameManager.master.CurrentLevel.ScoreAtCoord(Position) < ScoreNeeded)
		{
			IsActivated = false;
			foreach (IActivatable activatable in Activatables)
			{
				activatable.Deactivate();
			}
		}
	}
}
