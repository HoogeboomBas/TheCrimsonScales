using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class CharacterManager
{
	public List<CharacterStartHex> CharacterStartHexes { get; private set; }

	public List<Character> Characters { get; } = new List<Character>();

	public CharacterManager()
	{
	}

	public async GDTask PlaceCharacters()
	{
		CharacterStartHexes = GameController.Instance.Map.GetChildrenOfType<CharacterStartHex>();

		// Place all characters
		for(int i = 0; i < GameController.Instance.SavedCampaign.Characters.Count; i++)
		{
			SavedCharacter savedCharacter = GameController.Instance.SavedCampaign.Characters[i];

			CharacterStartHex characterStartHex = CharacterStartHexes[i];
			Character characterHexObject = savedCharacter.ClassModel.Scene.Instantiate<Character>();
			GameController.Instance.Map.AddChild(characterHexObject, true);
			Hex hex = characterStartHex.Hex;
			await characterHexObject.Init(hex);
			characterHexObject.Spawn(savedCharacter, i);

			Characters.Add(characterHexObject);
		}

		await GDTask.CompletedTask;
	}

	public Character GetCharacter(int index)
	{
		if(index > Characters.Count - 1)
		{
			return null;
		}

		return Characters[index];
	}

	public Character FirstAlive(bool firstCharacterIfAllDead = true)
	{
		foreach(Character character in Characters)
		{
			if(!character.IsDead)
			{
				return character;
			}
		}

		return firstCharacterIfAllDead ? GetCharacter(0) : null;
	}

	public async GDTask RemoveCharacterStartHexes()
	{
		foreach(CharacterStartHex characterStartHex in CharacterStartHexes)
		{
			await characterStartHex.Destroy();
		}
	}

	public async GDTask AddStartHexIndicators()
	{
		PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scenario/StartHexIndicator.tscn");
		foreach(CharacterStartHex characterStartHex in CharacterStartHexes)
		{
			StartHexIndicator startHexIndicator = scene.Instantiate<StartHexIndicator>();
			GameController.Instance.Map.AddChild(startHexIndicator);
			Hex hex = characterStartHex.Hex;
			await startHexIndicator.Init(hex);
		}
	}
}