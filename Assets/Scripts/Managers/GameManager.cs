using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameMode{PLAYING, MENU, LOST, WIN}
public class GameManager : MonoBehaviour
{
	public static GameManager master;
	public Player Player;
	public Level CurrentLevel;
	public CameraMover Camera;
	public MenuManager menuManager;
	public SfxManager sfxManager;
	public Controller controller;

	public List<LevelData> Levels = new List<LevelData>();
	public int LevelIndex = 0;
	public LevelLoader Loader;
	public Map Map;
	public GameObject LevelSelectMenu;
	public GameMode Mode = GameMode.MENU;

	public bool AlwaysShowGuide = false;
	public bool ShowGuide
	{
		get {return LevelIndex == 0 || AlwaysShowGuide;}
	}

	public void Awake()
	{
		if(master == null) master = this;
		else if(master != this) Destroy(gameObject);
	}

	void Start()
	{
		menuManager.CloseLevelSelector();
		menuManager.OpenMainMenu();
		menuManager.CloseEndMenu();
	}

	public void LoadLevel()
	{
		menuManager.CloseMainMenu();
		menuManager.CloseEndMenu();
		menuManager.CloseLevelSelector();
		Mode = GameMode.PLAYING;
		controller.SetControllerMap("Playing");
		Loader.LoadLevel(Levels[LevelIndex]);
	}

	public void WinLevel()
	{
		Mode = GameMode.WIN;
		SfxManager.PlaySfx(Sfx.WIN);
		controller.ResetMovement();
		StartCoroutine(AdvanceLevel());
	}

	IEnumerator AdvanceLevel()
	{
		yield return new WaitForSeconds(1);
		LevelIndex++;
		if(LevelIndex+1 > menuManager.TopLevelReached)
			menuManager.TopLevelReached = LevelIndex+1;
		if(LevelIndex >= Levels.Count)
		{
			menuManager.OpenEndMenu();	
		}
		else
		{
			LoadLevel();
		}
	}

	public void LoseLevel()
	{
		SfxManager.PlaySfx(Sfx.LOSE, 0.5f);
		TileMapManager.OccupantMap.SetTile(
			new Vector3Int(
				Player.Position.x,
				Player.Position.y,
				0),
			TileManager.GetTile(TileType.PLAYER_LOSE));
		controller.ResetMovement();
		Mode = GameMode.LOST;
	}

	public void LoadLevelNumber(int _number)
	{
		LevelIndex = _number;
		if(LevelIndex >= Levels.Count || LevelIndex < 0) LevelIndex = 0;
		LoadLevel();
	}

	public void ShowLevelMenu()
	{
		Mode = GameMode.MENU;
		controller.SetControllerMap("Menu");
		menuManager.OpenLevelSelector();
	}

	public void ToggleGuide()
	{
		AlwaysShowGuide = !AlwaysShowGuide;
		CurrentLevel.Refresh();
	}
}
