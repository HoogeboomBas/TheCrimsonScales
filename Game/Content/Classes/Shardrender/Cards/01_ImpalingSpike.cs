using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ImpalingSpike : ShardrenderCardModel<ImpalingSpike.CardTop, ImpalingSpike.CardBottom>
{
	public override string Name => "Impaling Spike";
	public override int Level => 1;
	public override int Initiative => 25;
	protected override int AtlasIndex => 1;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.48271462f, 0.23656511f)))
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red)
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
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(2, new PushCircle(this, new Vector2(0.5135573f, 0.76887506f)))
				.WithRange(1)
				.WithDuringPushSubscriptions(
					AdvanceCrystallizeSubscription<ScenarioEvents.DuringPush.Parameters>(async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Wound1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters(Icons.Inline(Icons.GetCondition(Conditions.Wound1)))))
				.Build())
		];
	}
}