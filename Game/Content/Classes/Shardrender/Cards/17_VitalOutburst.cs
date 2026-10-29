using System.Collections.Generic;
using Fractural.Tasks;

public class VitalOutburst : ShardrenderCardModel<VitalOutburst.CardTop, VitalOutburst.CardBottom>
{
	public override string Name => "Vital Outburst";
	public override int Level => 3;
	public override int Initiative => 29;
	protected override int AtlasIndex => 17;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveCharacterTokenBackAbility(2, false).Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(1)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.RetaliateEvent.Subscribe(state, this,
						parameters => parameters.RetaliatingFigure == state.Performer &&
						              GetActiveCrystallizeStates(state.Performer as Character).Count != 0 &&
						              RangeHelper.Distance(parameters.Performer.Hex, parameters.RetaliatingFigure.Hex) <= 1,
						async parameters =>
						{
							await AbilityCmd.AddCondition(state, parameters.Performer, Conditions.Poison1);

							await GDTask.CompletedTask;
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

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}