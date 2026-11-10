using Godot;

public partial class ItemEffectButton : EffectButton<ItemEffectButton.Parameters>
{
	public class Parameters : EffectButtonParameters
	{
		public override string ScenePath => "res://Scenes/Scenario/UI/EffectButtons/ItemEffectButton.tscn";

		public ItemModel ItemModel { get; }

		public Parameters(ItemModel itemModel)
		{
			ItemModel = itemModel;
		}
	}

	[Export]
	private ItemView _itemView;

	[Export]
	private Control _iconContainer;
	[Export]
	private TextureRect _iconTextureRect;

	protected override void Init(Parameters parameters)
	{
		base.Init(parameters);

		_itemView.SetItem(parameters.ItemModel, showCostItemCount: false);

		_iconContainer.SetVisible(parameters.ItemModel.Owner != null);
		if(parameters.ItemModel.Owner != null)
		{
			_iconTextureRect.SetTexture(parameters.ItemModel.Owner.ClassModel.IconTexture);
			_iconTextureRect.SetSelfModulate(parameters.ItemModel.Owner.ClassModel.PrimaryColor);
		}
	}
}
