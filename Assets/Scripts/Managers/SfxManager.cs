using UnityEngine;

public enum Sfx
{
	BOX_MOVE,
	WIN,
	LOSE
}

public class SfxManager : MonoBehaviour
{
	public AudioClip BoxMoveClip;
	public AudioClip WinClip;
	public AudioClip LoseClip;
	private AudioSource SfxSource;

	void Awake()
	{
		SfxSource = gameObject.AddComponent<AudioSource>();
		SfxSource.loop = false;
	}

	public AudioClip GetClip(Sfx _sfx)
	{
		switch(_sfx)
		{
			case Sfx.BOX_MOVE: return BoxMoveClip;
			case Sfx.WIN: return WinClip;
			case Sfx.LOSE: return LoseClip;
			default: return null;
		}
	}

	public static void PlaySfx(Sfx _sfx, float volume = 1f)
	{
		GameManager.master.sfxManager.SfxSource.PlayOneShot(
			GameManager.master.sfxManager.GetClip(_sfx), 
			volume);
	}

	public void PlayClip(AudioClip clip, float volume = 1f)
	{
		SfxSource.PlayOneShot(clip, volume);
	}
}