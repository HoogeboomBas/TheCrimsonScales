using System.Collections.Generic;
using System.Linq;

public class Road26 : CityEventModel<Road26.ChoiceA, Road26.ChoiceB>
{
	public override int Number => 26;

	public override string Text =>
		"""
		As you depart the city, you find a snake charmer sitting outside the gates with a pungi in hand. There is a basket filled with vipers by his feet and he's playing a soft tune. The vipers seem to be entranced by the music as they slowly wave their bodies in perfect sync.

		The charmer abruptly stops playing and looks up to you as he says, "I'm in need of a new pungi, could you please spare a few coins?" Before you can answer, he narrows his eyes and points to the basket filled with snakes. "My snakes would be quite unhappy if you don't."
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Help the charmer with a small donation.";

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
					You hand the charmer a few coins and proceed to continue on your journey. "Wait!" the charmer calls after you. "Here, have one of my vipers as a thank-you gift. Take good care of him for me, will you?"
					""";
			}
			else
			{
				return
					"""
					You don't seem to have enough gold for even a small donation, and the charmer seems disappointed. "A pity," he says. "Very well, have a safe journey."
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new LoseCollectiveGoldEventReward(5),
					new SummonEventReward(
						SummonAbility.Builder().WithSummonStats(new SummonStats()
							{
								Health = 3,
								Move = 3,
								Attack = 1,
								Traits =
								[
									new ApplyConditionTrait(Conditions.Poison1),
									new JumpTrait()
								]
							})
							.WithName("Slithering Viper")
							.WithTexturePath("res://Content/Classes/Chieftain/Summons/cottonmouth_snake_AI.png") //TODO: Generic or AI summon visual?
							.Build()
					)
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
		public override string ChoiceText => "Refuse to give the charmer any of your hard-earned gold.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You refuse to give the charmer any of your gold and turn away to continue with your journey. You feel a sharp sting running up your ankle and you turn around to see one of the vipers slithering away. The charmer smiles devilishly as he continues playing his pungi.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionEventReward(Conditions.Poison1)
		];
	}
}