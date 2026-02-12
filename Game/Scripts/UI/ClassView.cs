using Godot;

public partial class ClassView : Control
{
	[Export]
	private TextureRect _textureRect;
	[Export]
	private Control _colorOutline;
	[Export]
	private TextureRect _iconTexture;
	[Export]
	private TextureRect _iconShadowTexture;

	public void Init(ClassModel classModel)
	{
		_textureRect.SetTexture(classModel.PortraitTexture);
		_colorOutline.SetModulate(classModel.PrimaryColor);
		_iconTexture.SetModulate(classModel.PrimaryColor);
		_iconTexture.SetTexture(classModel.IconTexture);
		_iconShadowTexture.SetTexture(classModel.IconTexture);
	}
}