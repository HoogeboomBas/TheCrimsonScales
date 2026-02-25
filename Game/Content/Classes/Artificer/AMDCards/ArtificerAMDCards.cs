using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class ArtificerAMDCards
{
	public class MinusOneGainScrap : ArtificerAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, -1,
				extraText: $"Gain 1{Icons.Inline(Artificer.ScrapToken, richTextParameters)}");

		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => -1;

		public override Func<AttackAbility.State, GDTask> GetExtraEffects(AttackAbility.State attackAbilityState) =>
			async state =>
			{
				await ArtificerCardSide.GainScrapToken(GetCharacter(attackAbilityState));
			};
	}

	public class PlusOneGainScrap : ArtificerAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText: $"Gain 1{Icons.Inline(Artificer.ScrapToken, richTextParameters)}");

		protected override int AtlasIndex => 1;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override Func<AttackAbility.State, GDTask> GetExtraEffects(AttackAbility.State attackAbilityState) =>
			async state =>
			{
				await ArtificerCardSide.GainScrapToken(GetCharacter(attackAbilityState));
			};
	}

	public class PlusThreeDisarmGainScrap : ArtificerAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +3, [Conditions.Disarm],
				extraText: $"Gain 1{Icons.Inline(Artificer.ScrapToken, richTextParameters)}");

		protected override int AtlasIndex => 2;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +3;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Disarm];

		public override Func<AttackAbility.State, GDTask> GetExtraEffects(AttackAbility.State attackAbilityState) =>
			async state =>
			{
				await ArtificerCardSide.GainScrapToken(GetCharacter(attackAbilityState));
			};
	}

	public class PlusOne : ArtificerAMDCardModel
	{
		protected override int AtlasIndex => 3;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
	}

	public class PlusZeroPierceTwoRolling : ArtificerAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Pierce, richTextParameters)}2", rolling: true);

		protected override int AtlasIndex => 5;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override int? Pierce => 3;
	}

	public class PlusOneWoundIfDrawnBySummon : ArtificerAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText: $"Add {Icons.Inline(Icons.GetCondition(Conditions.Wound1), richTextParameters)} if drawn by a summon");

		protected override int AtlasIndex => 9;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) =>
			attackAbilityState.Performer is Summon ? [Conditions.Wound1] : [];
	}

	public class PlusZeroCreateDamageTwoTrapRolling : ArtificerAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"Create one {Icons.Inline(Icons.Damage, richTextParameters)}2 trap in an empty hex adjacent to the target",
				rolling: true);

		protected override int AtlasIndex => 12;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override Func<AttackAbility.State, GDTask> GetExtraEffects(AttackAbility.State attackAbilityState) =>
			async state =>
			{
				Hex hex = await AbilityCmd.SelectHex(state,
					hexes => hexes.AddRange(RangeHelper.GetHexesInRange(attackAbilityState.Target.Hex, 1).Where(hex => hex.IsEmpty())),
					hintText: $"Select a hex to create a {Icons.HintText(Icons.Damage)}2 trap");
				if(hex != null)
				{
					await AbilityCmd.CreateTrap(hex, "res://Content/OverlayTiles/Traps/BearTrap1H.tscn", 2);
				}
			};
	}

	public class PlusFour : ArtificerAMDCardModel
	{
		protected override int AtlasIndex => 15;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +4;
	}

	public class PlusOneRolling : ArtificerAMDCardModel
	{
		protected override int AtlasIndex => 16;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
	}
}