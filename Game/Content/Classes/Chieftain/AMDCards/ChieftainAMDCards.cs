using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class ChieftainAMDCards
{
	public class PlusZeroPoison : ChieftainAMDCardModel
	{
		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Poison1];
	}

	public class PlusZeroHealOneChieftain : ChieftainAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText:
				$"{Icons.Inline(Icons.Heal, richTextParameters)}1, {Icons.Inline(Icons.Targets, richTextParameters)}{Icons.Inline(ModelDB.Class<ChieftainModel>().IconPath, richTextParameters)}");

		protected override int AtlasIndex => 1;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder()
				.WithHealValue(1)
				.WithCustomGetTargets((_, figures) =>
				{
					Character character =
						GameController.Instance.CharacterManager.Characters.FirstOrDefault(character => character.ClassModel is ChieftainModel);
					if(character != null)
					{
						figures.Add(character);
					}
				})
				.Build()
		];
	}

	public class PlusZeroHealTargetAllYourSummons : ChieftainAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}1, {Icons.Inline(Icons.Targets, richTextParameters)}all of your summons");

		protected override int AtlasIndex => 3;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder()
				.WithHealValue(1)
				.WithTarget(Target.SelfOrAllies | Target.TargetAll)
				.WithCustomGetTargets((_, figures) =>
				{
					Character character = GetCharacter(attackAbilityState);

					if(character != null)
					{
						figures.AddRange(character.Summons);
					}
				})
				.Build()
		];
	}

	public class MinusTwoBlessSelf : ChieftainAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, -2,
				extraText: $"{Icons.Inline(Icons.GetCondition(Conditions.Bless), richTextParameters)}, self");

		protected override int AtlasIndex => 5;
		public override int? GetValue(AttackAbility.State attackAbilityState) => -2;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ConditionAbility.Builder().WithConditions(Conditions.Bless).WithTarget(Target.Self).Build()
		];
	}

	public class PlusZeroPushOneImmobilize : ChieftainAMDCardModel
	{
		protected override int AtlasIndex => 6;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override int? Push => 1;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Immobilize];
	}

	public class PlusZeroAddPlusOneForEachOfYourSummons : ChieftainAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"Add +1{Icons.Inline(Icons.Attack, richTextParameters)} for each of your summons");

		protected override int AtlasIndex => 7;

		public override int? GetValue(AttackAbility.State attackAbilityState) => GetCharacter(attackAbilityState)?.Summons.Count ?? 0;
	}

	public class PlusOneIfDrawnBySummonRolling : ChieftainAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText: $"If this card is drawn by one of your summons, it is considered {Icons.Inline(Icons.Rolling, richTextParameters)}");

		protected override int AtlasIndex => 9;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => attackAbilityState?.Performer is Summon;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
	}

	public class PlusZeroPierceOneWound : ChieftainAMDCardModel
	{
		protected override int AtlasIndex => 11;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override int? Pierce => 1;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Wound1];
	}

	public class PlusOneEarth : ChieftainAMDCardModel
	{
		protected override int AtlasIndex => 12;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Earth)];
	}

	public class PlusZeroPierceTwoUnaffectedByRetaliateRolling : ChieftainAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"This attack is unaffected by {Icons.Inline(Icons.Retaliate, richTextParameters)}", rolling: true);

		protected override int AtlasIndex => 14;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override int? Pierce => 2;

		public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
			async (state, _) =>
			{
				SingleTargetState singleTargetState = state.SingleTargetState;
				ScenarioEvents.RetaliateEvent.Subscribe(state, this,
					parameters => parameters.AbilityState.SingleTargetState == singleTargetState,
					async parameters =>
					{
						parameters.SetRetaliateBlocked();
						ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
						await GDTask.CompletedTask;
					});
				await GDTask.CompletedTask;
			};
	}

	public class PlusOne : ChieftainAMDCardModel
	{
		protected override int AtlasIndex => 16;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
	}
}