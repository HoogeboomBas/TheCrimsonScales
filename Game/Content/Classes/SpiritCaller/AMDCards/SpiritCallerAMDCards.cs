using System.Collections.Generic;

public class SpiritCallerAMDCards
{
	public class PlusZero : SpiritCallerAMDCardModel
	{
		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
	}

	public class PlusZeroPlusTwoIfSpiritAttacked : SpiritCallerAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText:
				$"If a Spirit performed the attack, {Icons.Inline(Icons.GetAMDValue("+2"), richTextParameters)} instead");

		protected override int AtlasIndex => 1;

		public override int? GetValue(AttackAbility.State attackAbilityState) =>
			attackAbilityState == null ? +0 : (Spirit.CountsAsSpirit(attackAbilityState.Performer) ? +2 : +0);
	}

	public class PlusZeroPoisonRolling : SpiritCallerAMDCardModel
	{
		protected override int AtlasIndex => 3;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Poison1];
	}

	public class PlusOneAir : SpiritCallerAMDCardModel
	{
		protected override int AtlasIndex => 5;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Air)];
	}

	public class PlusOneDark : SpiritCallerAMDCardModel
	{
		protected override int AtlasIndex => 7;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Dark)];
	}

	public class PlusZeroPierceThreeRolling : SpiritCallerAMDCardModel
	{
		protected override int AtlasIndex => 9;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override int? Pierce => 3;
	}

	public class PlusZeroAddTargetRolling : SpiritCallerAMDCardModel
	{
		protected override int AtlasIndex => 11;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? AddedTargets => 1;
	}

	public class PlusOnePierceTwo : SpiritCallerAMDCardModel
	{
		protected override int AtlasIndex => 12;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override int? Pierce => 2;
	}

	public class PlusTwoPushTwo : SpiritCallerAMDCardModel
	{
		protected override int AtlasIndex => 13;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
		public override int? Push => 2;
	}

	public class PlusOneCurse : SpiritCallerAMDCardModel
	{
		protected override int AtlasIndex => 15;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Curse];
	}
}