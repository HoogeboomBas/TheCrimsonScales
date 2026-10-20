using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BedOfRoses : ThornreaperLevelUpCardModel<BedOfRoses.CardTop, BedOfRoses.CardBottom>
{
	public override string Name => "Bed of Roses";
	public override int Level => 9;
	public override int Initiative => 10;
	protected override int AtlasIndex => 28;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(4, new RetaliateDiamondPlus(this, new Vector2(0.2f, 0.3f)))
				.Build()),

			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(2, new ShieldDiamondPlus(this, new Vector2(0.2f, 0.3f)))
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					state.SetPerformed();
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					if (state.Performer.Hex.IsFeatureless())
					{
						await CreateHazardousTerrain(state.Performer.Hex, state.Performer);
					}
				})
				.Build())
		];

		public override bool Round => true;
		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(state, this,
						parameters => parameters.Figure.EnemiesWith(state.Performer) &&
						parameters.Figure.Health <= 5,
						async parameters =>
							{
								await AbilityCmd.KillOrExhaust(parameters.Figure, state.Performer);
								await state.AdvanceUseSlot();
							});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots([
					new UseSlot(new Vector2(0.2869934f, 0.3959994f)),
					new UseSlot(new Vector2(0.5f, 0.3959994f), GainXP),
					new UseSlot(new Vector2(0.7025001f, 0.3959994f)),
					new UseSlot(new Vector2(0.5f, 0.3959994f), GainXP),
					new UseSlot(new Vector2(0.7025001f, 0.3959994f)),
					new UseSlot(new Vector2(0.5f, 0.3959994f), GainXP),
				])
				.Build())
		];

		public override bool Loss => true;
		public override bool Persistent => true;
	}
}
