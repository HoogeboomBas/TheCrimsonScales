using System.Collections.Generic;
using System.Linq;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweensGodot.Extensions;

public partial class Enhancer : BetweenScenariosAction
{
	[Export]
	private Node3D _3dRoot;
	[Export]
	private Node3D _crystalBall;

	[Export]
	private Control _cardContainer;
	[Export]
	private Control _cardRotationContainer;
	[Export]
	private AbilityCardView _cardView;
	[Export]
	private PackedScene _enhancementMarkButtonScene;

	[Export]
	private Control _cardListContainer;
	[Export]
	private CardSelectionList _cardSelectionList;

	[Export]
	private Control _optionsContainer;
	[Export]
	private PackedScene _enhancementOptionScene;
	[Export]
	private Control _enhancementOptionParent;

	[Export]
	private BetterButton _confirmButton;

	[Export]
	private ExclamationMark _exclamationMark;

	private readonly List<EnhancementMarkToggleButton> _enhancementMarkToggleButtons = new List<EnhancementMarkToggleButton>();
	private readonly List<EnhancementOptionToggleButton> _enhancementOptionToggleButtons = new List<EnhancementOptionToggleButton>();

	private SavedCharacter _selectedCharacter;
	private SavedAbilityCard _selectedAbilityCard;
	private EnhancementMarkToggleButton _selectedMark;
	private EnhancementOptionToggleButton _selectedOption;

	protected override bool SelectCharacter => true;

	private bool CanConfirm =>
		_selectedCharacter != null && _selectedAbilityCard != null && _selectedMark != null && _selectedOption != null &&
		_selectedCharacter.CanAfford(GetCost(_selectedCharacter, _selectedAbilityCard, _selectedMark.EnhancementMark,
			_selectedOption.EnhancementModel));

	public override void _Ready()
	{
		base._Ready();

		_3dRoot.SetVisible(false);

		_confirmButton.Pressed += OnConfirmPressed;

		Button.SetVisible(BetweenScenariosController.Instance.SavedCampaign.EnhancementsUnlocked);

		BetweenScenariosController.Instance.CharacterPortraitManager.SelectedPortraitChangedEvent += OnSelectedPortraitChanged;
		BetweenScenariosController.Instance.SavedCampaign.EnhancementsUnlockedChangedEvent += OnEnhancementsUnlocked;
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		if(BetweenScenariosController.Instance != null)
		{
			BetweenScenariosController.Instance.SavedCampaign.EnhancementsUnlockedChangedEvent -= OnEnhancementsUnlocked;
		}
	}

	protected override void AnimateIn(GTweenSequenceBuilder sequenceBuilder, BetweenScenariosAction previousActiveAction)
	{
		base.AnimateIn(sequenceBuilder, previousActiveAction);

		_exclamationMark.SetActive(false);

		_3dRoot.SetVisible(true);
		_crystalBall.SetVisible(false);

		_crystalBall.SetPosition(new Vector3(0f, 5f, 0f));
		_cardContainer.SetPosition(new Vector2(0f, 800f));
		_cardRotationContainer.SetRotationDegrees(30f);

		_cardListContainer.SetPosition(new Vector2(-600f, 0f));
		_optionsContainer.SetPosition(new Vector2(800f, 0f));

		sequenceBuilder
			.AppendTime(previousActiveAction is ItemShop ? 0.6f : 0.4f)
			.AppendCallback((() =>
			{
				_crystalBall.SetVisible(true);

				_cardContainer.TweenPositionY(0f, 0.6f).SetEasing(Easing.OutCubic).Play();
				_cardRotationContainer.TweenRotationDegrees(0f, 0.6f).SetEasing(Easing.OutCubic).Play();

				_cardListContainer.TweenPositionX(0f, 0.7f).SetEasing(Easing.OutBack).Play();

				GTweenSequenceBuilder.New()
					.Append(_optionsContainer.TweenPositionX(-40f, 0.45f).SetEasing(Easing.OutQuad))
					.Append(_optionsContainer.TweenPositionX(0f, 0.25f).SetEasing(Easing.OutQuad))
					.Build().Play();

				UpdateCardList();
			}))
			.Append(_crystalBall.TweenPositionY(0f, 0.7f).SetEasing(Easing.InQuad))
			.Append(_crystalBall.TweenPositionY(0.1f, 0.12f))
			.Append(_crystalBall.TweenPositionY(0f, 0.08f))
			.AppendTime(0.2f);
	}

