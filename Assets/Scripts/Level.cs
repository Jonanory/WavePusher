using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
	public Map Map;
	public List<Wave> Waves = new List<Wave>();
	public Dictionary<Vector2Int, Ghost> Ghosts = new Dictionary<Vector2Int, Ghost>();
	public Tilemap WaveMap;
	public Tile WaveTile;
	public Tile GhostTile;

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

	void RefreshGrid()
	{
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

		foreach(Ghost ghost in Ghosts.Values)
		{
			ApplyGhost(ghost);
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

	public void GenerateWave(Vector2Int _origin)
	{
		Wave newWave = new Wave(_origin);
		newWave.Init();
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
}
