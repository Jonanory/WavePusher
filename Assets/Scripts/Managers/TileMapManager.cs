using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class TileMapManager : MonoBehaviour
{
	public static TileMapManager master;

	[SerializeField]
	Tilemap _sceneMap;
	public static Tilemap SceneMap { get{ return master._sceneMap; }}

	[SerializeField]
	Tilemap _occupantMap;
	public static Tilemap OccupantMap { get{ return master._occupantMap; }}

	[SerializeField]
	Tilemap _interactableMap;
	public static Tilemap InteractableMap { get{ return master._interactableMap; }}

	[SerializeField]
	Tilemap _waveMap;
	public static Tilemap WaveMap { get{ return master._waveMap; }}

	[SerializeField]
	Tilemap _scoreMap;
	public static Tilemap ScoreMap { get{ return master._scoreMap; }}

	void Awake()
	{
		if(master == null) master = this;
		else if (master != this) Destroy(gameObject);
	}
}