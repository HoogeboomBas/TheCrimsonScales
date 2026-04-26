using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class HierophantAMDCards
{
	public class MinusOneGivePrayerCard : HierophantAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, -1,
				extraText: "Give one ally one PRAYER card");

		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => -1;

		public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
			async (state, potentialDeckOwner) =>
			{
				if(potentialDeckOwner is Hierophant hierophant)
				{
					Figure chosenFigure = await AbilityCmd.SelectFigure(hierophant,
						figures => figures.AddRange(
							GameController.Instance.Map.Figures.Where(possibleFigure =>
								possibleFigure.AlliedWith(hierophant) && possibleFigure is Character)),
						hintText: () => "Select an ally to give a PRAYER card");
					if(chosenFigure == null)
					{
						return;
					}

					await HierophantCardSide.GivePrayerCard(state, hierophant, chosenFigure);
				}
			};
	}

	public class PlusZero : HierophantAMDCardModel
	{
		protected override int AtlasIndex => 1;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
	}

	public class PlusZeroCurse : HierophantAMDCardModel
	{
		protected override int AtlasIndex => 2;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Curse];
	}

	public class PlusZeroLightRolling : HierophantAMDCardModel
	{
		protected override int AtlasIndex => 4;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class PlusZeroEarthRolling : HierophantAMDCardModel
	{
		protected override int AtlasIndex => 5;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Earth)];
	}

	public class PlusOneGrantOneAllyShieldOne : HierophantAMDCardModel
	{
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

	public class PlusThree : HierophantAMDCardModel
	{
		protected override int AtlasIndex => 7;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +3;
	}

	public class PlusOneWoundMuddle : HierophantAMDCardModel
	{
		protected override int AtlasIndex => 9;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Wound1, Conditions.Muddle];
	}

	public class PlusZeroHealOneAllyOrSelfRolling : HierophantAMDCardModel
	{
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