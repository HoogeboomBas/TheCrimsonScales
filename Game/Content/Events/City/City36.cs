using System.Collections.Generic;
using System.Linq;

public class City36 : CityEventModel<City36.ChoiceA, City36.ChoiceB>
{
	public override int Number => 36;

	public override string Text =>
		"""
		After returning from a rigorous adventure, you're enjoying the peace and quiet over a cold ale in the Sleeping Lion when all of a sudden a man taps you on the shoulder from behind.

		"Hello there," the man smirks. "I'm raising funds for the branch of Scientific Studies at the University. If you care to donate, you would greatly help us continue our efforts to advance research and development."
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Donate gold to the University in good faith.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Sum(character => character.Gold) >= 5;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					The man thanks you profusely for your donation and promises to put up a dedication plaque in your honor.
					""";
			}
			else
			{
				return
					"""
					You don't have enough gold and try to explain this to the man, but he curses you for your stinginess until he is kicked out by the bartender for harassment. Before leaving, the man vows to spread word of your distaste for the philantropic endeavors.
					""";
			}
		}

		public override List<SavedReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new LoseCollectiveGoldReward(5),
					new GainReputationReward(2)
				];
			}
			else
			{
				return
				[
					new LoseReputationReward(1)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Offer to donate gold but demand something in return.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Sum(character => character.Gold) >= 15;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					The man offers you an experimental drug from the laboratory in exchange for a larger donation. Curious, you happily comply and pay the man.
					""";
			}
			else
			{
				return
					"""
					You don't have enough gold and try to explain this to the man, but he curses you for your stinginess until he is kicked out by the bartender for harassment. Before leaving, the man vows to spread word of your distaste for the philantropic endeavors.
					""";
			}
		}

		public override List<SavedReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new LoseCollectiveGoldReward(15),
					new GainCollectiveItemReward(ModelDB.Item<BoosterShot>())
				];
			}
			else
			{
				return
				[
					new LoseReputationReward(1)
				];
			}
		}
	}
}