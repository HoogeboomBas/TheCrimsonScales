using System;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class AllStartScenarioWithMinusOnesReward : SavedReward
{
	[JsonProperty]
	private int _number;

	public override RewardType Type => RewardType.ScenarioStart;

	public AllStartScenarioWithMinusOnesReward()
	{
	}

	public AllStartScenarioWithMinusOnesReward(int number)
	{
		_number = number;
	}

	public override string GetLabelText(RichTextParameters textParameters) =>
		$"All characters start the next scenario with {Icons.Inline(Icons.MinusOneCard)} x{_number}.";

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			ScenarioEvents.AddMinusOnesEventReward.Parameters inflictConditionsParameters =
				await ScenarioEvents.AddMinusOnesEventRewardEvent.CreatePrompt(
					new ScenarioEvents.AddMinusOnesEventReward.Parameters(character), character);

			if(!inflictConditionsParameters.Prevented)
			{
				for(int i = 0; i < _number; i++)
				{
					character.AMDCardDeck.AddMinusOne();
				}
			}
		}
	}
}