using Godot;

public partial class BasicAbilityCardSideCurrentActionView : CurrentActionView<BasicAbilityCardSideCurrentActionView.Parameters>
{
	public class Parameters : CurrentActionViewParameters
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/CurrentActionViews/BasicAbilityCardSideCurrentActionView.tscn";
		public override object Source => AbilityCardSide;

		public AbilityCardSide AbilityCardSide { get; }
		public bool ShowTop { get; }

		public Parameters(AbilityCardSide abilityCardSide)
		{
			AbilityCardSide = abilityCardSide;
			ShowTop = abilityCardSide.AbilityCardSideType == AbilityCardSideType.BasicTop;
		}
	}

	[Export]
	private Control _topContainer;
	[Export]
	private Control _bottomContainer;

	public override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_topContainer.SetVisible(parameters.ShowTop);
		_bottomContainer.SetVisible(!parameters.ShowTop);
	}
}