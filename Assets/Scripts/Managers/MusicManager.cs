using UnityEngine;

public class MusicManager : MonoBehaviour
{
	private AudioSource AudioSource;
	public AudioClip MusicClip;
	public float Volume = 0.5f;

	private void Awake()
	{
		AudioSource = GetComponent<AudioSource>();
		AudioSource.clip = MusicClip;
		AudioSource.loop = true;
		AudioSource.volume = Volume;

		AudioSource.Play();
	}

	public void SetVolume(float volume)
	{
		AudioSource.volume = Mathf.Clamp01(volume);
	}

	public void Play()
	{
		if (!AudioSource.isPlaying)
			AudioSource.Play();
	}

	public void Stop()
	{
		if (AudioSource.isPlaying)
			AudioSource.Stop();
	}
}