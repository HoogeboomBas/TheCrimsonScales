using Godot;

public partial class AbilityCardViewCharacterToken : Control
{
	[Export]
	private TextureRect _textureRect;

	[Export]
	private RichTextLabel _countLabel;

	private Vector2 _normalizedPosition;

	public override void _Ready()
	{
		base._Ready();

		SetProcess(OS.IsDebugBuild());
	}

	public void Init(Texture2D texture, UseSlot useSlot)
	{
		Init(texture, useSlot.NormalizedPosition!.Value);
	}

	public void Init(Texture2D texture, Vector2 position, int count = 0)
	{
		_textureRect.Texture = texture;
		Control parent = GetParent<Control>();
		_normalizedPosition = position;
		Position = _normalizedPosition * parent.Size - 0.5f * Size;
		if(count > 0)
		{
			_countLabel.SetText(count.ToString());
			_countLabel.Show();
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		Vector2 moveInput = Vector2.Zero;
		float mult = 1f;
		if(Input.IsKeyPressed(Key.Shift))
		{
			mult = 10f;
		}

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
			_normalizedPosition += moveInput * mult * 0.0005f;
			Control parent = GetParent<Control>();
			Position = _normalizedPosition * parent.Size - 0.5f * Size;

			Log.Write($"({_normalizedPosition.X}f, {_normalizedPosition.Y}f)");
		}
	}
}