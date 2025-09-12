using System.Collections.Generic;
using Fractural.Tasks;

public class RadiantForestFungi : MirefootCardModel<RadiantForestFungi.CardTop, RadiantForestFungi.CardBottom>
{
	public override string Name => "Radiant Forest Fungi";
	public override int Level => 4;
	public override int Initiative => 06;
	protected override int AtlasIndex => 17;

	public class CardTop : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async abilityState =>
					{
						List<Hex> selectedHexes = await AbilityCmd.SelectHexes(abilityState,
							list =>
							{
								foreach(Hex possibleHex in RangeHelper.GetHexesInRange(abilityState.Performer.Hex, 1, true))
								{
									if(possibleHex != null && possibleHex.IsFeatureless())
									{
										list.Add(possibleHex);
									}
								}
							},
							0, 2, false, "Place difficult terrain in up to two adjacent hexes"
						);

						foreach(Hex selectedHex in selectedHexes)
						{
							await CreateDifficultTerrain(selectedHex);

							abilityState.SetPerformed();
						}
					}
				)
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
						parameters =>
							state.Performer.AlliedWith(parameters.Figure, true) &&
							parameters.Figure.Hex.HasHexObjectOfType<DifficultTerrain>(),
						applyParameters =>
						{
							applyParameters.AdjustShield(2);
						}
					);

					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters =>
							state.Performer.AlliedWith(parameters.Figure, true) && parameters.FromAttack &&
							parameters.Figure.Hex.HasHexObjectOfType<DifficultTerrain>(),
						async parameters =>
						{
							parameters.AdjustShield(2);

							await GDTask.CompletedTask;
						}
					);

					ScenarioEvents.FigureEnteredHexEvent.Subscribe(state, this,
						parameters => state.Performer.AlliedWith(parameters.Figure, true),
						async parameters =>
						{
							ScenarioCheckEvents.ShieldCheckEvent.FireChangedEvent();

							await GDTask.CompletedTask;
						},
						EffectType.Visuals
					);

					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(state, this,
						parameters => state.Performer.AlliedWith(parameters.Figure, true),
						parameters =>
						{
							parameters.Add(
								new FigureInfoTextExtraEffect.Parameters(
									$"Gain {Icons.Inline(Icons.Shield)}2 while occupying difficult terrain this round."));
						}
					);

					AppController.Instance.AudioController.PlayFastForwardable(SFX.Shield, delay: 0f);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
						ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
						ScenarioEvents.FigureEnteredHexEvent.Unsubscribe(state, this);
						ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		protected override int XP => 1;
		protected override bool Round => true;
	}

	public class CardBottom : MirefootCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.WithOnAbilityStarted(async abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Subscribe(abilityState, this,
						canApplyParameters =>
							canApplyParameters.AbilityState == abilityState &&
							(canApplyParameters.Hex.HasHexObjectOfType<DifficultTerrain>()),
						applyParameters =>
						{
							if(applyParameters.Hex.HasHexObjectOfType<DifficultTerrain>())
							{
								applyParameters.SetMoveCost(0);
							}
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async abilityState =>
					{
						ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(abilityState, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];
	}
}