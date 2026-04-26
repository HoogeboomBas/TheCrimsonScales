using System.Collections.Generic;
using System.Linq;

public class City03 : CityEventModel<City03.ChoiceA, City03.ChoiceB>
{
	public override int Number => 03;

	public override string Text =>
		"""
		"It's something we've been working on for quite some time," the Brightspark beams as he rubs the small bulge in his lab coat. "New technology like you've never seen before. For a simple donation of twenty-five gold to advance the scientific studies at the University, it could be all yours."
		""";

	private const string ConditionsMetKey = "ConditionsMet";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Offer to buy what the Brightspark is selling.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Sum(character => character.Gold) >= 25;
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					"Here you go!" The Brightspark pulls out a small circular device from underneath his lab coat. "Your coin will go a long way with helping us develop new technology at the University."
					""";
			}
			else
			{
				return
					"""
					"You don't seem to have enough gold! I'm sure I'll find someone else who will want it."
					""";
			}
		}

		public override List<SavedReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new LoseCollectiveGoldReward(25),
					new GainCollectiveItemReward(ModelDB.Item<TranslocationDevice>())
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
		public override string ChoiceText => "Attempt to negotiate a cheaper price.";

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
					"You drive a hard bargain." The Brightspark pauses with his finger on his lip. "You know what? For the University, I'll sell it to you for fifteen gold. Final offer."
					""";
			}
			else
			{
				return
					"""
					"You drive a hard bargain." The Brightspark pauses with his finger on his lip. "You know what? For the University, I would sell it to you for fifteen gold, but you don't seem to have enough. I'm sure I'll find someone else who will want it."
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
					new GainCollectiveItemReward(ModelDB.Item<TranslocationDevice>())
				];
			}
			else
			{
				return [];
			}
		}
	}
}