using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class ShardrenderAMDCards
{
	public class PlusZero : ShardrenderAMDCardModel
	{
		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
	}

	public class PlusOne : ShardrenderAMDCardModel
	{
		protected override int AtlasIndex => 1;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
	}

	public class PlusZeroShieldOneRolling : ShardrenderAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"{Icons.Inline(Icons.Shield, richTextParameters)}1 {Icons.Inline(Icons.Rolling, richTextParameters)}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Shield, richTextParameters)}1",
				rolling: true);

		protected override int AtlasIndex => 3;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ShieldAbility.Builder().WithShieldValue(1).Build()
		];
	}

	public class PlusZeroMoveCharacterTokenOnCrystallizeBackwardOneSlot : ShardrenderAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"{Icons.Inline(ShardrenderCardSide.CrystallizeIconPath, richTextParameters)}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText:
				$"Move the character token on one of your {Icons.Inline(ShardrenderCardSide.CrystallizeIconPath, richTextParameters)} abilities backward one slot");

		protected override int AtlasIndex => 5;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
			async (_, potentialDeckOwner) =>
			{
				await MoveCharacterTokenBack(potentialDeckOwner as Character, 1);
			};
	}

	public class PlusOneIfAttackHasPiercePlusTwoInsteadRolling : ShardrenderAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +1,
				$"{Icons.Inline(Icons.Pierce, richTextParameters)}:{Icons.Inline(Icons.GetAMDValue("+2"), richTextParameters)}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText:
				$"If the attack has {Icons.Inline(Icons.Pierce, richTextParameters)}, {Icons.Inline(Icons.GetAMDValue("+2"), richTextParameters)} instead",
				rolling: true);

		protected override int AtlasIndex => 9;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;

		public override int? GetValue(AttackAbility.State attackAbilityState)
		{
			return attackAbilityState?.SingleTargetPierce > 0 ? +2 : +1;
		}
	}

	public class PlusOneAdvanceCrystallizePlusOneAttack : ShardrenderAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +1,
				$"{Icons.Inline(ShardrenderCardSide.CrystallizeForwardIconPath, richTextParameters)}:+1");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText:
				$"{Icons.Inline(ShardrenderCardSide.CrystallizeForwardIconPath, richTextParameters)}: +1{Icons.Inline(Icons.Attack, richTextParameters)}");

		protected override int AtlasIndex => 11;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
			async (state, potentialDeckOwner) =>
			{
				await AbilityCmd.GenericChoice(state.Authority,
				[
					ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
						_ => ShardrenderCardSide.GetActiveCrystallizeStates(potentialDeckOwner as Character).Count != 0,
						async _ =>
						{
							Dictionary<AbilityCard, CrystallizeAbility.State> possibilities =
								ShardrenderCardSide.GetActiveCrystallizeStates(potentialDeckOwner as Character);
							if(possibilities.Count == 1)
							{
								await possibilities.First().Value.AdvanceUseSlot();
							}
							else
							{
								AbilityCard abilityCard = await AbilityCmd.SelectAbilityCard(potentialDeckOwner,
									cards => cards.AddRange(possibilities.Keys), null, true,
									hintText:
									$"Select a {Icons.HintText(ShardrenderCardSide.CrystallizeIconPath)} to move the character token backward one slot.");

								await possibilities[abilityCard].AdvanceUseSlot();
							}

							state.SingleTargetAdjustAttackValue(1);
						}, EffectType.Selectable,
						effectButtonParameters: new IconEffectButton.Parameters(ShardrenderCardSide.CrystallizeForwardIconPath),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Attack)}"))
				]);
			};
	}

	public class PlusZeroBrittle : ShardrenderAMDCardModel
	{
		protected override int AtlasIndex => 13;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Brittle];
	}
}