using System;
using System.Collections.Generic;
using Fractural.Tasks;

public class ChainguardAMDCards
{
	public class PlusOneShackle : ChainguardAMDCardModel
	{
		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Chainguard.Shackle];
	}

	public class PlusZeroIfTargetHasShacklePlusTwo : ChainguardAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText:
				$"If the target has {Icons.Inline(Icons.GetCondition(Chainguard.Shackle), richTextParameters)}, {Icons.Inline(Icons.GetAMDValue("+2"), richTextParameters)} instead");

		protected override int AtlasIndex => 2;

		public override int? GetValue(AttackAbility.State attackAbilityState) =>
			attackAbilityState?.Target.HasCondition(Chainguard.Shackle) == true ? +2 : 0;
	}

	public class PlusZeroShieldOneRolling : ChainguardAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Shield, richTextParameters)}1", rolling: true);

		protected override int AtlasIndex => 4;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ShieldAbility.Builder().WithShieldValue(1).Build()
		];
	}

	public class PlusZeroRetaliateOneRolling : ChainguardAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Retaliate, richTextParameters)}1", rolling: true);

		protected override int AtlasIndex => 6;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			RetaliateAbility.Builder().WithRetaliateValue(1).Build()
		];
	}

	public class PlusZeroSwingThreeRolling : ChainguardAMDCardModel
	{
		protected override int AtlasIndex => 8;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override int? Swing => 3;
	}

	public class PlusTwoWound : ChainguardAMDCardModel
	{
		protected override int AtlasIndex => 11;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Wound1];
	}

	public class PlusOneIfTargetHasShackleDisarm : ChainguardAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText:
				$"If the target has {Icons.Inline(Icons.GetCondition(Chainguard.Shackle), richTextParameters)}, add {Icons.Inline(Icons.GetCondition(Conditions.Disarm), richTextParameters)}");

		protected override int AtlasIndex => 12;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) =>
			attackAbilityState?.Target.HasCondition(Chainguard.Shackle) == true ? [Conditions.Disarm] : [];
	}

	public class PlusOneCreateDamageTwoTrap : ChainguardAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText:
				$"Create one {Icons.Inline(Icons.Damage, richTextParameters)}2 trap in an empty hex within {Icons.Inline(Icons.Range, richTextParameters)}2");

		protected override int AtlasIndex => 13;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
			async (state, _) =>
			{
				await AbilityCmd.CreateTraps(damage: 2, range: 2, performer: state.Performer,
					assetPath: "res://Content/Classes/Chainguard/Traps/ChainguardTrap.tscn");
			};
	}

	public class PlusZeroHealOneSelfRolling : ChainguardAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}1, self", rolling: true);

		protected override int AtlasIndex => 14;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()
		];
	}

	public class PlusTwoShackle : ChainguardAMDCardModel
	{
		protected override int AtlasIndex => 16;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Chainguard.Shackle];
	}
}