using Godot;

public partial class TreasureItemRewardPopup : Popup<TreasureItemRewardPopup.Request>
{
	public class Request : PopupRequest
	{
		public ItemModel ItemModel { get; init; }
		public SavedCharacter Looter { get; init; }
	}

	[Export]
	private ItemView _itemView;
	[Export]
	private RichTextLabel _itemAndLooterLabel;

	[Export]
	private BetterButton _confirmButton;

	public override void _Ready()
	{
		base._Ready();

		_confirmButton.Pressed += OnConfirmPressed;
	}

	protected override void OnOpen()
	{
		base.OnOpen();

		_itemView.SetItem(PopupRequest.ItemModel);

		_itemAndLooterLabel.Text = $"Found {PopupRequest.ItemModel.Name} for {PopupRequest.Looter.GetNameAndIcon()}.";
	}

	private void OnConfirmPressed()
	{
		Close();
	}
}