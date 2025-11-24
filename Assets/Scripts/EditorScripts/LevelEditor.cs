using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using System.Collections.Generic;

public enum DrawingState
{
	PLAYER = 0,
	WALL = 1,
	FLOOR = 2,
	RECEIVER = 4,
	BUTTON = 5,
	BOX = 6,
	EMITTER = 7,
	DOOR = 8,
	HOLE = 10,
	EXIT = 12,
	OUTER_WALL = 13,
}
[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour {
	public LevelData levelAsset;

	[HideInInspector] public List<LevelDataCell> draft = new();
	[HideInInspector] public List<Vector2Int> floors = new();
	[HideInInspector] public List<Vector2Int> walls = new();
	[HideInInspector] public List<Vector2Int> outerWalls = new();
	[HideInInspector] public List<Vector2Int> holes = new();
	[HideInInspector] public Vector2Int exit = new Vector2Int(0,0);
	[HideInInspector] public List<LevelDataLink> links = new();

	public List<Color> colorOptions;
	Dictionary<Vector2Int, int> ActivatorColors = new Dictionary<Vector2Int, int>();

	public float hexSize = .5f;
	public int Data = 0;
	public DrawingState drawingState;
	Vector3 AxialToWorld(int q, int r)
	{
		return new Vector3(
			r * 1.5f * hexSize,
			(q + Mod(r, 2) / 2f) * Mathf.Sqrt(3) * hexSize,
			0
		);
	}

	int Mod(int _value, int _base)
	{
		if (_value >= 0) return _value % _base;
		int valueHold = _value;
		while (valueHold < 0) valueHold += _base;
		return valueHold;
	}

#if UNITY_EDITOR
	public void LoadFromAsset() {
		draft = levelAsset ? new List<LevelDataCell>(levelAsset.Cells) : new List<LevelDataCell>();
		floors = levelAsset ? new List<Vector2Int>(levelAsset.Floors) : new List<Vector2Int>();
		walls = levelAsset ? new List<Vector2Int>(levelAsset.Walls) : new List<Vector2Int>();
		outerWalls = levelAsset ? new List<Vector2Int>(levelAsset.OuterWalls) : new List<Vector2Int>();
		holes = levelAsset ? new List<Vector2Int>(levelAsset.Holes) : new List<Vector2Int>();
		links = levelAsset ? new List<LevelDataLink>(levelAsset.Links) : new List<LevelDataLink>();
		exit = levelAsset ? levelAsset.Exit : new Vector2Int(0,0);
		SceneView.RepaintAll();
	}

	public void BakeToAsset() {
		if (!levelAsset) return;
		Undo.RecordObject(levelAsset, "Bake Level");
		levelAsset.Cells = new List<LevelDataCell>(draft);
		levelAsset.Floors = new List<Vector2Int>(floors);
		if(levelAsset.Floors.Contains(exit)) levelAsset.Floors.Remove(exit);
		levelAsset.Walls = new List<Vector2Int>(walls);
		if(levelAsset.Walls.Contains(exit)) levelAsset.Walls.Remove(exit);
		levelAsset.OuterWalls = new List<Vector2Int>(outerWalls);
		if(levelAsset.OuterWalls.Contains(exit)) levelAsset.OuterWalls.Remove(exit);
		levelAsset.Holes = new List<Vector2Int>(holes);
		if(levelAsset.Holes.Contains(exit)) levelAsset.Holes.Remove(exit);
		levelAsset.Links = new List<LevelDataLink>(links);
		levelAsset.Exit = exit;
		EditorUtility.SetDirty(levelAsset);
		AssetDatabase.SaveAssets();
	}

	public void RemoveCellAtId(int idx)
	{
		LevelDataCell cell = draft[idx];
		List<LevelDataLink> newLinks = new List<LevelDataLink>();
		if(cell.Type != CellType.BUTTON && 
				cell.Type != CellType.RECEIVER && 
				cell.Type != CellType.EMITTER && 
				cell.Type != CellType.DOOR)
		{
			draft.RemoveAt(idx);
			return;
		}
		foreach(LevelDataLink link in links)
		{
			if(!link.input.Equals(cell) && !link.output.Equals(cell))
				newLinks.Add(link);
		}
		links = newLinks;
		draft.RemoveAt(idx);
	}

	public void AddLink(LevelDataLink _newLink)
	{
		for(int i=0; i<links.Count;i++)
		{
			if(links[i].output.Equals(_newLink.output))
			{
				links[i]=_newLink;
				return;
			}
		}
		links.Add(_newLink);
	}

	void OnDrawGizmos() {
		ActivatorColors = new Dictionary<Vector2Int, int>();
		int activatorColorIndex = 0;
		if (draft == null) return;
		foreach (var c in draft) {
			var pos = AxialToWorld(c.Position.x, c.Position.y);
			string icon = "Floor.tiff";
			switch(c.Type)
			{
				case CellType.PLAYER:
					icon = "Player.tiff";
					break;
				case CellType.RECEIVER:
					ActivatorColors.Add(
						c.Position,
						activatorColorIndex++);
					icon = "Receiver.tif";
					break;
				case CellType.DOOR:
					icon = "Door.tif";
					break;
				case CellType.EMITTER:
					icon = "Emitter.tif";
					break;
				case CellType.BUTTON:
					ActivatorColors.Add(
						c.Position,
						activatorColorIndex++);
					icon = "Button.tif";
					break;
				case CellType.BOX:
					icon = "Box.tiff";
					break;
			}
			Gizmos.DrawIcon(pos, icon);
		}

		foreach (var c in floors) {
			if(c.x == exit.x && c.y == exit.y) continue;
			var pos = AxialToWorld(c.x, c.y);
			pos.z = 2;
			Gizmos.DrawIcon(pos, "Floor.tiff");
		}

		foreach (var c in walls) {
			var pos = AxialToWorld(c.x, c.y);
			pos.z = 2;
			Gizmos.DrawIcon(pos, "Wall.tif");
		}

		foreach (var c in outerWalls) {
			var pos = AxialToWorld(c.x, c.y);
			pos.z = 2;
			Gizmos.DrawIcon(pos, "OuterWall.tif");
		}

		foreach (var c in holes) {
			if(c.x == exit.x && c.y == exit.y) continue;
			var pos = AxialToWorld(c.x, c.y);
			pos.z = 2;
			Gizmos.DrawIcon(pos, "Hole.tif");
		}

		var exitPos = AxialToWorld(exit.x, exit.y);
		exitPos.z = 2;
		Gizmos.DrawIcon(exitPos, "Exit.tiff");

		foreach (var c in links) {
			var posInput = AxialToWorld(
				c.input.Position.x,
				c.input.Position.y);
			var posOutput = AxialToWorld(
				c.output.Position.x,
				c.output.Position.y);
			Gizmos.DrawIcon(
				new Vector3(posInput.x, posInput.y, posInput.z),
				"Connector.tiff",true,
				colorOptions[ActivatorColors[c.input.Position]]);
			Gizmos.DrawIcon(
				new Vector3(posOutput.x, posOutput.y, posOutput.z),
				"Connector.tiff",true,
				colorOptions[ActivatorColors[c.input.Position]]);
		}
	}
#endif
}
