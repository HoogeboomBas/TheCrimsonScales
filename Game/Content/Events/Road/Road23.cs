using System.Collections.Generic;
using System.Linq;

public class Road23 : RoadEventModel<Road23.ChoiceA, Road23.ChoiceB>
{
	public override int Number => 23;

	public override string Text =>
		"""
		While wandering the road at night, you suddenly see a flash of bright light, followed by a loud shattering noise originating from a cave nearby. Near it, an abandoned merchant cart is parked.

		As you head inside the cave, the sound of sword slashes, breaking glass and the occasional flash of light guides you deeper, when suddenly you see a merchant and his guard fighting an Orchid. On the ground near the Orchid, a sack filled with glowing red crystals that could be of either party.

		As you inspect the Orchid, which has a crystal skin far larger than you have ever seen, he breaks off a piece of his crystal skin which he threatens to throw at you. At the same time, the merchant manages to grab the sack and attempts to flee.
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Engage the Orchid.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => character.ClassModel.Ancestry is Ancestry.Orchid);
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					A fellow Orchid stops your engagement and shouts, "That is a member of the order of Shardrenders, do not intervene!" You watch as the Shardrender pierces the merchant's heart with a crystal and runs off with the sack.
					""";
			}
			else
			{
				return
					"""
					As your sword slashes into the crystal skin of the Orchid, a big shock wave of shards emits, injuring and knocking everyone prone, leaving the Orchid to flee with the sack.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return [];
			}
			else
			{
				return
				[
					new AllStartScenarioWithConditionEventReward(Conditions.Muddle, Conditions.Wound1)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Stop the merchant in his attempt to flee with the crystals.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You help the Orchid fend off the attackers.

			"You made the right decision, these were bandit thieves," the Orchid explains with an expression of gratitude. "These special crystals are of vital importance for my order, the order of Shardrenders, and your deed will not be forgotten." He hands you a small purse of gold before taking his leave.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new GainCollectiveGoldEventReward(15)
		];
	}
}