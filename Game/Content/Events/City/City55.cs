using System.Collections.Generic;
using System.Linq;

public class City55 : CityEventModel<City55.ChoiceA, City55.ChoiceB>
{
	public override int Number => 55;

	public override string Text =>
		"""
		You've just returned from a mission and are alerted by yells from a quickly growing crowd. A large gaggle of onlookers are watching an argument unfold with reactions ranging from interest to concern and even fear.

		You work your way past the crowd and see a small group of Savvas Craghearts facing off against a sneering Savvas merchant and its wary-looking contingent of caravan guards. You can't make out the words, but it's clear the disagreement is about to get out of hand.

		In the shadows behind the group of Savvas, your trained eyes notice a pair of sinister-looking Aesthers clad in dark colors observing the scene intently.
		""";

	public class ChoiceA : EventChoiceModel
	{
		private const string ConditionsMetKey = "ConditionsMet";

		private static readonly ClassModel[] ClassModels =
		[
			ModelDB.Class<HollowpactModel>()
		];

		public override string ChoiceText => "Speak with the Savvas to ease tensions before they boil over.";

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
					You successfully use your abilities to calm this tense situation. Both parties agree to let their grievances rest and pass each other in peace.

					The Aesthers are gone, as if they'd vanished into thin air.
					""";
			}
			else
			{
				return
					"""
					The tensions escalate and boil over despite your efforts. The crowd scatters as dangerous elemental powers are deployed in the middle of the city. Many buildings are damaged, and the Craghearts are routed.
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
					new LoseCollectiveGoldReward(10)
				];
			}
		}
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Investigate the two Aesthers and determine what their interest is here.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			Due to the escalating situation nearby serving as distraction, you are able to approach the lurking Aesthers unseen. One of the Aesthers holds an ominously glowing amethyst. You overhear them mutter, "...planned. Almost ready for us..."

			Suddenly, the Aesthers notice you. They quickly grab the lead Cragheart and together vanish into a hurriedly conjured portal.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainCheckmarkReward()
		];
	}
}