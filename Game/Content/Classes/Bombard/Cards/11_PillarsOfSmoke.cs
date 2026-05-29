using System.Collections.Generic;
using Fractural.Tasks;

public class PillarsOfSmoke : BombardCardModel<PillarsOfSmoke.CardTop, PillarsOfSmoke.CardBottom>
{
	public override string Name => "Pillars of Smoke";
	public override int Level => 1;
	public override int Initiative => 11;
	protected override int AtlasIndex => 11;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters =>
							state.Performer.AlliedWith(parameters.Figure) &&
							(!state.TryGetCustomValue(this, "LastUseRoundIndex", out int lastUseRoundIndex) ||
							 lastUseRoundIndex != GameController.Instance.ScenarioPhaseManager.RoundIndex) &&
							RangeHelper.Distance(parameters.Figure.Hex, state.Performer.Hex) == 1,
						async parameters =>
						{
							state.SetCustomValue(this, "LastUseRoundIndex", GameController.Instance.ScenarioPhaseManager.RoundIndex);

							await AbilityCmd.AddConditions(state, parameters.Figure, [Conditions.Immobilize, Conditions.Invisible]);
						},
						EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Invisible)),
						effectInfoViewParameters: new AbilityCardEffectInfoView.Parameters(GetAbilityCardSide(state))
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()),
			new AbilityCardAbility(AbilityCmd.AllOpposingAttacksGainDisadvantageActiveAbility())
		];

		public override bool Round => true;
	}
}