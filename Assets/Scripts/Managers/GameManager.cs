using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager master;
	public Player Player;
	public Level CurrentLevel;
	public Map Map;

	public void Awake()
	{
		if(master == null) master = this;
		else if(master != this) Destroy(gameObject);
	}
}
