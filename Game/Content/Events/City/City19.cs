using System.Collections.Generic;
using Fractural.Tasks;

public class City19 : CityEventModel<City19.ChoiceA, City19.ChoiceB>
{
	public override int Number => 19;

	public override string Text =>
		"""
		You're scheduled to spend the evening at the Sleeping Lion with a Quatryl you know well, Shiela, and decide to go there early to get a head-start with a few glasses of ale.

		As you make your way to the tavern, you are approached by an Orchid carrying a small barrel over his shoulder. He begins to explain that he's a Brewmaster who traveled down from the Monastery in the Copperneck Mountains.

		He offers to pay you good coin to help him secure a shipment of brews that needs to be delivered tonight.
		""";

	public class ChoiceAOnScenarioStartedReward : OnScenarioStartedReward, IEventSubscriber
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"Once, during the next scenario, a character can add +2{Icons.Inline(Icons.Attack, textParameters)} to an attack.";

		public override async GDTask OnScenarioSetupPhaseCompleted()
		{
			await base.OnScenarioSetupPhaseCompleted();

			ScenarioEvents.DuringAttackEvent.Subscribe(this,
				parameters => parameters.Performer is Character,
				async parameters =>
				{
					parameters.AbilityState.SingleTargetAdjustAttackValue(2);

					ScenarioEvents.DuringAttackEvent.Unsubscribe(this);

					await GDTask.CompletedTask;
				}
			);
		}
	}

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		public override string ChoiceText => "Accept the job to help the Brewmaster secure his shipment.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You spend the night accompanying the shipment with the Brewmaster, hoping Shiela won't mind your absence.

			At the end of the night, the Brewmaster thanks you and offers you a special brew he calls 'Liquid Rage' for your troubles.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldReward(10),
			new ChoiceAOnScenarioStartedReward()
		];
	}

	public class ChoiceBDowntimeShopPriceReward : DowntimeShopPriceReward
	{
		public override string GetLabelText(RichTextParameters textParameters) =>
			$"During this City Phase, each character may buy one Minor potion from the shop for free.";

		protected override void CalculatePriceApplyFunction(BetweenScenariosEvents.CalculateItemBuyPrice.Parameters parameters)
		{
			if(CanGetFreePotion(parameters.ItemModel, parameters.Buyer))
			{
				parameters.AdjustPrice(-parameters.Price);
			}
		}

		protected override void ItemBoughtApplyFunction(BetweenScenariosEvents.ItemBought.Parameters parameters)
		{
			if(CanGetFreePotion(parameters.ItemModel, parameters.Buyer))
			{
				SetCustomValue(parameters.Buyer.Guid.ToString(), true);
			}
		}

		private bool CanGetFreePotion(ItemModel itemModel, SavedCharacter buyer)
		{
			return
				!GetCustomValue<bool>(buyer.Guid.ToString()) &&
				(itemModel == ModelDB.Item<MinorHealingPotion>() ||
				 itemModel == ModelDB.Item<MinorManaPotion>() ||
				 itemModel == ModelDB.Item<MinorPowerPotion>() ||
				 itemModel == ModelDB.Item<MinorStaminaPotion>() ||
				 itemModel == ModelDB.Item<MinorCurePotion>());
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Decline the job and spend the evening with Shiela.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You decline the Brewmaster's offer and head straight to the Sleeping Lion. You find Shiela there waiting for you and she embraces you as you enter the tavern, drinks waiting on the table.

			At the end of the night, Shiela expresses her appreciation with an offer to stop by her potion shop in the morning for a free sampling.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new ChoiceBDowntimeShopPriceReward()
		];
	}
}