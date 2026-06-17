using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class BrightsparkAMDCards
{
	//TODO: Check that everything looks right with the perks once brightspark is implemented
	public class MinusTwoRecoverRandomCardFromDiscard : BrightsparkAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, -2,
				extraText: $"{Icons.Inline(Icons.RecoverCard, richTextParameters)} one random card from your discard pile");

		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => -2;

		public override Func<AttackAbility.State, Character, GDTask> GetExtraEffects() =>
			async (_, potentialDeckOwner) =>
			{
				if(potentialDeckOwner != null)
				{
					List<AbilityCard> discardedCards = potentialDeckOwner.Cards.Where(card => card.CardState is CardState.Discarded).ToList();
					if(!discardedCards.Any())
					{
						return;
					}

					AbilityCard card = discardedCards.PickRandom(GameController.Instance.StateRNG);
					if(card == null)
					{
						return;
					}

					await AbilityCmd.ReturnToHand(card);
				}
			};
	}

	public class PlusZeroConsumeElementForPlusTwo : BrightsparkAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText:
				$"Consume {Icons.Inline(Icons.WildElement, richTextParameters)} to add +2{Icons.Inline(Icons.Attack, richTextParameters)}");

		protected override int AtlasIndex => 1;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
			async (state, _) =>
			{
				await AbilityCmd.GenericChoice(state.Authority,
				[
					ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.ConsumeElement([CardElementConsumption.ConsumeWild()],
						applyFunction: async _ =>
						{
							state.SingleTargetAdjustAttackValue(2);
							await GDTask.CompletedTask;
						}, effectInfoViewParameters: new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Attack)}"),
						potentialConsumer: state.Performer)
				]);
			};
	}

	public class PlusOneHealOneAllyRangeTwo : BrightsparkAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText:
				$"{Icons.Inline(Icons.Heal, richTextParameters)}1, {Icons.Inline(Icons.Targets, richTextParameters)}1 ally, {Icons.Inline(Icons.Range, richTextParameters)}2");

		protected override int AtlasIndex => 4;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder().WithHealValue(1).WithTarget(Target.Allies).WithRange(2).Build()
		];
	}

	public class PlusOneGrantOneAllyWithinRangeTwoShieldOne : BrightsparkAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText:
				$"Grant one ally within {Icons.Inline(Icons.Range, richTextParameters)}2 {Icons.Inline(Icons.Shield, richTextParameters)}1");

		protected override int AtlasIndex => 6;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			GrantAbility.Builder()
				.WithAbilities(
				[
					ShieldAbility.Builder().WithShieldValue(1).Build()
				])
				.WithRange(2)
				.Build()
		];
	}

	public class PlusZeroConsumeElementToInfuseElementRolling : BrightsparkAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"Consume {Icons.Inline(Icons.WildElement, richTextParameters)} to {Icons.Inline(Icons.WildElement, richTextParameters)}",
				rolling: true);

		protected override int AtlasIndex => 7;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.ConsumeWildInfuseWild()];
	}

	public class PlusTwoWildElement : BrightsparkAMDCardModel
	{
		protected override int AtlasIndex => 10;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.InfuseWild()];
	}

	public class PlusOneStrengthenAllyRangeTwo : BrightsparkAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText:
				$"{Icons.Inline(Icons.GetCondition(Conditions.Strengthen), richTextParameters)}, {Icons.Inline(Icons.Targets, richTextParameters)}1 ally, {Icons.Inline(Icons.Range, richTextParameters)}2");

		protected override int AtlasIndex => 12;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ConditionAbility.Builder().WithConditions(Conditions.Strengthen).WithTarget(Target.Allies).WithRange(2).Build()
		];
	}

	public class PlusZeroImmobilizeIceRolling : BrightsparkAMDCardModel
	{
		protected override int AtlasIndex => 14;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Immobilize];
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Ice)];
	}

	public class PlusZeroPushOneOrPullOneAirRolling : BrightsparkAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Push, richTextParameters)}1 or {Icons.Inline(Icons.Pull, richTextParameters)}1", rolling: true);

		protected override int AtlasIndex => 15;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Air)];

		public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
			async (state, _) =>
			{
				await AbilityCmd.GenericChoice(state.Authority,
				[
					ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(applyFunction: async _ =>
						{
							state.SingleTargetAdjustPush(1);
							await GDTask.CompletedTask;
						}, effectType: EffectType.SelectableMandatory,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Push),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Push)}1")),
					ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(applyFunction: async _ =>
						{
							state.SingleTargetAdjustPull(1);
							await GDTask.CompletedTask;
						}, effectType: EffectType.SelectableMandatory,
						effectButtonParameters: new IconEffectButton.Parameters(Icons.Pull),
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Pull)}1")),
				]);
			};
	}

	public class PlusZeroPierceTwoFireRolling : BrightsparkAMDCardModel
	{
		protected override int AtlasIndex => 16;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override int? Pierce => 2;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Fire)];
	}

	public class PlusZeroHealOneRangeThreeLightRolling : BrightsparkAMDCardModel
	{
		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}1, {Icons.Inline(Icons.Range, richTextParameters)}3", rolling: true);

		protected override int AtlasIndex => 17;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Light)];

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder().WithHealValue(1).WithRange(3).Build()
		];
	}
}