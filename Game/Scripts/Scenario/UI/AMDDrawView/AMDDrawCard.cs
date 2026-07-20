using Fractural.Tasks;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class AMDDrawCard : Control
{
	[Export]
	private Control _container;
	[Export]
	private TextureRect _textureRect;
	[Export]
	private TextureRect _cardBack;

	public async GDTask DrawCard(AMDCard card, Control deckAnchor, Control discardAnchor)
	{
		GlobalPosition = deckAnchor.GlobalPosition;

		_textureRect.Hide();
		_textureRect.Texture = card.GetTexture();

		AppController.Instance.AudioController.PlayFastForwardable(SFX.GetCardDraw(), volumeDb: -8);

		this.TweenGlobalPosition(discardAnchor.GlobalPosition, 0.3f).SetEasing(Easing.InOutQuad).PlayFastForwardable();
		await GTweenSequenceBuilder.New()
			.Append(_container.TweenScale(new Vector2(0f, 1.3f), 0.15f).SetEasing(Easing.Linear))
			.AppendCallback(() =>
			{
				_cardBack.Visible = false;
				_textureRect.Visible = true;
			})
			.Join(_container.TweenScale(1.2f * Vector2.One, 0.15f).SetEasing(Easing.OutBack))
			.AppendTime(0.1f)
			.Append(_container.TweenScale(Vector2.One, 0.15f).SetEasing(Easing.OutQuad))
			.Build().PlayFastForwardableAsync();

		QueueFree();
	}
}