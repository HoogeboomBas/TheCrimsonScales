using Godot;

public partial class AbilityCardViewEnhancement : Control
{
	[Export]
	private EnhancementView _enhancementView;

	public void Init(EnhancementMark mark, EnhancementModel model)
	{
		_enhancementView.SetModel(model);
		Control parent = GetParent<Control>();
		SetPosition(mark.NormalizedPosition * parent.Size - 0.5f * Size);
	}
}