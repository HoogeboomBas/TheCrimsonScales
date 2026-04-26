using System.Collections.Generic;

public class City59 : CityEventModel<City59.ChoiceA, City59.ChoiceB>
{
	public override int Number => 59;

	public override string Text =>
		"""
		As you make your way to the Sanctuary of the Great Oak to make another donation, you are greeted by an entire group of men garbed in long white robes with a shimmering gold design that resembles the oak.

		"Travelers, your donations have been monumental in allowing us to continue helping those in need," one of the bearded men says as he bows slightly towards your direction, hands firmly pressed together. "We would like to hang a golden plaque in your honor, in the quarters of your choosing."

		"Tell us, would you like us to honor you in our hospital, or in our temple?"
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Ask to be honored in the hospital.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.StartingGroup is StartingGroup.Naturalists or StartingGroup.Protectors;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ask to be honored in the hospital, and the men assure you that your funds will ensure the Great Oak can continue to provide continuous care to those in need.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new UnlockPartyAMDReward(ModelDB.AMDCard<PartyAMDCard1>())
				];
			}
			else
			{
				return
				[
					new UnlockPartyAMDReward(ModelDB.AMDCard<PartyAMDCard2>())
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Ask to be honored in the temple.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.StartingGroup is StartingGroup.Militants or StartingGroup.Naturalists or StartingGroup.Protectors;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state) =>
			"""
			You ask to be honored in the temple, and the men assure you that your funds will ensure the Great Oak can continue to help carry their message throughout Gloomhaven.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new UnlockPartyAMDReward(ModelDB.AMDCard<PartyAMDCard3>())
				];
			}
			else
			{
				return
				[
					new UnlockPartyAMDReward(ModelDB.AMDCard<PartyAMDCard4>())
				];
			}
		}
	}
}