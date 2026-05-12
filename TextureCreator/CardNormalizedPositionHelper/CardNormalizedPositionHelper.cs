using System.Globalization;
using Godot;

public partial class CardNormalizedPositionHelper : Control
{
	[Export]
	private Texture2D _cards;
	[Export]
	private int _columnCount;
	[Export]
	private int _rowCount;

	[Export]
	private Control _cardContainer;
	[Export]
	private TextureRect _cardTextureRect;
	[Export]
	private Control _markerContainer;

	private int _cardIndex;

	private Vector2 _normalizedPosition;

	public override void _Ready()
	{
		base._Ready();

		SetIndex(0);
		SetNormalizedPosition(Vector2.Zero);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if(@event is InputEventKey inputEventKey && inputEventKey.Pressed)
		{
			if(inputEventKey.Keycode == Key.Left)
			{
				SetIndex(_cardIndex - 1);
			}

			if(inputEventKey.Keycode == Key.Right)
			{
				SetIndex(_cardIndex + 1);
			}

			if(inputEventKey.Keycode == Key.P)
			{
				string normalizedPositionText = $"new Vector2({_normalizedPosition.X.ToString(CultureInfo.InvariantCulture)}f, {_normalizedPosition.Y.ToString(CultureInfo.InvariantCulture)}f)";
				GD.Print(normalizedPositionText);
				DisplayServer.ClipboardSet(normalizedPositionText);
			}
			
			Vector2 moveInput = Vector2.Zero;
			float mult = 1f;
			if(Input.IsKeyPressed(Key.Shift))
			{
				mult = 10f;
			}

			if(inputEventKey.Keycode is Key.L or Key.D)
			{
				moveInput.X = 1f;
			}

			if(inputEventKey.Keycode is Key.J or Key.A)
			{
				moveInput.X = -1f;
			}

			if(inputEventKey.Keycode is Key.I or Key.W)
			{
				moveInput.Y = -1f;
			}

			if(inputEventKey.Keycode is Key.K or Key.S)
			{
				moveInput.Y = 1f;
			}

			if(moveInput != Vector2.Zero)
			{
				SetNormalizedPosition(_normalizedPosition + moveInput * mult * 0.0001f);
			}
		}

		if(@event is InputEventMouse inputEventMouse && inputEventMouse.ButtonMask == MouseButtonMask.Left)
		{
			SetNormalizedPosition(inputEventMouse.Position / _cardContainer.Size);
		}
	}

	private void SetIndex(int index)
	{
		_cardIndex = Mathf.Clamp(index, 0, _rowCount * _columnCount - 1);

		AtlasTexture texture = AtlasTextureHelper.CreateAtlasTexture(_cardIndex, _columnCount, _rowCount, _cards);
		_cardTextureRect.SetTexture(texture);

		Vector2 size = texture.GetSize();
		float scale = size.Y / Size.Y;
		_cardContainer.SetSize(size / scale);
		//_cardContainer.SetPosition(new Vector2((Size.X - size.X) / 2f, 0f));
	}

	private void SetNormalizedPosition(Vector2 normalizedPosition)
	{
		_normalizedPosition = normalizedPosition;
		_markerContainer.SetPosition(_normalizedPosition * _cardContainer.Size - 0.5f * _markerContainer.Size);
	}
}