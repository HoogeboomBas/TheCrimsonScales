using System.Collections.Generic;
using Fractural.Tasks;

public class BarbedArmor : BombardCardModel<BarbedArmor.CardTop, BarbedArmor.CardBottom>
{
	public override string Name => "Barbed Armor";
	public override int Level => 1;
	public override int Initiative => 13;
	protected override int AtlasIndex => 7;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder().WithRetaliateValue(1).Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.RetaliateEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.RetaliatingFigure == state.Performer &&
							RangeHelper.Distance(canApplyParameters.AbilityState.Performer.Hex, canApplyParameters.RetaliatingFigure.Hex) <= 1,
						async parameters =>
						{
							await AbilityCmd.AddCondition(state, parameters.AbilityState.Performer, Conditions.Wound1);
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

		protected override bool Round => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(2)
				.WithRequiredRangeType(RangeType.Melee)
				.Build())
		];

		protected override bool Round => true;
	}
}