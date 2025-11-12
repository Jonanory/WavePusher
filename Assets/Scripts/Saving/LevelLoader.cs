using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLoader : MonoBehaviour
{
	public LevelData testData;

	void Start()
	{
		LoadLevel(testData);
	}

	public void LoadLevel(LevelData _levelData)
	{
		GameManager.master.Map.ClearAll();
		GameManager.master.CurrentLevel.ClearAll();

		DrawFloor(_levelData);
		DrawWalls(_levelData);
		DrawHoles(_levelData);

		foreach (LevelDataCell cell in _levelData.Cells)
		{
			switch (cell.Type)
			{
				case CellType.PLAYER:
					Player newPlayer = new Player();
					newPlayer.Position = cell.Position;
					GameManager.master.Player = newPlayer;
					break;

				case CellType.RECEIVER:
					Receiver newReceiver = new Receiver();
					newReceiver.Position = cell.Position;
					GameManager.master.Map.Receivers.Add(cell.Position, newReceiver);
					break;

				case CellType.BUTTON:
					Button newButton = new Button();
					newButton.Position = cell.Position;
					GameManager.master.Map.Buttons.Add(cell.Position, newButton);
					break;

				case CellType.BOX:
					Box newBox = new Box();
					newBox.Position = cell.Position;
					GameManager.master.CurrentLevel.Boxes.Add(cell.Position, newBox);
					break;

				case CellType.EMITTER:
					Emitter newEmitter = new Emitter();
					newEmitter.Position = cell.Position;
					newEmitter.TriggerPosition = cell.SecondaryPosition;
					newEmitter.Strength = cell.Data;
					GameManager.master.Map.Emitters.Add(newEmitter);
					break;

				case CellType.DOOR:
					Door newDoor = new Door();
					newDoor.Position = cell.Position;
					GameManager.master.CurrentLevel.Doors.Add(newDoor);
					break;
			}
		}

		/* Set the activatables of buttons and receivers */
		foreach (Door door in GameManager.master.CurrentLevel.Doors)
		{
			if (GameManager.master.Map.Buttons.ContainsKey(door.TriggerPosition))
			{
				GameManager.master.Map.Buttons[door.TriggerPosition].Activatables.Add(door);
			}
			else if (GameManager.master.Map.Receivers.ContainsKey(door.TriggerPosition))
			{
				GameManager.master.Map.Receivers[door.TriggerPosition].Activatables.Add(door);
			}
			else
			{
				Debug.LogError("Door failed to find trigger object");
			}
		}

		foreach (Emitter emitter in GameManager.master.Map.Emitters)
		{
			if (GameManager.master.Map.Buttons.ContainsKey(emitter.TriggerPosition))
			{
				GameManager.master.Map.Buttons[emitter.TriggerPosition].Activatables.Add(emitter);
			}
			else if (GameManager.master.Map.Receivers.ContainsKey(emitter.TriggerPosition))
			{
				GameManager.master.Map.Receivers[emitter.TriggerPosition].Activatables.Add(emitter);
			}
			else
			{
				Debug.LogError("Door failed to find trigger object");
			}
		}
		GameManager.master.CurrentLevel.Refresh();
		GameManager.master.Map.Display();
	}
	
	void DrawFloor(LevelData _levelData)
	{
		foreach (Vector2Int floorPos in _levelData.Floors)
		{
			Tile tile;
			if (Map.Mod(floorPos.x, 3) == Map.Mod(floorPos.y + Map.Mod(floorPos.y,6) / 2, 3) )
			{
				tile = TileManager.GetTile(CellType.FLOOR_B);
			}
			else
			{
				tile = TileManager.GetTile(CellType.FLOOR);
			}
			GameManager.master.Map.AreaMap.SetTile(
				new Vector3Int(
					floorPos.x,
					floorPos.y,
					(int)MapLayer.FLOOR),
			tile);
		}
	}

	void DrawWalls(LevelData _levelData)
	{
		foreach(Vector2Int wallPos in _levelData.Walls)
			GameManager.master.Map.AreaMap.SetTile(
				new Vector3Int(
					wallPos.x,
					wallPos.y,
					(int)MapLayer.WALL),
			TileManager.master.WallTile);
	}

	void DrawHoles(LevelData _levelData)
	{
		foreach(Vector2Int holePos in _levelData.Holes)
			GameManager.master.Map.AreaMap.SetTile(
				new Vector3Int(
					holePos.x,
					holePos.y,
					(int)MapLayer.HOLE),
			TileManager.GetTile(CellType.HOLE));
	}
}
