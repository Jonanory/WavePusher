using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.UI
{
	[CustomEditor(typeof(HexMenuButton), true)]
	[CanEditMultipleObjects]
	public class HexMenuButtonEditor : ButtonEditor
	{
		public List<HexSelectableDirection> m_SelectableOptionsProperty;

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();
			serializedObject.Update();
			
			EditorGUILayout.PropertyField(
				serializedObject.FindProperty(
					"SelectableOptions"));

			serializedObject.ApplyModifiedProperties();
		}
	}
}
