using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Receiver : MonoBehaviour
{
	public int ScoreNeeded = 4;
	public Vector2Int Position;
	public bool IsActivated = false;
	public List<IActivatable> Activatables = new List<IActivatable>();

	public void Start()
	{
		GameManager.master.CurrentLevel.ElementsMap.SetTile(
			(Vector3Int)Position,
			GameManager.master.CurrentLevel.ReceiverTile);
	}

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
