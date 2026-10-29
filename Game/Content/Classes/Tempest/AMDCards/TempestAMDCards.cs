using System.Collections.Generic;

public class TempestAMDCards
{
	public class MinusOneAirLight : TempestAMDCardModel
	{
		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => -1;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse([Element.Air, Element.Light])];
	}

	public class PlusOneAirLight : TempestAMDCardModel
	{
		protected override int AtlasIndex => 1;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse([Element.Air, Element.Light])];
	}

	public class PlusZeroWound : TempestAMDCardModel
	{
		protected override int AtlasIndex => 2;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Wound1];
	}

	public class PlusZeroRegenerateRangeOneRolling : TempestAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"{Icons.InlineCondition(Conditions.Regenerate, richTextParameters)}, {Icons.Inline(Icons.Range, richTextParameters)}1{Icons.Inline(Icons.Rolling, richTextParameters)}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.InlineCondition(Conditions.Regenerate, richTextParameters)}, {Icons.Inline(Icons.Range, richTextParameters)}1",
				rolling: true);

		protected override int AtlasIndex => 4;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ConditionAbility.Builder().WithConditions(Conditions.Regenerate).WithRange(1).Build()
		];
	}

	public class PlusTwoMuddle : TempestAMDCardModel
	{
		protected override int AtlasIndex => 6;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Muddle];
	}

	public class PlusOneImmobilize : TempestAMDCardModel
	{
		protected override int AtlasIndex => 7;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Immobilize];
	}

	public class PlusOneDodgeSelf : TempestAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +1, $"{Icons.InlineCondition(Conditions.Dodge, richTextParameters)}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1, extraText: $"{Icons.InlineCondition(Conditions.Dodge, richTextParameters)}, self");

		protected override int AtlasIndex => 8;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ConditionAbility.Builder().WithConditions(Conditions.Dodge).WithTarget(Target.Self).Build()
		];
	}

	public class PlusTwoAirLight : TempestAMDCardModel
	{
		protected override int AtlasIndex => 10;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse([Element.Air, Element.Light])];
	}
}