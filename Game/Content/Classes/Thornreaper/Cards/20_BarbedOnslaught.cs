using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class BarbedOnslaught : ThornreaperLevelUpCardModel<BarbedOnslaught.CardTop, BarbedOnslaught.CardBottom>
{
	public override string Name => "Barbed Onslaught";
	public override int Level => 5;
	public override int Initiative => 58;
	protected override int AtlasIndex => 20;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					List<Hex> selected = await AbilityCmd.SelectHexes(state, list =>
					{
						foreach (Hex hex in RangeHelper.GetHexesInRange(state.Performer.Hex, 3, false))
						{
							if (hex != null && hex.IsFeatureless())
								list.Add(hex);
						}
					}, 0, 2, true, $"Create hazardous terrain in two featureless hexes within {Icons.Inline(Icons.Range)}3");

					foreach (Hex hex in selected)
					{
						await CreateHazardousTerrain(hex, state.Performer);
						foreach (Figure figure in hex.GetHexObjectsOfType<Figure>().Where(f => f.EnemiesWith(state.Performer)))
						{
							//may need to add skip
							await AbilityCmd.SufferDamage(figure, 2, state.Performer, false);
						}
					}
					state.SetPerformed();
				})
				.Build()),
		];

		public override int XP => 1;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					bool buffed = false;
					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							bool hazardous = parameters.Hex.HasHexObjectOfType<HazardousTerrain>();

							if (hazardous && !buffed)
							{
								await AbilityCmd.AddShield(parameters.Figure, this, 1);
								await AbilityCmd.AddRetaliate(parameters.Figure, this, 2, 1);
								buffed = true;
							}
							else if (!hazardous && buffed)
							{
								AbilityCmd.RemoveShield(parameters.Figure, this);
								AbilityCmd.RemoveRetaliate(parameters.Figure, this);
								buffed = false;
							}
							await GDTask.CompletedTask;

						});

					
					if (state.Performer.Hex.HasHexObjectOfType<HazardousTerrain>())
					{
						await AbilityCmd.AddShield(state.Performer, this, 1);
						await AbilityCmd.AddRetaliate(state.Performer, this, 2, 1);
						buffed = true;
					}
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
		public override bool Persistent => true;
	}
}
