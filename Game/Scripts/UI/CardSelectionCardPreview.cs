using Godot;
using GTweens.Builders;
using GTweens.Tweens;
using GTweensGodot.Extensions;

public partial class CardSelectionCardPreview : Control
{
	[Export]
	private CardView _cardView;

	private Control _parentContainer;

	private CardSelectionCard _focus;

	private GTween _tween;
	private float _originOffset;

	public override void _Ready()
	{
		base._Ready();

		_parentContainer = GetParent<Control>();
		_cardView.SetModulate(Colors.Transparent);
		Hide();
	}

	public void Focus(CardSelectionCard card)
	{
		_focus = card;
		_cardView.SetCard(card.SavedAbilityCard.Model);

		_originOffset = card.GlobalPosition.X > GlobalPosition.X ? 100f : -100f;

		if(!Visible)
		{
			_cardView.SetPosition(new Vector2(_originOffset, _cardView.Position.Y));
		}

		Show();

		_tween?.Kill();
		float animationDuration = 0.2f * (1f - _cardView.Modulate.A);
		_tween = GTweenSequenceBuilder.New()
			.Append(_cardView.TweenPositionX(0f, animationDuration))
			.Join(_cardView.TweenModulateAlpha(1f, animationDuration))
			.Build().Play();

		SetProcess(true);
	}

	public void Unfocus(CardSelectionCard card)
	{
		if(_focus == card)
		{
			_tween?.Kill();
			float animationDuration = 0.2f * _cardView.Modulate.A;
			_tween = GTweenSequenceBuilder.New()
				.AppendTime(0.05f)
				.Append(_cardView.TweenPositionX(_originOffset, animationDuration))
				.Join(_cardView.TweenModulateAlpha(0f, animationDuration))
				.AppendCallback(Hide)
				.Build().Play();

			_focus = null;
			SetProcess(false);
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if(_focus == null)
		{
			return;
		}

		Transform2D cardGlobalTransform = _focus.GetGlobalTransformWithCanvas();
		Vector2 cardViewportPosition = cardGlobalTransform.Origin / GetViewport().GetVisibleRect().Size;

		Vector2 cardSize = cardGlobalTransform.Scale * CardSelectionCard.Size;

		float targetX;
		if(cardViewportPosition.X < 0.5f)
		{
			targetX = _focus.GlobalPosition.X + cardSize.X + 30f;
		}
		else
		{
			targetX = _focus.GlobalPosition.X - Size.X - 50f;
		}

		targetX = Mathf.Clamp(
			targetX,
			_parentContainer.GlobalPosition.X,
			_parentContainer.GlobalPosition.X + _parentContainer.Size.X - Size.X);

		float targetY = _focus.GlobalPosition.Y + cardSize.Y * 0.5f - Size.Y * 0.5f;

		targetY = Mathf.Clamp(
			targetY,
			_parentContainer.GlobalPosition.Y,
			_parentContainer.GlobalPosition.Y + _parentContainer.Size.Y - Size.Y);

		SetGlobalPosition(new Vector2(targetX, targetY));
	}
}