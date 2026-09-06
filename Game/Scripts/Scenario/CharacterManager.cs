using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class CharacterManager
{
	public List<CharacterStartHex> CharacterStartHexes { get; private set; }

	public List<Character> Characters { get; } = new List<Character>();

	public CharacterManager()
	{
	}

	public async GDTask PlaceCharacters()
	{
		CharacterStartHexes = GameController.Instance.Map.GetChildrenOfType<CharacterStartHex>().Where(hex => hex.Visible).ToList();

		// Place all characters

		List<SavedCharacter> savedCharacters = [];
		if(GameController.Instance.ScenarioModel is SoloScenarioModel soloScenarioModel)
		{
			savedCharacters.Add(
				GameController.Instance.SavedCampaign.Characters.First(character => character.ClassModel == soloScenarioModel.ClassModel));
		}
		else
		{
			savedCharacters.AddRange(GameController.Instance.SavedCampaign.Characters);
		}

		int index = 0;
		foreach(SavedCharacter savedCharacter in savedCharacters)
		{
			CharacterStartHex characterStartHex = CharacterStartHexes[index];
			Character characterHexObject = savedCharacter.ClassModel.Scene.Instantiate<Character>();
			GameController.Instance.Map.AddChild(characterHexObject, true);
			Hex hex = characterStartHex.Hex;
			await characterHexObject.Init(hex);
			await characterHexObject.Spawn(savedCharacter, index);

			Characters.Add(characterHexObject);
			index++;
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
}