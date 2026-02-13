using System.Collections.Generic;
using System.Linq;

public class City37 : CityEventModel<City37.ChoiceA, City37.ChoiceB>
{
	public override int Number => 37;

	public override string Text =>
		"""
		You're taking your time perusing the various wares in the market when you take notice of a commanding Orchid garbed in tribal robes with a whistle in her hand. The Orchid blows the whistle, and several nearby animals turn their heads and begin walking towards the Orchid, as if in trance.

		Curious, you ask the Orchid about the whistle. "This piece? It's a common tool amongst our kind," the Orchid chuckles.

		"If you're so fascinated with it, it could be yours for a small price.
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string EnoughGoldKey = "EnoughGold";

		public override string ChoiceText => "Buy the whistle from the Orchid for fifteen gold.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool enoughGold = savedCampaign.Characters.Sum(character => character.Gold) >= 15;
			state.SetCustomValue(EnoughGoldKey, enoughGold);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(EnoughGoldKey))
			{
				return
					"""
					After a bit of price negotiation, the Orchid agrees to a fair price and sells you the whistle.
					""";
			}
			else
			{
				return
					"""
					You can't seem to agree on a fair price with the gold you have. Disappointed, the Orchid turns and quickly leaves.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(EnoughGoldKey))
			{
				return
				[
					new LoseCollectiveGoldEventReward(15),
					new GainCollectiveItemEventReward(ModelDB.Item<SummonersWhistle>())
				];
			}
			else
			{
				return [];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		private const string OrchidInPartyKey = "OrchidInParty";
		private const string EnoughGoldKey = "EnoughGold";

		public override string ChoiceText => "Try to charm the Orchid into giving it to you for free.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool orchidInParty = savedCampaign.Characters.Any(character => character.ClassModel.Ancestry == Ancestry.Orchid);
			state.SetCustomValue(OrchidInPartyKey, orchidInParty);

			bool enoughGold = savedCampaign.Characters.Sum(character => character.Gold) >= 15;
			state.SetCustomValue(EnoughGoldKey, enoughGold);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(OrchidInPartyKey))
			{
				return
					"""
					You begin to relay tales of your past, and the Orchid smiles and relates to your stories. After a few minutes the Orchid gleefully hands over the whistle, happy to have made a new friend.
					""";
			}
			else if(state.GetCustomValue<bool>(EnoughGoldKey))
			{
				return
					"""
					After a bit of price negotiation, the Orchid agrees to a fair price and sells you the whistle.
					""";
			}
			else
			{
				return
					"""
					You can't seem to agree on a fair price with the gold you have. Disappointed, the Orchid turns and quickly leaves.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(OrchidInPartyKey))
			{
				return
				[
					new GainCollectiveItemEventReward(ModelDB.Item<SummonersWhistle>())
				];
			}
			else if(state.GetCustomValue<bool>(EnoughGoldKey))
			{
				return
				[
					new LoseCollectiveGoldEventReward(15),
					new GainCollectiveItemEventReward(ModelDB.Item<SummonersWhistle>())
				];
			}
			else
			{
				return [];
			}
		}
	}
}