using System.Collections.Generic;
using System.Linq;

public class City01 : CityEventModel<City01.ChoiceA, City01.ChoiceB>
{
	public override int Number => 01;

	public override string Text =>
		"""
		"Come one, come all, and welcome to the county fair!" a Quatryl with red-and-white face paint and a clownish blue wig smiles as he waves you in through the entrance. You've decided to take the day off and visit the county fair, which you've enjoyed frequenting as a youth.

		"Step right up and try your luck!" an Inox Strongman wielding a giant hammer beckons you forward. "Do you have what it takes to hit the bell?"

		On the other side, an Aesther throws a dart and pops a balloon. "Try your aim! Can you hit the balloon? Find out here!"
		""";

	private const string ConditionsMetKey = "ConditionsMet";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Try your strength with the Inox Strongman's game.";

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
					You swing the hammer down with all your might and hear a loud ring. "You've done it!" the Inox Strongman cheers. "Come claim your prize!"
					""";
			}
			else
			{
				return
					"""
					You pick up the hammer and swing it down, but the booth hardly rumbles at all and the bell doesn't ring. "Oh well, maybe next time," says the Strongman as he takes the hammer back from you.
					""";
			}
		}

		public override List<SavedReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainCollectiveItemReward(ModelDB.Item<LightweightBoots>())
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
		public override string ChoiceText => "Test your aim by the Aesther's booth.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => character.ClassModel.Ancestry is Ancestry.Aesther or Ancestry.Orchid);
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You grip the dart tightly in your hand and fling it toward the balloon. You hear a 'pop' followed by applause. "Congratulations!" the Aesther claps. "Here's your prize!"
					""";
			}
			else
			{
				return
					"""
					You fling the dart toward the board but end up accidentally hitting the Aesther in the shoulder instead.
					""";
			}
		}

		public override List<SavedReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainCollectiveItemReward(ModelDB.Item<LightweightBoots>())
				];
			}
			else
			{
				return [];
			}
		}
	}
}