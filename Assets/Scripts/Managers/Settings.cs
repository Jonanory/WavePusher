using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
	[Header("Key Help Button")]
	public Image KeyHelpRenderer;
	public Sprite NoHelpSprite;
	public Sprite HelpSprite;

	[Header("Volume Button")]
	public Image VolumeRenderer;
	public Sprite NoVolumeSprite;
	public Sprite VolumeSprite;

	public void ToggleGuide()
	{
		GameManager.master.AlwaysShowGuide = !GameManager.master.AlwaysShowGuide;

		if(GameManager.master.AlwaysShowGuide)
		{
			KeyHelpRenderer.sprite = HelpSprite;
		}
		else
		{
			KeyHelpRenderer.sprite = NoHelpSprite;
		}
		GameManager.master.CurrentLevel.Refresh();
	}

	public void ToggleVolume()
	{
		GameManager.master.sfxManager.IsMuted = !GameManager.master.sfxManager.IsMuted;

		if(GameManager.master.sfxManager.IsMuted)
		{
			VolumeRenderer.sprite = NoVolumeSprite;
		}
		else
		{
			VolumeRenderer.sprite = VolumeSprite;
		}
	}

	public void RefreshDisplay()
	{
		if(GameManager.master.AlwaysShowGuide)
		{
			KeyHelpRenderer.sprite = HelpSprite;
		}
		else
		{
			KeyHelpRenderer.sprite = NoHelpSprite;
		}

		if(GameManager.master.sfxManager.IsMuted)
		{
			VolumeRenderer.sprite = NoVolumeSprite;
		}
		else
		{
			VolumeRenderer.sprite = VolumeSprite;
		}
	}
}
