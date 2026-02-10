using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class MarchOfMultitudes : AmberAegisCardModel<MarchOfMultitudes.CardTop, MarchOfMultitudes.CardBottom>
{
	public override string Name => "March of Multitudes";
	public override int Level => 1;
	public override int Initiative => 37;
	protected override int AtlasIndex => 12;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(1)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					await MoveColonyToken(state, 2);
				})
				.Build())
		];
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder()
						.WithDistance(3, new MoveCircle(this, new Vector2(0.62222224f, 0.7514227f)))
						.WithOnAbilityStarted(async state =>
						{
							ScenarioCheckEvents.MoveCanStopAtCheckEvent.Subscribe(state.Performer, this,
								parameters => parameters.AbilityState == state && !IsAdjacentToColonyToken<ColonyToken>(parameters.Hex),
								parameters =>
								{
									parameters.SetCannotStopAt();
								}
							);
							await GDTask.CompletedTask;
						})
						.WithOnAbilityEnded(async state =>
							{
								ScenarioCheckEvents.MoveCanStopAtCheckEvent.Unsubscribe(state.Performer, this);

								await GDTask.CompletedTask;
							}
						)
						.Build()
				])
				.WithTarget(Target.SelfOrAllies | Target.SelfCountsForTargets)
				.WithTargets(2)
				.WithRange(2)
				.Build())
		];
	}
}