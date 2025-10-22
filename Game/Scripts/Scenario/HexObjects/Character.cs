using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using Newtonsoft.Json;

public partial class Character : Figure
{
	private Sprite2D _staticSprite;
	private AnimatedSpriteSheet2D _animatedSprite;

	private AMDCardDeck _amdCardDeck;

	public SavedCharacter SavedCharacter { get; private set; }
	public ClassModel ClassModel { get; private set; }
	public int Index { get; private set; }

	public int PlayableAbilityCardCount { get; private set; }

	public List<AbilityCard> Cards { get; } = new List<AbilityCard>();
	public List<ItemModel> Items { get; } = new List<ItemModel>();

	public List<AbilityCard> RoundCards { get; } = new List<AbilityCard>();
	public bool LongResting { get; private set; }

	public int ShortRestSeed { get; private set; }

	public List<Summon> Summons { get; } = new List<Summon>();

	public int ObtainedCoins { get; private set; }
	public int ObtainedXP { get; private set; }

	public bool IsLocal => true;

	public override string DisplayName => SavedCharacter.Name;
	public override string DebugName => SavedCharacter.ClassModel.Name;

	public Texture2D PortraitTexture => ClassModel.PortraitTexture;

	public override AMDCardDeck AMDCardDeck => _amdCardDeck;

	public event Action<Character> ShortRestedEvent;
	public event Action<Character> CoinsChangedEvent;
	public event Action<Character> XPChangedEvent;
	public event Action<Character, AbilityCard> CardStateChangedEvent;
	public event Action<Character, AbilityCard> CardAddedEvent;
	public event Action<Character, AbilityCard> CardRemovedEvent;

	public override void _Ready()
	{
		base._Ready();

		_staticSprite = GetNode<Sprite2D>("Mask/Sprite2D");
		_animatedSprite = GetNode<AnimatedSpriteSheet2D>("Mask/AnimatedSpriteSheet2D");
	}

	public virtual void Spawn(SavedCharacter savedCharacter, int index)
	{
		SavedCharacter = savedCharacter;
		ClassModel = SavedCharacter.ClassModel;
		Index = index;

		int health = ClassModel.MaxHealthValues.Values[SavedCharacter.Level - 1];

		SetMaxHealth(health);
		SetHealth(health);

		SetAlignment(Alignment.Characters);
		SetEnemies(Alignment.Enemies);

		// Create AMD
		List<AMDCard> amdCards = AMDCardDeck.GetDefaultDeckCards($"res://Art/AMDs/Player{index + 1}AMD.jpg");
		_amdCardDeck = new AMDCardDeck(amdCards, true);

		PlayableAbilityCardCount = 2;

		_figureViewComponent.TurnStartPS.SelfModulate = _figureViewComponent.Outline.SelfModulate;
		_figureViewComponent.ActivePS.Modulate = _figureViewComponent.Outline.SelfModulate;

		GameController.Instance.Map.RegisterFigure(this);

		AppController.Instance.Options.AnimatedCharacters.ValueChangedEvent += OnAnimatedCharactersChanged;

		OnAnimatedCharactersChanged(AppController.Instance.Options.AnimatedCharacters.Value);
	}

