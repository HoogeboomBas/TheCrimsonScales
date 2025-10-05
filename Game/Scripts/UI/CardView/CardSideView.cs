using Godot;

public partial class CardSideView : Control
{
	[Export]
	private TextureRect[] _textureRects;

	[Export]
	private Control _topContainer;
	[Export]
	private Control _bottomContainer;

	// [Export]
	// private PackedScene _characterTokenScene;
	// [Export]
	// private Control _characterTokenParent;

	//private readonly List<CardViewCharacterToken> _tokens = new List<CardViewCharacterToken>();

	public void SetCard(AbilityCardSide abilityCardSide)
	{
		SetCard(abilityCardSide.AbilityCard, abilityCardSide.AbilityCard.Top == abilityCardSide);
	}

	public void SetCard(AbilityCard abilityCard, bool showTop = true)
	{
		// foreach(CardViewCharacterToken token in _tokens)
		// {
		// 	token.QueueFree();
		// }
		//
		// _tokens.Clear();

		Texture2D texture = abilityCard.Model.GetTexture();
		foreach(TextureRect textureRect in _textureRects)
		{
			textureRect.Texture = texture;
		}

		_topContainer.SetVisible(showTop);
		_bottomContainer.SetVisible(!showTop);

		CustomMinimumSize = showTop ? _topContainer.Size : _bottomContainer.Size;

		// foreach(ActionState activeActionState in abilityCard.ActiveActionStates)
		// {
		// 	foreach(AbilityState abilityState in activeActionState.AbilityStates)
		// 	{
		// 		if(abilityState is UseSlotAbility.State useSlotAbilityState && useSlotAbilityState.UseSlotIndex < useSlotAbilityState.Slots.Count)
		// 		{
		// 			UseSlot useSlot = useSlotAbilityState.Slots[useSlotAbilityState.UseSlotIndex];
		// 			Texture2D tokenTexture = abilityCard.Owner.ClassModel.CharacterTokenTexture;
		//
		// 			CardViewCharacterToken characterToken = _characterTokenScene.Instantiate<CardViewCharacterToken>();
		// 			_characterTokenParent.AddChild(characterToken);
		// 			characterToken.Init(tokenTexture, useSlot);
		// 			_tokens.Add(characterToken);
		// 		}
		// 	}
		// }
	}

	// public void SetCardMaterial(CardState cardState)
	// {
	// 	foreach(TextureRect textureRect in _textureRects)
	// 	{
	// 		UIHelper.SetCardMaterial(textureRect, cardState);
	// 	}
	// }
}