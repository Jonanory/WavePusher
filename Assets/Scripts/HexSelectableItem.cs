using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class HexSelectableItem : MonoBehaviour
{
	public List<HexSelectableDirection> Selectables;
	private Selectable self;

	void Awake()
	{
		self = GetComponent<Selectable>();
	}

	private void Update()
	{
		if (EventSystem.current == null || self == null)
			return;

		if (EventSystem.current.currentSelectedGameObject != gameObject)
			return;

		for (int i = 0; i < Selectables.Count; i++)
		{
			HexSelectableDirection option = Selectables[i];
			if (option == null || option.Selectable == null)
				continue;
/* TODO: FIXXXXXX*/
			if (Keyboard.current.qKey.wasPressedThisFrame)
			{
				option.Selectable.Select();
				break;
			}
		}
	}

	public void GoInDirection(MapDirection _direction)
	{
		foreach( HexSelectableDirection selectable in Selectables)
		{
			if(selectable.Direction == _direction)
			{
				selectable.Selectable.Select();
			}
		}
	}

}

[System.Serializable]
public class HexSelectableDirection
{
	public MapDirection Direction;
	public Selectable Selectable;
}