	protected override void AnimateOut(GTweenSequenceBuilder sequenceBuilder)
	{
		sequenceBuilder
			.Append(_crystalBall.TweenPositionY(5f, 0.5f))
			.Join(_cardContainer.TweenPositionY(800f, 0.5f).SetEasing(Easing.InQuad))
			.Join(_cardRotationContainer.TweenRotationDegrees(30f, 0.5f).SetEasing(Easing.OutQuad))
			.Join(_cardListContainer.TweenPositionX(-600f, 0.5f).SetEasing(Easing.InBack))
			.Join(_optionsContainer.TweenPositionX(800f, 0.5f).SetEasing(Easing.InQuad))
			.AppendTime(0.2f);

		base.AnimateOut(sequenceBuilder);
	}

	protected override void AfterAnimateOut()
	{
		base.AfterAnimateOut();

		_3dRoot.SetVisible(false);
	}

	private void UpdateCardList()
	{
		_selectedCharacter = BetweenScenariosController.Instance.CharacterPortraitManager.SelectedPortrait?.SavedCharacter;
		List<SavedAbilityCard> cards = _selectedCharacter?.AvailableAbilityCards;

		if(cards == null)
		{
			_cardSelectionList.Close();
			_cardContainer.SetVisible(false);
		}
		else
		{
			_cardSelectionList.Open(cards, OnCardPressed, null,
				(cardA, cardB) => cardA.Model.Initiative.CompareTo(cardB.Model.Initiative));
			_cardContainer.SetVisible(true);
			OnCardPressed(_cardSelectionList.Cards.First());
		}

		UpdateConfirmButton();
	}

	private void CreateEnhancementMarkButtons(AbilityCardSideModel cardSideModel, Dictionary<int, SavedEnhancement> savedEnhancements, bool top)
	{
		for(int i = 0; i < cardSideModel.Enhancements.Count; i++)
		{
			if(savedEnhancements.ContainsKey(i))
			{
				continue;
			}

			EnhancementMark enhancementMark = cardSideModel.Enhancements[i];
			EnhancementMarkToggleButton enhancementMarkToggleButton = _enhancementMarkButtonScene.Instantiate<EnhancementMarkToggleButton>();
			_cardView.AddChild(enhancementMarkToggleButton);
			enhancementMarkToggleButton.Init(enhancementMark, top, i);
			enhancementMarkToggleButton.PressedEvent += OnEnhancementMarkToggleButtonPressed;
			enhancementMarkToggleButton.SetSelected(false, true, true);
			_enhancementMarkToggleButtons.Add(enhancementMarkToggleButton);
		}
	}

	private void UpdateSelectedCard(SavedAbilityCard savedAbilityCard)
	{
		_selectedAbilityCard = savedAbilityCard;
		_cardView.SetCard(_selectedAbilityCard);

		foreach(EnhancementMarkToggleButton enhancementMarkToggleButton in _enhancementMarkToggleButtons)
		{
			enhancementMarkToggleButton.QueueFree();
		}

		_enhancementMarkToggleButtons.Clear();

		CreateEnhancementMarkButtons(_selectedAbilityCard.Model.Top, _selectedAbilityCard.SavedTopEnhancements, true);
		CreateEnhancementMarkButtons(_selectedAbilityCard.Model.Bottom, _selectedAbilityCard.SavedBottomEnhancements, false);

		_selectedMark = _enhancementMarkToggleButtons.FirstOrDefault();
		_selectedMark?.SetSelected(true, true, true);

		OnEnhancementMarkToggleButtonPressed(_selectedMark);

		UpdateConfirmButton();
	}

