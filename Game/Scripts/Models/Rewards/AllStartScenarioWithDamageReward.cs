using System;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public class AllStartScenarioWithDamageReward : SavedReward
{
	[JsonProperty]
	private int _damage;

	public override RewardType Type => RewardType.ScenarioStart;

	public AllStartScenarioWithDamageReward()
	{
	}

	public AllStartScenarioWithDamageReward(int damage)
	{
		_damage = damage;
	}

	public override string GetLabelText(RichTextParameters textParameters) =>
		$"All characters start the next scenario with {Icons.Inline(Icons.Damage, textParameters)}{_damage}.";

	public override async GDTask OnScenarioSetupPhaseCompleted()
	{
		await base.OnScenarioSetupPhaseCompleted();

		foreach(Character character in GameController.Instance.CharacterManager.Characters)
		{
			ScenarioEvents.SufferDamageEventReward.Parameters sufferDamageParameters =
				await ScenarioEvents.SufferDamageEventRewardEvent.CreatePrompt(
					new ScenarioEvents.SufferDamageEventReward.Parameters(character), character);

			if(!sufferDamageParameters.Prevented)
			{
				await AbilityCmd.SufferDamage(character, _damage, character);
			}
		}
	}
}