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
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters =>
							state.Performer.AlliedWith(parameters.Performer) &&
							(!state.TryGetCustomValue(this, "LastUseRoundIndex", out int lastUseRoundIndex) ||
							 lastUseRoundIndex != GameController.Instance.ScenarioPhaseManager.RoundIndex) &&
							RangeHelper.Distance(parameters.Performer.Hex, state.Performer.Hex) <= 1,
						async parameters =>
						{
							state.SetCustomValue(this, "LastUseRoundIndex", GameController.Instance.ScenarioPhaseManager.RoundIndex);

							await AbilityCmd.AddCondition(null, parameters.Performer, Conditions.Immobilize);
							await AbilityCmd.AddCondition(null, parameters.Performer, Conditions.Invisible);
						},
						EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(Conditions.Invisible)),
						effectInfoViewParameters: new AbilityCardEffectInfoView.Parameters(this)
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

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()),
			new AbilityCardAbility(AbilityCmd.AllOpposingAttacksGainDisadvantageActiveAbility())
		];

		protected override bool Round => true;
	}
}