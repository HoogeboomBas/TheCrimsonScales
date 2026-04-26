using System.Collections.Generic;

public class FireKnightAMDCards
{
	public class PlusZeroStrengthenAlly : FireKnightAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText:
				$"{Icons.Inline(Icons.GetCondition(Conditions.Strengthen), richTextParameters)}, {Icons.Inline(Icons.Targets, richTextParameters)}1 ally");

		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ConditionAbility.Builder()
				.WithConditions(Conditions.Strengthen)
				.WithTarget(Target.Allies)
				.WithInfiniteRange()
				.Build()
		];
	}

	public class PlusZeroHealOneRangeOneRolling : FireKnightAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}1, {Icons.Inline(Icons.Range, richTextParameters)}1", rolling: true);

		protected override int AtlasIndex => 2;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder()
				.WithHealValue(1)
				.WithRange(1)
				.Build()
		];
	}

	public class PlusZeroIfYouAreOnLadderPlusTwoInstead : FireKnightAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText:
				$"If you are on your {Icons.Inline(FireKnightCardSide.LadderIconPath, richTextParameters)}, {Icons.Inline(Icons.GetAMDValue("+2"), richTextParameters)} instead");

		protected override int AtlasIndex => 4;

		public override int? GetValue(AttackAbility.State attackAbilityState) =>
			attackAbilityState?.Performer.Hex.HasHexObjectOfType<Ladder>() == true ? +2 : +0;
	}

	public class PlusOneWound : FireKnightAMDCardModel
	{
		protected override int AtlasIndex => 8;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Wound1];
	}

	public class PlusOneFire : FireKnightAMDCardModel
	{
		protected override int AtlasIndex => 9;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Fire)];
	}

	public class PlusTwoWound : FireKnightAMDCardModel
	{
		protected override int AtlasIndex => 10;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Wound1];
	}

	public class PlusTwoFire : FireKnightAMDCardModel
	{
		protected override int AtlasIndex => 11;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Fire)];
	}

	public class PlusOneStrengthenAlly : FireKnightAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText:
				$"{Icons.Inline(Icons.GetCondition(Conditions.Strengthen), richTextParameters)}, {Icons.Inline(Icons.Targets, richTextParameters)}1 ally");

		protected override int AtlasIndex => 12;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ConditionAbility.Builder()
				.WithConditions(Conditions.Strengthen)
				.WithTarget(Target.Allies)
				.WithInfiniteRange()
				.Build()
		];
	}

	public class PlusZeroWoundRolling : FireKnightAMDCardModel
	{
		protected override int AtlasIndex => 13;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Wound1];
	}

	public class PlusZeroFireRolling : FireKnightAMDCardModel
	{
		protected override int AtlasIndex => 15;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Fire)];
	}
}