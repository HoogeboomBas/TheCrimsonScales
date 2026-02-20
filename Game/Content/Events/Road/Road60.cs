using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class Road60 : RoadEventModel<Road60.ChoiceA, Road60.ChoiceB>
{
	public override int Number => 60;

	public override string Text =>
		"""
		As the moonlight shines bright upon the field before you, you contemplate setting up your tent here for the evening when you suddenly hear a rustling from within the tall grass before you. An Inox jumps out from the grass, and as you reach for your weapon she puts out her hand, as if to express caution.

		"Fear not, I mean you no harm," the Inox tells you. "I'm here looking for a cave nearby, which is rumored to host the corpses of wealthy merchants, riddled with abandoned jewelry."

		"However," the Inox continues, "I fear for bandits. Would you care to escort me on this journey? I'd be willing to split the loot with you."
		""";

	public class ChoiceA : EventChoiceModel, IEventSubscriber
	{
		private const string ConditionsMetKey = "ConditionsMet";
		private const string OtherConditionsMetKey = "OtherConditionsMet";

		public override string ChoiceText => "Join her to search for the cave together.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.StartingGroup is StartingGroup.Naturalists;
			bool otherConditionsMet =
				savedCampaign.StartingGroup is StartingGroup.Naturalists or StartingGroup.Trailblazers or StartingGroup.Explorers;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
			state.SetCustomValue(OtherConditionsMetKey, otherConditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You accept the Inox's offer, and your savviness of the wilderness helps her navigate through terrain which leads you both to a small cave. The cave turns out to be empty, but there is an inscription on the wall which piques your curiosity.

					You see the word “Shaindy” written across the cave wall in hieroglyphics. You’re not sure what it means, but you take note to visit Councilman Raksani upon your return to the city. He’s well-versed in hieroglyphics and may be able to help you understand what it means.
					""";
			}
			else if(state.GetCustomValue<bool>(OtherConditionsMetKey))
			{
				return
					"""
					As you make your way through the tall grass, the Inox seems to struggle to keep up through the terrain and you're unsure how to help. She grows tired and gives up, wishing you luck as she disappears into the night.

					You decide to find the cave on your own, and after hours of searching, you come across a small cave. Although there are no corpses to be found, there is an inscription on the wall which piques your interest.

					You see the word “Shaindy” written across the cave wall in hieroglyphics. You’re not sure what it means, but you take note to visit Councilman Raksani upon your return to the city. He’s well-versed in hieroglyphics and may be able to help you understand what it means.
					""";
			}
			else
			{
				return
					"""
					As you make your way through the tall grass, the Inox seems to struggle to keep up through the terrain and you're unsure how to help. She grows tired and gives up, wishing you luck as she disappears into the night.

					You spend hours searching but find yourself cursing your navigation skills as the sun begins to rise. With daylight approaching, it's time to move on and continue with your journey, as there's not more time to rest.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainProsperityEventReward(5)
				];
			}
			else if(state.GetCustomValue<bool>(OtherConditionsMetKey))
			{
				return
				[
					new GainProsperityEventReward(5)
				];
			}
			else
			{
				return
				[
					new AllStartScenarioWithDamageEventReward(1),
					new AllStartScenarioWithConditionEventReward(Conditions.Muddle)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel, IEventSubscriber
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Decline her offer and seek the cave on your own.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.StartingGroup is StartingGroup.Naturalists or StartingGroup.Trailblazers or StartingGroup.Explorers;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You decide to find the cave on your own, and after hours of searching, you come across a small cave. Although there are no corpses to be found, there is an inscription on the wall which piques your interest.

					You see the word “Shaindy” written across the cave wall in hieroglyphics. You’re not sure what it means, but you take note to visit Councilman Raksani upon your return to the city. He’s well-versed in hieroglyphics and may be able to help you understand what it means.
					""";
			}
			else
			{
				return
					"""
					You spend hours searching but find yourself cursing your navigation skills as the sun begins to rise. With daylight approaching, it's time to move on and continue with your journey, as there's not more time to rest.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainProsperityEventReward(5)
				];
			}
			else
			{
				return
				[
					new AllStartScenarioWithDamageEventReward(1),
					new AllStartScenarioWithConditionEventReward(Conditions.Muddle)
				];
			}
		}
	}
}