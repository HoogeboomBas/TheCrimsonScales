using System.Collections.Generic;
using System.Data;
using Fractural.Tasks;
using Godot;

public class ShrewdOvergrowth : ThornreaperCardModel<ShrewdOvergrowth.CardTop, ShrewdOvergrowth.CardBottom>
{
	public override string Name => "Overgrowth";
	public override int Level => 1;
	public override int Initiative => 92;
	protected override int AtlasIndex => 10;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async abilityState =>
				{
					Hex hex = abilityState.Performer.Hex;

					if (hex.IsFeatureless())
					{
						List<Hex> selectedHexes =
							await AbilityCmd.SelectHexes(abilityState, list => list.Add(hex), 0, 1, true, "Create hazardous terrain?");

						foreach (Hex selectedHex in selectedHexes)
						{
							await CreateHazardousTerrain(selectedHex);
							abilityState.SetPerformed();
						}
					}
				})
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async abilityState =>
				{
					bool roundPlayed = true;

					ScenarioEvents.RoundEndedEvent.Subscribe(abilityState, this, parameters => true,
						async parameters =>
						{
							if (roundPlayed)
							{
								await AbilityCmd.AddRetaliate(abilityState.Performer, this, 2, 1);
								await AbilityCmd.AddShield(abilityState.Performer, this, 2);
								ScenarioCheckEvents.PotentialTargetCheckEvent.Subscribe(abilityState, this,
								targetParameters => targetParameters.PotentialTarget == abilityState.Performer,
								targetParameters =>
								{
									targetParameters.AdjustTargetSortingInitiative(1 * 10000000 - abilityState.Performer.Initiative.SortingInitiative);
								});
								roundPlayed = false;
								await GDTask.CompletedTask;
							}
							else
							{
								await AbilityCmd.DiscardOrLose(GetAbilityCard(abilityState));
							}
							await GDTask.CompletedTask;
						});
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.PotentialTargetCheckEvent.Unsubscribe(state, this);
					AbilityCmd.RemoveRetaliate(state.Performer, this);
					AbilityCmd.RemoveShield(state.Performer, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
		public override bool Persistent => true;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
					{
						if (GetLightElementState() is ElementState.Strong or ElementState.Waning)
						{
							await AbilityCmd.InfuseElement(state, Element.Earth);
						}
					})
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveSquare(this, new Vector2(0.2f, 0.3f)))
				.WithMoveType(MoveType.Jump)
				.Build())
		];
	}
}