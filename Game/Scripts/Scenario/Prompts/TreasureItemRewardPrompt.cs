using System;

public class TreasureItemRewardPrompt(Character lootingCharacter, ItemModel itemModel, EffectCollection effectCollection)
	: Prompt<TreasureItemRewardPrompt.Answer>(effectCollection, () => $"{lootingCharacter.DisplayName} found: {itemModel.Name}")
{
	public class Answer : PromptAnswer
	{
	}

	protected override bool CanSkip => false;

	protected override void Enable()
	{
		base.Enable();

		GameController.Instance.TreasureItemRewardView.Open(itemModel);
	}

	protected override void Disable()
	{
		base.Disable();

		GameController.Instance.TreasureItemRewardView.Close();
	}

	protected override Answer CreateAnswer()
	{
		return new Answer();
	}
}