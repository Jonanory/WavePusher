using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
	void Update()
	{
		if (Keyboard.current.qKey.wasPressedThisFrame)
		{
			if(GameManager.master.Mode == GameMode.PLAYING)
				GameManager.master.Player.TryMove(MapDirection.UP_LEFT);
			if(GameManager.master.Mode == GameMode.MENU)
				GameManager.master.menuManager.MoveDirection(MapDirection.UP_LEFT);
		}
		if (Keyboard.current.wKey.wasPressedThisFrame)
		{
			if(GameManager.master.Mode == GameMode.PLAYING)
				GameManager.master.Player.TryMove(MapDirection.UP);
			if(GameManager.master.Mode == GameMode.MENU)
				GameManager.master.menuManager.MoveDirection(MapDirection.UP);
		}
		if (Keyboard.current.eKey.wasPressedThisFrame)
		{
			if(GameManager.master.Mode == GameMode.PLAYING)
				GameManager.master.Player.TryMove(MapDirection.UP_RIGHT);
			if(GameManager.master.Mode == GameMode.MENU)
				GameManager.master.menuManager.MoveDirection(MapDirection.UP_RIGHT);
		}
		if (Keyboard.current.aKey.wasPressedThisFrame)
		{
			if(GameManager.master.Mode == GameMode.PLAYING)
				GameManager.master.Player.TryMove(MapDirection.DOWN_LEFT);
			if(GameManager.master.Mode == GameMode.MENU)
				GameManager.master.menuManager.MoveDirection(MapDirection.DOWN_LEFT);
		}
		if (Keyboard.current.sKey.wasPressedThisFrame)
		{
			if(GameManager.master.Mode == GameMode.PLAYING)
				GameManager.master.Player.TryMove(MapDirection.DOWN);
			if(GameManager.master.Mode == GameMode.MENU)
				GameManager.master.menuManager.MoveDirection(MapDirection.DOWN);
		}
		if (Keyboard.current.dKey.wasPressedThisFrame)
		{
			if(GameManager.master.Mode == GameMode.PLAYING)
				GameManager.master.Player.TryMove(MapDirection.DOWN_RIGHT);
			if(GameManager.master.Mode == GameMode.MENU)
				GameManager.master.menuManager.MoveDirection(MapDirection.DOWN_RIGHT);
		}

		if (Keyboard.current.spaceKey.wasPressedThisFrame)
		{
			if(GameManager.master.Mode == GameMode.PLAYING)
				GameManager.master.Player.TryGenerateWave();
			if(GameManager.master.Mode == GameMode.MENU)
				GameManager.master.menuManager.SelectButton();
		}

		if (Keyboard.current.fKey.wasPressedThisFrame)
		{
			if(GameManager.master.Mode == GameMode.PLAYING)
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


			if(GameManager.master.Mode != GameMode.PLAYING &&
					GameManager.master.Mode != GameMode.LOST)
				return;

			var prev = UndoManager.master.PopState();
			HistoryManager.master.RestoreState(prev);
		}
	}
}