	private void UpdateConfirmButton()
	{
		_confirmButton.SetEnabled(CanConfirm, true);
	}

	private static int GetBaseCost(SavedCharacter savedCharacter, SavedAbilityCard savedAbilityCard, EnhancementMark mark, EnhancementModel model)
	{
		Dictionary<int, SavedEnhancement> savedEnhancements =
			savedAbilityCard.GetEnhancements(mark.AbilityCardSideModel.AbilityCardSideType == AbilityCardSideType.Top);

		int cost = model.BaseCost;

		EnhancementCostType enhancementCostType = mark.EnhancementCostType;

		if(mark.EnhancementCostType.HasFlag(EnhancementCostType.AutoDetect))
		{
			if(mark.Abilities.FirstOrDefault(ability => ability is ITargetedAbility) is ITargetedAbility targetedAbility)
			{
				if(model == ModelDB.Enhancement<RedHexEnhancement>())
				{
					cost /=
						targetedAbility.AbilityAOEPattern.LocalHexes.Count(hex => hex.Type == AOEHexType.Red) +
						savedEnhancements.Count(enhancement => enhancement.Value.Model == ModelDB.Enhancement<RedHexEnhancement>());
				}
				else
				{
					if(targetedAbility.IsMultiTarget && model.DefaultDoubleCostOnDoubleTarget)
					{
						enhancementCostType |= EnhancementCostType.MultiTarget;
					}
				}
			}

			if(mark.AbilityCardSideModel.Persistent)
			{
				if(model.DefaultTripleCostOnPersistent)
				{
					enhancementCostType |= EnhancementCostType.Persistent;
				}
			}
			else if(mark.AbilityCardSideModel.Loss)
			{
				enhancementCostType |= EnhancementCostType.LossNoPersistent;
			}
		}

		if(enhancementCostType.HasFlag(EnhancementCostType.MultiTarget))
		{
			cost *= 2;
		}

		if(enhancementCostType.HasFlag(EnhancementCostType.Persistent))
		{
			cost *= 3;
		}

		if(enhancementCostType.HasFlag(EnhancementCostType.LossNoPersistent))
		{
			cost = Mathf.CeilToInt(cost * 0.5f);
		}

		// Level
		cost += (savedAbilityCard.Model.Level - 1) * 25;

		// Previous enhancements
		cost += savedEnhancements.Count * 75;

		return cost;
	}

	private static int GetCost(SavedCharacter savedCharacter, SavedAbilityCard savedAbilityCard, EnhancementMark mark, EnhancementModel model)
	{
		int cost = GetBaseCost(savedCharacter, savedAbilityCard, mark, model);

		BetweenScenariosEvents.CalculateEnhancementCost.Parameters parameters =
			BetweenScenariosEvents.CalculateEnhancementCostEvent.Fire(
				new BetweenScenariosEvents.CalculateEnhancementCost.Parameters(savedCharacter, savedAbilityCard, mark, model, cost));

		return parameters.Cost;
	}

	private void OnCardPressed(CardSelectionCard cardSelectionCard)
	{
		foreach(CardSelectionCard card in _cardSelectionList.Cards)
		{
			card.SetSelected(false);
		}

		cardSelectionCard.SetSelected(true);

		UpdateSelectedCard(cardSelectionCard.SavedAbilityCard);
	}

