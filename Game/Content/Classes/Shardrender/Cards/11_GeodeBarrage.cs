using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class GeodeBarrage : ShardrenderCardModel<GeodeBarrage.CardTop, GeodeBarrage.CardBottom>
{
	public override string Name => "Geode Barrage";
	public override int Level => 1;
	public override int Initiative => 23;
	protected override int AtlasIndex => 11;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(1)
				.WithRange(1)
				.WithDuringPushSubscriptions(
					AdvanceCrystallizeSubscription<ScenarioEvents.DuringPush.Parameters>(async parameters =>
					{
						parameters.AbilityState.AbilityAdjustPush(2);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Push)}")))
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(1, new MoveCircle(this, new Vector2(0.623959f, 0.3517465f)))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5)
				.WithMoveType(MoveType.Jump)
				.WithDuringMovementSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.DuringMovement.Parameters>(async parameters =>
					{
						parameters.AbilityState.AdjustMoveValue(2);

						await AbilityCmd.GainXP(parameters.Performer, 1);
					}, new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Move)}")))
				.WithOnAbilityEndedPerformed(async state =>
				{
					foreach(Figure figure in state.Hexes
						        .SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
						        .Where(f => state.Performer.EnemiesWith(f))
						        .Distinct())
					{
						await AbilityCmd.SufferDamage(state, figure, 1);
					}
				})
				.Build()),
		];

		public override int XP => 1;
		public override bool Loss => true;
	}
}