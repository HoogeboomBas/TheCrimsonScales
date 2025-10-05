using System.Collections.Generic;
using Godot;

public partial class PartyInfoPopup : Popup<PartyInfoPopup.Request>
{
	public class Request : PopupRequest
	{
		public List<Character> Characters { get; set; }
	}

	[Export]
	private PackedScene _partyInfoCharacterScene;

	[Export]
	private Control _partyInfoCharacterContainer;

	private readonly List<PartyInfoCharacter> _partyInfoCharacters = new List<PartyInfoCharacter>();

	protected override void OnOpen()
	{
		base.OnOpen();

		foreach(Character character in PopupRequest.Characters)
		{
			PartyInfoCharacter partyInfoCharacter = _partyInfoCharacterScene.Instantiate<PartyInfoCharacter>();
			_partyInfoCharacterContainer.AddChild(partyInfoCharacter);
			partyInfoCharacter.Init(character);
			_partyInfoCharacters.Add(partyInfoCharacter);
		}
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach(PartyInfoCharacter partyInfoCharacter in _partyInfoCharacters)
		{
			partyInfoCharacter.QueueFree();
		}

		_partyInfoCharacters.Clear();
	}
}