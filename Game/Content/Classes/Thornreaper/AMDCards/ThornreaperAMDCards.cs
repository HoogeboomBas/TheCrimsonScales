using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class ThornreaperAMDCards
{
	public class PlusZero : ThornreaperAMDCardModel
	{
		protected override int AtlasIndex => 1;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
	}

	public class PlusZeroCurse : ThornreaperAMDCardModel
	{
		protected override int AtlasIndex => 2;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Curse];
	}

	public class PlusZeroLightRolling : ThornreaperAMDCardModel
	{
		protected override int AtlasIndex => 4;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class PlusZeroEarthRolling : ThornreaperAMDCardModel
	{
		protected override int AtlasIndex => 5;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Earth)];
	}

	public class PlusOneGrantOneAllyShieldOne : ThornreaperAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +1, $"{Icons.Inline(Icons.Shield, richTextParameters)}1");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText: $"Grant one ally {Icons.Inline(Icons.Shield, richTextParameters)}1");

		protected override int AtlasIndex => 6;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			GrantAbility.Builder()
				.WithAbilities(
				[
					ShieldAbility.Builder().WithShieldValue(1).Build()
				])
				.WithInfiniteRange()
				.Build()
		];
	}

	public class PlusThree : ThornreaperAMDCardModel
	{
		protected override int AtlasIndex => 7;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +3;
	}

	public class PlusOneWoundMuddle : ThornreaperAMDCardModel
	{
		protected override int AtlasIndex => 9;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Wound1, Conditions.Muddle];
	}

	public class PlusZeroHealOneAllyOrSelfRolling : ThornreaperAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"{Icons.Inline(Icons.Heal, richTextParameters)}1 {Icons.Inline(Icons.Rolling, richTextParameters)}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText:
				$"{Icons.Inline(Icons.Heal, richTextParameters)}1, {Icons.Inline(Icons.Targets, richTextParameters)}1 ally or self");

		protected override int AtlasIndex => 11;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder()
				.WithHealValue(1)
				.WithInfiniteRange()
				.Build()
		];
	}
}