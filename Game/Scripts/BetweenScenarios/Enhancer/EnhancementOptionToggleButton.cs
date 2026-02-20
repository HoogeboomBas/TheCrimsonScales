using Godot;

public partial class EnhancementOptionToggleButton : ToggleButton<EnhancementOptionToggleButton>
{
	[Export]
	private EnhancementView _enhancementView;
	[Export]
	private Label _costLabel;

	public EnhancementModel EnhancementModel => _enhancementView.EnhancementModel;

	public void Init(EnhancementModel enhancementModel, int cost)
	{
		base.Init();

		_enhancementView.SetModel(enhancementModel);
		_costLabel.SetText(cost.ToString());
	}
}