using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class EncompassingAura : LuminaryCardModel<EncompassingAura.CardTop, EncompassingAura.CardBottom>
{
	public override string Name => "Encompassing Aura";
	public override int Level => 6;
	public override int Initiative => 11;
	protected override int AtlasIndex => 22;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithGetAbilities(state =>
				[
					ShieldAbility.Builder()
						.WithShieldValue(new DynamicInt<ShieldAbility.State>(_ => 1 + state.GetCustomValue<int>(this, "IceConsumed")))
						.Build(),
					RetaliateAbility.Builder()
						.WithRetaliateValue(new DynamicInt<RetaliateAbility.State>(_ => 1 + state.GetCustomValue<int>(this, "DarkConsumed")))
						.Build(),
				])
				.WithTarget(Target.SelfOrAllies | Target.TargetAll)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.West), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					]
				))
				.WithDuringGrantSubscriptions(
				[
					ScenarioEvents.DuringGrant.Subscription.ConsumeElement(Element.Ice,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.SetCustomValue(this, "IceConsumed", 1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Shield)}")
					),
					ScenarioEvents.DuringGrant.Subscription.ConsumeElement(Element.Dark,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.SetCustomValue(this, "DarkConsumed", 1);

							await GDTask.CompletedTask;
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Retaliate)}")
					)
				])
				.Build()),
		];

		public override bool Round => true;
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Immobilize)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithRange(1)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Light))
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6213844f, 0.7282153f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Wound1)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithRange(1)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Fire))
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.Build()),
		];
	}
}