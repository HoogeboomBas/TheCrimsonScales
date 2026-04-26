using System.Collections.Generic;
using System.Linq;

public class Road53 : RoadEventModel<Road53.ChoiceA, Road53.ChoiceB>
{
	public override int Number => 53;

	public override string Text =>
		"""
		You set out from the city far later than you had intended and the stormy weather that hasn't subsided all day isn't making the journey any easier. In fact, the path you'd planned on taking is flooded and wading through the mud would leave you vulnerable to whatever wild animals are lurking in the dark. There is a clearer path you could take, but that would add an extra few hours and you're already just so worn.
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		private static readonly ClassModel[] ClassModels =
		[
			ModelDB.Class<MirefootModel>(),
			ModelDB.Class<ChieftainModel>(),
		];

		public override string ChoiceText => "Stick to the plan and push through the mud.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => ClassModels.Contains(character.ClassModel));
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					Your familiarity with the wilderness helps guide your party through the mud without exposing yourselves to an attack.
					""";
			}
			else
			{
				return
					"""
					It starts off well, but it isn't long before you find yourself waist deep in mud and surrounded by wolves. You are able to fight them not, but not unscathed.

					You can't help but think that fighting off wolves is probably more exhausting than taking the long way around.
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
					new AllStartScenarioDiscardingReward(3),
					new AllStartScenarioWithConditionReward(Conditions.Wound1)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Take the long way around. It's not worth the risk.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You're not happy about the situation, but being attacked while knee-deep in mud is exactly the luck you've been having today, so you decide not to risk it. The rest of the trip is uneventful, but exhausting.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioDiscardingReward(2)
		];
	}
}