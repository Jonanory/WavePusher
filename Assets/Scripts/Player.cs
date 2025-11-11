using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	public Vector2Int Position;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Position = new Vector2Int(0, 0);
		transform.position = Map.CoordToWorldPoint(Position);
	}

	void TryMove(MapDirection _direction)
	{
		Vector2Int newPosition = Map.CoordAfterMovement(Position, _direction);

		if (GameManager.master.CurrentLevel.GetBox(newPosition))
		{
			if (GameManager.master.CurrentLevel.PushBox(newPosition, _direction))
			{
				GameManager.master.CurrentLevel.TimeStep();
				Position = newPosition;
				SetPosition(true);
			}
			return;
		}

		if (GameManager.master.Map.CoordIsBlocked(newPosition)) return;
		GameManager.master.CurrentLevel.TimeStep();
		Position = newPosition;
		SetPosition(true);
	}

	// Update is called once per frame
	void Update()
	{
		if (Keyboard.current.qKey.wasPressedThisFrame)
			TryMove(MapDirection.UP_LEFT);
		if (Keyboard.current.wKey.wasPressedThisFrame)
			TryMove(MapDirection.UP);
		if (Keyboard.current.eKey.wasPressedThisFrame)
			TryMove(MapDirection.UP_RIGHT);
		if (Keyboard.current.aKey.wasPressedThisFrame)
			TryMove(MapDirection.DOWN_LEFT);
		if (Keyboard.current.sKey.wasPressedThisFrame)
			TryMove(MapDirection.DOWN);
		if (Keyboard.current.dKey.wasPressedThisFrame)
			TryMove(MapDirection.DOWN_RIGHT);

		if (Keyboard.current.spaceKey.wasPressedThisFrame)
		{
			GameManager.master.CurrentLevel.TimeStep();
			GameManager.master.CurrentLevel.GenerateNewWave(Position);
		}

		if (Keyboard.current.fKey.wasPressedThisFrame)
		{
			GameManager.master.CurrentLevel.ToggleGhost(Position);
		}
	}

	void SetPosition(bool _forceRefresh = false)
	{
		transform.position = Map.CoordToWorldPoint(Position);
		if(_forceRefresh) GameManager.master.CurrentLevel.Refresh();
	}
}
