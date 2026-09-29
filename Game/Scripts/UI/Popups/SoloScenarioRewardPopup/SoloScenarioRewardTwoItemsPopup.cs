using Godot;

public partial class SoloScenarioRewardTwoItemsPopup : Popup<SoloScenarioRewardTwoItemsPopup.Request>
{
	public class Request : PopupRequest
	{
		public ItemModel ItemModel1 { get; init; }
		public ItemModel ItemModel2 { get; init; }
		public SavedCharacter SavedCharacter { get; init; }
	}

	[Export]
	private ItemView _itemView1;

	[Export]
	private RichTextLabel _itemLabel1;

	[Export]
	private ChoiceButton _itemButton1;

	[Export]
	private ItemView _itemView2;

	[Export]
	private RichTextLabel _itemLabel2;

	[Export]
	private ChoiceButton _itemButton2;

	[Export]
	private ChoiceButton _perkButton;

	public override void _Ready()
	{
		base._Ready();

		_itemButton1.BetterButton.Pressed += OnItem1ButtonPressed;
		_itemButton2.BetterButton.Pressed += OnItem2ButtonPressed;
		_perkButton.BetterButton.Pressed += OnPerkButtonPressed;
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		SetCanClose(false);

		_itemView1.SetItem(PopupRequest.ItemModel1);
		_itemView2.SetItem(PopupRequest.ItemModel2);
		_itemLabel1.SetText($"Gain the {PopupRequest.ItemModel1.Name}.");
		_itemLabel2.SetText($"Gain the {PopupRequest.ItemModel2.Name}.");
	}

	private void OnItem1ButtonPressed()
	{
		SetCanClose(true);

		PopupRequest.SavedCharacter.AddItem(PopupRequest.ItemModel1);

		Close();
	}

	private void OnItem2ButtonPressed()
	{
		SetCanClose(true);

		PopupRequest.SavedCharacter.AddItem(PopupRequest.ItemModel2);

		Close();
	}

	private void OnPerkButtonPressed()
	{
		SetCanClose(true);

		PopupRequest.SavedCharacter.AddAvailablePerk();

		Close();
	}
}