using Fractural.Tasks;

public class AllDiscardCardScenarioEffect : ScenarioEffect
{
	public AllDiscardCardScenarioEffect()
	{
	}

	public override string GetDescription(RichTextParameters textParameters) =>
		$"All characters discard a card.";

	public override async GDTask Activate(Character character)
	{
		AbilityCard card = await AbilityCmd.SelectAbilityCard(character, CardState.Hand, true, hintText: "Select a card to discard");

		if(card != null)
		{
			await card.SetCardState(CardState.Discarded);
		}
	}
}