using Godot;

public partial class CardViewCharacterToken : Control
{
	[Export]
	private TextureRect _textureRect;

	private Vector2 _normalizedPosition;

	public override void _Ready()
	{
		base._Ready();

		SetProcess(OS.IsDebugBuild());
	}

	public void Init(Texture2D texture, UseSlot useSlot)
	{
		_textureRect.Texture = texture;
		Control parent = GetParent<Control>();
		_normalizedPosition = useSlot.NormalizedPosition!.Value;
		Position = _normalizedPosition * parent.Size - 0.5f * Size;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		Vector2 moveInput = Vector2.Zero;

		if(Input.IsKeyPressed(Key.L))
		{
			moveInput.X = 1f;
		}

		if(Input.IsKeyPressed(Key.J))
		{
			moveInput.X = -1f;
		}

		if(Input.IsKeyPressed(Key.I))
		{
			moveInput.Y = -1f;
		}

		if(Input.IsKeyPressed(Key.K))
		{
			moveInput.Y = 1f;
		}

		if(moveInput != Vector2.Zero)
		{
			_normalizedPosition += moveInput * 0.0005f;
			Control parent = GetParent<Control>();
			Position = _normalizedPosition * parent.Size - 0.5f * Size;

			Log.Write(_normalizedPosition.ToString());
		}
	}
}