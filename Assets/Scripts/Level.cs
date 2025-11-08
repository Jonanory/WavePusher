using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
	public Map Map;
	public List<Wave> Waves = new List<Wave>();
	public Dictionary<Vector2Int, Ghost> Ghosts = new Dictionary<Vector2Int, Ghost>();
	public Dictionary<Vector2Int, Box> Boxes = new Dictionary<Vector2Int, Box>();
	public Tilemap WaveMap;
	public Tilemap ScoreMap;
	public Tilemap ElementsMap;
	public Tile WaveTile;
	public Tile GhostTile;
	public Tile ReceiverTile;
	public Tile EmitterTile;
	public Tile DoorTile;
	public Tile BoxTile;
	public List<Tile> NumberTiles = new List<Tile>();

	public void TimeStamp()
	{
		foreach (Wave wave in Waves)
		{
			wave.TimeStamp();
		}
		foreach (Ghost ghost in Ghosts.Values)
		{
			ghost.TimeStamp();
		}
		RefreshGrid();
	}
	Dictionary<Vector2Int, int> Scores;
	public void RefreshGrid()
	{
		Scores = new Dictionary<Vector2Int, int>();
		ScoreMap.ClearAllTiles();
		WaveMap.ClearAllTiles();

		List<Wave> CurrentWaves = new List<Wave>();
		foreach (Wave wave in Waves)
		{
			CurrentWaves.Add(wave);
		}

		foreach (Wave wave in CurrentWaves)
		{
			ApplyWave(wave);
		}

		foreach (Ghost ghost in Ghosts.Values)
		{
			ApplyGhost(ghost);
		}

		foreach(Receiver reciever in Map.Receivers)
		{
			reciever.TimeStep();
		}
		
		foreach(KeyValuePair<Vector2Int,int> score in Scores)
		{
			int toScore = score.Value;
			if (toScore > 9) toScore = 9;
			ScoreMap.SetTile((Vector3Int)score.Key, NumberTiles[toScore]);
			ScoreMap.SetColor(
				(Vector3Int)score.Key,
				new Color(0f, 0f, 0.6f));
		}
	}

	void ApplyWave(Wave _wave)
	{
		if (_wave.Radius > 10)
		{
			Waves.Remove(_wave);
			return;
		}

		foreach (KeyValuePair<Vector2Int, WaveElement> waveElement in _wave.Elements)
		{
			int xPlace = waveElement.Key.x + _wave.Center.x;
			int yPlace = waveElement.Key.y + _wave.Center.y;
			WaveMap.SetTile((Vector3Int)waveElement.Key, WaveTile);
			WaveMap.SetColor(
				(Vector3Int)waveElement.Key,
				new Color(1f, 1f, 1f, waveElement.Value.GetTransparency()));
			if (Scores.ContainsKey(waveElement.Key))
			{
				Scores[waveElement.Key] += waveElement.Value.Strength;
			}
			else
			{
				Scores.Add(waveElement.Key, waveElement.Value.Strength);
			}
		}
	}

	void ApplyGhost(Ghost _ghost)
	{
		WaveMap.SetTile((Vector3Int)_ghost.Position, GhostTile);
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

	public void GenerateWave(Vector2Int _origin, int? _strength = null)
	{
		Wave newWave = new Wave(_origin);
		newWave.Init(_strength);
		Waves.Add(newWave);
		RefreshGrid();
	}

	public void ToggleGhost(Vector2Int _position)
	{
		if (Ghosts.ContainsKey(_position))
			Ghosts.Remove(_position);
		else
			Ghosts.Add(_position, new Ghost(_position));
		RefreshGrid();
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
