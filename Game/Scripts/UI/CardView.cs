using Godot;

public abstract partial class CardView : Control
{
	private Control _container;
	private RoundedCornersTextureRect _textureRect;

	public override void _Ready()
	{
		base._Ready();

		_container = GetNode<Control>("Container");
		_textureRect = GetNode<RoundedCornersTextureRect>("Container/TextureRect");
	}

	protected void Init(Texture2D texture)
	{
		if(texture != null)
		{
			_textureRect.SetTexture(texture);
		}

		_container.SetScale(Size / _container.Size);
		this.DelayedCall(() =>
		{
			_container.SetScale(Size / _container.Size);
		});
	}
}