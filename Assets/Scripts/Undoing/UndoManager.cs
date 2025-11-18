using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
	public static UndoManager master;
	private Stack<GameState> History = new Stack<GameState>();

	public int MaxHistory = 100;

	void Awake()
	{
		if(master == null) master = this;
		else if(master != this) Destroy(gameObject);
	}

	public void PushState(GameState _state)
	{
		History.Push(_state);
		if (History.Count > MaxHistory)
		{
			var arr = History.ToArray();
			History.Clear();
			for (int i = arr.Length - 2; i >= 0; i--)
				History.Push(arr[i]);
		}
	}

	public bool CanUndo { get { return History.Count > 0; } }

	public GameState PopState()
	{
		return History.Count > 0 ? History.Pop() : null;
	}

	public void ClearHistory()
	{
		History.Clear();
	}
}