using System.Collections.Generic;
using System.Linq;

public class RoadTemplate : CityEventModel<RoadTemplate.ChoiceA, RoadTemplate.ChoiceB>
{
	public override int Number => 00;

	public override string Text =>
		"""
		TODO
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "TODO";

		public override string GetStoryText(SavedEventState state) =>
			"""
			TODO
			""";

		public override List<EventReward> GetRewards(SavedEventState state) => []; //TODO
	}

	public class ChoiceB : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "TODO";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			//TODO
			bool conditionsMet = savedCampaign.Characters.Any(character => character.ClassModel.Ancestry is Ancestry.Vermling);
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					TODO
					""";
			}
			else
			{
				return
					"""
					TODO
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				//TODO
				return [];
			}
			else
			{
				//TODO
				return [];
			}
		}
	}
}