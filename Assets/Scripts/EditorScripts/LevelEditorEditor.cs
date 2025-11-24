#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelEditor))]
public class LevelEditorEditor : Editor {
	int currentMode;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		var t = (LevelEditor)target;
		EditorGUILayout.Space();
		using (new EditorGUILayout.HorizontalScope()) {
			GUI.enabled = t.levelAsset;
			if (GUILayout.Button("Load From Asset")) t.LoadFromAsset();
			if (GUILayout.Button("Bake To Asset"))  t.BakeToAsset();
			GUI.enabled = true;
		}

		currentMode = GUILayout.Toolbar(currentMode, new string[] {"Drawing", "Linking"});
	}
	
	public static Vector2Int WorldPointToCoord(Vector2 worldPoint, float _scale)
	{
		int y = Mathf.RoundToInt(worldPoint.x / 1.5f/ _scale);
		float rowParity = Mod(y, 2) / 2f;
		int x = Mathf.RoundToInt(worldPoint.y / Mathf.Sqrt(3f)/_scale - rowParity);
		return new Vector2Int(x, y);
	}

	static int Mod(int _value, int _base)
	{
		if (_value >= 0) return _value % _base;
		int valueHold = _value;
		while (valueHold < 0) valueHold += _base;
		return valueHold;
	}

	// simple click-to-paint
	void OnSceneGUI() {
		var t = (LevelEditor)target;
		if (t.draft == null) return;

		if(currentMode == 0)
		{
			DrawingMode(t);
		}
		else
		{
			LinkingMode(t);
		}
	}

	void DrawingMode(LevelEditor t)
	{
		// pick hex under mouse
		var e = Event.current;
		if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
		{
			var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
			if (new Plane(Vector3.forward, t.transform.position).Raycast(ray, out var d))
			{
				var hit = ray.GetPoint(d);
				var local = t.transform.InverseTransformPoint(hit);
				Undo.RecordObject(t, "Paint Cell");
				Vector2Int pos = WorldPointToCoord( new Vector2(local.x, local.y), t.hexSize);
				switch(t.drawingState)
				{
					case DrawingState.FLOOR:
						if(t.floors.Contains(pos)){
							t.floors.Remove(pos);
						}
						else 
						{
							if(t.walls.Contains(pos)) t.walls.Remove(pos);
							if(t.holes.Contains(pos)) t.holes.Remove(pos);
							if(t.outerWalls.Contains(pos)) t.outerWalls.Remove(pos);
							t.floors.Add(pos);
						}
						break;
					case DrawingState.WALL:
						if(t.walls.Contains(pos)) {
							t.walls.Remove(pos);
						}
						else
						{
							if(t.floors.Contains(pos)) t.floors.Remove(pos);
							if(t.holes.Contains(pos)) t.holes.Remove(pos);
							if(t.outerWalls.Contains(pos)) t.outerWalls.Remove(pos);
							t.walls.Add(pos);
						}
						break;
					case DrawingState.OUTER_WALL:
						if(t.outerWalls.Contains(pos)) {
							t.outerWalls.Remove(pos);
						}
						else
						{
							if(t.floors.Contains(pos)) t.floors.Remove(pos);
							if(t.holes.Contains(pos)) t.holes.Remove(pos);
							if(t.walls.Contains(pos)) t.walls.Remove(pos);
							t.outerWalls.Add(pos);
						}
						break;
					case DrawingState.HOLE:
						if(t.holes.Contains(pos)) {
							t.holes.Remove(pos);
						}
						else
						{
							if(t.floors.Contains(pos)) t.floors.Remove(pos);
							if(t.walls.Contains(pos)) t.walls.Remove(pos);
							if(t.outerWalls.Contains(pos)) t.outerWalls.Remove(pos);
							t.holes.Add(pos);
						}
						break;
					case DrawingState.EXIT:
						t.exit = pos;
						break;
					default:
						var idx = t.draft.FindIndex(c => c.Position.x==pos.x && c.Position.y==pos.y);
						if (idx >= 0) {
							if(t.draft[idx].Type == (CellType)t.drawingState)
							{
								if(t.draft[idx].Data != t.Data)
								{
									t.draft[idx] = 
										new LevelDataCell{
											Position = pos,
											Type=(CellType)t.drawingState,
											Data=t.Data
										};
								}
								else
								{
									t.RemoveCellAtId(idx);
								}
							}
							else
							{
								t.RemoveCellAtId(idx);
								t.draft.Add(
									new LevelDataCell{
										Position = pos,
										Type=(CellType)t.drawingState,
										Data=t.Data
								});
							}
						} else {
							t.draft.Add(
								new LevelDataCell{
									Position = pos,
									Type=(CellType)t.drawingState,
									Data=t.Data
							});
						}

						break;
				}
				EditorUtility.SetDirty(t);
				e.Use();
				SceneView.RepaintAll();
			}
		}
	}

	Vector2Int startPos;
	LevelDataCell startCell;

	void LinkingMode(LevelEditor t)
	{
		var e = Event.current;

		// Start link
		if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
		{
			if (TryPickNode(t, MouseWorldPoint(t), out var pos))
			{
				startPos = new Vector2Int(1000,1000);
				startCell = new LevelDataCell(CellType.GHOST, new Vector2Int(1000,1000), null, null);
				foreach(LevelDataCell cell in t.draft)
				{
					if(cell.Position == pos)
					{
						startPos = cell.Position;
						startCell = cell;
						e.Use();
						break;
					}
				}
			}
		}

		// Finish link
		if (startPos.x != 1000 & startPos.y != 1000 &&
			e.type == EventType.MouseUp &&
			e.button == 0)
		{
			if (TryPickNode(t, MouseWorldPoint(t), out var endPos))
			{
				LevelDataCell endCell = new LevelDataCell(
					CellType.GHOST, 
					new Vector2Int(1000,1000), 
					null, 
					null);
				foreach(LevelDataCell cell in t.draft)
				{
					if(cell.Position == endPos)
					{
						endCell = cell;
						break;
					}
				}
				if (IsValidEdge(startCell, endCell))
				{
					Undo.RecordObject(t, "Add Link");
					t.AddLink(new LevelDataLink
					{
						input = startCell,
						output = endCell
					});
					EditorUtility.SetDirty(t);
				}
			}

			startPos = new Vector2Int(1000,1000);
			e.Use();
			SceneView.RepaintAll();
		}

		// Cancel
		if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
		{
			startPos = new Vector2Int(1000,1000);
			e.Use();
		}
	}

	public Vector2 MouseWorldPoint(LevelEditor t)
	{
		var e = Event.current;
		var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
		if (new Plane(Vector3.forward, t.transform.position).Raycast(ray, out var d))
		{
			var hit = ray.GetPoint(d);
			var local = t.transform.InverseTransformPoint(hit);
			return local;
		}
		return new Vector2(1000,1000);
	}

	bool TryPickNode(LevelEditor t, Vector2 gui, out Vector2Int pos) {
		
		pos = new Vector2Int(1000,1000);
		Vector2Int mousePos = WorldPointToCoord( new Vector2(gui.x, gui.y), t.hexSize);
		foreach (var n in t.levelAsset.Cells) {
			if (n.Position == mousePos) { pos = n.Position; }
		}
		return pos != new Vector2Int(1000,1000);
	}

	bool IsSource(LevelDataCell n) => n.Type == CellType.BUTTON || n.Type == CellType.RECEIVER;
	bool IsTarget(LevelDataCell n) => n.Type == CellType.DOOR || n.Type == CellType.EMITTER;
	bool IsValidEdge(LevelDataCell from, LevelDataCell to) => IsSource(from) && IsTarget(to) && from.Position != to.Position;
}
#endif