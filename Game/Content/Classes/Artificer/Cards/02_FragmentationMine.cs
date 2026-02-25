using System.Collections.Generic;
using System.Linq;
using Godot;

public class FragmentationMine : ArtificerCardModel<FragmentationMine.CardTop, FragmentationMine.CardBottom>
{
	public override string Name => "Fragmentation Mine";
	public override int Level => 1;
	public override int Initiative => 45;
	protected override int AtlasIndex => 2;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateTrapAbility.Builder()
				.WithDamage(3)
				.WithRange(2)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					CreateTrapAbility.State trapState = state.ActionState.GetAbilityState<CreateTrapAbility.State>(0);

					ScenarioEvents.TrapTriggeredEvent.Subscribe(state, this,
						canApply: canApplyParameters => trapState.CreatedTraps.Contains(canApplyParameters.Trap),
						async applyParameters =>
						{
							foreach(Figure figure in RangeHelper.GetFiguresInRange(applyParameters.Hex, 1)
								        .Where(figure => figure.EnemiesWith(state.Performer)))
							{
								await AbilityCmd.SufferDamage(state, figure, 1);
							}

							await AbilityCmd.GainXP(state.Performer, 1);
							await state.ActionState.RequestDiscardOrLose();
						}
					);
					await AbilityCmd.AddCharacterToken(state, trapState.CreatedTraps[0],
						$"All adjacent enemies suffer {Icons.Inline(Icons.Damage)}1 when this trap is sprung.");
					state.ActionState.SetOverridePersistent();
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.TrapTriggeredEvent.Unsubscribe(state, this);
					await AbilityCmd.RemoveCharacterToken(state, state.ActionState.GetAbilityState<CreateTrapAbility.State>(0).CreatedTraps[0]);
				})
				.WithConditionalAbilityCheck(async state => await AbilityCmd.HasPerformedAbility(state, 0) &&
				                                            await LoseScrapTokensConditionalAbilityCheck(state.Performer, 1,
					                                            new TextEffectInfoView.Parameters(
						                                            $"All enemies adjacent to the created trap suffer {Icons.Inline(Icons.Damage)}1 when it is sprung")))
				.Build())
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62138146f, 0.70201635f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					//TODO: Change to directly selecting the overlay tile
					Hex hex = await AbilityCmd.SelectHex(state,
						list => list.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 1).Where(hex =>
							hex.HasHexObjectOfType<Trap>() || (hex.TryGetHexObjectOfType(out Obstacle obs) &&
							                                   obs.HexObjectShape == HexObjectShape.Single && !obs.CannotBeDestroyed))),
						hintText: "Select a trap or 1-hex obstacle to destroy");
					if(hex == null)
					{
						return;
					}

					if(hex.TryGetHexObjectOfType(out Obstacle obstacle))
					{
						await obstacle.Destroy();
					}
					else
					{
						await hex.GetHexObjectOfType<Trap>().Destroy();
					}

					await GainScrapToken(state);
					await AbilityCmd.GainXP(state.Performer, 1);
					state.SetPerformed();
				})
				.Build())
		];
	}
}