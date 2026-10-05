using System;
using System.Collections.Generic;
using Fractural.Tasks;

public class BruiserAMDCards
{
	public class PlusOne : BruiserAMDCardModel
	{
		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;
	}

	public class PlusZeroShieldOneRolling : BruiserAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"{Icons.Inline(Icons.Shield)}1 {Icons.Inline(Icons.Rolling, richTextParameters)}");

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

	public class PlusZeroRetaliateOneRolling : BruiserAMDCardModel
	{
		public override bool RemoveAfterDraw => true;
		public override bool StaysActive => true;


		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"{Icons.Inline(Icons.Retaliate)}2 {Icons.Inline(Icons.Rolling, richTextParameters)}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.Inline(Icons.Retaliate, richTextParameters)}2", rolling: true);
		protected override int AtlasIndex => 6;

		public override async GDTask OnBecomeActive(AMDCard card, Character character)
    	{
			ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(character, card,
				parameters => parameters.Figure == character,
				parameters =>
				{
					parameters.Add(new InfoTextExtraEffect.Parameters(
							richText => $"{Icons.Inline(Icons.Retaliate, richText)}2 when attacked by an adjacent enemy"));
				});
        	ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(character, card,
            	parameters =>
                parameters.AbilityState.Target == character &&
                parameters.AbilityState.Performer.EnemiesWith(character) &&
                RangeHelper.Distance(parameters.AbilityState.Performer.Hex, character.Hex) <= 1,
            async parameters =>
            {
                ScenarioEvents.RetaliateEvent.Subscribe(character, card,
                    retaliate => retaliate.RetaliatingFigure == character,
                    async retaliate =>
                    {
                        retaliate.AdjustRetaliate(2);
                        ScenarioEvents.RetaliateEvent.Unsubscribe(character, card);
                        await GDTask.CompletedTask;
                    });

                character.ActiveModifiers.Remove(card);
                character.AMDCardDeck.DiscardPile.Add(
                    new AMDCard(card.Model, card.Owner, card.PotentialDeckOwner));
                ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(character, card);
				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(character, card);
            });

        await GDTask.CompletedTask;
    	}
	}

	public class PlusZeroStun : BruiserAMDCardModel
	{
		protected override int AtlasIndex => 8;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Stun];
	}

	public class PlusOneHealTwoSelfRolling : BruiserAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +1,
				$"{Icons.Inline(Icons.Heal, richTextParameters)}2 {Icons.Inline(Icons.Rolling, richTextParameters)}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText: $"{Icons.Inline(Icons.Heal, richTextParameters)}2, self",
				rolling: true);

		protected override int AtlasIndex => 9;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build()
		];
	}

	public class PlusTwoPushTwo : BruiserAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +2,
				$"{Icons.Inline(Icons.Push, richTextParameters)}2");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +2,
				extraText: $"{Icons.Inline(Icons.Push, richTextParameters)}2");	

		protected override int AtlasIndex => 11;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
		public override int? Push => 2;
	}

	public class PlusThree : BruiserAMDCardModel
	{
		protected override int AtlasIndex => 13;
		public override int? GetValue(AttackAbility.State attackAbilityState) => 3;
	}

	public class PlusZeroDisarmRolling : BruiserAMDCardModel
	{
		protected override int AtlasIndex => 14;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Disarm];
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
	}

	public class PlusZeroMuddleRolling : BruiserAMDCardModel
	{
		protected override int AtlasIndex => 15;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<ConditionModel> GetConditionModels(AttackAbility.State attackAbilityState) => [Conditions.Muddle];
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
	}
}
