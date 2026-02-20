using System.Collections.Generic;

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
					Hex targetHex = await AbilityCmd.SelectHex(
						user,
						list =>
						{
							foreach(Figure figure in GameController.Instance.Map.Figures)
							{
								if(user.EnemiesWith(figure))
								{
									foreach(Hex adjacentHex in RangeHelper.GetHexesInRange(user.Hex, 1))
									{
										if(adjacentHex.IsEmpty())
										{
											list.AddIfNew(adjacentHex);
										}
									}
								}
							}
						},
						hintText: $"Select a hex to place the trap"
					);

					if(targetHex != null)
					{
						await AbilityCmd.CreateTrap(targetHex, "res://Content/OverlayTiles/Traps/SpikePitTrap1H.tscn", damage: 4,
							conditions: [Conditions.Wound1]);
					}
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
					Hex targetHex = await AbilityCmd.SelectHex(
						user,
						list =>
						{
							foreach(Figure figure in GameController.Instance.Map.Figures)
							{
								if(user.EnemiesWith(figure))
								{
									foreach(Hex adjacentHex in RangeHelper.GetHexesInRange(user.Hex, 1))
									{
										if(adjacentHex.IsEmpty())
										{
											list.AddIfNew(adjacentHex);
										}
									}
								}
							}
						},
						hintText: $"Select a hex to place the trap"
					);

					if(targetHex != null)
					{
						await AbilityCmd.CreateTrap(targetHex, "res://Content/OverlayTiles/Traps/BearTrap1H.tscn",
							conditions: [Conditions.Stun]);
					}
				},
				color =>
					$"At the start of the next scenario, place one {Icons.Inline(Icons.GetCondition(Conditions.Stun), color: color)} trap in an empty hex adjacent to any enemy."
			)
		];
	}
}