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
		GameManager.master.Map.Exit = _levelData.Exit;

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
					GameManager.master.CurrentLevel.Receivers.Add(cell.Position, newReceiver);
					break;

				case CellType.BUTTON:
					Button newButton = new Button();
					newButton.Position = cell.Position;
					GameManager.master.CurrentLevel.Buttons.Add(cell.Position, newButton);
					break;

				case CellType.BOX:
					Box newBox = new Box();
					newBox.Position = cell.Position;
					GameManager.master.CurrentLevel.Boxes.Add(cell.Position, newBox);
					break;

				case CellType.EMITTER:
					Emitter newEmitter = new Emitter();
					newEmitter.Position = cell.Position;
					newEmitter.Strength = cell.Data;
					GameManager.master.CurrentLevel.Emitters.Add(cell.Position, newEmitter);
					break;

				case CellType.DOOR:
					Door newDoor = new Door();
					newDoor.Position = cell.Position;
					GameManager.master.CurrentLevel.Doors.Add(cell.Position, newDoor);
					break;
			}
		}

		Door door;
		Emitter emitter;
		Receiver receiver;
		Button button;
		/* Set the activatables of buttons and receivers */
		foreach(LevelDataLink link in _levelData.Links)
		{
			switch(link.output.Type)
			{
				case CellType.DOOR:
					door = GameManager.master.CurrentLevel.Doors[link.output.Position];
					switch(link.input.Type)
					{
						case CellType.RECEIVER:
							receiver = GameManager.master.CurrentLevel.Receivers[link.input.Position];
							receiver.Activatables.Add(door);
							break;
						case CellType.BUTTON:
							button = GameManager.master.CurrentLevel.Buttons[link.input.Position];
							button.Activatables.Add(door);
							break;
					}
					break;
				case CellType.EMITTER:
					emitter = GameManager.master.CurrentLevel.Emitters[link.output.Position];
					switch(link.input.Type)
					{
						case CellType.RECEIVER:
							receiver = GameManager.master.CurrentLevel.Receivers[link.input.Position];
							receiver.Activatables.Add(emitter);
							break;
						case CellType.BUTTON:
							button = GameManager.master.CurrentLevel.Buttons[link.input.Position];
							button.Activatables.Add(emitter);
							break;
					}
					break;
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
				tile = TileManager.GetTile(TileType.FLOOR_EXTRA);
			}
			else
			{
				tile = TileManager.GetTile(TileType.FLOOR_MAIN);
			}
			TileMapManager.SceneMap.SetTile(
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
			TileMapManager.SceneMap.SetTile(
				new Vector3Int(
					wallPos.x,
					wallPos.y,
					(int)MapLayer.WALL),
				TileManager.master.WallTile);
	}

	void DrawHoles(LevelData _levelData)
	{
		foreach(Vector2Int holePos in _levelData.Holes)
			TileMapManager.SceneMap.SetTile(
				new Vector3Int(
					holePos.x,
					holePos.y,
					(int)MapLayer.HOLE),
				TileManager.master.HoleTile);
	}
}
