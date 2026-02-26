using System;
using System.Collections.Generic;
using System.Linq;

public class Road44 : RoadEventModel<Road44.ChoiceA, Road44.ChoiceB>
{
	public override int Number => 44;

	public override string Text =>
		"""
		About halfway through your journey, you happen upon a merchant's caravan set up on the side of the road. There is a small shop sign which piques your curiosity, and you enter the caravan. You're taken aback when you find an old friend, the Chainguard, who greets you as you enter.

		"I've started a bit of a side business selling some of my traps," the Chainguard says while placing a couple different traps on the table in front of you. "For you, I'll let you have one for free. Which would you like? These rusty spikes or this clamping snare?"
		""";

	public class ChoiceA : EventChoiceModel
	{
		public override string ChoiceText => "Choose the rusty spikes trap.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			"The rusty spikes trap! Great choice, use it wisely." The Chainguard hands you the trap and bids you farewell.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					Character user = GameController.Instance.CharacterManager.Characters[0];

					await AbilityCmd.CreateTraps(damage: 4, conditions: [Conditions.Wound1], performer: user,
						customSelectHexes: list => CustomSelectHexes(list, user), assetPath: "res://Content/Classes/Chainguard/Traps/ChainguardWoodSpikeTrap.tscn");
				},
				color =>
					$"At the start of the next scenario, place one {Icons.Inline(Icons.Damage, color: color)}4, {Icons.Inline(Icons.GetCondition(Conditions.Wound1), color: color)} trap in an empty hex adjacent to any enemy."
			)
		];
	}

	public class ChoiceB : EventChoiceModel
	{
		public override string ChoiceText => "Choose the clamping snare trap.";

		public override string GetStoryText(SavedEventState state) =>
			"""
			"The clamping snare. Great for stopping anything in its tracks!" The Chainguard hands you the trap and wishes you the best of luck on your journey.
			""";

		public override List<EventReward> GetRewards(SavedEventState state) =>
		[
			new OnScenarioStartedEventReward(
				async () =>
				{
					Character user = GameController.Instance.CharacterManager.Characters[0];

					await AbilityCmd.CreateTraps(damage: 0, conditions: [Conditions.Stun], performer: user,
						customSelectHexes: list => CustomSelectHexes(list, user), assetPath: "res://Content/Classes/Chainguard/Traps/ChainguardRopeTrap.tscn");
				},
				color =>
					$"At the start of the next scenario, place one {Icons.Inline(Icons.GetCondition(Conditions.Stun), color: color)} trap in an empty hex adjacent to any enemy."
			)
		];
	}

	private static void CustomSelectHexes(List<Hex> list, Character user)
	{
		foreach(Figure figure in GameController.Instance.Map.Figures.Where(figure => user.EnemiesWith(figure)))
		{
			foreach(Hex adjacentHex in RangeHelper.GetHexesInRange(user.Hex, 1).Where(hex => hex.IsEmpty()))
			{
				list.AddIfNew(adjacentHex);
			}
		}
	}
}