﻿using System;
using Fractural.Tasks;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public abstract class SyncedAction
{
	[JsonProperty]
	public int OwnerCharacterIndex { get; set; }

	[JsonProperty]
	public int PromptIndex { get; set; }

	public Character Owner => GameController.Instance.CharacterManager.GetCharacter(OwnerCharacterIndex);

	protected SyncedAction()
	{
	}

	public SyncedAction(Character character)
	{
		OwnerCharacterIndex = character.Index;
		PromptIndex = GameController.Instance.SavedScenario.PromptAnswers.Count;
	}

	public abstract GDTask Perform();
}