using System.Collections.Generic;

public class RimehearthAMDCards
{
	public class PlusZeroChill : RimehearthAMDCardModel
	{
		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Chill];
	}

	public class PlusZeroWoundRolling : RimehearthAMDCardModel
	{
		protected override int AtlasIndex => 2;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Wound1];
	}

	public class PlusOneIce : RimehearthAMDCardModel
	{
		protected override int AtlasIndex => 4;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Ice)];
	}

	public class PlusZeroFireRolling : RimehearthAMDCardModel
	{
		protected override int AtlasIndex => 6;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Fire)];
	}

	public class PlusZeroHealThreeSelfWoundSelfRolling : RimehearthAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}3, self, {Icons.InlineCondition(Conditions.Wound1, richTextParameters)}",
				rolling: true);

		protected override int AtlasIndex => 8;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder().WithHealValue(3).WithTarget(Target.Self).Build(),
			ConditionAbility.Builder().WithConditions(Conditions.Wound1).WithTarget(Target.Self).WithMandatory(true).Build()
		];
	}

	public class PlusOneRolling : RimehearthAMDCardModel
	{
		protected override int AtlasIndex => 9;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
	}

	public class PlusOneWound : RimehearthAMDCardModel
	{
		protected override int AtlasIndex => 10;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Wound1];
	}

	public class PlusOneHealOneSelf : RimehearthAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1, extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}1, self", rolling: true);

		protected override int AtlasIndex => 11;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()
		];
	}

	public class PlusThreeChill : RimehearthAMDCardModel
	{
		protected override int AtlasIndex => 12;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +3;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Chill];
	}

	public class PlusTwoFireIce : RimehearthAMDCardModel
	{
		protected override int AtlasIndex => 13;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse([Element.Fire, Element.Ice])];
	}

	public class PlusZeroBrittle : RimehearthAMDCardModel
	{
		protected override int AtlasIndex => 15;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Brittle];
	}

	public class PlusZeroFireIceRolling : RimehearthAMDCardModel
	{
		protected override int AtlasIndex => 16;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse([Element.Fire, Element.Ice])];
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
	}
}