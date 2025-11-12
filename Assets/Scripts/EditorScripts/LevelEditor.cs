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
	HOLE = 10
}
[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour {
	public LevelData levelAsset;

	[HideInInspector] public List<LevelDataCell> draft = new();
	[HideInInspector] public List<Vector2Int> floors = new();
	[HideInInspector] public List<Vector2Int> walls = new();
	[HideInInspector] public List<Vector2Int> holes = new();

	public float hexSize = .5f;
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
		holes = levelAsset ? new List<Vector2Int>(levelAsset.Holes) : new List<Vector2Int>();
		SceneView.RepaintAll();
	}

	public void BakeToAsset() {
		if (!levelAsset) return;
		Undo.RecordObject(levelAsset, "Bake Level");
		levelAsset.Cells = new List<LevelDataCell>(draft);
		levelAsset.Floors = new List<Vector2Int>(floors);
		levelAsset.Walls = new List<Vector2Int>(walls);
		levelAsset.Holes = new List<Vector2Int>(holes);
		EditorUtility.SetDirty(levelAsset);
		AssetDatabase.SaveAssets();
	}

	void OnDrawGizmos() {
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
					icon = "Receiver.tif";
					break;
				case CellType.DOOR:
					icon = "Door.tif";
					break;
				case CellType.EMITTER:
					icon = "Emitter.tif";
					break;
				case CellType.BUTTON:
					icon = "Button.tif";
					break;
				case CellType.BOX:
					icon = "Box.tiff";
					break;
			}
			Gizmos.DrawIcon(pos, icon);
		}

		foreach (var c in floors) {
			var pos = AxialToWorld(c.x, c.y);
			pos.z = 2;
			Gizmos.DrawIcon(pos, "Floor.tiff");
		}

		foreach (var c in walls) {
			var pos = AxialToWorld(c.x, c.y);
			pos.z = 2;
			Gizmos.DrawIcon(pos, "Wall.tif");
		}

		foreach (var c in holes) {
			var pos = AxialToWorld(c.x, c.y);
			pos.z = 2;
			Gizmos.DrawIcon(pos, "Hole.tif");
		}
	}
#endif
}
