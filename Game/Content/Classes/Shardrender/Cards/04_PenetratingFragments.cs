using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class PenetratingFragments : ShardrenderCardModel<PenetratingFragments.CardTop, PenetratingFragments.CardBottom>
{
	public override string Name => "Penetrating Fragments";
	public override int Level => 1;
	public override int Initiative => 76;
	protected override int AtlasIndex => 4;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithTargets(2)
				.WithRange(3, new RangeSquare(this, new Vector2(0.66121036f, 0.24376732f)))
				.WithDuringAttackSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.DuringAttack.Parameters>(async parameters =>
					{
						parameters.AbilityState.AbilityAdjustPierce(1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Pierce)}1")))
				.Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						AdvanceCrystallizeSubscription<ScenarioEvents.DuringAttack.Parameters>(async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							parameters.AbilityState.SingleTargetAdjustPierce(1);

							await GDTask.CompletedTask;
						}, new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}, {Icons.Inline(Icons.Pierce)}1"), true));
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}