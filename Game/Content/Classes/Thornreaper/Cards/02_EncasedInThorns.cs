using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Fractural.Tasks;
using Godot;

public class EncasedInThorns : ThornreaperCardModel<EncasedInThorns.CardTop, EncasedInThorns.CardBottom>
{
	public override string Name => "Encased in Thorns";
	public override int Level => 1;
	public override int Initiative => 22;
	protected override int AtlasIndex => 0;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(1)
				.WithRange(2)
				.WithOnAbilityStarted(async abilityState =>
					{
						if (abilityState.Performer.Hex.HasHexObjectOfType<HazardousTerrain>())
						{
							abilityState.AdjustRetaliateValue(1);
							await AbilityCmd.GainXP(abilityState.Performer, 1);
						}
					})
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
					{
						ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this, canApplyParameters => 
							canApplyParameters.Figure == state.Performer && GetLightElementState() is ElementState.Strong or ElementState.Waning,
							applyParameters =>
								{
									applyParameters.AdjustShield(1);
								}
							);
						AppController.Instance.AudioController.PlayFastForwardable(SFX.Shield, delay: 0f);

						await GDTask.CompletedTask;
					})
					.WithOnDeactivate(async state =>
						{
							ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
							await GDTask.CompletedTask;
						})
					.Build())
		];
		public override bool Round => true;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveSquare(this, new Vector2(0.2f, 0.3f)))
				.WithOnAbilityStarted(async abilityState =>
				{
					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(abilityState, this,
						canApplyParameters => canApplyParameters.PotentialAbilityState?.Performer == abilityState.Performer,
						applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							return GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async abilityState =>
				{
					ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(abilityState, this);

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.WithConditionalAbilityCheck(async state => 
					{
						await GDTask.CompletedTask;
						return state.Performer.Hex.HasHexObjectOfType<HazardousTerrain>();
					}
				).Build()),
		];
		public override bool Round => true;
	}
}