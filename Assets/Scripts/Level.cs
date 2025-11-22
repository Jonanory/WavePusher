using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
	public List<Wave> Waves = new List<Wave>();
	public Dictionary<Vector2Int, Ghost> Ghosts = new Dictionary<Vector2Int, Ghost>();
	public Dictionary<Vector2Int, Box> Boxes = new Dictionary<Vector2Int, Box>();
	public Dictionary<Vector2Int, Door> Doors = new Dictionary<Vector2Int, Door>();
	public Dictionary<Vector2Int, int> Scores = new Dictionary<Vector2Int, int>();
	public Dictionary<Vector2Int, Emitter> Emitters = new Dictionary<Vector2Int, Emitter>();
	public Dictionary<Vector2Int, Receiver> Receivers = new Dictionary<Vector2Int, Receiver>();
	public Dictionary<Vector2Int, Button> Buttons = new Dictionary<Vector2Int, Button>();

	public void ClearAll()
	{
		TileMapManager.WaveMap.ClearAllTiles();
		TileMapManager.ScoreMap.ClearAllTiles();
		TileMapManager.GhostMap.ClearAllTiles();
		TileMapManager.OccupantMap.ClearAllTiles();
		TileMapManager.InteractableMap.ClearAllTiles();
		Waves = new List<Wave>();
		Ghosts = new Dictionary<Vector2Int, Ghost>();
		Boxes = new Dictionary<Vector2Int, Box>();
		Doors = new Dictionary<Vector2Int, Door>();
		Scores = new Dictionary<Vector2Int, int>();
		Emitters = new Dictionary<Vector2Int, Emitter>();
		Receivers = new Dictionary<Vector2Int, Receiver>();
		Buttons = new Dictionary<Vector2Int, Button>();
	}

	/* Keeps the buttons, receivers, doors and emitters intact (for undoing purposes) */
	public void ClearMost()
	{
		TileMapManager.WaveMap.ClearAllTiles();
		TileMapManager.ScoreMap.ClearAllTiles();
		TileMapManager.GhostMap.ClearAllTiles();
		TileMapManager.OccupantMap.ClearAllTiles();
		TileMapManager.InteractableMap.ClearAllTiles();
		Waves = new List<Wave>();
		Ghosts = new Dictionary<Vector2Int, Ghost>();
		Boxes = new Dictionary<Vector2Int, Box>();
		Scores = new Dictionary<Vector2Int, int>();
	}

	public void TimeStep()
	{
		foreach(Emitter emitter in Emitters.Values)
		{
			emitter.InitiateTimestep();
		}

		List<Wave> CurrentWaves = new List<Wave>();
		foreach (Wave wave in Waves)
		{
			CurrentWaves.Add(wave);
		}

		foreach (Wave wave in CurrentWaves)
		{
			if (wave.Radius > 10)
			{
				Waves.Remove(wave);
				continue;
			}
			wave.Flow();
		}

		foreach (Ghost ghost in Ghosts.Values)
		{
			ghost.TimeStep();
		}

		foreach (Button button in Buttons.Values)
		{
			button.CheckCondition(true);
		}

		RecalculateScores();
	}

	public void CheckButtons()
	{
		foreach (Button button in Buttons.Values)
		{
			button.CheckCondition(false);
		}

		foreach(Emitter emitter in Emitters.Values)
		{
			emitter.TryGenerate();
		}

		RecalculateScores();

		foreach (Receiver reciever in Receivers.Values)
		{
			reciever.TimeStep();
		}

		/* Make closed doors block waves */
		foreach (Door door in Doors.Values)
		{
			if (door.Open) continue;
			foreach (Wave wave in Waves)
			{
				if (wave.Elements.ContainsKey(door.Position))
				{
					wave.Elements.Remove(door.Position);
				}
			}
		}

		/* Make boxes block waves */
		foreach (Box box in Boxes.Values)
		{
			foreach (Wave wave in Waves)
			{
				if (wave.Elements.ContainsKey(box.Position))
					wave.Elements.Remove(box.Position);
			}
		}

		RecalculateScores();
	}

	public void RecalculateScores()
	{
		Scores = new Dictionary<Vector2Int, int>();
		foreach (Wave wave in Waves)
		{
			foreach (WaveElement element in wave.Elements.Values)
			{
				if (Scores.ContainsKey(element.Position))
				{
					Scores[element.Position] += element.Strength;
				}
				else
				{
					Scores.Add(element.Position, element.Strength);
				}
			}
		}
	}

	public void Refresh()
	{
		TileMapManager.WaveMap.ClearAllTiles();
		TileMapManager.ScoreMap.ClearAllTiles();
		TileMapManager.GhostMap.ClearAllTiles();
		TileMapManager.OccupantMap.ClearAllTiles();
		TileMapManager.InteractableMap.ClearAllTiles();

		Tile tileToUse;
		foreach (Button button in Buttons.Values)
		{
			if(button.IsActivated)
				tileToUse = TileManager.GetTile(TileType.BUTTON_PRESSED);
			else
				tileToUse = TileManager.GetTile(TileType.BUTTON);
			TileMapManager.InteractableMap.SetTile(
				new Vector3Int(
					button.Position.x,
					button.Position.y,
					0),
				tileToUse);
		}

		foreach (Emitter emitter in Emitters.Values)
		{
			if(emitter.IsActive)
				tileToUse = TileManager.GetActiveEmitterTile(emitter.currentSteps);
			else
				tileToUse = TileManager.GetTile(TileType.EMITTER);
			TileMapManager.OccupantMap.SetTile(
				new Vector3Int(
					emitter.Position.x,
					emitter.Position.y,
					0),
				tileToUse);
		}

		foreach (Receiver receiver in Receivers.Values)
		{
			if(receiver.IsActivated)
				tileToUse = TileManager.GetTile(TileType.RECEIVER_ACTIVATED);
			else
				tileToUse = TileManager.GetTile(TileType.RECEIVER);
			TileMapManager.OccupantMap.SetTile(
				new Vector3Int(
					receiver.Position.x,
					receiver.Position.y,
					0),
				tileToUse);
		}

		TileMapManager.OccupantMap.SetTile(
			new Vector3Int(
				GameManager.master.Player.Position.x,
				GameManager.master.Player.Position.y,
				0),
			TileManager.GetTile(TileType.PLAYER));

		foreach(Vector2Int boxPos in Boxes.Keys)
		{
			TileMapManager.OccupantMap.SetTile(
				new Vector3Int(
					boxPos.x,
					boxPos.y,
					0),
				TileManager.GetTile(TileType.BOX));
		}

		foreach (Wave wave in Waves)
		{
			DrawWave(wave);
		}

		foreach(Ghost ghost in Ghosts.Values)
		{
			TileMapManager.GhostMap.SetTile(
				new Vector3Int(
					ghost.Position.x,
					ghost.Position.y,
					0),
				TileManager.GetGhostTile(ghost.currentSteps));
		}

		RedrawNumbers();

		foreach(Door door in Doors.Values)
		{
			if(door.Open)
				tileToUse = TileManager.GetTile(TileType.DOOR_OPEN);
			else
				tileToUse = TileManager.GetTile(TileType.DOOR);

			TileMapManager.InteractableMap.SetTile(
				new Vector3Int(
					door.Position.x,
					door.Position.y,
					0), 
				tileToUse);
		}
		CheckWin();
	}

	public void RedrawNumbers()
	{
		foreach(KeyValuePair<Vector2Int,int> score in Scores)
		{
			int toScore = score.Value;
			if (toScore > 9) toScore = 9;
			Vector3Int position = 
				new Vector3Int(
					score.Key.x,
					score.Key.y,
					0);
			TileMapManager.ScoreMap.SetTile(
				position, 
				TileManager.GetNumberTile(toScore));
			TileMapManager.ScoreMap.SetColor(
				position,
				new Color(0f, 0f, 0.6f));
		}
	}

	void CheckWin()
	{
		if(GameManager.master.Player.Position == GameManager.master.Map.Exit)
		{
			GameManager.master.NextLevel();
		}
	}

	void DrawWave(Wave _wave)
	{
		foreach (KeyValuePair<Vector2Int, WaveElement> waveElement in _wave.Elements)
		{
			int xPlace = waveElement.Key.x + _wave.Center.x;
			int yPlace = waveElement.Key.y + _wave.Center.y;
			Vector3Int position = new Vector3Int(
				waveElement.Key.x,
				waveElement.Key.y,
				0
			);
			TileMapManager.WaveMap.SetTile(position, TileManager.GetTile(TileType.WAVE));
			TileMapManager.WaveMap.SetColor(
				position,
				new Color(1f, 1f, 1f, waveElement.Value.GetTransparency()));
		}
	}

	public int ScoreAtCoord(Vector2Int _coord)
	{
		int score = 0;
		WaveElement currentElement = null;
		foreach(Wave wave in Waves)
		{
			currentElement = wave.ElementAtCoord(_coord);
			if(currentElement != null) score += currentElement.Strength;
		}
		return score;
	}

	public void GenerateNewWave(Vector2Int _origin, int? _strength = null)
	{
		Wave newWave = new Wave(_origin);
		newWave.Init(_strength);
		Waves.Add(newWave);
		Refresh();
	}

	public void ToggleGhost(Vector2Int _position)
	{
		if (Ghosts.ContainsKey(_position))
			Ghosts.Remove(_position);
		else
			Ghosts.Add(_position, new Ghost(_position));
		Refresh();
	}

	public bool PushBox(Vector2Int _boxPosition, MapDirection _direction)
	{
		Box boxToPush = GetBox(_boxPosition);
		if (boxToPush == null)
		{
			Debug.LogError("Tried to push box that doesn't exist");
			return false;
		}
		if (!boxToPush.Push(_direction)) return false;
		Boxes.Remove(_boxPosition);
		Boxes.Add(boxToPush.Position, boxToPush);
		return true;
	}

	public Box GetBox(Vector2Int _position)
	{
		if (Boxes.ContainsKey(_position)) return Boxes[_position];
		return null;
	}
}
