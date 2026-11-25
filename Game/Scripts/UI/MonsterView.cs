using Godot;

public partial class MonsterView : Control
{
	[Export]
	private TextureRect _textureRect;
	[Export]
	private Control _colorOutline;

	public void Init(MonsterModel monsterModel)
	{
		_textureRect.Texture = ResourceLoader.Load<Texture2D>(monsterModel.PortraitTexturePath);
		_colorOutline.SetModulate(Colors.White);
	}
}
