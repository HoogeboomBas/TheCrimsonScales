using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class LadderAssault : FireKnightLevelUpCardModel<LadderAssault.CardTop, LadderAssault.CardBottom>
{
	public override string Name => "Ladder Assault";
	public override int Level => 3;
	public override int Initiative => 47;
	protected override int AtlasIndex => 13;

	public class CardTop : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.5020886f, 0.22517207f)))
				.WithPush(2)
				.WithOnAbilityStarted(async state =>
				{
					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters =>
							parameters.PotentialAbilityState == state &&
							parameters.Hex.HasHexObjectOfType<Ladder>(),
						async parameters =>
						{
							await AbilityCmd.SufferDamage(state, parameters.Figure, 2);
							await AbilityCmd.GainXP(state.Performer, 1);
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
					{
						ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6176377f, 0.67424977f)))
				.WithOnAbilityStarted(async abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Subscribe(abilityState, this,
						canApplyParameters =>
							canApplyParameters.AbilityState == abilityState &&
							(canApplyParameters.Hex.HasHexObjectOfType<DifficultTerrain>() ||
							 canApplyParameters.Hex.HasHexObjectOfType<HazardousTerrain>()),
						applyParameters =>
						{
							if(applyParameters.Hex.HasHexObjectOfType<DifficultTerrain>())
							{
								applyParameters.SetMoveCost(1);
							}

							if(applyParameters.Hex.HasHexObjectOfType<HazardousTerrain>())
							{
								applyParameters.SetAffectedByNegativeHex(false);
							}
						}
					);

					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(abilityState, this,
						canApplyParameters => canApplyParameters.PotentialAbilityState?.Performer == abilityState.Performer,
						async applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							await GDTask.CompletedTask;
						}
					);

					if(abilityState.Performer.Hex.HasHexObjectOfType<Ladder>())
					{
						abilityState.AdjustMoveValue(1);
						abilityState.AddJump();
					}

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async abilityState =>
					{
						ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(abilityState, this);
						ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(abilityState, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];
	}
}