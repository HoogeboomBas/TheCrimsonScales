using Godot;

public partial class EnhancementView : Control
{
	[Export]
	private TextureRect _textureRect;

	public EnhancementModel EnhancementModel { get; private set; }

	public void SetModel(EnhancementModel enhancementModel)
	{
		EnhancementModel = enhancementModel;

		_textureRect.SetTexture(EnhancementModel.GetTexture());
	}
}