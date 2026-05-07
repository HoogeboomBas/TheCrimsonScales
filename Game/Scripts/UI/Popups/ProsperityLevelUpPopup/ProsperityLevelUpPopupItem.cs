using Godot;

public partial class ProsperityLevelUpPopupItem : Control
{
	[Export]
	private ItemView _itemView;

	public void Init(ItemModel itemModel)
	{
		_itemView.SetItem(itemModel);
	}
}