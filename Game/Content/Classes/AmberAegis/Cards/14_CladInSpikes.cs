using System.Collections.Generic;
using Fractural.Tasks;

public class CladInSpikes : AmberAegisCardModel<CladInSpikes.CardTop, CladInSpikes.CardBottom>
{
	public override string Name => "Clad in Spikes";
	public override int Level => 2;
	public override int Initiative => 12;
	protected override int AtlasIndex => 14;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.RetaliateEvent.Subscribe(state, this,
						parameters => parameters.RetaliatingFigure == state.Performer &&
						              parameters.AbilityState.SingleTargetRangeType == RangeType.Range &&
						              RangeHelper.Distance(state.Performer.Hex, parameters.Performer.Hex) <= 4,
						async parameters =>
						{
							parameters.AdjustRetaliate(4);
							await AbilityCmd.GainXP(state.Performer, 1);
							await state.ActionState.RequestDiscardOrLose();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AfterSufferDamageEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer &&
						              parameters.SufferDamageParameters.PotentialDamageDealer.EnemiesWith(state.Performer) && parameters.Damage >= 2,
						async parameters =>
						{
							await AbilityCmd.SufferDamage(state, parameters.SufferDamageParameters.PotentialDamageDealer, 1);
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}