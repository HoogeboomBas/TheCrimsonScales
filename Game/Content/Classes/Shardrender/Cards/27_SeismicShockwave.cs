using System.Collections.Generic;
using Godot;

public class SeismicShockwave : ShardrenderCardModel<SeismicShockwave.CardTop, SeismicShockwave.CardBottom>
{
	public override string Name => "Seismic Shockwave";
	public override int Level => 8;
	public override int Initiative => 32;
	protected override int AtlasIndex => 27;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithPush(2)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
					]
				))
				.WithDuringAttackSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.DuringAttack.Parameters>(async parameters =>
					{
						parameters.AbilityState.AbilityAdjustPierce(1);

						await AbilityCmd.GainXP(parameters.Performer, 1);
					}, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Pierce)}1")))
				.Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(1)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(2)
				.WithConditions(Conditions.Muddle)
				.Build()),
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(1)
				.WithConditionalAbilityCheck(state => AdvanceCrystallizeConditionalAbilityCheck(state.Performer,
					new TextEffectInfoView.Parameters($"All adjacent enemies suffer {Icons.Inline(Icons.Damage)}1")))
				.Build())
		];
	}
}