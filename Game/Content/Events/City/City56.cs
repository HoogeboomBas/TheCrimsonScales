using System.Collections.Generic;
using System.Linq;

public class City56 : CityEventModel<City56.ChoiceA, City56.ChoiceB>
{
	public override int Number => 56;

	public override string Text =>
		"""
		As you return to the city, you find the Savvas Hollowpact who once traveled with you standing in your way. Its cloak is in tatters and large patches of its skin are badly warped and discolored. Its rocky limbs appear decrepit and decaying.

		"I can't... I can't...", the Hollowpact mutters. The look on its face is dangerous and feels vaguely threatening. Suddenly, it relaxes and speaks with a soft, gravelly voice. "The Void takes a toll. Can't pay the price of admission. No more..."

		A hapless pedestrian passerby accidentally jostles the Savvas, causing it to react in a swirl of rage and menace. It appears ready to obliterate this passing towns-person without a second thought. Something must be done.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Try to calm the Hollowpact and repair its broken mind and body.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			Despite your efforts, the Savvas deteriorates quickly. It visibly goes from agitated to enraged and swinging its fists at shadows. It teleports away in a fury. You fear your paths may cross again.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) => [];
	}

	public class ChoiceB : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		private static readonly ClassModel[] ClassModels =
		[
			ModelDB.Class<HollowpactModel>()
		];

		public override string ChoiceText => "It's clear the Hollowpact is a danger to itself and others. It must be slain.";

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
					Subtly, you misdirect the distracted Hollowpact. In its weakened mental and physical state, your old ally isn't able to react fast enough to avoid your deathblow. The Savvas disintegrates into black sand.
					""";
			}
			else
			{
				return
					"""
					The Hollowpact moves with amazing speed despite its debilitated state, flattening half of a shop with a blast and injuring bystanders. Eventually you are able to meet a deleporting dodge with lethal force, ending this threat for good.
					""";
			}
		}

		public override List<EventReward> GetRewards(SavedEventState state)
		{
			if(state.GetCustomValue<bool>(ConditionsMetKey))
			{
				return
				[
					new GainCheckmarkEventReward(),
					new GainGoldEachEventReward(2)
				];
			}
			else
			{
				return
				[
					new LoseReputationEventReward(2),
					new GainXPEventReward(10)
				];
			}
		}
	}
}