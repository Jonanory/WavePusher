using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
	public GameObject LevelSelectMenu;
	public GameObject MainMenu;
	public GameObject EndMenu;
	public GameObject LevelNamePanel;

	public List<HexMenuButton> LevelButtons;
	public HexMenuButton CurrentLevelButton;
	public int TopLevelReached = 1;

	public void MoveDirection(MapDirection _direction)
	{
		if(CurrentLevelButton == null) return;

		HexMenuButton newButton = CurrentLevelButton.ButtonInDirection(_direction);
		if(newButton == null) return;
		if(!newButton.interactable) return;
		CurrentLevelButton.OnPointerExit(null);
		CurrentLevelButton = newButton;
		CurrentLevelButton.OnPointerEnter(null);
	}

	public void SelectButton()
	{
		if(CurrentLevelButton == null) return;
		CurrentLevelButton.onClick.Invoke();
	}

	public void OpenLevelSelector()
	{
		LevelNamePanel.SetActive(false);
		GameManager.master.Mode = GameMode.MENU;
		SetUnlockedLevels();
		LevelSelectMenu.SetActive(true);
		if(CurrentLevelButton != null) CurrentLevelButton.OnPointerExit(null);
		CurrentLevelButton = LevelButtons[0];
		CurrentLevelButton.OnPointerEnter(null);
	}

	void SetUnlockedLevels()
	{
		for(int i=0; i<TopLevelReached; i++)
		{
			LevelButtons[i].interactable = true;
		}
		for(int i=TopLevelReached;i < LevelButtons.Count; i++)
		{
			LevelButtons[i].interactable = false;
		}
	}

	public void CloseLevelSelector()
	{
		LevelSelectMenu.SetActive(false);
	}

	public void OpenMainMenu()
	{
		GameManager.master.Mode = GameMode.MENU;
		LevelNamePanel.SetActive(false);
		MainMenu.SetActive(true);
	}

	public void CloseMainMenu()
	{
		MainMenu.SetActive(false);
	}

	public void OpenEndMenu()
	{
		GameManager.master.Mode = GameMode.MENU;
		LevelNamePanel.SetActive(false);
		EndMenu.SetActive(true);
	}

	public void CloseEndMenu()
	{
		EndMenu.SetActive(false);
	}

	public void OpenLevelName()
	{
		LevelNamePanel.SetActive(true);
	}
}