	public override async GDTask Destroy(bool immediately = false, bool forceDestroy = false)
	{
		await base.Destroy(immediately, forceDestroy);

		if(GameController.Instance.CharacterManager.Characters.All(character => character.IsDestroyed))
		{
			await AbilityCmd.Lose();
		}

		for(int index = Summons.Count - 1; index >= 0; index--)
		{
			Summon summon = Summons[index];
			await summon.Destroy(immediately, forceDestroy);
		}

		// Make sure items can no longer trigger by setting their owner to null
		for(int i = Items.Count - 1; i >= 0; i--)
		{
			ItemModel item = Items[i];
			item.SetOwner(null);
		}

		for(int i = Cards.Count - 1; i >= 0; i--)
		{
			AbilityCard card = Cards[i];
			await card.RemoveFromActive();
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		AppController.Instance.Options.AnimatedCharacters.ValueChangedEvent -= OnAnimatedCharactersChanged;
	}

	public void OnRoundCardsChanged()
	{
		UpdateInitiative();
	}

	public void GainXP(int amount)
	{
		ObtainedXP += amount;
		XPChangedEvent?.Invoke(this);
	}

	public override void AddCoin()
	{
		base.AddCoin();

		ObtainedCoins++;
		CoinsChangedEvent?.Invoke(this);
	}

	public override void RemoveCoin()
	{
		base.RemoveCoin();

		ObtainedCoins--;
		CoinsChangedEvent?.Invoke(this);
	}

	public override void UpdateInitiative()
	{
		base.UpdateInitiative();

		foreach(Summon summon in Summons)
		{
			summon.UpdateInitiative();
		}
	}

	protected override Initiative GetInitiative()
	{
		if(LongResting)
		{
			return new Initiative()
			{
				MainInitiative = 99,
				SortingInitiative = 99 * 10000000 + 99999
			};
		}

		if(RoundCards.Count == 0)
		{
			return new Initiative()
			{
				Null = true,
				SortingInitiative = Index
			};
		}

		int primaryInitiative = RoundCards[0].Model.Initiative;
		int secondaryInitiative = RoundCards.Count > 1 ? RoundCards[1].Model.Initiative : 99;

		return new Initiative()
		{
			MainInitiative = primaryInitiative,
			SortingInitiative = primaryInitiative * 10000000 + secondaryInitiative * 10000 + Index * 100
		};
	}

	public void SetLongResting(bool longResting)
	{
		if(longResting == LongResting)
		{
			return;
		}

		LongResting = longResting;

		if(LongResting)
		{
			RoundCards.Clear();
		}
	}

	public void SetShortRestSeed(int seed)
	{
		ShortRestSeed = seed;
	}

	public void AddCard(AbilityCard abilityCard)
	{
		Cards.Add(abilityCard);
		abilityCard.SetOwner(this);

		abilityCard.CardStateChangedEvent += OnCardStateChanged;

		CardAddedEvent?.Invoke(this, abilityCard);
	}

	public void RemoveCard(AbilityCard abilityCard)
	{
		Cards.Remove(abilityCard);
		abilityCard.SetOwner(null);

		abilityCard.CardStateChangedEvent -= OnCardStateChanged;

		CardRemovedEvent?.Invoke(this, abilityCard);
	}

	public void AddItem(ItemModel item)
	{
		item.AssertMutable();

		Items.Add(item);
		item.SetOwner(this);
	}

	public void RemoveItem(ItemModel item)
	{
		item.AssertMutable();

		Items.Remove(item);
		item.SetOwner(null);
	}

	public void RegisterSummon(Summon summon)
	{
		Summons.Add(summon);
		summon.SetSummonIndex(Summons.Count - 1);
	}

	public void DeregisterSummon(Summon summon)
	{
		Summons.Remove(summon);

		for(int i = 0; i < Summons.Count; i++)
		{
			Summon otherSummon = Summons[i];
			otherSummon.SetSummonIndex(i);
		}
	}

	protected override async GDTask TakeTurn()
	{
		await base.TakeTurn();

		if(LongResting)
		{
			await LongRest();
		}
		else
		{
			bool topPlayed = false;
			bool bottomPlayed = false;
			List<CardPlayCardData> cardDatas = new List<CardPlayCardData>();

			foreach(AbilityCard card in RoundCards)
			{
				cardDatas.Add(new CardPlayCardData()
				{
					AbilityCard = card,
					CanPlayTop = true,
					CanPlayBottom = true
				});
			}

			for(int i = 0; i < cardDatas.Count; i++)
			{
				if(IsDead)
				{
					break;
				}

				EffectCollection cardSideSelectionEffectCollection =
					ScenarioEvents.CardSideSelectionEvent.CreateEffectCollection(new ScenarioEvents.CardSideSelection.Parameters(this));

				AbilityCardSectionSelectionPrompt.Answer cardSectionAnswer = await PromptManager.Prompt(
					new AbilityCardSectionSelectionPrompt(cardDatas, cardSideSelectionEffectCollection, () => "Select card side to play"), this);

				AbilityCard card = GameController.Instance.ReferenceManager.Get<AbilityCard>(cardSectionAnswer.CardReferenceId);
				AbilityCardSection section = cardSectionAnswer.AbilityCardSection;

				if(!GameController.FastForward)
				{
					Log.Write($"Playing {card.Model.Name} {section}.");
				}

				switch(section)
				{
					case AbilityCardSection.Top:
						topPlayed = true;
						await card.Top.Perform(this);
						break;
					case AbilityCardSection.Bottom:
						bottomPlayed = true;
						await card.Bottom.Perform(this);
						break;
					case AbilityCardSection.BasicTop:
						await card.BasicTop.Perform(this);
						topPlayed = true;
						break;
					case AbilityCardSection.BasicBottom:
						await card.BasicBottom.Perform(this);
						bottomPlayed = true;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				foreach(CardPlayCardData cardData in cardDatas)
				{
					if(cardData.AbilityCard == card)
					{
						cardData.CanPlayTop = false;
						cardData.CanPlayBottom = false;
					}
				}

				if(i == cardDatas.Count - 2)
				{
					// Only one card left, make sure both a top and bottom are played

					if(!topPlayed)
					{
						foreach(CardPlayCardData cardData in cardDatas)
						{
							cardData.CanPlayBottom = false;
						}
					}

					if(!bottomPlayed)
					{
						foreach(CardPlayCardData cardData in cardDatas)
						{
							cardData.CanPlayTop = false;
						}
					}
				}
			}

			if(!IsDead)
			{
				await ScenarioEvents.AfterCardsPlayedEvent.CreatePrompt(new ScenarioEvents.AfterCardsPlayed.Parameters(this), this, "End turn?");
			}
		}
	}

	protected override async GDTask EndOfTurnLooting()
	{
		await base.EndOfTurnLooting();

		await AbilityCmd.LootHex(this, Hex);
	}

	private async GDTask LongRest()
	{
		EffectCollection cardSelectionEffectCollection =
			ScenarioEvents.LongRestCardSelectionEvent.CreateEffectCollection(new ScenarioEvents.LongRestCardSelection.Parameters(this));

		AbilityCard cardToLose = await AbilityCmd.SelectAbilityCard(this, CardState.Discarded, true, null, cardSelectionEffectCollection,
			"Select a card to lose for your long rest");

		if(cardToLose != null)
		{
			await AbilityCmd.LoseCard(cardToLose);
		}

		foreach(AbilityCard card in Cards)
		{
			if(card.CardState == CardState.Discarded)
			{
				await AbilityCmd.ReturnToHand(card);
			}
		}

		foreach(ItemModel item in Items)
		{
			if(item.ItemUseType == ItemUseType.Spend)
			{
				await AbilityCmd.RefreshItem(item);
			}
		}

		ActionState actionState = new ActionState(this,
		[
			HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.WithCanPerformWhileStunned(true)
				.Build()
		]);
		await actionState.Perform();
	}

	public async GDTask ShortRest()
	{
		ScenarioEvents.ShortRestStarted.Parameters shortRestParameters =
			await ScenarioEvents.ShortRestStartedEvent.CreatePrompt(
				new ScenarioEvents.ShortRestStarted.Parameters(this), this);

		AbilityCard lostCard = null;

		if(shortRestParameters.CanSelectCardToLose)
		{
			lostCard = await AbilityCmd.SelectAbilityCard(this, CardState.Discarded, mandatory: true, hintText: "Select a card to lose");
		}
		else
		{
			ShortRestPrompt.Answer shortRestAnswer =
				await PromptManager.Prompt(new ShortRestPrompt(this, true, null, () => "Lose this card for your Short Rest?"), this);

			if(shortRestAnswer.Redraw)
			{
				await AbilityCmd.SufferDamage(null, this, 1);

				AbilityCard cardRedrawnFor = GameController.Instance.ReferenceManager.Get<AbilityCard>(shortRestAnswer.AbilityCardReferenceId);
				await AbilityCmd.ReturnToHand(cardRedrawnFor);

				ShortRestPrompt.Answer redrawAnswer =
					await PromptManager.Prompt(new ShortRestPrompt(this, false, null, () => "Confirm Short Rest"), this);

				lostCard = GameController.Instance.ReferenceManager.Get<AbilityCard>(redrawAnswer.AbilityCardReferenceId);
			}
			else
			{
				lostCard = GameController.Instance.ReferenceManager.Get<AbilityCard>(shortRestAnswer.AbilityCardReferenceId);
			}
		}

		await AbilityCmd.LoseCard(lostCard);

		foreach(AbilityCard card in Cards)
		{
			if(card.CardState == CardState.Discarded)
			{
				await AbilityCmd.ReturnToHand(card);
			}
		}

		RoundCards.Clear();
		//OnRoundCardsChanged();

		ShortRestedEvent?.Invoke(this);

		// Exhaust if this character does not have enough cards left
		int playableCardCount = Cards.Count(card => card.CardState == CardState.Hand);
		int discardedCardCount = Cards.Count(card => card.CardState == CardState.Discarded);

		if(playableCardCount < 2 && discardedCardCount < 2)
		{
			await AbilityCmd.KillOrExhaust(null, this);
		}
	}

	private async GDTask LoseCardToCancelDamage(ScenarioEvents.SufferDamage.Parameters parameters)
	{
		AbilityCard card = await AbilityCmd.SelectAbilityCard(this, CardState.Hand, true, card => card.OriginalOwner == this,
			hintText: "Select a card to lose");
		await AbilityCmd.LoseCard(card);

		parameters.SetDamagePrevented();
	}

	private async GDTask LoseDiscardedCardsToCancelDamage(ScenarioEvents.SufferDamage.Parameters parameters)
	{
		foreach(AbilityCard card in await AbilityCmd.SelectAbilityCards(this, CardState.Discarded, 2, 2,
			        card => card.OriginalOwner == this, hintText: "Select two discarded cards to lose"))
		{
			await AbilityCmd.LoseCard(card);
		}

		parameters.SetDamagePrevented();
	}

	private void OnCardStateChanged(AbilityCard card)
	{
		CardStateChangedEvent?.Invoke(this, card);
	}

	public virtual async GDTask OnScenarioSetupCompleted()
	{
		// Copy over all ability cards from the character
		foreach(int handAbilityCardIndex in SavedCharacter.HandAbilityCardIndices)
		{
			SavedAbilityCard savedAbilityCard = SavedCharacter.AvailableAbilityCards[handAbilityCardIndex];
			AbilityCard abilityCard = new AbilityCard(savedAbilityCard, this);
			AddCard(abilityCard);
		}

		// Add and initialize all equipped items
		foreach(string baseSlotItem in SavedCharacter.EquippedBaseSlotItems)
		{
			if(baseSlotItem == null)
			{
				continue;
			}

			ItemModel item = ModelDB.GetById<ItemModel>(baseSlotItem).ToMutable();
			item.Init(this);
			Items.Add(item);
		}

		foreach(string smallItem in SavedCharacter.EquippedSmallItems)
		{
			if(smallItem == null)
			{
				continue;
			}

			ItemModel item = ModelDB.GetById<ItemModel>(smallItem).ToMutable();
			item.Init(this);
			Items.Add(item);
		}

		foreach(ItemModel item in Items)
		{
			//TODO: Check for perk that ignores -1 cards
			for(int i = 0; i < item.MinusOneCount; i++)
			{
				AMDCardDeck.AddMinusOne();
			}

			//item.SetupForScenario();
		}

		object loseHandCardToCancelDamageSubscriber = new object();
		ScenarioEvents.SufferDamageEvent.Subscribe(this, loseHandCardToCancelDamageSubscriber,
			parameters => parameters.Figure == this && parameters.WouldSufferDamage &&
			              Cards.Any(card => card.CardState == CardState.Hand && card.OriginalOwner == this),
			LoseCardToCancelDamage, EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.LoseCard),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Lose a card from hand to negate the damage"));

		object loseDiscardedCardsToCancelDamageSubscriber = new object();
		ScenarioEvents.SufferDamageEvent.Subscribe(this, loseDiscardedCardsToCancelDamageSubscriber,
			parameters => parameters.Figure == this && parameters.WouldSufferDamage &&
			              Cards.Count(card => card.CardState == CardState.Discarded && card.OriginalOwner == this) >= 2,
			LoseDiscardedCardsToCancelDamage, EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(Icons.LoseDiscardedCards),
			effectInfoViewParameters: new TextEffectInfoView.Parameters("Lose two cards from your discard pile to negate the damage"));

		await GDTask.CompletedTask;
	}

	private void OnAnimatedCharactersChanged(bool animatedCharacters)
	{
		_staticSprite.SetVisible(!ClassModel.HasAnimatedSprite || !animatedCharacters);
		_animatedSprite.SetVisible(ClassModel.HasAnimatedSprite && animatedCharacters);
	}

	public override void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
		base.AddInfoItemParameters(parametersList);

		parametersList.Add(new CharacterInfoItem.Parameters(this));
	}
}