using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class EmpoweringRays : LuminaryCardModel<EmpoweringRays.CardTop, EmpoweringRays.CardBottom>
{
	public override string Name => "Empowering Rays";
	public override int Level => 4;
	public override int Initiative => 57;
	protected override int AtlasIndex => 18;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Strengthen)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.45917222f, 0.24375446f)))
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
					]
				))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, state.UniqueTargetedFigures.Count);
				})
				.Build()),
		];

		public override IEnumerable<CardElementInfusion> Elements =>
		[
			CardElementInfusion.Infuse(Element.Fire), CardElementInfusion.Infuse(Element.Light), CardElementInfusion.Infuse(Element.Dark)
		];

		public override bool Loss => true;
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						canApply: parameters => parameters.AbilityState.Performer == state.Performer &&
						                        parameters.AbilityState.GetCustomValue<bool>(state.Performer, "Glow Ability"),
						apply: async parameters =>
						{
							if(parameters.AbilityState is TargetedAbilityState targetedAbilityState)
							{
								targetedAbilityState.AbilityAddCondition(Conditions.Poison1);
							}

							await state.ActionState.RequestDiscardOrLose();
							//TODO: Add Remove Immediately
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6215359f, 0.8255312f)))
				.Build()),
		];

		public override bool Round => true;
	}
}