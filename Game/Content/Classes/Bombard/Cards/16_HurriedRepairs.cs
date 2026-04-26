using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class HurriedRepairs : BombardCardModel<HurriedRepairs.CardTop, HurriedRepairs.CardBottom>
{
	public override string Name => "Hurried Repairs";
	public override int Level => 4;
	public override int Initiative => 25;
	protected override int AtlasIndex => 16;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(new DynamicInt<HealAbility.State>(state => 1 + state.Performer.TurnMovedHexes.Count))
				.WithTarget(Target.Self)
				.Build()),

			new AbilityCardAbility(AbilityCmd.AllOpposingAttacksGainDisadvantageActiveAbility())
		];

		public override bool Round => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6209843f, 0.6982149f)))
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
						List<Figure> figures = new List<Figure>();

						foreach(Hex hex in state.Hexes)
						{
							foreach(Figure figure in hex.GetHexObjectsOfType<Figure>().Where(figure => figure != state.Performer))
							{
								figures.AddIfNew(figure);
							}
						}

						foreach(Figure figure in figures)
						{
							await AbilityCmd.SufferDamage(state, figure, 1);
						}
					}
				)
				.Build())
		];
	}
}