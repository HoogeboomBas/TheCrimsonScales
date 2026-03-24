using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class DefenseMechanism : BombardCardModel<DefenseMechanism.CardTop, DefenseMechanism.CardBottom>
{
	public override string Name => "Defense Mechanism";
	public override int Level => 8;
	public override int Initiative => 18;
	protected override int AtlasIndex => 24;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(3, new RetaliateDiamondPlus(this, new Vector2(0.6193324f, 0.22017238f)))
				.Build()),

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

		public override bool Round => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6219222f, 0.7174603f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Disarm)
				.WithRange(2)
				.Build())
		];
	}
}