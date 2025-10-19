using Godot;
using GTweensGodot.Extensions;

public partial class TreasureItemRewardView : Control
{
	[Export]
	private ItemView _itemView;

	public override void _Ready()
	{
		base._Ready();

		Hide();
		this.TweenModulateAlpha(0f, 0f).Play(true);
	}

	public void Open(ItemModel itemModel)
	{
		_itemView.SetItem(itemModel);

		Show();
		this.TweenModulateAlpha(1f, 0.3f).Play();
	}

	public void Close()
	{
		this.TweenModulateAlpha(0f, 0.3f).OnComplete(Hide).Play();
	}
}