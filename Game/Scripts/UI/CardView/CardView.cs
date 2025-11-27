using System.Collections.Generic;
using Godot;

public partial class CardView : Control
{
	[Export]
	private TextureRect[] _textureRects;

	[Export]
	private Control _topContainer;
	[Export]
	private Control _bottomContainer;

	[Export]
	private PackedScene _characterTokenScene;
	[Export]
	private Control _characterTokenParent;

	private readonly List<CardViewCharacterToken> _tokens = new List<CardViewCharacterToken>();

	public void SetCard(SavedAbilityCard savedAbilityCard, bool enableTop = true, bool enableBottom = true)
	{
		Texture2D texture = savedAbilityCard.Model.GetTexture();
		foreach(TextureRect textureRect in _textureRects)
		{
			textureRect.Texture = texture;
		}

		Color grayedOutColor = new Color(0.25f, 0.25f, 0.25f, 1f);
		_topContainer.Modulate = enableTop ? Colors.White : grayedOutColor;
		_bottomContainer.Modulate = enableBottom ? Colors.White : grayedOutColor;

		foreach(CardViewCharacterToken token in _tokens)
		{
			token.QueueFree();
		}

		_tokens.Clear();

		AbilityCard abilityCard = GameController.Instance?.CardManager.Get(savedAbilityCard);
		if(abilityCard != null)
		{
			foreach(ActionState activeActionState in abilityCard.ActiveActionStates)
			{
				foreach(AbilityState abilityState in activeActionState.AbilityStates)
				{
					if(abilityState is UseSlotAbility.State useSlotAbilityState && useSlotAbilityState.UseSlotIndex < useSlotAbilityState.Slots.Count)
					{
						UseSlot useSlot = useSlotAbilityState.Slots[useSlotAbilityState.UseSlotIndex];

						if(useSlot.NormalizedPosition.HasValue)
						{
							Texture2D tokenTexture = abilityCard.Owner.ClassModel.CharacterTokenTexture;

							CardViewCharacterToken characterToken = _characterTokenScene.Instantiate<CardViewCharacterToken>();
							_characterTokenParent.AddChild(characterToken);
							characterToken.Init(tokenTexture, useSlot);
							_tokens.Add(characterToken);
						}
					}
				}
			}
		}
	}
}