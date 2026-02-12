using System.Collections.Generic;
using System.Linq;

public class Road02 : CityEventModel<Road02.ChoiceA, Road02.ChoiceB>
{
	public override int Number => 02;

	public override string Text =>
		"""
		You approach a rat corpse lying on the side of the road. You feel your stomach grumble as you remember your last meal. It's been quite a few days since you've had the opportunity to fill your belly.

		With no sign of life in sight, you contemplate the fact that this might be the only meal available for some time.
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		public override string ChoiceText => "Light a fire and prepare to roast the dead rat.";

		public override void InitState(SavedEventState state, SavedCampaign savedCampaign)
		{
			base.InitState(state, savedCampaign);

			bool conditionsMet = savedCampaign.Characters.Any(character => character.ClassModel.Ancestry is Ancestry.Vermling);
			state.SetCustomValue(ConditionsMetKey, conditionsMet);
		}

		public override string GetStoryText(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
					"""
					The sweet smell of burning flesh begins to fill the air as you roast the dead rat. It ends up cooking beautifully and you enjoy a hearty meal.
					""";
			}
			else
			{
				return
					"""
					When you try to roast the rat corpse over the fire, the smell repulses you and makes you wonder how you could've considered eating it in the first place.

					You spend hours searching for another meal, but the land seems to be devoid of anything remotely edible. You reach your destination weary and hungry.
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
				return
				[
					new AllStartScenarioWithConditionEventReward(Conditions.Muddle)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Carry on and try to find a more suitable meal.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You spend hours searching for another meal, but the land seems to be devoid of anything remotely edible. You reach your destination weary and hungry.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new AllStartScenarioWithConditionEventReward(Conditions.Muddle)
		];
	}
}