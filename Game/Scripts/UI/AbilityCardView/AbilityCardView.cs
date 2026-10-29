using System;
using System.Collections.Generic;
using Godot;

public partial class AbilityCardView : CardView
{
	[Export]
	private PackedScene _enhancementScene;
	[Export]
	private Control _enhancementParent;

	[Export]
	private PackedScene _characterTokenScene;
	[Export]
	private Control _characterTokenParent;

	[Export]
	private InspectOverlay _inspectOverlay;

	private AbilityCardModel _model;
	private SavedAbilityCard _savedAbilityCard;

	private readonly List<AbilityCardViewEnhancement> _enhancements = new List<AbilityCardViewEnhancement>();
	private readonly List<AbilityCardViewCharacterToken> _tokens = new List<AbilityCardViewCharacterToken>();

	public override void _Ready()
	{
		base._Ready();

		_inspectOverlay.InspectRequestedEvent += OnInspectRequested;
	}

	public void SetCard(AbilityCardModel model)
	{
		Clear();

		_model = model;

		Texture2D texture = model.GetTexture();
		Init(texture);
	}

	public void SetCard(SavedAbilityCard savedAbilityCard)
	{
		SetCard(savedAbilityCard.Model);

		_savedAbilityCard = savedAbilityCard;

		AddEnhancements(savedAbilityCard.Model.Top.Enhancements, savedAbilityCard.SavedTopEnhancements);
		AddEnhancements(savedAbilityCard.Model.Bottom.Enhancements, savedAbilityCard.SavedBottomEnhancements);

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

							AbilityCardViewCharacterToken characterToken = _characterTokenScene.Instantiate<AbilityCardViewCharacterToken>();
							_characterTokenParent.AddChild(characterToken);
							characterToken.Init(tokenTexture, useSlot);
							_tokens.Add(characterToken);
						}
					}

					if(abilityState is ActiveAbilityState activeAbilityState && activeAbilityState.CharacterTokens > 0)
					{
						Vector2 position = activeAbilityState.CharacterTokenPosition;
						Texture2D tokenTexture = abilityCard.Owner.ClassModel.CharacterTokenTexture;

						AbilityCardViewCharacterToken characterToken = _characterTokenScene.Instantiate<AbilityCardViewCharacterToken>();
						_characterTokenParent.AddChild(characterToken);
						characterToken.Init(tokenTexture, position, activeAbilityState.CharacterTokens);
						_tokens.Add(characterToken);
					}
				}
			}
		}
	}

	private void AddEnhancements(List<EnhancementMark> enhancementMarks, Dictionary<int, SavedEnhancement> savedEnhancements)
	{
		foreach((int index, SavedEnhancement savedEnhancement) in savedEnhancements)
		{
			EnhancementMark enhancementMark = enhancementMarks[index];
			AbilityCardViewEnhancement abilityCardViewEnhancement = _enhancementScene.Instantiate<AbilityCardViewEnhancement>();
			_enhancementParent.AddChild(abilityCardViewEnhancement);
			abilityCardViewEnhancement.Init(enhancementMark, savedEnhancement.Model);
			_enhancements.Add(abilityCardViewEnhancement);
		}
	}

	private void Clear()
	{
		_model = null;
		_savedAbilityCard = null;

		foreach(AbilityCardViewEnhancement enhancement in _enhancements)
		{
			enhancement.QueueFree();
		}

		_enhancements.Clear();

		foreach(AbilityCardViewCharacterToken token in _tokens)
		{
			token.QueueFree();
		}

		_tokens.Clear();
	}

	private void OnInspectRequested()
	{
		AppController.Instance.PopupManager.RequestPopup(new InspectPopup.Request()
		{
			SavedAbilityCard = _savedAbilityCard,
			Model = _model,
		});
	}
}