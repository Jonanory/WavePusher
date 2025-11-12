using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
	public static TileManager master;
	[SerializeField]
	Tile PlayerTile;
	[SerializeField]
	Tile WaveTile;

	[SerializeField]
	Tile GhostTile;

	[Header("Layout")]
	[SerializeField]
	Tile FloorTile;
	[SerializeField]
	Tile FloorTileB;
	[SerializeField]
	public HexagonalRuleTile  WallTile;
	[SerializeField]
	Tile HoleTile;

	[Header("Activators")]
	[SerializeField]
	Tile ButtonTile;
	[SerializeField]
	Tile ReceiverTile;
	[SerializeField]
	Tile BoxTile;
	
	[Header("Activatable")]
	[SerializeField]
	Tile EmitterTile;
	[SerializeField]
	Tile DoorTile;
	[SerializeField]
	List<Tile> NumberTiles = new List<Tile>();

	void Awake()
	{
		if(master == null) master = this;
		else if(master != this) Destroy(gameObject);
	}

	public static Tile GetTile(CellType _type)
	{
		switch(_type)
		{
			case CellType.PLAYER:
				return master.PlayerTile;

			case CellType.HOLE:
				return master.HoleTile;
			case CellType.FLOOR:
				return master.FloorTile;
			case CellType.FLOOR_B:
				return master.FloorTileB;

			case CellType.RECEIVER:
				return master.ReceiverTile;
			case CellType.BUTTON:
				return master.ButtonTile;

			case CellType.BOX:
				return master.BoxTile;

			case CellType.EMITTER:
				return master.EmitterTile;
			case CellType.DOOR:
				return master.DoorTile;
			case CellType.WAVE:
				return master.WaveTile;
		}
		return null;
	}

	public static Tile GetNumberTile(int _number)
	{
		int number = _number;
		if(number > 9) number = 9;
		return master.NumberTiles[number];
	}
}