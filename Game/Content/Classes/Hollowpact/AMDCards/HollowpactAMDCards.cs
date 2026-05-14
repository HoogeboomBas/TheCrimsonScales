using System;
using System.Collections.Generic;
using Fractural.Tasks;

public class HollowpactAMDCards
{
	public class PlusZeroHealTwoSelf : HollowpactAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}2, self");

		protected override int AtlasIndex => 0;

		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()
		];
	}

	public class PlusZeroVoidsight : HollowpactAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"Voidsight");

		protected override int AtlasIndex => 2;

		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			Hollowpact.VoidsightAbilityBuilder().Build()
		];
	}

	public class MinusTwoEarth : HollowpactAMDCardModel
	{
		protected override int AtlasIndex => 4;

		public override int? GetValue(AttackAbility.State attackAbilityState) => -2;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Earth)];
	}

	public class PlusTwoDark : HollowpactAMDCardModel
	{
		protected override int AtlasIndex => 5;

		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Dark)];
	}
	
	public class MinusTwoStun : HollowpactAMDCardModel
	{
		protected override int AtlasIndex => 10;

		public override int? GetValue(AttackAbility.State attackAbilityState) => -2;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Stun];
	}

	public class PlusZeroDisarm : HollowpactAMDCardModel
	{
		protected override int AtlasIndex => 12;

		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Disarm];
	}

	public class MinusOneWildElement : HollowpactAMDCardModel
	{
		protected override int AtlasIndex => 13;

		public override int? GetValue(AttackAbility.State attackAbilityState) => -1;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.InfuseWild()];
	}

	public class PlusOneVoidEnergyRolling : HollowpactAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText: $"{Icons.Inline(Hollowpact.VoidEnergy, richTextParameters)}", rolling: true);
		protected override int AtlasIndex => 14;

		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
			async (state, _) =>
			{
				if(state.Performer is Hollowpact hollowpact)
				{
					hollowpact.GainVoidEnergy(1);
				}

				await GDTask.CompletedTask;
			};
	}
	
	public class MinusOneCurseRolling : HollowpactAMDCardModel
	{
		protected override int AtlasIndex => 15;

		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => -1;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Curse];
	}

	public class PlusThreeRegenerateSelf : HollowpactAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +3,
				extraText: $"{Icons.Inline(Icons.GetCondition(Conditions.Regenerate), richTextParameters)}, self");

		protected override int AtlasIndex => 18;

		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ConditionAbility.Builder().WithConditions(Conditions.Regenerate).WithTarget(Target.Self).Build()
		];
	}

	public class PlusOneVoidPitRangeTwo : HollowpactAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText: $"Create a void pit in an empty hex within {Icons.Inline(Icons.Range, richTextParameters)}2");
	
		protected override int AtlasIndex => 20;

		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			Hollowpact.CreateVoidPitObstacleAbilityBuilder()
				.WithRange(2)
				.Build(),
		];
	}

	public class PlusZeroWardSelf : HollowpactAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.GetCondition(Conditions.Ward), richTextParameters)}, self");

		protected override int AtlasIndex => 22;

		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ConditionAbility.Builder().WithConditions(Conditions.Ward).WithTarget(Target.Self).Build()
		];
	}
}