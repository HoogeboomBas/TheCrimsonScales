using System;
using System.Collections.Generic;
using Fractural.Tasks;

public class IncarnateAMDCards
{
	public class PlusZeroRitualistConquerorReaverRolling : IncarnateAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0, Icons.Inline(Incarnate.ThreeSpiritIconPath, richTextParameters));

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0, extraText: Icons.Inline(Incarnate.ThreeSpiritIconPath, richTextParameters), rolling: true);

		protected override int AtlasIndex => 0;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
			async (state, _) =>
			{
				if(state.ActionState.ActionSource is AbilityCardSide cardSide && cardSide.Model is IncarnateCardSide)
				{
					ScenarioEvents.ChangeIncarnateSpiritEvent.Subscribe(state, this,
						parameters => parameters.Incarnate == state.Performer,
						async parameters =>
						{
							parameters.AddSpiritChoices([IncarnateSpirit.Ritualist, IncarnateSpirit.Conqueror, IncarnateSpirit.Reaver]);
							ScenarioEvents.ChangeIncarnateSpiritEvent.Unsubscribe(state, this);

							await GDTask.CompletedTask;
						});
				}
				else
				{
					ScenarioEvents.ActionEndedEvent.Subscribe(state, this,
						_ => true,
						async _ =>
						{
							await IncarnateCardSide.ChooseSpirit(state.Performer,
								[IncarnateSpirit.Ritualist, IncarnateSpirit.Conqueror, IncarnateSpirit.Reaver]);

							ScenarioEvents.ActionEndedEvent.Unsubscribe(state, this);
						});
				}

				await GDTask.CompletedTask;
			};
	}

	public class PlusZeroPierceTwoFireRolling : IncarnateAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"{Icons.Inline(Icons.GetElement(Element.Fire), richTextParameters)}{Icons.Inline(Icons.Rolling, richTextParameters)}");

		protected override int AtlasIndex => 1;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override int? Pierce => 2;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Fire)];
	}

	public class PlusZeroPushOneAirRolling : IncarnateAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"{Icons.Inline(Icons.GetElement(Element.Air), richTextParameters)}{Icons.Inline(Icons.Rolling, richTextParameters)}");

		protected override int AtlasIndex => 2;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override int? Push => 1;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Air)];
	}

	public class PlusZeroShieldOneEarthRolling : IncarnateAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"{Icons.Inline(Icons.GetElement(Element.Earth), richTextParameters)}{Icons.Inline(Icons.Rolling, richTextParameters)}");

		protected override int AtlasIndex => 3;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Earth)];

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			ShieldAbility.Builder().WithShieldValue(1).Build()
		];
	}

	public class PlusOneRitualistEnfeebleConquerorEmpowerSelf : IncarnateAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +1,
				$"{Icons.InlineCondition(Incarnate.Enfeeble, richTextParameters)}{Icons.InlineCondition(Incarnate.Empower, richTextParameters)}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText:
				$"{Icons.Inline(Incarnate.RitualistIconPath, richTextParameters)}: {Icons.InlineCondition(Incarnate.Enfeeble, richTextParameters)}, {Icons.Inline(Incarnate.ConquerorIconPath, richTextParameters)}: {Icons.InlineCondition(Incarnate.Empower, richTextParameters)}, self");

		protected override int AtlasIndex => 4;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<ConditionModel> GetConditionModels(AttackAbility.State state) =>
			InSpirit(state.Performer, IncarnateSpirit.Ritualist) ? [Incarnate.Enfeeble] : [];

		public override List<Ability> GetAbilities(AttackAbility.State state) =>
			InSpirit(state.Performer, IncarnateSpirit.Conqueror)
				? [ConditionAbility.Builder().WithConditions(Incarnate.Empower).WithTarget(Target.Self).Build()]
				: [];
	}

	public class PlusOneRitualistEnfeebleReaverRupture : IncarnateAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +1,
				$"{Icons.InlineCondition(Incarnate.Enfeeble, richTextParameters)}{Icons.InlineCondition(Conditions.Rupture, richTextParameters)}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText:
				$"{Icons.Inline(Incarnate.RitualistIconPath, richTextParameters)}: {Icons.InlineCondition(Incarnate.Enfeeble, richTextParameters)}, {Icons.Inline(Incarnate.ReaverIconPath, richTextParameters)}: {Icons.InlineCondition(Conditions.Rupture, richTextParameters)}");

		protected override int AtlasIndex => 6;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<ConditionModel> GetConditionModels(AttackAbility.State state) =>
			InSpirit(state.Performer, IncarnateSpirit.Ritualist) ? [Incarnate.Enfeeble] :
			InSpirit(state.Performer, IncarnateSpirit.Reaver) ? [Conditions.Rupture] : [];
	}

	public class PlusOneConquerorEmpowerSelfReaverRupture : IncarnateAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +1,
				$"{Icons.InlineCondition(Incarnate.Empower, richTextParameters)}{Icons.InlineCondition(Conditions.Rupture, richTextParameters)}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +1,
				extraText:
				$"{Icons.Inline(Incarnate.ConquerorIconPath, richTextParameters)}: {Icons.InlineCondition(Incarnate.Empower, richTextParameters)}, self, {Icons.Inline(Incarnate.ReaverIconPath, richTextParameters)}: {Icons.InlineCondition(Conditions.Rupture, richTextParameters)}");

		protected override int AtlasIndex => 8;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +1;

		public override List<ConditionModel> GetConditionModels(AttackAbility.State state) =>
			InSpirit(state.Performer, IncarnateSpirit.Reaver) ? [Conditions.Rupture] : [];

		public override List<Ability> GetAbilities(AttackAbility.State state) =>
			InSpirit(state.Performer, IncarnateSpirit.Conqueror)
				? [ConditionAbility.Builder().WithConditions(Incarnate.Empower).WithTarget(Target.Self).Build()]
				: [];
	}

	public class PlusZeroRecoverOneOrTwoHandItemRolling : IncarnateAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"{Icons.Inline(Icons.RecoverCard, richTextParameters)}{Icons.Inline(Icons.Rolling, richTextParameters)}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText:
				$"{Icons.Inline(Icons.RecoverCard, richTextParameters)} one {Icons.Inline(Icons.GetItem(ItemType.OneHand), richTextParameters)} or {Icons.Inline(Icons.GetItem(ItemType.TwoHands), richTextParameters)} item",
				rolling: true);

		protected override int AtlasIndex => 3;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override List<Ability> GetAbilities(AttackAbility.State attackAbilityState) =>
		[
			HealAbility.Builder().WithHealValue(1).WithTarget(Target.Self).WithConditions(Incarnate.Empower).Build()
		];
	}
}