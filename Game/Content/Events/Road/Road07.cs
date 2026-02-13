using System.Collections.Generic;
using System.Linq;

public class Road07 : RoadEventModel<Road07.ChoiceA, Road07.ChoiceB>
{
	public override int Number => 07;

	public override string Text =>
		"""
		You aim your bow and arrow at a bird in the sky and smile as your dinner falls to the ground. As you approach your meal, and Inox hops out of the bushes with a bow arched over his back.

		"Stay back, that bird is mine," the Inox snarls. At first you think he must be joking, but he insists that this is his kill to claim.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "It's not worth the fight. Stand down and find a different animal to hunt.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You back away and let the Inox keep the bird. 

			You spend the next few hours wandering around but the skies are empty, and eventually settle on a half-eaten bird carcass lying on the floor. You roast it and proceed to eat it, but the taste is foul and leaves you feeling sick.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionEventReward(Conditions.Poison1)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "You're not going anywhere. Prepare to defend your dinner rights.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => character.ClassModel.Ancestry is Ancestry.Inox or Ancestry.Valrath);
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You bang your chest and let out a battle cry. The Inox's eyes bulge and he backs away, agreeing to let you keep the bird. It makes for a tasty meal and you feel satisfied.
					""";
			}
			else
			{
				return
					"""
					You draw your blade but the Inox is unimpressed. He lunges toward you and a brawl ensues, but you both pause at the sight of a fox running of with your meal.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new AllStartScenarioWithConditionEventReward(Conditions.Strengthen)
				];
			}
			else
			{
				return [];
			}
		}
	}
}