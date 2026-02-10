using System.Collections.Generic;
using System.Linq;
using Godot;

public class SteelsilkWeaver : AmberAegisCardModel<SteelsilkWeaver.CardTop, SteelsilkWeaver.CardBottom>
{
	public override string Name => "Steelsilk Weaver";
	public override int Level => 2;
	public override int Initiative => 41;
	protected override int AtlasIndex => 15;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Immobilize)
				.WithTargets(3, new TargetsSquare(this, new Vector2(0.4849852f, 0.26382118f)))
				.WithRange(2, new RangeSquare(this, new Vector2(0.697037f, 0.26382118f)))
				.WithOnAbilityEndedPerformed(async state =>
				{
					foreach(Figure figure in state.UniqueTargetedFigures)
					{
						await AbilityCmd.SufferDamage(state, figure, 1);
					}
				})
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithRange(1)
				.WithConditions(Conditions.Regenerate)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Immobilize)
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.ActionState.GetAbilityState<HealAbility.State>(0).UniqueTargetedFigures);
				})
				.WithMandatory(true)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Ward)
				.WithTarget(Target.Allies | Target.TargetAll)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(GameController.Instance.Map.Figures.Where(figure =>
						figure.AlliedWith(state.Performer) && figure.HasCondition(Conditions.Immobilize)));
				})
				.Build())
		];
	}
}