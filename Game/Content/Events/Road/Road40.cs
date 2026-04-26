using System.Collections.Generic;
using System.Linq;

public class Road40 : RoadEventModel<Road40.ChoiceA, Road40.ChoiceB>
{
	public override int Number => 40;

	public override string Text =>
		"""
		"Halt, stranger!" a Quatryl encased in a device surrounded by cannons extends his arm out and shakes his head. "You're approaching official military territory. I'm afraid you can't pass through here. You'll have to turn around and head through the forest instead."

		Going around through the forest could add days to your trips. There's no choice but to pass through.
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Attempt to persuade the Quatryl with the hope that he's in a reasonable mood.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Reputation > 10;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You explain to the Quatryl who you are, and as he listens he decides to introduce you to his commander. You recognize the Bombard, exchange smiles and he gives you full permission to pass through.
					""";
			}
			else
			{
				return
					"""
					You explain to the Quatryl who you are, but he doesn't seem to be impressed. Several more Quatryls appear and forcefully escort you off the grounds.
					""";
			}
		}

		public override List<SavedReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return [];
			}
			else
			{
				return
				[
					new AllStartScenarioWithDamageReward(3)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Offer to bribe the Quatryl in exchange for letting you pass.";

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
					The Quatryl looks around to ensure there are no witnesses and hastily ushers you through to the other side.
					""";
			}
			else
			{
				return
					"""
					The Quatryl shakes his head. "This is no trivial matter. I should have you jailed for this." The Quatryl motions for backup, and you hastily scurry off before things get out of hand.
					""";
			}
		}

		public override List<SavedReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new LoseCollectiveGoldReward(5)
				];
			}
			else
			{
				return
				[
					new AllStartScenarioDiscardingReward(1)
				];
			}
		}
	}
}