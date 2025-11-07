using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager master;
	public Level CurrentLevel;

	public void Awake()
	{
		if(master == null) master = this;
		else if(master != this) Destroy(gameObject);
	}
}
