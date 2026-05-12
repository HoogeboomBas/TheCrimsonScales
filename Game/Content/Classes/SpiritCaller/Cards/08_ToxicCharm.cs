using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ToxicCharm : SpiritCallerCardModel<ToxicCharm.CardTop, ToxicCharm.CardBottom>
{
	public override string Name => "Toxic Charm";
	public override int Level => 1;
	public override int Initiative => 59;
	protected override int AtlasIndex => 28 - 8;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithRange(3, new RangeSquare(this, new Vector2(0.44632727f, 0.2367115f)))
				.WithConditions(Conditions.Poison1)
				.WithAOEPattern(new AOEPattern(
				[
					new AOEHex(Vector2I.Zero, AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
				]))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Air,
						applyFunction: async parameters =>
						{
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Dark);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"Infuse {Icons.Inline(Icons.GetElement(Element.Dark))}")
					))
				.Build()),
		];
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6212088f, 0.6698124f)))
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return Spirit.HasSpirit(state.Performer.Hex);
				})
				.Build()),
		];
	}
}