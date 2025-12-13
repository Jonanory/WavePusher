using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;


public class Controller : MonoBehaviour
{
	public InputActionAsset inputActionAsset;

	static string[] DirectionButtonNames = new string[]{
		"Up","Up-Right","Down-Right","Down","Down-Left","Up-Left"
	};

	public float repeatDelay = 0.25f;
	public float repeatInterval = 0.15f;
	private Coroutine[] repeatCoroutines;

	void Awake()
	{
		repeatCoroutines = new Coroutine[6];
		InputAction directionKeyAction;
		var gameplay = inputActionAsset.FindActionMap("Playing", true);
		for(int x=0;x<6;x++)
		{
			var dir = (MapDirection)x;
			directionKeyAction = gameplay.FindAction(DirectionButtonNames[x], true);
			directionKeyAction.performed += _ => MoveDirection(dir);
			directionKeyAction.canceled += _ => StopRepeat(dir);
		}

		InputAction ActionButton = gameplay.FindAction("MenuButton");
		ActionButton.performed += _ => GameManager.master.ShowLevelMenu();

		ActionButton = gameplay.FindAction("ResetButton");
		ActionButton.performed += _ => GameManager.master.LoadLevel();

		ActionButton = gameplay.FindAction("UndoButton");
		ActionButton.performed += _ => UndoAction();

		gameplay = inputActionAsset.FindActionMap("Menu", true);
		for(int x=0;x<6;x++)
		{
			var dir = (MapDirection)x;
			directionKeyAction = gameplay.FindAction(DirectionButtonNames[x], true);
			directionKeyAction.performed += _ => MenuMoveDirection(dir);
		}
	}

	public void SetControllerMap(string _newMap)
	{
		inputActionAsset.FindActionMap("Playing", true).Disable();
		inputActionAsset.FindActionMap("Menu", true).Disable();
		inputActionAsset.FindActionMap(_newMap, true).Enable();
	}

	public void MoveDirection(MapDirection _direction)
	{
		if(GameManager.master.Mode == GameMode.PLAYING)
		{
			StartRepeat(_direction);
		}
	}

	public void MenuMoveDirection(MapDirection _direction)
	{
		if(GameManager.master.Mode == GameMode.MENU)
			GameManager.master.menuManager.MoveDirection(_direction);
	}

	void OnDisable()
	{
		for (int i = 0; i < repeatCoroutines.Length; i++)
			StopRepeat((MapDirection)i);
	}

	public void UndoAction()
	{
		if (!UndoManager.master.CanUndo)
			return;

		if(GameManager.master.Mode != GameMode.PLAYING &&
				GameManager.master.Mode != GameMode.LOST)
			return;
		GameManager.master.Mode = GameMode.PLAYING;
		var prev = UndoManager.master.PopState();
		HistoryManager.master.RestoreState(prev);
	}

	private void StartRepeat(MapDirection dir)
	{
		int index = (int)dir;
		if (repeatCoroutines[index] != null)
			return;

		repeatCoroutines[index] = StartCoroutine(RepeatMove(dir, index));
	}

	public void ResetMovement()
	{
		for (int i = 0; i < repeatCoroutines.Length; i++)
			StopRepeat((MapDirection)i);
	}

	private void StopRepeat(MapDirection dir)
	{
		int index = (int)dir;
		if (repeatCoroutines[index] == null)
			return;

		StopCoroutine(repeatCoroutines[index]);
		repeatCoroutines[index] = null;
	}

	private IEnumerator RepeatMove(MapDirection dir, int index)
	{
		GameManager.master.Player.TryMove(dir);

		yield return new WaitForSeconds(repeatDelay);

		while (repeatCoroutines[index] != null)
		{
			GameManager.master.Player.TryMove(dir);
			yield return new WaitForSeconds(repeatInterval);
		}
	}
}
