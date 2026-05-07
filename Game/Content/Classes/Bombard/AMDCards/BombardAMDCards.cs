using System.Collections.Generic;
using System.Linq;

public class BombardAMDCards
{
	public class PlusZeroShieldOneRolling : BombardAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Shield, richTextParameters)}1",
				rolling: true);

		protected override int AtlasIndex => 0;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ShieldAbility.Builder().WithShieldValue(1).Build()
		];
	}

	public class PlusZeroPlusThreeIfProjectile : BombardAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText:
				$"If this attack is a {Icons.Inline(BombardCardSide.ProjectileIconPath, richTextParameters)}, {Icons.Inline(Icons.GetAMDValue("+3"), richTextParameters)} instead");

		protected override int AtlasIndex => 2;

		public override int? GetValue(AttackAbility.State attackAbilityState) =>
			attackAbilityState?.ActionState.ParentActionState != null &&
			attackAbilityState.ActionState.ParentActionState.AbilityStates.Any(potentialProjectileAbility =>
				potentialProjectileAbility is ProjectileAbility.State)
				? +3
				: +0;
	}

	public class PlusZeroPierceThreeRolling : BombardAMDCardModel
	{
		protected override int AtlasIndex => 4;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override int? Pierce => 3;
	}

	public class PlusOneWound : BombardAMDCardModel
	{
		protected override int AtlasIndex => 6;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Wound1];
	}

	public class PlusZeroStun : BombardAMDCardModel
	{
		protected override int AtlasIndex => 7;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Stun];
	}

	public class PlusOneRetaliateOne : BombardAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText: $"{Icons.Inline(Icons.Retaliate, richTextParameters)}1, {Icons.Inline(Icons.Range, richTextParameters)}3");

		protected override int AtlasIndex => 8;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			RetaliateAbility.Builder().WithRetaliateValue(1).WithRange(3).Build()
		];
	}

	public class PlusZeroStrengthenSelf : BombardAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.GetCondition(Conditions.Strengthen), richTextParameters)}, self");

		protected override int AtlasIndex => 10;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ConditionAbility.Builder().WithConditions(Conditions.Strengthen).WithTarget(Target.Self).Build()
		];
	}

	public class PlusTwoImmobilize : BombardAMDCardModel
	{
		protected override int AtlasIndex => 11;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Immobilize];
	}

	public class PlusZeroHealOneSelfRolling : BombardAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}1, self",
				rolling: true);

		protected override int AtlasIndex => 13;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).Build()
		];
	}

	public class PlusOnePullSelfTowardTarget : BombardAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText: $"{Icons.Inline(Icons.Pull, richTextParameters)}3, self, toward the target");

		protected override int AtlasIndex => 15;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			PullSelfAbility.Builder().WithPullSelfValue(3).WithCustomGetTargets(((state, list) => list.Add(attackAbilityState.Target))).Build()
		];
	}
}