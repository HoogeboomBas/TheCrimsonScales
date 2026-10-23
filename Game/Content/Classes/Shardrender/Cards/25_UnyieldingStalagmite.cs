using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class UnyieldingStalagmite : ShardrenderCardModel<UnyieldingStalagmite.CardTop, UnyieldingStalagmite.CardBottom>
{
	public override string Name => "Unyielding Stalagmite";
	public override int Level => 7;
	public override int Initiative => 71;
	protected override int AtlasIndex => 25;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.4507958f, 0.23711912f)))
				.WithPierce(1)
				.WithConditions(Conditions.Brittle)
				.WithDuringAttackSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.DuringAttack.Parameters>(async parameters =>
					{
						parameters.AbilityState.AbilityAdjustPush(3);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Push)}3")))
				.Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5)
				.WithRange(5)
				.WithPierce(3)
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(
						_ => true,
						async parameters =>
						{
							foreach(Figure figure in RangeHelper.GetFiguresInRange(parameters.AbilityState.Target, 1, false)
								        .Where(figure => parameters.Performer.EnemiesWith(figure)))
							{
								await AbilityCmd.SufferDamage(parameters.AbilityState, figure, 2);
							}
						}))
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}