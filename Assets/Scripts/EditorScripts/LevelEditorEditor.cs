#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelEditor))]
public class LevelEditorEditor : Editor {
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
							t.walls.Add(pos);
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
							t.holes.Add(pos);
						}
						break;
					default:
						var idx = t.draft.FindIndex(c => c.Position.x==pos.x && c.Position.y==pos.y);
						if (idx >= 0) {
							if(t.draft[idx].Type == (CellType)t.drawingState)
							{
								t.draft.RemoveAt(idx);
							}
							else
							{
								var c = t.draft[idx];
								c.Type = (CellType)t.drawingState;
								t.draft[idx] = c;
							}
						} else {
							t.draft.Add(
								new LevelDataCell{ 
									Position = pos, 
									Type=(CellType)t.drawingState, 
									Data=0 
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
}
#endif
