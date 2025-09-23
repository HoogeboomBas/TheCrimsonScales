using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class HurriedRepairs : BombardCardModel<HurriedRepairs.CardTop, HurriedRepairs.CardBottom>
{
	public override string Name => "Hurried Repairs";
	public override int Level => 4;
	public override int Initiative => 25;
	protected override int AtlasIndex => 16;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(new DynamicInt<HealAbility.State>(state => 1 + state.Performer.TurnMovedHexCount))
				.Build()),

			new AbilityCardAbility(AbilityCmd.AllOpposingAttacksGainDisadvantageActiveAbility())
		];

		protected override bool Round => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithOnAbilityStarted(async state =>
				{
					ScenarioCheckEvents.CanPassEnemyCheckEvent.Subscribe(state, this,
						parameters =>
							parameters.AbilityState == state &&
							parameters.Figure == state.Performer,
						parameters =>
						{
							parameters.SetCanPass();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
				{
					ScenarioCheckEvents.CanPassEnemyCheckEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEndedPerformed(async state =>
					{
						MoveAbility.State moveAbilityState = state.ActionState.GetAbilityState<MoveAbility.State>(0);

						List<Figure> figures = new List<Figure>();

						foreach(Hex hex in moveAbilityState.Hexes)
						{
							foreach(Figure figure in hex.GetHexObjectsOfType<Figure>())
							{
								if(state.Performer.AlliedWith(figure))
								{
									figures.AddIfNew(figure);
								}
							}
						}

						foreach(Figure figure in figures)
						{
							await AbilityCmd.SufferDamage(null, figure, 1);
						}
					}
				)
				.Build())
		];
	}
}