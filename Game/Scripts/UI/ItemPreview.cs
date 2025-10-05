using Godot;
using GTweens.Builders;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public partial class ItemPreview : Control
{
	[Export]
	private ItemView _itemView;

	private Control _parentContainer;

	private Control _focus;

	private GTween _tween;
	private float _originOffset;

	public override void _Ready()
	{
		base._Ready();

		_parentContainer = GetParent<Control>();
		_itemView.SetModulate(Colors.Transparent);
		Hide();
	}

	public void Focus(Control focus, ItemModel itemModel)
	{
		_focus = focus;
		_itemView.SetItem(itemModel);

		_originOffset = _focus.GlobalPosition.X > GlobalPosition.X ? 100f : -100f;

		if(!Visible)
		{
			_itemView.SetPosition(new Vector2(_originOffset, _itemView.Position.Y));
		}

		Show();

		_tween?.Kill();
		float animationDuration = 0.2f * (1f - _itemView.Modulate.A);
		_tween = GTweenSequenceBuilder.New()
			.Append(_itemView.TweenPositionX(0f, animationDuration))
			.Join(_itemView.TweenModulateAlpha(1f, animationDuration))
			.Build().Play();

		SetProcess(true);
	}

	public void Unfocus(Control focus)
	{
		if(_focus != focus)
		{
			return;
		}

		_tween?.Kill();
		float animationDuration = 0.2f * _itemView.Modulate.A;
		_tween = GTweenSequenceBuilder.New()
			.AppendTime(0.05f)
			.Append(_itemView.TweenPositionX(_originOffset, animationDuration))
			.Join(_itemView.TweenModulateAlpha(0f, animationDuration))
			.AppendCallback(Hide)
			.Build().Play();

		_focus = null;
		SetProcess(false);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if(_focus == null)
		{
			return;
		}

		Transform2D focusGlobalTransform = _focus.GetGlobalTransformWithCanvas();
		Vector2 focusViewportPosition = focusGlobalTransform.Origin / GetViewport().GetVisibleRect().Size;

		Vector2 focusSize = focusGlobalTransform.Scale * _focus.Size;

		float targetX;
		if(focusViewportPosition.X < 0.5f)
		{
			targetX = _focus.GlobalPosition.X + focusSize.X + 30f;
		}
		else
		{
			targetX = _focus.GlobalPosition.X - Size.X - 50f;
		}

		targetX = Mathf.Clamp(
			targetX,
			_parentContainer.GlobalPosition.X,
			_parentContainer.GlobalPosition.X + _parentContainer.Size.X - Size.X);

		float targetY = _focus.GlobalPosition.Y + focusSize.Y * 0.5f - Size.Y * 0.5f;

		targetY = Mathf.Clamp(
			targetY,
			_parentContainer.GlobalPosition.Y,
			_parentContainer.GlobalPosition.Y + _parentContainer.Size.Y - Size.Y);

		SetGlobalPosition(new Vector2(targetX, targetY));
	}
}