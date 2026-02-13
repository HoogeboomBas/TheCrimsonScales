using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ShimmeringScuttle : LuminaryCardModel<ShimmeringScuttle.CardTop, ShimmeringScuttle.CardBottom>
{
	public override string Name => "Shimmering Scuttle";
	public override int Level => 1;
	public override int Initiative => 21;
	protected override int AtlasIndex => 7;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Empty),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Empty),
					]
				))
				.WithDuringAttackSubscriptions(
				[
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}")
					),
				])
				.Build()),
			Scuttle(2, [Element.Fire]),
		];
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62128437f, 0.7070308f)))
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						canApply: parameters => parameters.AbilityState.Performer == state.Performer &&
						                        parameters.AbilityState.GetCustomValue<bool>(state.Performer, "Glow Ability") &&
						                        parameters.AbilityState is TargetedAbilityState targetedAbilityState &&
						                        targetedAbilityState.AbilityTarget.HasFlag(Target.Enemies),
						apply: async parameters =>
						{
							((TargetedAbilityState)parameters.AbilityState).AbilityAddCondition(Conditions.Muddle);

							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
		];

		public override bool Round => true;
	}
}