	private void OnEnhancementMarkToggleButtonPressed(EnhancementMarkToggleButton enhancementMarkToggleButton)
	{
		foreach(EnhancementMarkToggleButton otherEnhancementMarkToggleButton in _enhancementMarkToggleButtons)
		{
			otherEnhancementMarkToggleButton.SetSelected(false, true);
		}

		_selectedMark = enhancementMarkToggleButton;
		_selectedMark?.SetSelected(true, true);

		foreach(EnhancementOptionToggleButton enhancementOptionToggleButton in _enhancementOptionToggleButtons)
		{
			enhancementOptionToggleButton.QueueFree();
		}

		_enhancementOptionToggleButtons.Clear();

		EnhancementModel[] enhancementModels = _selectedMark?.EnhancementMark.PossibleEnhancements ?? [];

		foreach(EnhancementModel enhancementModel in enhancementModels)
		{
			int cost = GetCost(_selectedCharacter, _selectedAbilityCard, _selectedMark!.EnhancementMark, enhancementModel);

			EnhancementOptionToggleButton enhancementOptionToggleButton = _enhancementOptionScene.Instantiate<EnhancementOptionToggleButton>();
			_enhancementOptionParent.AddChild(enhancementOptionToggleButton);
			enhancementOptionToggleButton.Init(enhancementModel, cost);
			enhancementOptionToggleButton.PressedEvent += OnEnhancementOptionPressed;
			enhancementOptionToggleButton.SetSelected(false, true, true);
			_enhancementOptionToggleButtons.Add(enhancementOptionToggleButton);
		}

		_selectedOption = _enhancementOptionToggleButtons.FirstOrDefault();
		_selectedOption?.SetSelected(true, true, true);

		UpdateConfirmButton();
	}

	private void OnEnhancementOptionPressed(EnhancementOptionToggleButton button)
	{
		foreach(EnhancementOptionToggleButton enhancementOptionToggleButton in _enhancementOptionToggleButtons)
		{
			enhancementOptionToggleButton.SetSelected(false, true);
		}

		_selectedOption = button;
		_selectedOption?.SetSelected(true, true);

		UpdateConfirmButton();
	}

	private void OnConfirmPressed()
	{
		if(!CanConfirm)
		{
			return;
		}

		int cost = GetCost(_selectedCharacter, _selectedAbilityCard, _selectedMark.EnhancementMark, _selectedOption.EnhancementModel);
		int baseCost = GetBaseCost(_selectedCharacter, _selectedAbilityCard, _selectedMark.EnhancementMark, _selectedOption.EnhancementModel);

		AppController.Instance.PopupManager.OpenPopupOnTop(new TextPopup.Request("Buy Enhancement",
			$"Would you like to spend {Icons.Inline(Icons.Coins)}{cost} to buy this {Icons.Inline(_selectedOption.EnhancementModel.TexturePath)} enhancement?",
			new TextButton.Parameters("Cancel",
				() =>
				{
				}
			),
			new TextButton.Parameters("Confirm",
				() =>
				{
					_selectedCharacter.RemoveGold(cost);
					_selectedAbilityCard.AddSavedEnhancement(_selectedMark.Top, _selectedMark.Index,
						new SavedEnhancement(_selectedOption.EnhancementModel));

					AppController.Instance.AudioController.Play(SFX.Buy, delay: 0.0f);

					AppController.Instance.SaveGame();

					BetweenScenariosEvents.EnhancementBoughtEvent.Fire(
						new BetweenScenariosEvents.EnhancementBought.Parameters(_selectedCharacter, _selectedAbilityCard,
							_selectedMark.EnhancementMark, _selectedOption.EnhancementModel, baseCost, cost));

					UpdateSelectedCard(_selectedAbilityCard);
					UpdateConfirmButton();
				},
				TextButton.ColorType.Green
			)
		));
	}

	private void OnSelectedPortraitChanged(BetweenScenariosCharacterPortrait portrait)
	{
		UpdateCardList();
	}

	private void OnEnhancementsUnlocked()
	{
		Button.SetVisible(true);
		_exclamationMark.SetActive(true);
	}
}