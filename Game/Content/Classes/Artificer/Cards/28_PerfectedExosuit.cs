using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class PerfectedExosuit : ArtificerCardModel<PerfectedExosuit.CardTop, PerfectedExosuit.CardBottom>
{
	public override string Name => "Perfected Exosuit";
	public override int Level => 9;
	public override int Initiative => 08;
	protected override int AtlasIndex => 28;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(2)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							parameters.AbilityState.SingleTargetAdjustPierce(2);
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build()),
		];

		public override Func<Figure, GDTask<bool>> OnCardSideStarted => async figure => await TryLoseScrapTokens(figure, 4);
		public override bool Persistent => true;
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(5, new MoveCircle(this, new Vector2(0.5173371f, 0.69100523f)))
				.WithMoveType(MoveType.Jump)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					foreach(Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 1)
						        .Where(figure => figure.EnemiesWith(state.Performer)))
					{
						await AbilityCmd.SufferDamage(state, figure, 1);
						state.SetPerformed();
					}
				})
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await GainScrapToken(state);
					state.SetPerformed();
				})
				.Build())
		];
	}
}