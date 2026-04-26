using System;
using System.Collections.Generic;
using System.Threading;
using Fractural.Tasks;
using Newtonsoft.Json;

public class City57 : CityEventModel<City57.ChoiceA, City57.ChoiceB>
{
	public override int Number => 57;

	public override string Text =>
		"""
		You are strolling through the Sinking Market when suddenly you hear a loud shriek. A young Valrath mother has collapsed on the ground and before a crowd can form, someone shouts "SNAKE!"... Then you see them - some juvenile ghost vipers like the ones found in that pit recently. Somehow they've found their way into the city. Before you have time to process this information, your realize several more are dead. The whole area has erupted into chaos.

		The Viper Hunter isn't with you, and you begin to suspect something sinister has happened.
		""";

	[Serializable, JsonObject(MemberSerialization.OptIn)]
	public class RetireViperHunterReward : SavedReward
	{
		public override RewardType Type => RewardType.Immediate;

		public override string GetLabelText(RichTextParameters textParameters) =>
			"""
			The Character with the "An Adder Divides" Personal Quest retires immediately. No retirement events are added for this Character.
			""";

		public override async GDTask ImmediateResolve(SavedCampaign savedCampaign, CancellationToken cancellationToken)
		{
			AnAdderDivides personalQuest = ModelDB.PersonalQuest<AnAdderDivides>();

			foreach(SavedCharacter savedCharacter in savedCampaign.AllCharacters)
			{
				if(savedCharacter.SavedPersonalQuest.Model == personalQuest)
				{
					AppController.Instance.RetireCharacter(savedCharacter, savedCampaign, false);
				}
			}

			await GDTask.CompletedTask;
		}
	}

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Run to search for the Viper Hunter.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You search in all the usual places but the Viper Hunter is nowhere to be found.

			After giving up, you go back to the Sinking Market to investigate. The guards have taken care of the snake problem, but based on the bodies in the streets, it wasn't nearly quick enough. Then you notice the Viper Hunter's lifeless body sitting on a bench nearby, looking almost peaceful.

			On the ground beside the bench you find a large rucksack - big enough to contain a few juvenile vipers - and a note... "Maybe I am mad after all."
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new RetireViperHunterReward(),
			new LoseProsperityReward(1),
			new LoseReputationReward(2)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Fight off the vipers before they do more harm.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You charge headstrong into the chaos and begin slaughtering the vipers. They're thankfully a little more manageable than the adults, and peace is restored again quickly. Then you notice the Viper Hunter's lifeless body sitting on a bench nearby, looking almost peaceful.

			On the ground beside the bench you find a large rucksack - big enough to contain a few juvenile vipers - and a note... "Maybe I am mad after all."
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new RetireViperHunterReward(),
			new LoseReputationReward(1),
			new GainXPReward(10)
		];
	}
}