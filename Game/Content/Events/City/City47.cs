using System.Collections.Generic;

public class City47 : CityEventModel<City47.ChoiceA, City47.ChoiceB>
{
	public override int Number => 47;

	public override string Text =>
		"""
		As you make your way back home after a night of laughs and good drinks at the Sleeping Lion, you notice an unusual green light glowing brightly from within an alleyway which would connect you to a shorter path home. You approach the alley and find a Vermling with her knees bent, waving a staff through the air and murmuring indiscernibly.

		"The ritual is almost complete," the Vermling looks up and opens her eyes. "You there! Leave at once, for you cannot handle the awesome presence of these spirits!"
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Tell the Vermling that she has no authority to tell you what to do.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You tell the Vermling to mind her own business and she angrily waves her staff toward you. "You have been warned! These spirits will haunt your dreams!" You make nothing of it and ignore her rambling as you make your way through the alley.

			After making it home safely, you crawl into bed and close your eyes but wake up with an insufferable headache. You can't quite remember your dreams from last night, but the sweat stains on your sheets indicate that they weren't too pleasant.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new LoseCheckmarkReward()
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Heed the Vermling's warnings and take the longer route home instead.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You turn around and take the longer route home. You weren't sure what the Vermling meant with her warning, but you head to bed and find yourself well-rested the next morning, ready for a brand new day.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new GainCheckmarkReward()
		];
	}
}