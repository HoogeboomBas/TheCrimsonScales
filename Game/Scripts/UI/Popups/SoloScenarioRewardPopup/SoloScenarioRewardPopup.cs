using Godot;

public partial class SoloScenarioRewardPopup : Popup<SoloScenarioRewardPopup.Request>
{
	public class Request : PopupRequest
	{
		public ItemModel ItemModel { get; init; }
		public SavedCharacter SavedCharacter { get; init; }
	}

	[Export]
	private ItemView _itemView;

	[Export]
	private RichTextLabel _itemLabel;

	[Export]
	private ChoiceButton _itemButton;

	[Export]
	private ChoiceButton _perkButton;

	public override void _Ready()
	{
		base._Ready();

		_itemButton.BetterButton.Pressed += OnItemButtonPressed;
		_perkButton.BetterButton.Pressed += OnPerkButtonPressed;
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		SetCanClose(false);

		_itemView.SetItem(PopupRequest.ItemModel);
		_itemLabel.SetText($"Gain the {PopupRequest.ItemModel.Name}.");
	}

	private void OnItemButtonPressed()
	{
		SetCanClose(true);

		PopupRequest.SavedCharacter.AddItem(PopupRequest.ItemModel);

		Close();
	}

	private void OnPerkButtonPressed()
	{
		SetCanClose(true);

		PopupRequest.SavedCharacter.AddAvailablePerk();

		Close();
	}
}