using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
	void Update()
	{
		if (Keyboard.current.qKey.wasPressedThisFrame)
			GameManager.master.Player.TryMove(MapDirection.UP_LEFT);
		if (Keyboard.current.wKey.wasPressedThisFrame)
			GameManager.master.Player.TryMove(MapDirection.UP);
		if (Keyboard.current.eKey.wasPressedThisFrame)
			GameManager.master.Player.TryMove(MapDirection.UP_RIGHT);
		if (Keyboard.current.aKey.wasPressedThisFrame)
			GameManager.master.Player.TryMove(MapDirection.DOWN_LEFT);
		if (Keyboard.current.sKey.wasPressedThisFrame)
			GameManager.master.Player.TryMove(MapDirection.DOWN);
		if (Keyboard.current.dKey.wasPressedThisFrame)
			GameManager.master.Player.TryMove(MapDirection.DOWN_RIGHT);

		if (Keyboard.current.spaceKey.wasPressedThisFrame)
		{
			GameState gameState = HistoryManager.master.CaptureState();
			UndoManager.master.PushState(gameState);
			GameManager.master.CurrentLevel.TimeStep();
			GameManager.master.CurrentLevel.GenerateNewWave(GameManager.master.Player.Position);
			GameManager.master.CurrentLevel.RecalculateScores();
			GameManager.master.CurrentLevel.Refresh();
		}

		if (Keyboard.current.fKey.wasPressedThisFrame)
		{
			GameManager.master.CurrentLevel.ToggleGhost(GameManager.master.Player.Position);
		}

		if (Keyboard.current.rKey.wasPressedThisFrame)
		{
			GameManager.master.LoadLevel();
		}

		if(Keyboard.current.escapeKey.wasPressedThisFrame)
		{
			GameManager.master.ShowLevelMenu();
		}

		if(Keyboard.current.zKey.wasPressedThisFrame)
		{
			if (!UndoManager.master.CanUndo)
				return;

			var prev = UndoManager.master.PopState();
			HistoryManager.master.RestoreState(prev);
		}
	}
}
