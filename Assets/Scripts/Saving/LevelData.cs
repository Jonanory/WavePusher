using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "Levels/LevelData")]
public class LevelData : ScriptableObject
{
	public string Name;
	public int Number;
	public List<LevelImage> Images;
	public List<LevelDataCell> Cells;
	public List<LevelDataLink> Links;
	public List<Vector2Int> Floors;
	public List<Vector2Int> Holes;
	public List<Vector2Int> Walls;
	public List<Vector2Int> OuterWalls;
	public Vector2Int Exit;
}

[System.Serializable]
public struct LevelImage
{
	public Vector2 Position;
	public Sprite Sprite;
	public float Scale;
}