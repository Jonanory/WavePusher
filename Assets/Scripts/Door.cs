using UnityEngine;

public class Door: MonoBehaviour, IActivatable, IBlockable
{
	public bool Open = false;
	public Vector2Int Position;

	public Receiver Opener;

	bool _Blocking;
	public bool IsBlocking{
		get { return _Blocking; }
	}

	public void Start()
	{
		Deactivate();
		Opener.Activatables.Add(this);
	}

	public void Activate()
	{
		Open = true;
		_Blocking = false;
		GameManager.master.CurrentLevel.Map.WallMap.SetTile(
			(Vector3Int)Position,
			null);
	}

	public void Deactivate()
	{
		Open = false;
		_Blocking = true;
		GameManager.master.CurrentLevel.Map.WallMap.SetTile(
			(Vector3Int)Position,
			GameManager.master.CurrentLevel.DoorTile);
	}
}