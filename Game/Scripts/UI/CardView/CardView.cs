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

	public void SetCard(AbilityCard abilityCard, bool enableTop = true, bool enableBottom = true)
	{
		foreach(CardViewCharacterToken token in _tokens)
		{
			token.QueueFree();
		}

		_tokens.Clear();

		SetCard(abilityCard.Model, enableTop, enableBottom);

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

	public void SetCard(AbilityCardModel abilityCard, bool enableTop = true, bool enableBottom = true)
	{
		Texture2D texture = abilityCard.GetTexture();
		foreach(TextureRect textureRect in _textureRects)
		{
			textureRect.Texture = texture;
		}

		Color grayedOutColor = new Color(0.25f, 0.25f, 0.25f, 1f);
		_topContainer.Modulate = enableTop ? Colors.White : grayedOutColor;
		_bottomContainer.Modulate = enableBottom ? Colors.White : grayedOutColor;
	}

	// public void SetCardMaterial(CardState cardState)
	// {
	// 	foreach(TextureRect textureRect in _textureRects)
	// 	{
	// 		UIHelper.SetCardMaterial(textureRect, cardState);
	// 	}
	// }
}