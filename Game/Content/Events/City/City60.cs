using System.Collections.Generic;

public class City60 : CityEventModel<City60.ChoiceA, City60.ChoiceB>
{
	public override int Number => 60;

	public override string Text =>
		"""
		After taking the final sip from the now-empty mug of ale on the table before you, you reach into your pocket and pull out your last coin. As you place it on the table and stand up to leave, a Quatryl strapped with various weapons around his waist comes from behind and grabs the coin from the table.

		"The bartender already received plenty of tips tonight, he doesn't need this. I'm collecting for the military, and this coin will do much better in the hands of those protecting the very city which hosts this rundown place."

		The tavern grows quiet as the bartender looks up to you with an expecting gaze.
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Insist the Quatryl releases the coin; the bartender deserves the tip.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.StartingGroup is StartingGroup.Protectors;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You insist that the coin goes to the bartender and the tavern erupts in applause. The bartender hands you a free glass of ale to thank you, and as you take it from his hands, he whispers something in your ear.

					You hear the word "Shaindy" whispered into your ear. You’re not sure what to make of it, but you take note to visit Shiela in the morning to ask her if she knows what it means.
					""";
			}
			else
			{
				return
					"""
					You insist that the coin goes to the bartender, but the Quatryl refuses to surrender it and reaches for a weapon from his belt.

					The bartender motions towards a group of hulky Inox, who immediately approach you to inform you that it's time to go home. You were leaving anyways.
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
					new LoseReputationEventReward(2)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Support the military; there are plenty of other patrons to tip the bartender tonight.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.StartingGroup is StartingGroup.Militants or StartingGroup.Protectors;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					The Quatryl pats you on the back and thanks you for your contribution. Before you have the opportunity to say anything else, he leans over and whispers something in your ear before quickly scurrying out of the tavern.

					You hear the word "Shaindy" whispered into your ear. You’re not sure what to make of it, but you take note to visit Shiela in the morning to ask her if she knows what it means.
					""";
			}
			else
			{
				return
					"""
					The bartender motions towards a group of hulky Inox, who immediately approach you to inform you that it's time to go home. You were leaving anyways.
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
					new LoseReputationEventReward(2)
				];
			}
		}
	}
}