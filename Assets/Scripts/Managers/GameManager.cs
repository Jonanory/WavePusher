using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameMode{PLAYING, MENU}
public class GameManager : MonoBehaviour
{
	public static GameManager master;
	public Player Player;
	public Level CurrentLevel;

	public List<LevelData> Levels = new List<LevelData>();
	public int LevelIndex = 0;
	public LevelLoader Loader;
	public Map Map;
	public GameObject LevelSelectMenu;
	public GameMode Mode = GameMode.MENU;

	public void Awake()
	{
		if(master == null) master = this;
		else if(master != this) Destroy(gameObject);
	}

	void Start()
	{
		LoadLevel();
	}

	public void LoadLevel()
	{
		LevelSelectMenu.SetActive(false);
		Mode = GameMode.PLAYING;
		Loader.LoadLevel(Levels[LevelIndex]);
	}

	public void NextLevel()
	{
		LevelIndex++;
		if(LevelIndex >= Levels.Count) LevelIndex = 0;
		LoadLevel();
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
		LevelSelectMenu.SetActive(true);
	}
}
