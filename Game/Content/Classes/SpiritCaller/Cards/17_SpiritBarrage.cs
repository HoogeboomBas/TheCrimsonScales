using System.Collections.Generic;
using System.Linq;
using Godot;

public class SpiritBarrage : SpiritCallerCardModel<SpiritBarrage.CardTop, SpiritBarrage.CardBottom>
{
	public override string Name => "Spirit Barrage";
	public override int Level => 4;
	public override int Initiative => 51;
	protected override int AtlasIndex => 28 - 17;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.618984f, 0.16982488f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Air,
						applyFunction: async parameters =>
						{
							parameters.AbilityState.AdjustTargets(1);

							await GainXP(parameters.AbilityState);
						}))
				.WithOnAbilityStarted(async state =>
				{
					Figure spirit = await Spirit.SelectSpirit(state);

					if(spirit != null)
					{
						state.SetCustomValue(this, "Hex", spirit.Hex);
						state.AbilityAdjustAttackValue(1);
					}
				})
				.WithCustomGetPerformHex(state => state.GetCustomValue<Hex>(this, "Hex"))
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(state.TryGetCustomValue(this, "Hex", out Hex hex))
					{
						await GainXP(state);
					}
				})
				.Build())
		];
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
					MoveAbility.Builder()
						.WithDistance(2)
						.Build())
				.WithCustomGetTargets((state, list) =>
				{
					list.AddRange(Spirit.GetAllSpirits());
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Dark))
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Figure spirit = await Spirit.SelectSpirit(state);

					if(spirit == null)
					{
						return;
					}

					Figure swapped = await AbilityCmd.SelectFigure(state, list =>
					{
						list.AddRange(RangeHelper.GetFiguresInRange(spirit, 6, requiresLineOfSight: false)
							.Where(figure => AbilityCmd.CanSwap(figure, spirit)));
					}, mandatory: false, hintText: () => "Choose a figure for the spirit to swap hexes with");

					if(swapped == null)
					{
						return;
					}

					await AbilityCmd.TrySwap(state, spirit, swapped);
				})
				.Build())
		];
	}
}