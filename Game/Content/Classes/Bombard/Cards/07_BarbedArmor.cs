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
			new AbilityCardAbility(new RetaliateAbility(1)),
			new AbilityCardAbility(new OtherActiveAbility(
				async state =>
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
				},
				async state =>
				{
					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}))
		];

		protected override bool Round => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new ShieldAbility(2, requiredRangeType: RangeType.Melee))
		];

		protected override bool Round => true;
	}
}