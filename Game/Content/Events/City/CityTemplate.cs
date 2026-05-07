using System.Collections.Generic;

public class CityTemplate : CityEventModel<CityTemplate.ChoiceA, CityTemplate.ChoiceB>
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

		public override List<SavedReward> GetRewards(SavedEventState state) => [];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "TODO";

		public override string GetStoryText(SavedEventState state) =>
			"""
			TODO
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) => [];
	}
}