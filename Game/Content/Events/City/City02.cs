using System.Collections.Generic;
using System.Linq;

public class City02 : CityEventModel<City02.ChoiceA, City02.ChoiceB>
{
	public override int Number => 02;

	public override string Text =>
		"""
		You are spending the evening enjoying mellow music at a Quatryl concert in the Brown Door. During intermission, one of the band members approaches you and asks if you'd be willing to join them on stage as a guest musician.
		""";

	private const string ConditionsMetKey = "ConditionsMet";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Agree to join them on stage to sing.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => character.ClassModel.Ancestry is Ancestry.Harrower);
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					The Harrower in your party approaches the stage fearlessly as the band begins playing their mellow music. Their hisses and chitters enchant the crowd and mesh surprisingly well with the style of music. 
					""";
			}
			else
			{
				return
					"""
					You begin to sing but can instantly tell the crowd is unimpressed. Halfway through the song, you're booed off the stage and you sulk away back into the crowd.
					""";
			}
		}

		public override List<SavedReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainReputationReward(1)
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
		public override string ChoiceText => "Offer to play an instrument.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => character.ClassModel.Ancestry is Ancestry.Quatryl);
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You begin playing an instrument and the crowd goes wild. Participants in the crowd begin throwing coin in your direction, and after the show the band members bring you backstage for a round of celebratory drinks.
					""";
			}
			else
			{
				return
					"""
					You begin playing an instrument but can instantly tell the crowd is unimpressed. Halfway through the song, you're booed off the stage and you sulk away back into the crowd.
					""";
			}
		}

		public override List<SavedReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainCollectiveGoldReward(10)
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