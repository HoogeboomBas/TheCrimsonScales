using Godot;

public partial class BattleGoalView : CardView
{
	[Export]
	private ResizingLabel _titleLabel;
	[Export]
	private Label _description;
	[Export]
	private TextureRect _secondCheckmark;

	public BattleGoalModel Model { get; private set; }

	public void SetModel(BattleGoalModel model)
	{
		Model = model;

		_titleLabel.SetText(Model.Title);
		_description.SetText(Model.Description);
		_secondCheckmark.SetVisible(model.CheckmarkCount == BattleGoalCheckmarkCount.Two);

		Init(null);
	}
}