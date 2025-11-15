using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "Levels/LevelData")]
public class LevelData : ScriptableObject
{
	public List<LevelDataCell> Cells;
	public List<LevelDataLink> Links;
	public List<Vector2Int> Floors;
	public List<Vector2Int> Holes;
	public List<Vector2Int> Walls;
	public Vector2Int Exit;
}

[System.Serializable]
public struct FloorAreaData
{
	public Vector2Int TopLeft;
	public Vector2Int BottomRight;
}