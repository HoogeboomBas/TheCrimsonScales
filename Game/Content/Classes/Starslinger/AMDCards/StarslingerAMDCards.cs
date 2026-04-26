using System.Collections.Generic;

public class StarslingerAMDCards
{
	public class MinusOneInvisibleSelf : StarslingerAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, -1,
				extraText: $"{Icons.Inline(Icons.GetCondition(Conditions.Invisible), richTextParameters)}, self");

		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => -1;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ConditionAbility.Builder().WithConditions(Conditions.Invisible).WithTarget(Target.Self).Build()
		];
	}

	public class PlusZeroImmobilizeRolling : StarslingerAMDCardModel
	{
		protected override int AtlasIndex => 1;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Immobilize];
	}

	public class PlusZeroControlTargetMoveOneRolling : StarslingerAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"Control the target: {Icons.Inline(Icons.Move, richTextParameters)}1", rolling: true);

		protected override int AtlasIndex => 3;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ControlAbility.Builder()
				.WithAbilities(
				[
					MoveAbility.Builder().WithDistance(1).Build()
				])
				.WithCustomGetTargets((_, figures) =>
				{
					figures.Add(attackAbilityState.Target);
				})
				.Build()
		];
	}

	public class PlusOneLight : StarslingerAMDCardModel
	{
		protected override int AtlasIndex => 5;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class PlusOneHealOneRangeThree : StarslingerAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}1, {Icons.Inline(Icons.Range)}3");

		protected override int AtlasIndex => 7;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder().WithHealValue(1).WithRange(3).Build()
		];
	}

	public class PlusOneIfYouAreUndamagedPlusThreeInstead : BombardAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText: $"If you are undamaged, {Icons.Inline(Icons.GetAMDValue("+3"), richTextParameters)} instead");

		protected override int AtlasIndex => 9;
		public override int? GetValue(AttackAbility.State attackAbilityState) => attackAbilityState?.Performer.IsDamaged() == false ? +3 : +1;
	}

	public class PlusZeroHealOneRangeOneRolling : StarslingerAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}1, {Icons.Inline(Icons.Range)}1", rolling: true);

		protected override int AtlasIndex => 11;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder().WithHealValue(1).WithRange(1).Build()
		];
	}

	public class PlusZeroLootOneRolling : StarslingerAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Loot, richTextParameters)}1", rolling: true);

		protected override int AtlasIndex => 13;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			LootAbility.Builder().WithRange(1).Build()
		];
	}

	public class PlusZeroDark : StarslingerAMDCardModel
	{
		protected override int AtlasIndex => 5;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Dark)];
	}
}