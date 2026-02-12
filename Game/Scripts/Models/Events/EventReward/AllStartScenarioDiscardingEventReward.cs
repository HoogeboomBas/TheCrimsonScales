using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class AllStartScenarioDiscardingEventReward(int discardCount) : EventReward
{
	public override EventRewardType Type => EventRewardType.ScenarioStart;

	public override string GetLabelText(Color textColor) =>
		$"All characters start the next scenario discarding {discardCount} {(discardCount == 1 ? "card" : "cards")} each.";

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			List<AbilityCard> cards = await AbilityCmd.SelectAbilityCards(character, CardState.Hand, discardCount, discardCount,
				card => card.OriginalOwner == character, hintText: $"Select {discardCount} {(discardCount == 1 ? "card" : "cards")} to discard");
			foreach(AbilityCard card in cards)
			{
				await AbilityCmd.DiscardCard(card);
			}
		}
	}
}