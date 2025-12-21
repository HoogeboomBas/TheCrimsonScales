using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class EnvelopeBCircle : Control
{
	[Export]
	private Label _label;
	[Export]
	private Control _checkmark;

	public bool Checked { get; private set; }

	public void Init(int number, bool @checked)
	{
		Checked = @checked;
		_checkmark.SetVisible(Checked);
		_label.SetText(number.ToString());
	}

	public void Check()
	{
		if(Checked)
		{
			return;
		}

		Checked = true;

		_checkmark.SetPivotOffset(_checkmark.Size * 0.5f);
		_checkmark.SetVisible(true);
		_checkmark.SetScale(Vector2.Zero);
		_checkmark.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).Play();
	}
}