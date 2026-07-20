using Godot;
using GTweens.Builders;
using GTweensGodot.Extensions;

public partial class VoidSightView : Control
{
	[Export]
	private Control _overlay;
	[Export]
	private VoidSightViewEye _eye;

	public void Open()
	{
		AudioStreamPlayer audioPlayer =
			AppController.Instance.AudioController.PlayFastForwardable("res://Audio/SFX/CHARGE_Sci-Fi_High_Pass_Sweep_12_Semi_Low_loop_mono.mp3",
				freeAutomatically: false);
		if(audioPlayer != null)
		{
			GTweenSequenceBuilder.New()
				.Append(
					CustomGTweenExtensions.Tween(value => audioPlayer.SetVolumeLinear(value), 1f / AppController.Instance.GameplayTimeScale))
				.AppendTime(1.5f / AppController.Instance.GameplayTimeScale)
				.Append(
					CustomGTweenExtensions.Tween(value => audioPlayer.SetVolumeLinear(1 - value), 1f / AppController.Instance.GameplayTimeScale))
				.AppendCallback(audioPlayer.QueueFree)
				.Build().PlayFastForwardable();
		}

		Show();
		_overlay.Show();
		_overlay.TweenModulateAlpha(1f, 0f).Play(true);
		_eye.Open();
	}

	public void Close()
	{
		_overlay.TweenModulateAlpha(0f, 0f).OnComplete(() =>
		{
			_overlay.Hide();
			Hide();
		}).Play(true);
	}
}