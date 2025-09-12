using System.Threading;
using Fractural.Tasks;
using Godot;
using GTweensGodot.Extensions;

public partial class LoadingSceneController : SingletonNode<LoadingSceneController>
{
	[Export]
	private Control _background;

	public async GDTask FadeIn(CancellationToken ct)
	{
		_background.Modulate = new Color(1f, 1f, 1f, 0f);
		await _background.TweenModulateAlpha(1f, 0.1f).PlayAsync(ct);
	}

	public async GDTask FadeOut(CancellationToken ct)
	{
		await _background.TweenModulateAlpha(0f, 0.1f).PlayAsync(ct);
	}
}