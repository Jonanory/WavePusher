
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HexMenuButton : UnityEngine.UI.Button
{
	void Update()
	{
		bool test = IsInteractable();
	}
	public List<HexSelectableDirection> SelectableOptions;

	public HexMenuButton ButtonInDirection( MapDirection _direction )
	{
		foreach( HexSelectableDirection selectable in SelectableOptions )
		{
			if(selectable.Direction == _direction)
			{
				return selectable.Button;
			}
		}
		return null;
	}
}

[System.Serializable]
public class HexSelectableDirection
{
	public MapDirection Direction;
	public HexMenuButton Button;
}
