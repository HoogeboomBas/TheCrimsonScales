using System.Collections.Generic;
using Godot;

public class VolatileTonic : MirefootCardModel<VolatileTonic.CardTop, VolatileTonic.CardBottom>
{
	public override string Name => "Volatile Tonic";
	public override int Level => 1;
	public override int Initiative => 31;
	protected override int AtlasIndex => 11;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithRange(2)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => (parameters.AbilityState.Target.HasPoison() || parameters.AbilityState.Target.HasWound()),
						async parameters =>
						{
							if(parameters.AbilityState.Target.HasPoison())
							{
								parameters.AbilityState.SingleTargetAddCondition(Conditions.Wound2);
							}

							if(parameters.AbilityState.Target.HasWound())
							{
								parameters.AbilityState.SingleTargetAddCondition(Conditions.Poison2);
							}

							await AbilityCmd.GainXP(parameters.AbilityState.Performer, 1);
						})
				)
				.Build())
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Wound2)
				.WithRange(2)
				.WithAOEPattern(new AOEPattern([
						new AOEHex(Vector2I.Zero, AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					]
				))
				.Build())
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}
}