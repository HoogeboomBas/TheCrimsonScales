using System;
using System.Collections.Generic;
using Godot;

public partial class ItemRewardCharacterSelectionPopup : Popup<ItemRewardCharacterSelectionPopup.Request>
{
	public class Request : PopupRequest
	{
		public ItemModel ItemModel { get; init; }
		public List<SavedCharacter> Characters { get; init; }
		public Action<SavedCharacter> OnCharacterConfirmed { get; init; }
	}

	[Export]
	private ItemView _itemView;

	[Export]
	private RichTextLabel _label;

	[Export]
	private PackedScene _characterScene;
	[Export]
	private Control _characterParent;

	[Export]
	private ChoiceButton _confirmButton;

	private ItemRewardCharacterSelectionPopupCharacter _selectedCharacter;

	private readonly List<ItemRewardCharacterSelectionPopupCharacter> _characters = new List<ItemRewardCharacterSelectionPopupCharacter>();

	public override void _Ready()
	{
		base._Ready();

		_confirmButton.BetterButton.Pressed += OnConfirmPressed;
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		SetCanClose(false);

		_itemView.SetItem(PopupRequest.ItemModel);
		_label.SetText($"Select who will receive the {PopupRequest.ItemModel.Name}.");

		foreach(SavedCharacter character in PopupRequest.Characters)
		{
			ItemRewardCharacterSelectionPopupCharacter popupCharacter = _characterScene.Instantiate<ItemRewardCharacterSelectionPopupCharacter>();
			_characterParent.AddChild(popupCharacter);
			popupCharacter.Init(character);
			popupCharacter.PressedEvent += OnCharacterPressed;
			_characters.Add(popupCharacter);
		}

		OnCharacterPressed(_characters[0]);
	}

	protected override void OnClosed()
	{
		base.OnClosed();

		foreach(ItemRewardCharacterSelectionPopupCharacter character in _characters)
		{
			character.QueueFree();
		}

		_characters.Clear();
	}

	private void OnCharacterPressed(ItemRewardCharacterSelectionPopupCharacter character)
	{
		_selectedCharacter = character;

		foreach(ItemRewardCharacterSelectionPopupCharacter otherCharacter in _characters)
		{
			otherCharacter.SetSelected(otherCharacter == _selectedCharacter, true);
		}
	}

	private void OnConfirmPressed()
	{
		SetCanClose(true);

		PopupRequest.OnCharacterConfirmed?.Invoke(_selectedCharacter.SavedCharacter);

		Close();
	}
}