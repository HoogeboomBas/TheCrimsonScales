using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class AllStartScenarioDiscardingReward : SavedReward
{
	[JsonProperty]
	private int _discardCount;

	public override RewardType Type => RewardType.ScenarioStart;

	public AllStartScenarioDiscardingReward()
	{
	}

	public AllStartScenarioDiscardingReward(int discardCount)
	{
		_discardCount = discardCount;
	}

	public override string GetLabelText(RichTextParameters textParameters) =>
		$"All characters start the next scenario discarding {_discardCount} {(_discardCount == 1 ? "card" : "cards")} each.";

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			List<AbilityCard> cards = await AbilityCmd.SelectAbilityCards(character, CardState.Hand, _discardCount, _discardCount,
				card => card.OriginalOwner == character, hintText: $"Select {_discardCount} {(_discardCount == 1 ? "card" : "cards")} to discard");
			foreach(AbilityCard card in cards)
			{
				await AbilityCmd.DiscardCard(card);
			}
		}
	}
}