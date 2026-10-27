using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class DevouredByThorns : ThornreaperLevelUpCardModel<DevouredByThorns.CardTop, DevouredByThorns.CardBottom>
{
	public override string Name => "Devoured by Thorns";
	public override int Level => 6;
	public override int Initiative => 72;
	protected override int AtlasIndex => 23;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			ConsumeEarthOrSkipAction(),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.2f, 0.3f)))
				.WithTargets(3)
				.WithPull(2)
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(2)
				.WithCustomGetTargets(async (state, list) =>
				{
					Hex hex = await AbilityCmd.SelectHex(state,
						validTerrain => RangeHelper.GetOverlayTilesInRange<HazardousTerrain>(state.Performer.Hex, RangeHelper.InfiniteRange, true, true), true, "Select a hazardous terrain tile:");

					state.SetCustomValue(this, "SelectedHex", hex);
					
					foreach (Figure figure in RangeHelper.GetFiguresInRange(hex, 2, true, false))
					{
						list.Add(figure);
					}
				})
				.WithCustomGetPerformHex(performHex => performHex.GetCustomValue<Hex>(this, "SelectedHex"))
				.WithOnAbilityEnded(async pullAbilityState =>
				{
					ScenarioEvents.FigureKilledEvent.Subscribe(pullAbilityState, this,
						parameters => true,
						async applyParameters =>
							{
								await AbilityCmd.LootHex(pullAbilityState.Performer, applyParameters.Figure.Hex);
								await AbilityCmd.GainXP(pullAbilityState.Performer, 1);
							});
					await GDTask.CompletedTask;
				})
				.Build())
		];
	}
}
