using UnityEngine;

public enum Sfx
{
	BOX_MOVE
}

public class SfxManager : MonoBehaviour
{
	public AudioClip BoxMoveClip;
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
			default: return null;
		}
	}

	public void PlaySfx(Sfx _sfx)
	{
		SfxSource.PlayOneShot(GetClip(_sfx));
	}

	public void PlayClip(AudioClip clip, float volume = 1f)
	{
		SfxSource.PlayOneShot(clip, volume);
	}
}