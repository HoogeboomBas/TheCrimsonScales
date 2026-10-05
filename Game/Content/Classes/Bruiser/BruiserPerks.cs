using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Fractural.Tasks;

public class BruiserPerks
{
	public abstract class BruiserPerk : PerkModel
	{
	}

	public class ReplaceOneMinusOneWithOnePlusOne : BruiserPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BruiserAMDCards.PlusOne>()
		];
	}

	public class ReplaceOneMinusOneWithOnePlusZeroShieldOneRolling : BruiserPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BruiserAMDCards.PlusZeroShieldOneRolling>(),
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusZeroRetaliateTwo : BruiserPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BruiserAMDCards.PlusZeroRetaliateOneRolling>(),
		]; 
	}

	public class ReplaceOnePlusZeroWithOnePlusZeroStun : BruiserPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BruiserAMDCards.PlusZeroStun>(),
		];
	}

	public class AddOnePlusOneHealTwoSelf : BruiserPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BruiserAMDCards.PlusOneHealTwoSelfRolling>(),
		];
	}

	public class AddOnePlusTwoPushTwo : BruiserPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BruiserAMDCards.PlusTwoPushTwo>(),
		];
	}

	public class AddOnePlusThree : BruiserPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BruiserAMDCards.PlusThree>(),
		];
	}

	public class AddOnePlusZeroDisarmRollingAndOnePlusZeroMuddleRolling : BruiserPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BruiserAMDCards.PlusZeroDisarmRolling>(),
			ModelDB.AMDCard<BruiserAMDCards.PlusZeroMuddleRolling>()
		];
	}

	public class IgnoreItemMinusOneEffectsAddTwoPlusOne : BruiserPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<BruiserAMDCards.PlusOne>(),
			ModelDB.AMDCard<BruiserAMDCards.PlusOne>()
		];

		public override bool IgnoreItemMinusOneEffects => true;
	}

	public class PatchArmor : BruiserPerk, IEventSubscriber
	{
		public override int PerkBoxCount => 2;

		protected override string Title => "Patch Armor";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Once each scenario, during your turn, you may perform: {Icons.Inline(Icons.Loot, richTextParameters)}1, if this ability loots at least one money token, you may {Icons.Inline(Icons.Refresh, richTextParameters)} one {Icons.Inline(Icons.GetItem(ItemType.Body), richTextParameters)} item";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			bool used = false;

			ScenarioEvents.CardSideSelectionEvent.Subscribe(this,
				parameters => parameters.Character == character && !used,
				async parameters =>
				{
					used = true;
					ScenarioEvents.CardSideSelectionEvent.Unsubscribe(this);

					int coinsBefore = character.ObtainedCoins;
					ActionState actionState = new ActionState(character,
					[
						LootAbility.Builder().WithRange(1).Build()
					]);
					await actionState.Perform();

					if (character.ObtainedCoins > coinsBefore)
					{
						// ItemModel bodyItem = character.Items.FirstOrDefault(item =>
						// 	item.ItemType == ItemType.Body && item.ItemState == ItemState.Spent);

						// if(bodyItem != null)
						// {
						// 	await bodyItem.Refresh();
						// }
						List<ItemModel> spentBodyItems = character.Items
							.Where(item => item.ItemType == ItemType.Body && item.ItemState == ItemState.Spent)
							.ToList();

						foreach(ItemModel bodyItem in spentBodyItems)
						{
							ScenarioEvents.GenericChoiceEvent.Subscribe(this,
								parameters => parameters.Source == this,
								async parameters =>
								{
									parameters.SetChoiceMade();
									await bodyItem.Refresh();
								},
								EffectType.Selectable,
								effectButtonParameters: new IconEffectButton.Parameters(Icons.GetItem(ItemType.Body)),
								effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Refresh)} {bodyItem.Name}"));
						}

						if(spentBodyItems.Count > 0)
						{
							await ScenarioEvents.GenericChoiceEvent.CreatePrompt(
								new ScenarioEvents.GenericChoice.Parameters(this), character);
							ScenarioEvents.GenericChoiceEvent.Unsubscribe(this);
						}
					}
				},
				EffectType.Selectable,
				effectButtonParameters: new IconEffectButton.Parameters(Icons.Loot),
				effectInfoViewParameters: new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.Loot)}1, if this ability loots at least one money token, you may {Icons.Inline(Icons.Refresh)} one {Icons.Inline(Icons.GetItem(ItemType.Body))} item"));
		}	
	}

	public class RestedAndReady : BruiserPerk, IEventSubscriber
	{
		protected override string Title => "Rested and Ready";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you long rest, add +1{Icons.Inline(Icons.Move, richTextParameters)} to your first move ability the following round";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.LongRestEndedEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async parameters =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(this,
						turnParameters => turnParameters.Figure == character,
						async turnParameters =>
						{
							ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(this);

							bool available = true;

							ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(this,
								infoParameters => infoParameters.Figure == character && available,
								infoParameters =>
								{
									infoParameters.Add(new InfoTextExtraEffect.Parameters(
										richText => $"+1{Icons.Inline(Icons.Move, richText)} to your first move ability this round"));
								});

							ScenarioEvents.AbilityStartedEvent.Subscribe(this,
								abilityParameters =>
									available &&
									abilityParameters.AbilityState is MoveAbility.State &&
									abilityParameters.AbilityState.Performer == character,
								async abilityParameters =>
								{
									available = false;
									((MoveAbility.State)abilityParameters.AbilityState).AdjustMoveValue(1);
									Clear();
									await GDTask.CompletedTask;
								});

							ScenarioEvents.RoundEndedEvent.Subscribe(this,
								_ => true,
								async _ =>
								{
									available = false;
									Clear();
									await GDTask.CompletedTask;
								});

							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				});

			void Clear()
			{
				ScenarioEvents.AbilityStartedEvent.Unsubscribe(this);
				ScenarioEvents.RoundEndedEvent.Unsubscribe(this);
				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(this);
			}
		}
	}

	public class FearlessLeadership : BruiserPerk, IEventSubscriber
	{
		protected override string Title => "Fearless Leadership";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"TODO - Each character gains advantage on their first attack during the first round of each scenario";

	}
}
