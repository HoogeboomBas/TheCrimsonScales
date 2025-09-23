using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;

public static class AbilityCmd
{
	public static async GDTask DiscardCard(AbilityCard card)
	{
		await card.RemoveFromActive();

		await card.SetCardState(CardState.Discarded);
	}

	public static async GDTask LoseCard(AbilityCard card)
	{
		await card.RemoveFromActive();

		await card.SetCardState(CardState.Lost);
	}

	public static async GDTask DiscardOrLose(AbilityCard card)
	{
		if(card.CardState == CardState.Round || card.CardState == CardState.Persistent)
		{
			await DiscardCard(card);
		}

		if(card.CardState == CardState.RoundLoss || card.CardState == CardState.PersistentLoss)
		{
			await LoseCard(card);
		}
	}

	public static async GDTask ReturnToHand(AbilityCard card)
	{
		await card.SetCardState(CardState.Hand);
	}

	public static OtherActiveAbility AllOpposingAttacksGainDisadvantageActiveAbility()
	{
		object subscriber = new object();

		return OtherActiveAbility.Builder()
			.WithOnActivate(state =>
			{
				ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, subscriber,
					parameters => parameters.AbilityState.Target == state.Performer,
					async parameters =>
					{
						parameters.AbilityState.SingleTargetSetHasDisadvantage();

						await GDTask.CompletedTask;
					}
				);

				ScenarioCheckEvents.DisadvantageCheckEvent.Subscribe(state, subscriber,
					parameters => parameters.Target == state.Performer,
					parameters => parameters.SetDisadvantage()
				);

				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(state, subscriber,
					parameters => state.Performer == parameters.Figure,
					parameters => parameters.Add(
						new FigureInfoTextExtraEffect.Parameters("All attacks targeting this figure this round gain disadvantage."))
				);

				return GDTask.CompletedTask;
			})
			.WithOnDeactivate(state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, subscriber);
					ScenarioCheckEvents.DisadvantageCheckEvent.Unsubscribe(state, subscriber);
					ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, subscriber);

					return GDTask.CompletedTask;
				}
			)
			.Build();
	}

	public static async GDTask<int> SufferDamage(AttackAbility.State potentialAttackAbilityState, Figure target, int damage)
	{
		ScenarioEvents.SufferDamage.Parameters sufferDamageParameters =
			new ScenarioEvents.SufferDamage.Parameters(potentialAttackAbilityState, target, damage);
		EffectCollection sufferDamageCollection = ScenarioEvents.SufferDamageEvent.CreateEffectCollection(sufferDamageParameters);
		await PromptManager.Prompt(new SufferDamagePrompt(sufferDamageParameters, sufferDamageCollection,
			() => $"Suffer {Icons.HintText(Icons.Damage)}{sufferDamageParameters.CalculatedCurrentDamage}?"), target);

		potentialAttackAbilityState?.SetPerformed();

		if(sufferDamageParameters.DamagePrevented)
		{
			return 0;
		}

		int finalDamage = sufferDamageParameters.CalculatedCurrentDamage;

		ScenarioEvents.JustBeforeSufferDamage.Parameters justBeforeSufferDamageParameters =
			await ScenarioEvents.JustBeforeSufferDamageEvent.CreatePrompt(
				new ScenarioEvents.JustBeforeSufferDamage.Parameters(target, finalDamage, potentialAttackAbilityState, sufferDamageParameters),
				potentialAttackAbilityState?.Authority ?? target);

		if(justBeforeSufferDamageParameters.Prevented)
		{
			return 0;
		}

		int newHealth = Mathf.Max(target.Health - finalDamage, 0);

		target.SetHealth(newHealth);

		if(newHealth == 0)
		{
			await KillOrExhaust(potentialAttackAbilityState, target);
		}

		if (finalDamage > 0)
		{
			await ScenarioEvents.AfterSufferDamageEvent.CreatePrompt(
				new ScenarioEvents.AfterSufferDamage.Parameters(target, finalDamage, potentialAttackAbilityState, sufferDamageParameters),
				potentialAttackAbilityState?.Authority ?? target);
		}

		return finalDamage;
	}

	public static async GDTask KillOrExhaust(AbilityState state, Figure target)
	{
		state?.SetPerformed();

		await target.Destroy();

		ScenarioEvents.FigureKilled.Parameters parameters =
			await ScenarioEvents.FigureKilledEvent.CreatePrompt(
				new ScenarioEvents.FigureKilled.Parameters(state, target), state?.Authority ?? target);
	}

	public static GDTask AddCondition(AbilityState potentialAbilityState, Figure target, ConditionModel conditionModel)
	{
		return AddConditions(potentialAbilityState, target, [conditionModel]);
	}

	public static async GDTask AddConditions(AbilityState potentialAbilityState, Figure target, List<ConditionModel> conditionModels)
	{
		ScenarioEvents.InflictConditions.Parameters inflictConditionsParameters =
			await ScenarioEvents.InflictConditionsEvent.CreatePrompt(
				new ScenarioEvents.InflictConditions.Parameters(potentialAbilityState, target, conditionModels), target);

		foreach(ConditionModel conditionModel in inflictConditionsParameters.ConditionModels)
		{
			ConditionModel condition = conditionModel.ToMutable();

			ScenarioEvents.InflictCondition.Parameters inflictConditionParameters =
				await ScenarioEvents.InflictConditionEvent.CreatePrompt(
					new ScenarioEvents.InflictCondition.Parameters(potentialAbilityState, target, condition),
					potentialAbilityState?.Authority ?? target);

			if(!inflictConditionParameters.Prevented)
			{
				ScenarioEvents.InflictConditionDuplicatesCheck.Parameters inflictConditionDuplicatesCheckParameters =
					await ScenarioEvents.InflictConditionDuplicatesCheckEvent.CreatePrompt(
						new ScenarioEvents.InflictConditionDuplicatesCheck.Parameters(potentialAbilityState, target, condition),
						potentialAbilityState?.Authority ?? target);

				if(!inflictConditionDuplicatesCheckParameters.Prevented)
				{
					await target.AddCondition(condition);
				}
			}
		}

		potentialAbilityState?.SetPerformed();
	}

	public static async GDTask<bool> RemoveCondition(Figure target, ConditionModel conditionModel)
	{
		if(conditionModel.IsMutable)
		{
			conditionModel = conditionModel.ImmutableInstance;
		}

		ConditionModel condition = target.GetCondition(conditionModel);
		if(condition != null)
		{
			await target.RemoveCondition(conditionModel);

			return true;
		}

		return false;
	}

	public static async GDTask GainXP(Figure figure, int xp)
	{
		if(figure is Character character)
		{
			character.GainXP(xp);
		}

		await GDTask.CompletedTask;
	}

	public static async GDTask DestroyDifficultTerrain(DifficultTerrain difficultTerrain)
	{
		if(!difficultTerrain.CannotBeDestroyed)
		{
			await difficultTerrain.Destroy();
		}
	}

	public static async GDTask<DifficultTerrain> CreateDifficultTerrain(Hex hex, PackedScene scene)
	{
		return await CreateOverlayTile<DifficultTerrain>(hex, scene);
	}

	public static async GDTask SpawnCoin(Hex hex)
	{
		if(!hex.TryGetHexObjectOfType(out CoinStack coinStack))
		{
			PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scenario/CoinStack.tscn");
			coinStack = scene.Instantiate<CoinStack>();
			GameController.Instance.Map.AddChild(coinStack);
			await coinStack.Init(hex);
		}
		else
		{
			coinStack.AddCoin();
		}

		await GDTask.CompletedTask;
	}

	public static async GDTask LootHex(Figure figure, Hex hex)
	{
		foreach(LootableObject lootableObject in hex.GetHexObjectsOfType<LootableObject>())
		{
			if(lootableObject.CanLoot(figure))
			{
				await lootableObject.Loot(figure);
			}
		}
	}

	public static async GDTask<Monster> SummonMonster(MonsterModel monsterModel, MonsterType monsterType, Hex hex)
	{
		return await GameController.Instance.Map.CreateMonster(monsterModel, monsterType, hex.Coords, true);
	}

	public static async GDTask<Monster> SpawnMonster(MonsterModel monsterModel, MonsterType monsterType, Hex hex)
	{
		return await GameController.Instance.Map.CreateMonster(monsterModel, monsterType, hex.Coords, false);
	}

	public static async GDTask<T> CreateOverlayTile<T>(Hex hex, PackedScene scene)
		where T : HexObject
	{
		if(!hex.IsFeatureless())
		{
			Log.Error("Trying to create an overlay tile in a hex that already has a feature!");
			return null;
		}

		HexObject hexObject = scene.Instantiate<HexObject>();
		GameController.Instance.Map.AddChild(hexObject);
		await hexObject.Init(hex);

		hexObject.Scale = Vector2.Zero;
		hexObject.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardable();

		await GDTask.CompletedTask;

		return (T)hexObject;
	}

	public static async GDTask<Trap> CreateTrap(Hex hex, string assetPath, int damage = 0, ConditionModel[] conditions = null)
	{
		PackedScene scene = ResourceLoader.Load<PackedScene>(assetPath);
		Trap trap = scene.Instantiate<Trap>();
		GameController.Instance.Map.AddChild(trap);
		trap.SetTrapValues(damage, conditions ?? []);
		await trap.Init(hex);

		trap.Scale = Vector2.Zero;
		trap.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardable();

		await GDTask.CompletedTask;

		return trap;
	}

	public static GDTask<List<Hex>> SelectHexes(AbilityState state, Action<List<Hex>> getValidHexes, int minSelectionCount, int maxSelectionCount,
		bool autoSelectIfMaxCountIsValidCount, string hintText)
	{
		return SelectHexes(state.Authority, getValidHexes, minSelectionCount, maxSelectionCount, autoSelectIfMaxCountIsValidCount, hintText);
	}

	public static async GDTask<List<Hex>> SelectHexes(Figure authority, Action<List<Hex>> getValidHexes, int minSelectionCount, int maxSelectionCount,
		bool autoSelectIfMaxCountIsValidCount, string hintText)
	{
		HexSelectionPrompt.Answer answer = await PromptManager.Prompt(
			new HexSelectionPrompt(getValidHexes, autoSelectIfMaxCountIsValidCount, null, () => hintText, minSelectionCount, maxSelectionCount),
			authority);

		return answer.Skipped ? [] : answer.CoordSets.Select(coords => GameController.Instance.Map.GetHex(coords)).ToList();
	}

	public static GDTask<Hex> SelectHex(AbilityState state, Action<List<Hex>> getValidHexes, bool mandatory = false, string hintText = "Select a hex")
	{
		return SelectHex(state.Authority, getValidHexes, mandatory, hintText);
	}

	public static async GDTask<Hex> SelectHex(Figure authority, Action<List<Hex>> getValidHexes, bool mandatory = false,
		string hintText = "Select a hex")
	{
		HexSelectionPrompt.Answer answer = await PromptManager.Prompt(
			new HexSelectionPrompt(getValidHexes, false, null, () => hintText, mandatory ? 1 : 0, 1), authority);

		return answer.Skipped ? null : answer.CoordSets.Select(coords => GameController.Instance.Map.GetHex(coords)).FirstOrDefault();
	}

	public static GDTask<Figure> SelectFigure(AbilityState state, Action<List<Figure>> getValidTargets, bool mandatory = false,
		bool autoSelectIfOne = true, string hintText = "Select a target")
	{
		return SelectFigure(state.Authority, getValidTargets, mandatory, autoSelectIfOne, hintText);
	}

	public static async GDTask<Figure> SelectFigure(Figure authority, Action<List<Figure>> getValidTargets, bool mandatory = false,
		bool autoSelectIfOne = true, string hintText = "Select a target")
	{
		TargetSelectionPrompt.Answer targetAnswer = await PromptManager.Prompt(
			new TargetSelectionPrompt(getValidTargets, autoSelectIfOne, mandatory, null, () => hintText), authority);

		if(targetAnswer.Skipped)
		{
			return null;
		}

		return GameController.Instance.ReferenceManager.Get<Figure>(targetAnswer.FigureReferenceId);
	}

	public static async GDTask<AbilityCard> SelectAbilityCard(Character character, CardState? requiredCardState, bool mandatory = false,
		Func<AbilityCard, bool> canSelectFunc = null, EffectCollection effectCollection = null, string hintText = "Select a card")
	{
		return (await SelectAbilityCards(character, requiredCardState, mandatory ? 1 : 0, 1, canSelectFunc, effectCollection, hintText))
			.FirstOrDefault();
	}

	public static async GDTask<AbilityCard> SelectAbilityCard(Figure authority, Action<List<AbilityCard>> getAllCards, CardState? requiredCardState,
		bool mandatory = false, EffectCollection effectCollection = null, string hintText = "Select a card")
	{
		CardSelectionPrompt.Answer answer = await PromptManager.Prompt(new CardSelectionPrompt(getAllCards,
			requiredCardState, mandatory ? 1 : 0, 1, effectCollection, () => hintText), authority);

		return answer.CardReferenceIds == null || answer.CardReferenceIds.Count == 0 ? null
			: GameController.Instance.ReferenceManager.Get<AbilityCard>(answer.CardReferenceIds[0]);
	}

	public static async GDTask<List<AbilityCard>> SelectAbilityCards(Character character, CardState? requiredCardState,
		int minSelectionCount, int maxSelectionCount, Func<AbilityCard, bool> canSelectFunc = null, EffectCollection effectCollection = null,
		string hintText = "Select cards")
	{
		CardSelectionPrompt.Answer answer = await PromptManager.Prompt(new CardSelectionPrompt(
			cards =>
			{
				foreach(AbilityCard abilityCard in character.Cards)
				{
					if(canSelectFunc != null && !canSelectFunc(abilityCard))
					{
						continue;
					}

					cards.Add(abilityCard);
				}
			},
			requiredCardState, minSelectionCount, maxSelectionCount, effectCollection, () => hintText), character);

		return answer.CardReferenceIds == null ? [] : answer.CardReferenceIds
			.Select(referenceId => GameController.Instance.ReferenceManager.Get<AbilityCard>(referenceId)).ToList();
	}

	public static async GDTask EnterHex(AbilityState state, Figure figure, Figure authority, Hex hex, bool triggerHexEffects)
	{
		figure.SetOriginHexAndRotation(hex);

		await ScenarioEvents.FigureEnteredHexEvent.CreatePrompt(new ScenarioEvents.FigureEnteredHex.Parameters(state, figure), authority);

		HazardousTerrain hazardousTerrain = hex.GetHexObjectOfType<HazardousTerrain>();
		if(hazardousTerrain != null && triggerHexEffects)
		{
			ScenarioCheckEvents.FlyingCheck.Parameters flyingCheckParameters =
				ScenarioCheckEvents.FlyingCheckEvent.Fire(new ScenarioCheckEvents.FlyingCheck.Parameters(figure));

			if(!flyingCheckParameters.HasFlying)
			{
				ScenarioEvents.HazardousTerrainTriggered.Parameters hazardousTerrainParameters =
					await ScenarioEvents.HazardousTerrainTriggeredEvent.CreatePrompt(
						new ScenarioEvents.HazardousTerrainTriggered.Parameters(state, hex, hazardousTerrain, true), authority);
				if(hazardousTerrainParameters.AffectedByHazardousTerrain)
				{
					int damage = HazardousTerrain.DamageAmount;
					await SufferDamage(null, figure, damage);
				}
			}
		}

		Trap trap = hex.GetHexObjectOfType<Trap>();
		if(trap != null && triggerHexEffects)
		{
			ScenarioCheckEvents.FlyingCheck.Parameters flyingCheckParameters =
				ScenarioCheckEvents.FlyingCheckEvent.Fire(new ScenarioCheckEvents.FlyingCheck.Parameters(figure));

			if(!flyingCheckParameters.HasFlying)
			{
				ScenarioEvents.TrapTriggered.Parameters trapTriggeredParameters =
					await ScenarioEvents.TrapTriggeredEvent.CreatePrompt(
						new ScenarioEvents.TrapTriggered.Parameters(state, hex, trap, figure, true), authority);
				if(trapTriggeredParameters.TriggersTrap)
				{
					await trap.Trigger(state, figure);
				}
			}
		}
	}

	public static async GDTask<bool> HasPerformedAbility(AbilityState abilityState, int abilityIndex)
	{
		await GDTask.CompletedTask;

		AbilityState otherAbilityState = abilityState.ActionState.GetAbilityState<AbilityState>(abilityIndex);

		return otherAbilityState.Performed;
	}

	public static async GDTask GenericChoice(Figure authority, IEnumerable<ScenarioEvents.GenericChoice.Subscription> subscriptions,
		bool canSelectMultiple = false, string hintText = "Make a selection")
	{
		object subscriber = new object();
		foreach(ScenarioEvents.GenericChoice.Subscription subscription in subscriptions)
		{
			ScenarioEvents.GenericChoice.CanApplyFunction oldCanApplyFunction = subscription.CanApplyFunction;
			ScenarioEvents.GenericChoice.CanApplyFunction newCanApplyFunction = parameters =>
			{
				return
					(canSelectMultiple || !parameters.ChoiceMade) &&
					(oldCanApplyFunction == null || oldCanApplyFunction.Invoke(parameters));
			};

			ScenarioEvents.GenericChoice.ApplyFunction oldApplyFunction = subscription.ApplyFunction;
			ScenarioEvents.GenericChoice.ApplyFunction newApplyFunction = async parameters =>
			{
				if(oldApplyFunction != null)
				{
					await oldApplyFunction.Invoke(parameters);
				}

				parameters.SetChoiceMade();
			};

			ScenarioEvents.GenericChoice.Subscription newSubscription =
				ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
					newCanApplyFunction, newApplyFunction, subscription.EffectType, subscription.Order,
					subscription.CanApplyMultipleTimesDuringSubscription, subscription.CanApplyMultipleTimesInEffectCollection,
					subscription.EffectButtonParameters, subscription.EffectInfoViewParameters);

			ScenarioEvents.GenericChoiceEvent.Subscribe(authority, subscriber, newSubscription, false);
		}

		await ScenarioEvents.GenericChoiceEvent.CreatePrompt(new ScenarioEvents.GenericChoice.Parameters(), authority, hintText);

		ScenarioEvents.GenericChoiceEvent.ClearAllSubscriptions();
	}

	public static GDTask InfuseWildElement(Figure authority)
	{
		return InfuseElement(authority, Elements.All);
	}

	public static GDTask InfuseElement(Figure authority, IReadOnlyCollection<Element> possibleElements)
	{
		List<ScenarioEvents.GenericChoice.Subscription> subscriptions = new List<ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription>();
		foreach(Element possibleElement in possibleElements)
		{
			subscriptions.Add(ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
				applyFunction: async parameters =>
				{
					await InfuseElement(possibleElement);
				},
				effectType: EffectType.SelectableMandatory,
				effectButtonParameters: new IconEffectButton.Parameters(Icons.GetElement(possibleElement)),
				effectInfoViewParameters: new TextEffectInfoView.Parameters($"Infuse {possibleElement}")
			));
		}

		return GenericChoice(authority, subscriptions);
	}

	public static async GDTask InfuseElement(Element element, bool immediately = false)
	{
		if(immediately)
		{
			GameController.Instance.ElementManager.InfuseImmediately(element);
		}
		else
		{
			GameController.Instance.ElementManager.StartInfuse(element);
		}

		await GDTask.CompletedTask;
	}

	public static GDTask<Element?> AskConsumeWildElement(Figure authority, bool mandatory = false)
	{
		return AskConsumeElement(authority, Elements.All, mandatory);
	}

	public static async GDTask<Element?> AskConsumeElement(Figure authority, IReadOnlyCollection<Element> possibleElements, bool mandatory = false)
	{
		object subscriber = new object();

		foreach(Element element in possibleElements)
		{
			Element possibleElement = element;
			ScenarioEvents.ConsumeElementElement.Subscribe(authority, subscriber,
				canApplyParameters =>
					!canApplyParameters.Consumed && canApplyParameters.Elements.Contains(possibleElement) &&
					GameController.Instance.ElementManager.GetState(possibleElement) > ElementState.Inert,
				async applyParameters =>
				{
					applyParameters.SetConsumed(possibleElement);
					await TryConsumeElement(possibleElement);
				},
				mandatory ? EffectType.SelectableMandatory : EffectType.Selectable, 0, false, false,
				new ConsumeElementEffectButton.Parameters(possibleElement),
				new TextEffectInfoView.Parameters($"Consume {Icons.Inline(Icons.GetElement(possibleElement))}"), checkDuplicates: false);
		}

		ScenarioEvents.ConsumeElement.Parameters consumeEventParameters =
			await ScenarioEvents.ConsumeElementElement.CreatePrompt(new ScenarioEvents.ConsumeElement.Parameters(possibleElements), authority,
				"Select element to consume");

		ScenarioEvents.ConsumeElementElement.Unsubscribe(authority, subscriber);

		return consumeEventParameters.Consumed ? consumeEventParameters.ConsumedElement : null;
	}

	public static async GDTask<bool> AskConsumeElement(Figure authority, Element element, string effectInfoText = null, string hintText = null)
	{
		object subscriber = new object();
		ScenarioEvents.ConsumeElementElement.Subscribe(authority, subscriber,
			canApplyParameters => canApplyParameters.Elements.Contains(element) &&
			                      GameController.Instance.ElementManager.GetState(element) > ElementState.Inert,
			async applyParameters =>
			{
				applyParameters.SetConsumed(element);
				await TryConsumeElement(element);
			}, EffectType.Selectable, 0, false, false,
			new ConsumeElementEffectButton.Parameters(element),
			new TextEffectInfoView.Parameters(effectInfoText ?? $"Consume {Icons.Inline(Icons.GetElement(element))}"));

		ScenarioEvents.ConsumeElement.Parameters consumeEventParameters =
			await ScenarioEvents.ConsumeElementElement.CreatePrompt(
				new ScenarioEvents.ConsumeElement.Parameters([element]), authority,
				hintText ?? $"Consume {Icons.HintText(Icons.GetElement(element))}?");

		ScenarioEvents.ConsumeElementElement.Unsubscribe(authority, subscriber);

		return consumeEventParameters.Consumed;
	}

	public static async GDTask<bool> TryConsumeElement(Element element)
	{
		if(GameController.Instance.ElementManager.GetState(element) == ElementState.Inert)
		{
			return false;
		}

		GameController.Instance.ElementManager.Consume(element);

		await GDTask.CompletedTask;

		return true;
	}

	public static async GDTask MoveElementToWaning(Element element)
	{
		GameController.Instance.ElementManager.SetState(element, ElementState.Waning);

		await GDTask.CompletedTask;
	}

	public static async GDTask<ItemModel> SelectItem(Character characterAndAuthority, ItemState requiredItemState, ItemType? requiredItemType = null, string hintText = "Select an item")
	{
		List<ScenarioEvents.GenericChoice.Subscription> subscriptions
			= new List<ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription>();

		ItemModel selectedItem = null;

		foreach(ItemModel item in characterAndAuthority.Items)
		{
			if(item.ItemState != requiredItemState)
			{
				continue;
			}
			
			if(requiredItemType.HasValue && item.ItemType != requiredItemType)
			{
				continue;
			}

			subscriptions.Add(ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
				applyFunction: async parameters =>
				{
					selectedItem = item;

					await GDTask.CompletedTask;
				},
				effectType: EffectType.SelectableMandatory,
				effectButtonParameters: new ItemEffectButton.Parameters(item),
				effectInfoViewParameters: new ItemEffectInfoView.Parameters(item)
			));
		}

		await GenericChoice(characterAndAuthority, subscriptions, hintText: hintText);

		return selectedItem;
	}

	public static async GDTask<ItemModel> SelectItem(Figure authority, List<ItemModel> items, string hintText = "Select an item")
	{
		List<ScenarioEvents.GenericChoice.Subscription> subscriptions =
			new List<ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription>();

		ItemModel selectedItem = null;

		foreach(ItemModel item in items)
		{
			subscriptions.Add(ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
				applyFunction: async parameters =>
				{
					selectedItem = item;

					await GDTask.CompletedTask;
				},
				effectType: EffectType.SelectableMandatory,
				effectButtonParameters: new ItemEffectButton.Parameters(item),
				effectInfoViewParameters: new ItemEffectInfoView.Parameters(item)
			));
		}

		await GenericChoice(authority, subscriptions, hintText: hintText);

		return selectedItem;
	}

	public static async GDTask RefreshItem(ItemModel item)
	{
		await item.Refresh();
	}

	public static async GDTask<AbilityCardSection> PerformAbilityCardTopOrBottom(Figure performer, AbilityCard abilityCard)
	{
		List<CardPlayCardData> cardDatas = new List<CardPlayCardData>();

		cardDatas.Add(new CardPlayCardData()
		{
			AbilityCard = abilityCard,
			CanPlayTop = true,
			CanPlayBottom = true
		});

		AbilityCardSectionSelectionPrompt.Answer cardSectionAnswer = await PromptManager.Prompt(
			new AbilityCardSectionSelectionPrompt(cardDatas, null, () => "Select card side to play"), performer);

		AbilityCard card = GameController.Instance.ReferenceManager.Get<AbilityCard>(cardSectionAnswer.CardReferenceId);
		AbilityCardSection section = cardSectionAnswer.AbilityCardSection;

		if(!GameController.FastForward)
		{
			Log.Write($"Playing {card.Model.Name} {section}.");
		}

		switch(section)
		{
			case AbilityCardSection.Top:
				await card.Top.Perform(performer);
				break;
			case AbilityCardSection.Bottom:
				await card.Bottom.Perform(performer);
				break;
			case AbilityCardSection.BasicTop:
				await card.BasicTop.Perform(performer);
				break;
			case AbilityCardSection.BasicBottom:
				await card.BasicBottom.Perform(performer);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		return section;
	}

	public static ItemModel GetRandomAvailableOrb()
	{
		return GetRandomAvailableItem(
		[
			ModelDB.Item<OrbOfConfusion>(),
			ModelDB.Item<OrbOfMomentum>(),
			ModelDB.Item<OrbOfAgility>(),
			ModelDB.Item<OrbOfVigor>(),
			ModelDB.Item<OrbOfRetribution>(),
			ModelDB.Item<OrbOfInfection>(),
			ModelDB.Item<OrbOfVitality>(),
			ModelDB.Item<OrbOfProtection>(),
			ModelDB.Item<OrbOfFortune>(),
			ModelDB.Item<OrbOfDespair>(),
		]);
	}

	public static ItemModel GetRandomAvailableStone()
	{
		return GetRandomAvailableItem(
		[
			ModelDB.Item<FrostStone>(),
			ModelDB.Item<StormStone>(),
			ModelDB.Item<InfernoStone>(),
			ModelDB.Item<TremorStone>(),
			ModelDB.Item<BrilliantStone>(),
			ModelDB.Item<DarkStone>(),
			ModelDB.Item<WonderStone>(),
		]);
	}

	private static ItemModel GetRandomAvailableItem(IEnumerable<ItemModel> itemModels)
	{
		List<ItemModel> availableOrbs = new List<ItemModel>();
		foreach(ItemModel orbModel in itemModels)
		{
			SavedItem savedItem = GameController.Instance.SavedCampaign.GetSavedItem(orbModel);
			int unlockedCount = savedItem.UnlockedCount;
			for(int i = 0; i < 2 - unlockedCount; i++)
			{
				availableOrbs.Add(orbModel);
			}
		}

		return availableOrbs.Count == 0 ? null : availableOrbs.PickRandom(GameController.Instance.StateRNG);
	}

	public static GDTask Lose()
	{
		GameController.Instance.MarkScenarioEnded();
		GameController.Instance.ScenarioLostView.Open();
		return GDTask.Never(GameController.CancellationToken);
	}

	public static GDTask Win()
	{
		GameController.Instance.MarkScenarioEnded();
		GameController.Instance.ScenarioWonView.Open();
		return GDTask.Never(GameController.CancellationToken);
	}
}