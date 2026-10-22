using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class ThornreaperAMDCards
{
	public class PlusZero : ThornreaperAMDCardModel
	{
		protected override int AtlasIndex => 0;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
	}

	public class PlusTwo : ThornreaperAMDCardModel
	{
		protected override int AtlasIndex => 1;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +2;
	}

	public class PlusZeroLightRolling : ThornreaperAMDCardModel
	{
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"{Icons.InlineElement(Element.Light, richTextParameters)}, {Icons.Inline(Icons.Rolling)}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.InlineElement(Element.Light, richTextParameters)}", rolling: true);
	
		protected override int AtlasIndex => 2; //to 3
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override List<CardElementInfusion> ElementInfusions => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class CreateHazardousTerrain : ThornreaperAMDCardModel
	{
		protected override int AtlasIndex => 4; //to 5
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0, $"Create one 1-hex hazardous terrain in one adjacent featureless hex");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"Create one 1-hex hazardous terrain in one adjacent featureless hex");
		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
			async (state, _) =>
			{
				Hex hex = await AbilityCmd.SelectHex(state, 
					hexes => 
					{
						foreach (Hex possibleHex in RangeHelper.GetHexesInRange(state.Performer.Hex, 1, true))
						{
							if (possibleHex != null && possibleHex.IsFeatureless())
							{
								hexes.Add(possibleHex);
							}
						}
					},
					hintText: "Place hazardous terrain in one adjacent hex");
		 		PackedScene scene = SceneLoader.LoadPackedScene("res://Content/OverlayTiles/HazardousTerrain/Thorns1H.tscn");
				await AbilityCmd.CreateHazardousTerrain(hex, scene);
			};
	}

	public class InfuseEarthIfLightIsStrongOrWaningRolling : ThornreaperAMDCardModel
	{
		protected override int AtlasIndex => 6; // to 8
		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0, $"{Icons.InlineElement(Element.Earth, richTextParameters)} if {Icons.InlineElement(Element.Light, richTextParameters)} is strong or waning");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"{Icons.InlineElement(Element.Earth, richTextParameters)} if {Icons.InlineElement(Element.Light, richTextParameters)} is strong or waning", rolling: true);

		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;
		public override Func<AttackAbility.State, Figure, GDTask> GetExtraEffects() =>
			async (state, _) =>
			{
				if (GameController.Instance.ElementManager.GetState(Element.Light) is ElementState.Strong or ElementState.Waning)
				{
					CardElementInfusion.Infuse(Element.Earth);
				}
			};
	}

	public class PlusOneIfLightIsStrongOrWaningRolling : ThornreaperAMDCardModel
	{
		protected override int AtlasIndex => 9; //to 17
		public override int? GetValue(AttackAbility.State attackAbilityState) =>
			GameController.Instance.ElementManager.GetState(Element.Light) is ElementState.Strong or ElementState.Waning ? +1 : 0;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;

		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"+1 if {Icons.InlineElement(Element.Light, richTextParameters)} is strong or waning, {Icons.Rolling}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"+1 if {Icons.InlineElement(Element.Light, richTextParameters)} is strong or waning", rolling: true);
	}

	public class PersistentRetaliateThreeOnNextAdjacentAttackInHazardousTerrain : ThornreaperAMDCardModel
	{
		protected override int AtlasIndex => 18; // to 19
		public override bool RemoveAfterDraw => true;
		public override bool StaysActive => true;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;

		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"On the next attack from adjacent enemy while you occupy hazardous terrain, gain {Icons.Inline(Icons.Retaliate)}3 and discard this card, {Icons.Rolling}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"On the next attack from adjacent enemy while you occupy hazardous terrain, gain {Icons.Inline(Icons.Retaliate)}3 and discard this card", rolling: true);

		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override async GDTask OnBecomeActive(AMDCard card, Character character)
		{
			ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(character, card,
				parameters => parameters.Figure == character,
				parameters =>
				{
					parameters.Add(new InfoTextExtraEffect.Parameters(
						richText => $"{Icons.Inline(Icons.Retaliate)}3 when attacked by an adjacent enemy if you occupy hazardous terrain"
					));
				});
			ScenarioEvents.RetaliateEvent.Subscribe(character, card,
				retaliateParameters => retaliateParameters.RetaliatingFigure == character && 
				RangeHelper.Distance(retaliateParameters.AbilityState.Performer.Hex, character.Hex) <= 1 &&
				retaliateParameters.RetaliatingFigure.Hex.HasHexObjectOfType<HazardousTerrain>(),
				async retaliateParameters =>
				{
					retaliateParameters.AdjustRetaliate(3);
					ScenarioEvents.RetaliateEvent.Unsubscribe(character, card);
					character.ActiveModifiers.Remove(card);
					character.AMDCardDeck.DiscardPile.Add(
						new AMDCard(card.Model, card.Owner, card.PotentialDeckOwner));
					await GDTask.CompletedTask;
				});
		}
	}

	public class PersistentShieldThreeOnNextAttackDamageInHazardousTerrain : ThornreaperAMDCardModel
	{
		protected override int AtlasIndex => 20; // to 21
		public override bool RemoveAfterDraw => true;
		public override bool StaysActive => true;
		public override bool GetRolling(AttackAbility.State attackAbilityState) => true;

		public override string GetSimpleString(RichTextParameters richTextParameters) =>
			GetSimpleString(richTextParameters, +0,
				$"On the next time you would suffer damage from an attack while you occupy hazardous terrain, gain {Icons.Inline(Icons.Shield)}3 and discard this card, {Icons.Rolling}");

		public override string ToString(RichTextParameters richTextParameters) =>
			GetBasicString(richTextParameters, +0,
				extraText: $"On the next time you would suffer damage from an attack while you occupy hazardous terrain, gain {Icons.Inline(Icons.Shield)}3 and discard this card", rolling: true);

		public override int? GetValue(AttackAbility.State attackAbilityState) => +0;

		public override async GDTask OnBecomeActive(AMDCard card, Character character)
		{
			ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(character, card,
				parameters => parameters.Figure == character,
				parameters =>
				{
					parameters.Add(new InfoTextExtraEffect.Parameters(
						richText => $"{Icons.Inline(Icons.Shield)}3 on next attack if you occupy hazardous terrain"
					));
				});
			ScenarioCheckEvents.ShieldCheckEvent.Subscribe(character, card,
				shieldParameters => shieldParameters.Figure == character && 
				shieldParameters.Figure.Hex.HasHexObjectOfType<HazardousTerrain>(),
				async shieldParameters =>
				{
					shieldParameters.AdjustShield(3);
					ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(character, card);
					character.ActiveModifiers.Remove(card);
					character.AMDCardDeck.DiscardPile.Add(
						new AMDCard(card.Model, card.Owner, card.PotentialDeckOwner));
					await GDTask.CompletedTask;
				});
		}
	}
}