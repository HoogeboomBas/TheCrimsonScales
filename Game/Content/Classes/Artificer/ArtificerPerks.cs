using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class ArtificerPerks
{
	public abstract class ArtificerPerk : PerkModel
	{
	}

	public class ReplaceOneMinusTwoWithOneMinusOneGainScrap : ArtificerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ArtificerAMDCards.MinusOneGainScrap>()
		];
	}

	public class ReplaceOneMinusOneGainScrapWithOnePlusOneGainScrap : ArtificerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<ArtificerAMDCards.MinusOneGainScrap>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ArtificerAMDCards.PlusOneGainScrap>()
		];
	}

	public class ReplaceOnePlusOneGainScrapWithOnePlusThreeDisarmGainScrap : ArtificerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<ArtificerAMDCards.PlusOneGainScrap>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ArtificerAMDCards.PlusThreeDisarmGainScrap>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusOne : ArtificerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ArtificerAMDCards.PlusOne>()
		];
	}

	public class ReplaceOneMinusOneWithTwoPlusZeroPierceTwoRolling : ArtificerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ArtificerAMDCards.PlusZeroPierceTwoRolling>(),
			ModelDB.AMDCard<ArtificerAMDCards.PlusZeroPierceTwoRolling>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusOneWoundIfDrawnBySummon : ArtificerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ArtificerAMDCards.PlusOneWoundIfDrawnBySummon>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusZeroCreateDamageTwoTrapRolling : ArtificerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ArtificerAMDCards.PlusZeroCreateDamageTwoTrapRolling>()
		];
	}

	public class ReplaceOnePlusTwoWithOnePlusFour : ArtificerPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusTwoAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ArtificerAMDCards.PlusFour>()
		];
	}

	public class IgnoreNegativeScenarioEffectsAddPlusOneRolling : ArtificerPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<ArtificerAMDCards.PlusOneRolling>()
		];

		public override bool IgnoreNegativeScenarioEffects => true;
	}

	public class SpareParts : ArtificerPerk
	{
		protected override string Title => "Spare Parts";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"At the start of each scenario, you may lose one {Icons.Inline(Icons.GetItem(ItemType.Head), richTextParameters)}, {Icons.Inline(Icons.GetItem(ItemType.Feet), richTextParameters)}, {Icons.Inline(Icons.GetItem(ItemType.OneHand), richTextParameters)}, or {Icons.Inline(Icons.GetItem(ItemType.TwoHands), richTextParameters)} item for no effect to gain 2 {Icons.Inline(Artificer.ScrapToken, richTextParameters)}.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ItemModel item = await AbilityCmd.SelectItem(character,
				character.Items.Where(itemModel =>
					itemModel.ItemState is not ItemState.Consumed &&
					itemModel.ItemType is ItemType.Head or ItemType.Feet or ItemType.OneHand or ItemType.TwoHands).ToList(),
				effectType: EffectType.Selectable,
				hintText: $"Select an item to {Icons.HintText(Icons.LoseCard)}");
			if(item == null)
			{
				return;
			}

			await item.SetItemState(ItemState.Consumed);
			await ArtificerCardSide.GainScrapToken(character);
			await ArtificerCardSide.GainScrapToken(character);
		}
	}

	public class QuickTinkering : ArtificerPerk, IEventSubscriber
	{
		protected override string Title => "Quick Tinkering";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you short rest, you may {Icons.Inline(Icons.RecoverCard, richTextParameters)} one spent {Icons.Inline(Icons.GetItem(ItemType.Head), richTextParameters)}, {Icons.Inline(Icons.GetItem(ItemType.Feet), richTextParameters)}, {Icons.Inline(Icons.GetItem(ItemType.OneHand), richTextParameters)}, or {Icons.Inline(Icons.GetItem(ItemType.TwoHands), richTextParameters)} item.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.ShortRestStartedEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async parameters =>
				{
					ItemModel item = await AbilityCmd.SelectItem(character,
						character.Items.Where(itemModel =>
							itemModel.ItemState is ItemState.Spent &&
							itemModel.ItemType is ItemType.Head or ItemType.Feet or ItemType.OneHand or ItemType.TwoHands).ToList(),
						effectType: EffectType.Selectable,
						hintText: $"Select an item to {Icons.HintText(Icons.RecoverCard)}");
					if(item != null)
					{
						await AbilityCmd.RefreshItem(item);
					}
				});
		}
	}

	public class Reconjigger : ArtificerPerk, IEventSubscriber
	{
		protected override string Title => "Reconjigger";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you long rest, you may spend 1{Icons.Inline(Artificer.ScrapToken, richTextParameters)} to {Icons.Inline(Icons.RecoverCard, richTextParameters)} one lost {Icons.Inline(Icons.GetItem(ItemType.Head), richTextParameters)}, {Icons.Inline(Icons.GetItem(ItemType.Feet), richTextParameters)}, {Icons.Inline(Icons.GetItem(ItemType.OneHand), richTextParameters)}, or {Icons.Inline(Icons.GetItem(ItemType.TwoHands), richTextParameters)} item.";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.LongRestStartedEvent.Subscribe(this,
				parameters => parameters.Character == character && ArtificerCardSide.HasXScrapTokens(character, 1),
				async _ =>
				{
					ArtificerCardSide.LoseScrapTokens(character);
					ItemModel item = await AbilityCmd.SelectItem(character,
						character.Items.Where(itemModel =>
							itemModel.ItemState is ItemState.Consumed &&
							itemModel.ItemType is ItemType.Head or ItemType.Feet or ItemType.OneHand or ItemType.TwoHands).ToList(),
						effectType: EffectType.Selectable,
						hintText: $"Select an item to {Icons.HintText(Icons.RecoverCard)}");
					if(item != null)
					{
						await AbilityCmd.RefreshItem(item);
					}
				}, EffectType.Selectable,
				effectButtonParameters: new TextEffectButton.Parameters($"1{Icons.HintText(Artificer.ScrapToken)}"),
				effectInfoViewParameters: new TextEffectInfoView.Parameters(
					$"{Icons.HintText(Icons.RecoverCard)} one lost {Icons.Inline(Icons.GetItem(ItemType.Head))}, {Icons.Inline(Icons.GetItem(ItemType.Feet))}, {Icons.Inline(Icons.GetItem(ItemType.OneHand))}, or {Icons.Inline(Icons.GetItem(ItemType.TwoHands))} item"));
		}
	}
}