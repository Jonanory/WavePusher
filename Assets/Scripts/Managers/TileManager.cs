using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public enum TileType
{
	PLAYER,
	PLAYER_LOSE,
	WAVE,
	GHOST_0,
	GHOST_1,
	GHOST_2,
	GHOST_3,
	GHOST_4,
	GHOST_5,
	GHOST_6,
	FLOOR_MAIN,
	FLOOR_EXTRA,
	EXIT,
	BUTTON,
	BUTTON_PRESSED,
	RECEIVER,
	RECEIVER_ACTIVATED,
	BOX,
	EMITTER,
	EMITTER_ACTIVE_0,
	EMITTER_ACTIVE_1,
	EMITTER_ACTIVE_2,
	EMITTER_ACTIVE_3,
	EMITTER_ACTIVE_4,
	EMITTER_ACTIVE_5,
	EMITTER_ACTIVE_6,
	DOOR,
	DOOR_OPEN,
	HOLE
}

public class TileManager : MonoBehaviour
{
	public static TileManager master;
	[SerializeField]
	Tile PlayerTile;
	[SerializeField]
	Tile PlayerTile_Lose;
	[SerializeField]
	List<Tile> WaveTiles = new List<Tile>();

	[SerializeField]
	List<Tile> GhostTiles = new List<Tile>();

	[Header("Layout")]
	[SerializeField]
	Tile FloorTile;
	[SerializeField]
	Tile FloorTileB;
	[SerializeField]
	public HexagonalRuleTile  WallTile;
	[SerializeField]
	Tile HoleTile;
	[SerializeField]
	Tile ExitTile;

	[Header("Activators")]
	[SerializeField]
	Tile ButtonTile;
	[SerializeField]
	Tile ButtonTile_Pressed;
	[SerializeField]
	Tile ReceiverTile;
	[SerializeField]
	Tile ReceiverTile_Activated;
	[SerializeField]
	Tile BoxTile;
	
	[Header("Activatable")]
	[SerializeField]
	List<Tile> EmitterTiles = new List<Tile>();
	[SerializeField]
	Tile DoorTile;
	[SerializeField]
	Tile DoorTile_Open;
	[SerializeField]
	List<Tile> NumberTiles = new List<Tile>();

	void Awake()
	{
		if(master == null) master = this;
		else if(master != this) Destroy(gameObject);
	}

	public static Tile GetTile(TileType _type)
	{
		switch(_type)
		{
			case TileType.FLOOR_MAIN:
				return master.FloorTile;
			case TileType.FLOOR_EXTRA:
				return master.FloorTileB;
			case TileType.EXIT:
				return master.ExitTile;

			case TileType.PLAYER:
				return master.PlayerTile;
			case TileType.PLAYER_LOSE:
				return master.PlayerTile_Lose;

			case TileType.RECEIVER:
				return master.ReceiverTile;
			case TileType.RECEIVER_ACTIVATED:
				return master.ReceiverTile_Activated;
			case TileType.BUTTON:
				return master.ButtonTile;
			case TileType.BUTTON_PRESSED:
				return master.ButtonTile_Pressed;

			case TileType.BOX:
				return master.BoxTile;

			case TileType.HOLE:
				return master.HoleTile;

			case TileType.EMITTER:
				return master.EmitterTiles[0];
			case TileType.DOOR:
				return master.DoorTile;
			case TileType.DOOR_OPEN:
				return master.DoorTile_Open;
		}
		return null;
	}

	public static Tile GetWaveTile(int _number)
	{
		if(_number >=0 && _number < 6)
			return master.WaveTiles[_number];
		else
			return master.WaveTiles[0];
	}

	public static Tile GetGhostTile(int _number)
	{
		if(_number >=0 && _number < 7)
			return master.GhostTiles[_number];
		else
			return master.GhostTiles[0];
	}

	public static Tile GetActiveEmitterTile(int _number)
	{
		if(_number >=0 && _number < 7)
			return master.EmitterTiles[_number+1];
		else
			return master.EmitterTiles[0];
	}

	public static Tile GetNumberTile(int _number)
	{
		int number = _number;
		if(number > 9) number = 9;
		return master.NumberTiles[number];
	}
}