using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Fractural.Tasks;
using Godot;

public class SweepingBlow : BruiserCardModel<SweepingBlow.CardTop, SweepingBlow.CardBottom>
{
	public override string Name => "Sweeping Blow";
	public override int Level => 1;
	public override int Initiative => 23;
	protected override int AtlasIndex => 12;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithRange(1)
				.WithConditions(Conditions.Muddle)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red)
				]),
				new AOEHexMark(Vector2I.Zero.Add(Direction.NorthWest), this, new Vector2(0.62f, 0.278f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters =>
						{
							Figure target = parameters.AbilityState.Target;
							return target != null && target.Health == target.MaxHealth;
						},
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAttackValue += 1;
							await GDTask.CompletedTask;
						}
					)
				)
				.WithGetTargetingHintText(state =>
					$"{Icons.HintText(Icons.Attack)}2 ({Icons.HintText(Icons.Attack)}3 if undamaged)")
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.Build()),
		];
	}
}