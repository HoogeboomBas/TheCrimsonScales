using System.Collections.Generic;
using System.Linq;

public class City51 : CityEventModel<City51.ChoiceA, City51.ChoiceB>
{
	public override int Number => 51;

	public override string Text =>
		"""
		You are perusing the shops in the Ward of Scales district when one of the wooden shop stalls nearby suddenly collapses on top of the elderly Valrath shop attendant. A crowd gathers as the shouts maniacally from underneath the wreckage, cursing the Vermling who allegedly rigged her stand to collapse.

		"That rat has been a thorn in my side ever since I moved my shop to this district!" she wails. She appears uninjured, but cannot free herself from the heap of wooden beams piled over her.

		Meanwhile, you spot a small pack of Vermlings opposite the collapsed shop rifling through the unattended belongings of the other distracted shop owners.
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		private static readonly ClassModel[] ClassModels =
		[
			ModelDB.Class<FireKnightModel>(),
			ModelDB.Class<ChainguardModel>(),
			ModelDB.Class<HollowpactModel>()
		];

		public override string ChoiceText => "Free the trapped woman.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => ClassModels.Contains(character.ClassModel));
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					You quickly jump into action, removing board after board to free the elderly Valrath from the wreckage. Though she is visibly shaken from being trapped, she reaches into her bag, saying, "I appreciate your assistance. Please take this as my thanks."
					""";
			}
			else
			{
				return
					"""
					You manage to convince the onlooking crowd to help clear the wooden rubble, and before long the Valrath is freed.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainCollectiveItemEventReward(ModelDB.Item<AmuletOfLife>())
				];
			}
			else
			{
				return
				[
					new GainReputationEventReward(1)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		private static readonly ClassModel[] ClassModels =
		[
			ModelDB.Class<SpiritCallerModel>()
		];

		public override string ChoiceText => "Take advantage of the distraction to loot some valuables.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => ClassModels.Contains(character.ClassModel));
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					With the crowd's attention focused on the collapse, you make your way through the unattended shop stalls, inconspicuously looting small valuables as you go.
					""";
			}
			else
			{
				return
					"""
					You manage to grab a few coins before your lack of stealth gives you away. Time to run before the authorities arrive!
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainGoldEachEventReward(10)
				];
			}
			else
			{
				return
				[
					new GainGoldEachEventReward(3),
					new LoseReputationEventReward(1)
				];
			}
		}
	}
}