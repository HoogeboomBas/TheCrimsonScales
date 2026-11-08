using Godot;

public partial class TooltipView : Control
{
	[Export]
	private ResizingLabel _nameLabel;
	[Export]
	private TextureRect _iconTexture;
	// [Export]
	// private Label _categoryLabel;
	[Export]
	private RichTextLabel _textLabel;

	public void Init(Tooltip tooltip)
	{
		_nameLabel.SetText(tooltip.Name);
		_iconTexture.SetVisible(tooltip.IconPath != null);
		if(tooltip.IconPath != null)
		{
			_iconTexture.SetTexture(ResourceLoader.Load<Texture2D>(tooltip.IconPath));
		}

		// _categoryLabel.SetVisible(tooltip.Category != null);
		// if(tooltip.Category != null)
		// {
		// 	_categoryLabel.SetText(tooltip.Category);
		// }

		_textLabel.SetText(tooltip.GetText(_textLabel.GetRichTextParameters()));
	}
}
