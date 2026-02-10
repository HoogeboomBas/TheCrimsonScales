using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class DivideAndConquer : AmberAegisCardModel<DivideAndConquer.CardTop, DivideAndConquer.CardBottom>
{
	public override string Name => "Divide and Conquer";
	public override int Level => 9;
	public override int Initiative => 97;
	protected override int AtlasIndex => 28;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					int damage = await AbilityCmd.SufferDamage(state, state.Performer, (state.Performer.Health + 1) / 2);
					state.SetPerformed();
					state.SetCustomValue(this, "DamagedSuffered", damage);
				})
				.Build()),
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Beta Aegis")
				.WithTexturePath("res://Content/Classes/AmberAegis/beta_aegis.png")
				.WithHealth(0)
				.WithMove(3)
				.WithTraits()
				//TODO: Turn End trait (requires brightspark)
				.WithOnAbilityStarted(async state =>
				{
					state.AdjustHealth(state.ActionState.GetAbilityState<OtherAbility.State>(0)
						.GetCustomValue<int>(this, "DamagedSuffered"));
					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build()),
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					ColonyToken colonyToken = await PlaceAnyColonyToken(state, list => list.AddRange(RangeHelper
						.GetHexesInRange(state.Performer.Hex, 2)
						.Where(hex => hex.IsEmpty() && !hex.HasHexObjectOfType<ColonyToken>())));
					state.SetCustomValue(this, "ColonyToken", colonyToken);
				})
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Disarm)
				.WithTargets(2)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(RangeHelper.GetFiguresInRange(
						state.ActionState.GetAbilityState<OtherAbility.State>(0).GetCustomValue<ColonyToken>(this, "ColonyToken").Hex, 1));
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
				.Build())
		];
	}
}