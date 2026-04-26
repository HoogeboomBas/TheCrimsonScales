using System.Collections.Generic;

public class City15 : CityEventModel<City15.ChoiceA, City15.ChoiceB>
{
	public override int Number => 15;

	public override string Text =>
		"""
		Everyone has heard about the recent excape of a rare bird from the mansion of the famous Sir Kenhaven. You double-take when you see a colorful bird land on your open windowsill, and instantly recognize it as the missing bird.

		You grab a large pot from a nearby shelf and hope to catch it so you can turn it in for a handsome reward.
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Whistle and coo the bird into the pot.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			You softly whistle and catch the bird's attention. It turns its head sideways as you make clicking noises with your tongue and slowly approach the bird. It flies toward you as you turn the pot over, trapping it inside.

			You make your way to the Traveler's District where Sir Kenhaven resides and knock on his mansion door. After handing the bird over, the servant who answers the door assures you that Sir Kenhaven will hear about this when he returns from his travels, and welcomes you back to visit another time.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) =>
		[
			new AddCityReward(ModelDB.Event<City31>())
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Sneak up on the bird and try to trap it in the pot.";

		public override EventResolveType GetEventResolveType(SavedEventState state) => EventResolveType.ReturnCardToBottom;

		public override string GetStoryText(SavedEventState state) =>
			"""
			You quietly tiptoe toward the bird, and as you approach arm's length you lunge toward the it with the pot. Frightened, it flaps its wings and proceeds to speedily fly away.
			""";

		public override List<SavedReward> GetRewards(SavedEventState state) => [];
	}
}