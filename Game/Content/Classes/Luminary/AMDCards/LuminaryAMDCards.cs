using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class LuminaryAMDCards
{
	public class MinusTwoPerformGlowAbilityWithoutConsumingElement : LuminaryAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, -2,
				extraText: $"Perform a {Icons.Inline(LuminaryCardSide.GlowIconPath, richTextParameters)} without consuming an element");

		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => -2;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			LuminaryCardSide.PerformFreeGlow()
		];
	}

	public class PlusZeroFire : LuminaryAMDCardModel
	{
		protected override int AtlasIndex => 1;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Fire)];
	}

	public class PlusZeroIce : LuminaryAMDCardModel
	{
		protected override int AtlasIndex => 2;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Ice)];
	}

	public class PlusZeroLight : LuminaryAMDCardModel
	{
		protected override int AtlasIndex => 3;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class PlusZeroDark : LuminaryAMDCardModel
	{
		protected override int AtlasIndex => 4;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Dark)];
	}

	public class PlusZeroWild : LuminaryAMDCardModel
	{
		protected override int AtlasIndex => 5;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.InfuseWild()];
	}

	public class PlusTwo : LuminaryAMDCardModel
	{
		protected override int AtlasIndex => 7;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
	}

	public class PlusZeroPerformPoisonAbility : LuminaryAMDCardModel
	{
		//TODO: Some way to show what the actual area of effect is
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"Perform {Icons.Inline(Icons.GetCondition(Conditions.Poison1), richTextParameters)} area of effect ability");

		protected override int AtlasIndex => 8;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ConditionAbility.Builder()
				.WithConditions(Conditions.Poison1)
				.WithAOEPattern(new AOEPattern(
				[
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red)
				]))
				.Build()
		];
	}

	public class PlusOneHealOneSelfRolling : LuminaryAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}1, self", rolling: true);

		protected override int AtlasIndex => 10;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()
		];
	}

	public class PlusZeroConsumeElementToInfuseElementRolling : LuminaryAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"Consume {Icons.Inline(Icons.WildElement, richTextParameters)} to {Icons.Inline(Icons.WildElement, richTextParameters)}",
				rolling: true);

		protected override int AtlasIndex => 12;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.ConsumeWildInfuseWild()];
	}
}