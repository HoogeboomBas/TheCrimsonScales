using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ReveredProtector : HierophantLevelUpCardModel<ReveredProtector.CardTop, ReveredProtector.CardBottom>
{
	public override string Name => "Revered Protector";
	public override int Level => 7;
	public override int Initiative => 15;
	protected override int AtlasIndex => 15 - 10;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.FromAttack &&
						                      ((AttackAbility.State)canApplyParameters.PotentialAbilityState).Target
						                      .AlliedWith(state.Performer, true) &&
						                      RangeHelper.Distance(((AttackAbility.State)canApplyParameters.PotentialAbilityState).Target.Hex,
							                      state.Performer.Hex) <= 1,
						async applyParameters =>
						{
							int shieldVal = state.UseSlotIndex switch
							{
								0 => 1,
								1 => 2,
								2 or 3 => 3,
								4 => 4,
								_ => 0
							};
							applyParameters.AdjustShield(shieldVal);
							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.AfterSufferDamageEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.2825028f, 0.31599984f)),
						new UseSlot(new Vector2(0.48800266f, 0.31599984f)),
						new UseSlot(new Vector2(0.6910024f, 0.31599984f)),
						new UseSlot(new Vector2(0.37300277f, 0.43199965f)),
						new UseSlot(new Vector2(0.57800215f, 0.43199965f))
					]
				)
				.Build())
		];

		public override bool Round => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.WithMoveType(MoveType.Jump)
				.WithOnAbilityEndedPerformed(async state =>
				{
					foreach(Figure figure in state.Hexes
						        .SelectMany(hex => hex.GetHexObjectsOfType<Figure>().Where(figure => figure.AlliedWith(state.Performer))).Distinct())
					{
						await AbilityCmd.RemoveOneNegativeCondition(figure);
					}
				})
				.Build()),
		];
	}
}