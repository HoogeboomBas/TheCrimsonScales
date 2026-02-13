using System;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public abstract class SyncedAction
{
	[JsonProperty]
	public int OwnerCharacterIndex { get; private set; }

	[JsonProperty]
	public int PromptIndex { get; private set; }

	public Character Owner => GameController.Instance.CharacterManager.GetCharacter(OwnerCharacterIndex);

	protected SyncedAction()
	{
	}

	protected SyncedAction(Character character)
	{
		OwnerCharacterIndex = character.Index;
		PromptIndex = GameController.Instance.SavedScenario.PromptAnswers.Count;
	}

	public abstract bool Validate();

	public abstract GDTask Perform();
}