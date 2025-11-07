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
		Vector2Int NewPosition = Map.CoordAfterMovement(Position, _direction);
		if (GameManager.master.CurrentLevel.Map.CoordIsBlocked(NewPosition)) return;
		Position = NewPosition;
		SetPosition();
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
			GameManager.master.CurrentLevel.TimeStamp();
			GameManager.master.CurrentLevel.GenerateWave(Position);
		}

		if (Keyboard.current.fKey.wasPressedThisFrame)
		{
			GameManager.master.CurrentLevel.ToggleGhost(Position);
		}
	}
	
	void SetPosition()
	{
		transform.position = Map.CoordToWorldPoint(Position);
		GameManager.master.CurrentLevel.TimeStamp();
	}
}
