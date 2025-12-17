using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{
	public LevelData testData;
	public GameObject LevelNamePanel;
	public TMP_Text LevelNumberText;
	public TMP_Text LevelNameText;
	public Transform ImageHolder;
	public float ImageZCoord = -1.6f;

	public void LoadLevel(LevelData _levelData)
	{
		UndoManager.master.ClearHistory();
		GameManager.master.Map.ClearAll();
		GameManager.master.CurrentLevel.ClearAll();
		GameManager.master.CurrentLevel.Time = 0;

		GameManager.master.menuManager.OpenLevelName();
		LevelNumberText.text = "LEVEL " + _levelData.Number;
		LevelNameText.text = _levelData.Name;
		LevelNamePanel.SetActive(true);

		foreach(Transform image in ImageHolder)
		{
			Destroy(image.gameObject);
		}

		foreach(LevelImage image in _levelData.Images)
		{
			CreateImageObject(image);
		}

		DrawFloor(_levelData);
		DrawHoles(_levelData);
		DrawSingleFloor(_levelData.Exit);
		GameManager.master.Map.Exit = _levelData.Exit;

		foreach (LevelDataCell cell in _levelData.Cells)
		{
			switch (cell.Type)
			{
				case CellType.PLAYER:
					Player newPlayer = new Player();
					newPlayer.Position = cell.Position;
					newPlayer.RechargeAmount = Player.RECHARGE_LENGTH;
					GameManager.master.Player = newPlayer;
					break;

				case CellType.RECEIVER:
					Receiver newReceiver = new Receiver();
					newReceiver.Position = cell.Position;
					GameManager.master.CurrentLevel.Receivers.Add(cell.Position, newReceiver);
					break;

				case CellType.BUTTON:
					InGameButton newButton = new InGameButton();
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
					newEmitter.Offset = cell.Data;
					newEmitter.IsActive = true;
					GameManager.master.CurrentLevel.Emitters.Add(cell.Position, newEmitter);
					break;

				case CellType.DOOR:
					Door newDoor = new Door();
					newDoor.Position = cell.Position;
					GameManager.master.CurrentLevel.Doors.Add(cell.Position, newDoor);
					break;
			}
		}
		DrawWalls(_levelData);
		DrawThinWalls(_levelData);

		Door door;
		Emitter emitter;
		Receiver receiver;
		InGameButton button;
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
							door.ReceiverActivator = receiver;
							break;
						case CellType.BUTTON:
							button = GameManager.master.CurrentLevel.Buttons[link.input.Position];
							button.Activatables.Add(door);
							door.ButtonActivator = button;
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
							emitter.ReceiverActivator = receiver;
							break;
						case CellType.BUTTON:
							button = GameManager.master.CurrentLevel.Buttons[link.input.Position];
							button.Activatables.Add(emitter);
							emitter.ButtonActivator = button;
							break;
					}
					break;
			}
		}

		GameManager.master.CurrentLevel.Refresh();
		GameManager.master.Map.Display();
		GameManager.master.settings.RefreshDisplay();
	}
	
	void DrawFloor(LevelData _levelData)
	{
		foreach (Vector2Int floorPos in _levelData.Floors)
			DrawSingleFloor(floorPos);
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

		int[] extents = new int[]{
			_levelData.OuterWalls[0].x,
			_levelData.OuterWalls[0].y,
			_levelData.OuterWalls[0].x,
			_levelData.OuterWalls[0].y};

		foreach(Vector2Int outerWallPos in _levelData.OuterWalls)
		{
			TileMapManager.SceneMap.SetTile(
				new Vector3Int(
					outerWallPos.x,
					outerWallPos.y,
					(int)MapLayer.WALL),
				TileManager.master.WallTile);
			TileMapManager.SceneMap.SetColor(
				new Vector3Int(
					outerWallPos.x,
					outerWallPos.y,
					(int)MapLayer.WALL),
				new Color(1f, 1f, 1f,0.0f));

			if(outerWallPos.x < extents[0]) extents[0] = outerWallPos.x;
			if(outerWallPos.x > extents[2]) extents[2] = outerWallPos.x;
			if(outerWallPos.y < extents[3]) extents[3] = outerWallPos.y;
			if(outerWallPos.y > extents[1]) extents[1] = outerWallPos.y;
		}

		Vector2 bottomLeft = Map.CoordToWorldPoint(extents[0],extents[3]);
		Vector2 topRight = Map.CoordToWorldPoint(extents[2],extents[1]);

		GameManager.master.Camera.SetLevelRect(
			bottomLeft.x,
			topRight.y,
			topRight.x,
			bottomLeft.y
		);
	}

	void DrawThinWalls(LevelData _levelData)
	{
		foreach(BlockedPath element in _levelData.ThinWalls)
		{
			if(!GameManager.master.Map.BlockedPaths.ContainsKey(element.Position))
			{
				GameManager.master.Map.BlockedPaths.Add(element.Position, new HashSet<MapDirection>());
			}
			foreach(MapDirection direction in element.Directions)
			{
				GameManager.master.Map.BlockedPaths[element.Position].Add(direction);
				TileMapManager.ThinWallMap.SetTile(
					new Vector3Int(
						element.Position.x,
						element.Position.y,
						(int)direction),
					TileManager.GetThinWall(direction));
			}
		}
	}

	void DrawSingleFloor(Vector2Int _position)
	{
		Color floorColor;
		Tile tile;
		if (Map.Mod(_position.x, 3) == Map.Mod(_position.y + Map.Mod(_position.y,6) / 2, 3) )
		{
			floorColor = new Color(0.75f,0.75f,0.75f,1f);
		}
		else
		{
			floorColor = new Color(.84f,0.84f,0.84f,1f);
		}
		TileMapManager.SceneMap.SetTile(
			new Vector3Int(
				_position.x,
				_position.y,
				(int)MapLayer.FLOOR),
			TileManager.GetTile(TileType.FLOOR_MAIN));
		TileMapManager.SceneMap.SetColor(
			new Vector3Int(
				_position.x,
				_position.y,
				(int)MapLayer.FLOOR),
			floorColor);
	}

	void DrawHoles(LevelData _levelData)
	{
		foreach(Vector2Int holePos in _levelData.Holes)
		{
			TileMapManager.SceneMap.SetTile(
				new Vector3Int(
					holePos.x,
					holePos.y,
					(int)MapLayer.HOLE),
				TileManager.GetTile(TileType.HOLE));
			TileMapManager.SceneMap.SetColor(
				new Vector3Int(
					holePos.x,
					holePos.y,
					(int)MapLayer.HOLE),
				new Color(0f,0f,0f));
		}
	}

	public void CreateImageObject(LevelImage _image)
	{
		GameObject obj = new GameObject("SpriteObject");
		obj.transform.SetParent(ImageHolder);
		obj.transform.position = new Vector3(
			_image.Position.x,
			_image.Position.y,
			ImageZCoord);
		obj.transform.localScale = new Vector3(
			_image.Scale,
			_image.Scale,
			_image.Scale
		);

		SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
		sr.sprite = _image.Sprite;
	}
}
