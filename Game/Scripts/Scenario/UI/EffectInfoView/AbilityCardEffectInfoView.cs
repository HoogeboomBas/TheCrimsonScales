using Godot;

public partial class AbilityCardEffectInfoView : EffectInfoView<AbilityCardEffectInfoView.Parameters>
{
	public class Parameters : EffectInfoViewParameters
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/EffectInfoViews/AbilityCardEffectInfoView.tscn";

		public AbilityCardSide AbilityCardSide { get; }

		public Parameters(AbilityCardSide abilityCardSide)
		{
			AbilityCardSide = abilityCardSide;
		}
	}

	[Export]
	private AbilityCardSideView _cardSideView;

	public override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_cardSideView.SetCard(parameters.AbilityCardSide);
	}
}