using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

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

		if(card.Unrecoverable)
		{
			await card.SetCardState(CardState.UnrecoverablyLost);
		}
		else
		{
			await card.SetCardState(CardState.Lost);
		}
	}

	public static async GDTask DiscardOrLose(AbilityCard card)
	{
		if(card.CardState == CardState.Round || card.CardState == CardState.Persistent || card.CardState == CardState.PersistentNoDeactivate)
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
					parameters => parameters.SetDisadvantage(true)
				);

				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(state, subscriber,
					parameters => state.Performer == parameters.Figure,
					parameters => parameters.Add(
						new InfoTextExtraEffect.Parameters("All attacks targeting this figure this round gain disadvantage."))
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

	public static async GDTask<int> SufferDamage(AbilityState potentialAbilityState, Figure target, int damage,
		Figure potentialDamageDealer = null, bool fromAttack = false)
	{
		potentialDamageDealer ??= potentialAbilityState?.Authority;

		ScenarioEvents.SufferDamage.Parameters sufferDamageParameters =
			new ScenarioEvents.SufferDamage.Parameters(potentialAbilityState, target, potentialDamageDealer, damage, fromAttack);
		EffectCollection sufferDamageCollection = ScenarioEvents.SufferDamageEvent.CreateEffectCollection(sufferDamageParameters);
		await PromptManager.Prompt(new SufferDamagePrompt(sufferDamageParameters, sufferDamageCollection,
			() => $"Suffer {Icons.HintText(Icons.Damage)}{sufferDamageParameters.CalculatedCurrentDamage}?"), target);

		if(sufferDamageParameters.DamagePrevented)
		{
			return 0;
		}

		int finalDamage = sufferDamageParameters.CalculatedCurrentDamage;

		ScenarioEvents.JustBeforeSufferDamage.Parameters justBeforeSufferDamageParameters =
			await ScenarioEvents.JustBeforeSufferDamageEvent.CreatePrompt(
				new ScenarioEvents.JustBeforeSufferDamage.Parameters(target, finalDamage, potentialAbilityState, sufferDamageParameters), target);

		if(justBeforeSufferDamageParameters.Prevented)
		{
			return 0;
		}

		potentialAbilityState?.DamagedFigures.Add(target);

		int newHealth = Mathf.Max(target.Health - finalDamage, 0);

		target.SetHealth(newHealth);

		if(newHealth == 0)
		{
			if(potentialAbilityState == null)
			{
				await KillOrExhaust(target, potentialDamageDealer);
			}
			else
			{
				await KillOrExhaust(potentialAbilityState, target);
			}
		}

		if(finalDamage > 0)
		{
			await ScenarioEvents.AfterSufferDamageEvent.CreatePrompt(
				new ScenarioEvents.AfterSufferDamage.Parameters(target, finalDamage, potentialAbilityState, sufferDamageParameters), target);
		}

		return finalDamage;
	}

	public static async GDTask<int> SufferDamage(Figure target, int damage, Figure potentialDamageDealer, bool fromAttack = false)
	{
		return await SufferDamage(null, target, damage, potentialDamageDealer, fromAttack);
	}

	public static async GDTask KillOrExhaust(AbilityState potentialAbilityState, Figure target, Figure potentialKiller)
	{
		await ScenarioEvents.BeforeFigureKilledEvent.CreatePrompt(
			new ScenarioEvents.BeforeFigureKilled.Parameters(potentialAbilityState, target), potentialKiller);
		await target.Destroy();

		await ScenarioEvents.FigureKilledEvent.CreatePrompt(
			new ScenarioEvents.FigureKilled.Parameters(potentialAbilityState, target, potentialKiller), target);
	}

	public static async GDTask KillOrExhaust(Figure target, Figure potentialKiller)
	{
		await KillOrExhaust(null, target, potentialKiller);
	}

	public static async GDTask KillOrExhaust(AbilityState state, Figure target)
	{
		await KillOrExhaust(state, target, state.Authority);
	}

	public static bool CheckImmunity(ConditionModel conditionModel, ConditionModel immunityConditionModel)
	{
		return conditionModel.ImmunityCompareBaseConditions.Contains(immunityConditionModel);
	}

	public static GDTask AddCondition(AbilityState potentialAbilityState, Figure target, ConditionModel conditionModel)
	{
		return AddConditions(potentialAbilityState, target, [conditionModel]);
	}

	public static async GDTask AddConditions(AbilityState potentialAbilityState, Figure target, List<ConditionModel> conditionModels,
		Figure potentialConditionGiver = null)
	{
		potentialConditionGiver ??= potentialAbilityState?.Authority;

		ScenarioEvents.InflictConditions.Parameters inflictConditionsParameters =
			await ScenarioEvents.InflictConditionsEvent.CreatePrompt(
				new ScenarioEvents.InflictConditions.Parameters(potentialAbilityState, target, conditionModels), target);

		foreach(ConditionModel conditionModel in inflictConditionsParameters.ConditionModels)
		{
			ScenarioEvents.InflictCondition.Parameters inflictConditionParameters =
				await ScenarioEvents.InflictConditionEvent.CreatePrompt(
					new ScenarioEvents.InflictCondition.Parameters(potentialAbilityState, target, potentialConditionGiver, conditionModel), target);

			if(!inflictConditionParameters.Prevented)
			{
				ScenarioEvents.InflictConditionDuplicatesCheck.Parameters inflictConditionDuplicatesCheckParameters =
					await ScenarioEvents.InflictConditionDuplicatesCheckEvent.CreatePrompt(
						new ScenarioEvents.InflictConditionDuplicatesCheck.Parameters(potentialAbilityState, target, conditionModel), target);

				if(!inflictConditionDuplicatesCheckParameters.Prevented)
				{
					if(inflictConditionDuplicatesCheckParameters.AddStack)
					{
						await target.AddConditionStack(conditionModel);
					}
					else
					{
						await target.AddCondition(conditionModel, potentialAbilityState?.Performer);
					}
				}
			}
		}

		potentialAbilityState?.SetPerformed();
	}

	public static async GDTask RemoveCondition(Condition condition)
	{
		ScenarioEvents.RemoveCondition.Parameters removeConditionParameters =
			await ScenarioEvents.RemoveConditionEvent.CreatePrompt(
				new ScenarioEvents.RemoveCondition.Parameters(condition), condition.Owner);

		await condition.Owner.RemoveCondition(condition);
	}

	public static async GDTask<bool> RemoveCondition(Figure target, ConditionModel conditionModel)
	{
		Condition condition = target.GetCondition(conditionModel);
		if(condition != null)
		{
			await RemoveCondition(condition);
			ScenarioEvents.AfterRemoveCondition.Parameters afterRemoveConditionParameters =
				await ScenarioEvents.AfterRemoveConditionEvent.CreatePrompt(
					new ScenarioEvents.AfterRemoveCondition.Parameters(target, conditionModel), target);

			return true;
		}

		return false;
	}

	public static async GDTask RemoveOneNegativeCondition(Figure target)
	{
		List<ScenarioEvents.GenericChoice.Subscription> subscriptions =
			new List<ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription>();
		foreach(Condition condition in target.Conditions)
		{
			if(condition.ConditionModel.ConditionPolarity == ConditionPolarity.Negative)
			{
				subscriptions.Add(ScenarioEvents.GenericChoice.Subscription.New(
					applyFunction: async applyParameters =>
					{
						await RemoveCondition(target, condition.ConditionModel);
					},
					effectType: EffectType.SelectableMandatory,
					effectButtonParameters: new IconEffectButton.Parameters(Icons.GetCondition(condition.ConditionModel)),
					effectInfoViewParameters: new TextEffectInfoView.Parameters(
						$"Remove {Icons.Inline(Icons.GetCondition(condition.ConditionModel))}")
				));
			}
		}

		await GenericChoice(target, subscriptions, hintText: "Select a condition to remove");
	}

	public static async GDTask<int> RemoveAllNegativeConditions(Figure target)
	{
		int removedConditionsCount = 0;
		while(target.Conditions.Any(condition => condition.ConditionModel.IsNegative))
		{
			Condition condition = target.Conditions.First(condition => condition.ConditionModel.IsNegative);

			if(await RemoveCondition(target, condition.ConditionModel))
			{
				removedConditionsCount++;
			}
		}

		return removedConditionsCount;
	}

	public static async GDTask RemoveAllPositiveConditions(Figure target)
	{
		while(target.Conditions.Any(condition => condition.ConditionModel.IsPositive))
		{
			Condition condition = target.Conditions.First(condition => condition.ConditionModel.IsPositive);

			await RemoveCondition(target, condition.ConditionModel);
		}
	}

	public static async GDTask RemoveAllChill(Figure target)
	{
		while(target.HasCondition(Conditions.Chill))
		{
			await RemoveCondition(target, Conditions.Chill);
		}
	}

	public static async GDTask AddCharacterToken(AbilityState abilityState, Figure target, string effectText)
	{
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(abilityState, target,
			parameters => parameters.Figure == target,
			parameters => parameters.Add(new InfoTextExtraEffect.Parameters(effectText))
		);

		if(abilityState.Performer is Character character)
		{
			target.AddEffectView<CharacterTokenHexObjectEffectView>(new CharacterTokenHexObjectEffectView.Parameters(character, abilityState));
		}

		await GDTask.CompletedTask;
	}

	public static async GDTask AddCharacterToken(Character character, Figure target, string effectText)
	{
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(character, target,
			parameters => parameters.Figure == target,
			parameters => parameters.Add(new InfoTextExtraEffect.Parameters(effectText))
		);

		target.AddEffectView<CharacterTokenHexObjectEffectView>(new CharacterTokenHexObjectEffectView.Parameters(character, target));

		await GDTask.CompletedTask;
	}

	public static async GDTask RemoveCharacterToken(AbilityState abilityState, Figure target)
	{
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(abilityState, target);

		foreach(HexObjectEffectViewBase effect in target.Effects)
		{
			if(effect is CharacterTokenHexObjectEffectView characterTokenHexObjectEffectView)
			{
				if(characterTokenHexObjectEffectView.ViewParameters.Subscriber == abilityState)
				{
					target.RemoveEffectView(characterTokenHexObjectEffectView);
					return;
				}
			}
		}

		await GDTask.CompletedTask;
	}

	public static async GDTask GainXP(Figure figure, int xp)
	{
		if(figure is Character character)
		{
			character.GainXP(xp);
		}

		await GDTask.CompletedTask;
	}

	public static async GDTask<bool> TryDestroyObstacle(Obstacle obstacle)
	{
		if(!obstacle.CannotBeDestroyed)
		{
			await obstacle.Destroy();
			return true;
		}

		return false;
	}

	public static async GDTask DestroyDifficultTerrain(DifficultTerrain difficultTerrain)
	{
		if(!difficultTerrain.CannotBeDestroyed)
		{
			await difficultTerrain.Destroy();
		}
	}

	public static async GDTask DisarmTrap(Trap trap, Figure potentialDisarmer)
	{
		if(!trap.CannotBeDestroyed)
		{
			await trap.Disarm();

			await ScenarioEvents.TrapDisarmedEvent.CreatePrompt(
				new ScenarioEvents.TrapDisarmed.Parameters(trap, potentialDisarmer));
		}
	}

	public static async GDTask<DifficultTerrain> CreateDifficultTerrain(Hex hex, PackedScene scene)
	{
		return await CreateOverlayTile<DifficultTerrain>(hex, scene);
	}

	public static async GDTask<List<Coin>> SpawnCoin(Hex hex, Figure dropper = null)
	{
		ScenarioCheckEvents.SpawnCoinCheck.Parameters spawnCoinCheckEventParameters =
			ScenarioCheckEvents.SpawnCoinCheckEvent.Fire(new ScenarioCheckEvents.SpawnCoinCheck.Parameters(dropper));

		List<Coin> coins = new List<Coin>();
		for(int i = 0; i < spawnCoinCheckEventParameters.CoinsToSpawn; i++)
		{
			PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/Scenario/Coin.tscn");
			Coin coin = scene.Instantiate<Coin>();
			GameController.Instance.Map.AddChild(coin);
			await coin.Init(hex);

			await ScenarioEvents.CoinSpawnedEvent.CreatePrompt(new ScenarioEvents.CoinSpawned.Parameters(dropper, coin));

			coins.Add(coin);
		}

		return coins;
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

	public static async GDTask<Monster> SummonMonster(MonsterModel monsterModel, MonsterType monsterType, Hex hex, int? monsterLevel = null,
		Alignment alignment = Alignment.Enemies, Alignment enemies = Alignment.Characters)
	{
		return await GameController.Instance.Map.CreateMonster(monsterModel, monsterType, hex.Coords, true, monsterLevel, alignment, enemies);
	}

	public static async GDTask<Monster> SpawnMonster(MonsterModel monsterModel, MonsterType monsterType, Hex hex, int? monsterLevel = null,
		Alignment alignment = Alignment.Enemies, Alignment enemies = Alignment.Characters)
	{
		return await GameController.Instance.Map.CreateMonster(monsterModel, monsterType, hex.Coords, false, monsterLevel, alignment, enemies);
	}

	public static async GDTask<T> CreateOverlayTile<T>(Hex hex, PackedScene scene, Action<OverlayTile> onInstantiate = null)
		where T : OverlayTile
	{
		if(!hex.IsFeatureless())
		{
			Log.Error("Trying to create an overlay tile in a hex that already has a feature!");
			return null;
		}

		OverlayTile overlayTile = scene.Instantiate<OverlayTile>();
		GameController.Instance.Map.AddChild(overlayTile);
		onInstantiate?.Invoke(overlayTile);
		await overlayTile.Init(hex);

		overlayTile.Scale = Vector2.Zero;
		overlayTile.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardable();

		await ScenarioEvents.OverlayTileCreatedEvent.CreatePrompt(
			new ScenarioEvents.OverlayTileCreated.Parameters(overlayTile));

		return (T)overlayTile;
	}

	public static async GDTask<Hex> MoveOverlayTile(Figure performer, OverlayTile overlayTile, Action<List<Hex>> moveToHexes)
	{
		Hex movedToHex = await SelectHex(performer, moveToHexes, mandatory: true,
			hintText: $"Select a hex to move the {overlayTile.GetType().ToString().ToLower()} to");

		if(movedToHex == null)
		{
			return null;
		}

		await overlayTile.TweenGlobalPosition(movedToHex.GlobalPosition, 0.3f).SetEasing(Easing.OutSine)
			.PlayFastForwardableAsync();
		await GDTask.DelayFastForwardable(0.03f);
		overlayTile.SetOriginHexAndRotation(movedToHex);

		await ScenarioEvents.OverlayTileMovedEvent.CreatePrompt(
			new ScenarioEvents.OverlayTileMoved.Parameters(overlayTile));

		return overlayTile.Hex;
	}

	public static async GDTask<Trap> CreateTrap(Hex hex, string assetPath, int damage = 0, ConditionModel[] conditions = null)
	{
		PackedScene scene = ResourceLoader.Load<PackedScene>(assetPath);

		return await CreateOverlayTile<Trap>(hex, scene, trap => ((Trap)trap).SetTrapValues(damage, conditions ?? []));
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
		bool autoSelectIfOne = true, EffectCollection effectCollection = null, Func<string> hintText = null)
	{
		return SelectFigure(state.Authority, getValidTargets, mandatory, autoSelectIfOne, effectCollection, hintText);
	}

	public static async GDTask<Figure> SelectFigure(Figure authority, Action<List<Figure>> getValidTargets, bool mandatory = false,
		bool autoSelectIfOne = true, EffectCollection effectCollection = null, Func<string> hintText = null)
	{
		TargetSelectionPrompt.Answer targetAnswer = await PromptManager.Prompt(
			new TargetSelectionPrompt(getValidTargets, autoSelectIfOne, mandatory, effectCollection, hintText ?? (() => "Select a target")),
			authority);

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

		return answer.CardReferenceIds == null || answer.CardReferenceIds.Count == 0
			? null
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

		return answer.CardReferenceIds == null
			? []
			: answer.CardReferenceIds
				.Select(referenceId => GameController.Instance.ReferenceManager.Get<AbilityCard>(referenceId)).ToList();
	}

	public static async GDTask ExitHex(AbilityState potentialAbilityState, Figure figure, Figure authority)
	{
		await ScenarioEvents.FigureExitingHexEvent.CreatePrompt(
			new ScenarioEvents.FigureExitingHex.Parameters(potentialAbilityState, figure), authority);
	}

	public static async GDTask EnterHex(AbilityState potentialAbilityState, Figure figure, Figure authority, Hex hex, bool triggerHexEffects,
		bool setPosition)
	{
		figure.SetOriginHexAndRotation(hex, setPosition: setPosition);

		await ScenarioEvents.FigureEnteredHexEvent.CreatePrompt(new ScenarioEvents.FigureEnteredHex.Parameters(potentialAbilityState, figure),
			authority);

		HazardousTerrain hazardousTerrain = hex.GetHexObjectOfType<HazardousTerrain>();
		if(hazardousTerrain != null && triggerHexEffects)
		{
			ScenarioCheckEvents.FlyingCheck.Parameters flyingCheckParameters =
				ScenarioCheckEvents.FlyingCheckEvent.Fire(new ScenarioCheckEvents.FlyingCheck.Parameters(figure));

			if(!flyingCheckParameters.HasFlying)
			{
				ScenarioEvents.HazardousTerrainTriggered.Parameters hazardousTerrainParameters =
					await ScenarioEvents.HazardousTerrainTriggeredEvent.CreatePrompt(
						new ScenarioEvents.HazardousTerrainTriggered.Parameters(potentialAbilityState, hex, hazardousTerrain, true), authority);
				if(hazardousTerrainParameters.AffectedByHazardousTerrain)
				{
					int damage = HazardousTerrain.DamageAmount;
					await SufferDamage(potentialAbilityState, figure, damage);
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
						new ScenarioEvents.TrapTriggered.Parameters(potentialAbilityState, hex, trap, figure, true), authority);
				if(trapTriggeredParameters.TriggersTrap)
				{
					await trap.Trigger(potentialAbilityState, figure);
				}
			}
		}
	}

	public static GDTask<bool> TrySwap(Figure authority, Figure figureA, Figure figureB)
	{
		return TrySwap(null, authority, figureA, figureB);
	}

	public static GDTask<bool> TrySwap(AbilityState abilityState, Figure figureA, Figure figureB)
	{
		return TrySwap(abilityState, abilityState.Authority, figureA, figureB);
	}

	public static bool CanSwap(Figure figureA, Figure figureB)
	{
		if(figureA.Hex.TryGetHexObjectOfType(out Obstacle obstacle) &&
		   !ScenarioCheckEvents.FlyingCheckEvent.Fire(new ScenarioCheckEvents.FlyingCheck.Parameters(figureB)).HasFlying)
		{
			ScenarioCheckEvents.CanEnterObstacleCheck.Parameters canEnterObstacleParameters =
				ScenarioCheckEvents.CanEnterObstacleCheckEvent.Fire(
					new ScenarioCheckEvents.CanEnterObstacleCheck.Parameters(figureB, figureA.Hex, obstacle, true));

			if(!canEnterObstacleParameters.CanEnter)
			{
				return false;
			}
		}

		if(figureB.Hex.TryGetHexObjectOfType(out Obstacle obstacle2) &&
		   !ScenarioCheckEvents.FlyingCheckEvent.Fire(new ScenarioCheckEvents.FlyingCheck.Parameters(figureA)).HasFlying)
		{
			ScenarioCheckEvents.CanEnterObstacleCheck.Parameters canEnterObstacleParameters =
				ScenarioCheckEvents.CanEnterObstacleCheckEvent.Fire(
					new ScenarioCheckEvents.CanEnterObstacleCheck.Parameters(figureA, figureB.Hex, obstacle2, true));

			if(!canEnterObstacleParameters.CanEnter)
			{
				return false;
			}
		}

		if(ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Fire(
			   new ScenarioCheckEvents.ImmuneToForcedMovementCheck.Parameters(figureA)).ImmuneToForcedMovement)
		{
			return false;
		}

		if(ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Fire(
			   new ScenarioCheckEvents.ImmuneToForcedMovementCheck.Parameters(figureB)).ImmuneToForcedMovement)
		{
			return false;
		}

		ScenarioCheckEvents.CanEnterCheck.Parameters canEnter =
			ScenarioCheckEvents.CanEnterCheckEvent.Fire(
				new ScenarioCheckEvents.CanEnterCheck.Parameters(figureA, figureB.Hex));

		ScenarioCheckEvents.CanEnterCheck.Parameters canEnter2 =
			ScenarioCheckEvents.CanEnterCheckEvent.Fire(
				new ScenarioCheckEvents.CanEnterCheck.Parameters(figureB, figureA.Hex));

		if(!canEnter.CanEnter || !canEnter.CanEnter)
		{
			return false;
		}

		return true;
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
					parameters.Source == subscriber &&
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

		await ScenarioEvents.GenericChoiceEvent.CreatePrompt(new ScenarioEvents.GenericChoice.Parameters(subscriber), authority, hintText);
		ScenarioEvents.GenericChoiceEvent.ClearAllSubscriptions();
	}

	public static GDTask InfuseWildElement(AbilityState potentialAbilityState, Figure potentialInfuser = null)
	{
		return InfuseElement(potentialAbilityState, Elements.All, potentialInfuser);
	}

	public static async GDTask<Element?> InfuseElement(AbilityState potentialAbilityState, IReadOnlyCollection<Element> possibleElements,
		Figure potentialInfuser = null)
	{
		if(possibleElements.Count == 1)
		{
			Element onlyElement = possibleElements.First();
			await InfuseElement(potentialAbilityState, possibleElements.First(), potentialInfuser);
			return onlyElement;
		}

		potentialInfuser ??= potentialAbilityState?.Performer;

		Element? element = null;

		List<ScenarioEvents.GenericChoice.Subscription> subscriptions =
			new List<ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription>();
		foreach(Element possibleElement in possibleElements)
		{
			subscriptions.Add(ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
				applyFunction: async parameters =>
				{
					element = possibleElement;
					await InfuseElement(potentialAbilityState, possibleElement, potentialInfuser);
				},
				effectType: EffectType.SelectableMandatory,
				effectButtonParameters: new IconEffectButton.Parameters(Icons.GetElement(possibleElement)),
				effectInfoViewParameters: new TextEffectInfoView.Parameters($"Infuse {possibleElement}")
			));
		}

		await GenericChoice(potentialInfuser, subscriptions);

		return element;
	}

	public static async GDTask InfuseElement(AbilityState potentialAbilityState, Element element, Figure potentialInfuser = null,
		bool immediately = false)
	{
		potentialInfuser ??= potentialAbilityState?.Performer;

		if(immediately)
		{
			await GameController.Instance.ElementManager.InfuseImmediately(element);
		}
		else
		{
			GameController.Instance.ElementManager.StartInfuse(element);
		}

		await ScenarioEvents.ElementInfusedEvent.CreatePrompt(
			new ScenarioEvents.ElementInfused.Parameters(potentialAbilityState, element, potentialInfuser));
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
			ScenarioEvents.ConsumeElementEvent.Subscribe(authority, subscriber,
				canApplyParameters =>
					!canApplyParameters.Consumed && canApplyParameters.Elements.Contains(possibleElement) &&
					GameController.Instance.ElementManager.GetState(possibleElement) > ElementState.Inert &&
					ScenarioCheckEvents.CanConsumeElementCheckEvent
						.Fire(new ScenarioCheckEvents.CanConsumeElementCheck.Parameters(authority, possibleElement)).CanConsume,
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
			await ScenarioEvents.ConsumeElementEvent.CreatePrompt(
				new ScenarioEvents.ConsumeElement.Parameters(possibleElements), authority,
				"Select element to consume");

		ScenarioEvents.ConsumeElementEvent.Unsubscribe(authority, subscriber);

		return consumeEventParameters.Consumed ? consumeEventParameters.ConsumedElement : null;
	}

	public static async GDTask<bool> AskConsumeElement(Figure authority, Element element, string effectInfoText = null, string hintText = null)
	{
		object subscriber = new object();
		ScenarioEvents.ConsumeElementEvent.Subscribe(authority, subscriber,
			canApplyParameters =>
				canApplyParameters.Elements.Contains(element) &&
				GameController.Instance.ElementManager.GetState(element) > ElementState.Inert &&
				ScenarioCheckEvents.CanConsumeElementCheckEvent
					.Fire(new ScenarioCheckEvents.CanConsumeElementCheck.Parameters(authority, element)).CanConsume,
			async applyParameters =>
			{
				applyParameters.SetConsumed(element);
				await TryConsumeElement(element);
			}, EffectType.Selectable, 0, false, false,
			new ConsumeElementEffectButton.Parameters(element),
			new TextEffectInfoView.Parameters(effectInfoText ?? $"Consume {Icons.Inline(Icons.GetElement(element))}"));

		ScenarioEvents.ConsumeElement.Parameters consumeEventParameters =
			await ScenarioEvents.ConsumeElementEvent.CreatePrompt(
				new ScenarioEvents.ConsumeElement.Parameters([element]), authority,
				hintText ?? $"Consume {Icons.HintText(Icons.GetElement(element))}?");

		ScenarioEvents.ConsumeElementEvent.Unsubscribe(authority, subscriber);

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

	public static async GDTask<ItemModel> SelectItem(Character characterAndAuthority, ItemState requiredItemState, ItemType? requiredItemType = null,
		string hintText = "Select an item")
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
		await item.RemoveFromActive();

		await item.Refresh();
	}

	public static async GDTask SpendItem(ItemModel item)
	{
		await item.RemoveFromActive();

		await item.SetItemState(ItemState.Spent);
	}

	public static async GDTask ConsumeItem(ItemModel item)
	{
		await item.RemoveFromActive();

		if(item.Unrecoverable)
		{
			await item.SetItemState(ItemState.UnrecoverablyConsumed);
		}
		else
		{
			await item.SetItemState(ItemState.Consumed);
		}
	}

	public static async GDTask SpendOrConsume(ItemModel item)
	{
		if(item.ItemUseType == ItemUseType.Spend)
		{
			await SpendItem(item);
		}

		if(item.ItemUseType == ItemUseType.Consume)
		{
			await ConsumeItem(item);
		}
	}

	public static async GDTask<AbilityCardSection> PerformAbilityCardTopOrBottom(Figure performer, AbilityCard abilityCard)
	{
		List<CardPlayCardData> cardDatas = new List<CardPlayCardData>();

		cardDatas.Add(new CardPlayCardData()
		{
			AbilityCard = abilityCard,
			CanPlayTop = true,
			CanPlayBottom = true,
			CanPlayBasicTop = true,
			CanPlayBasicBottom = true
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

	public static async GDTask PermanentlyGiveItem(Character character, ItemModel itemModel)
	{
		ItemModel item = itemModel.ToMutable();
		item.Init(character);
		character.AddItem(item);

		await PromptManager.Prompt(new TreasureItemRewardPrompt(character, itemModel, null, false), character);

		void OnScenarioEnd(ScenarioResult scenarioResult, SavedScenarioProgress savedScenarioProgress)
		{
			SavedItem savedItem = GameController.Instance.SavedCampaign.GetSavedItem(itemModel);
			savedItem.AddUnlocked(1);
			character.SavedCharacter.AddItem(itemModel);
		}

		GameController.Instance.EndEvent += OnScenarioEnd;
	}

	public static async GDTask GainItemDesign(Character character, ItemModel itemModel)
	{
		ItemModel item = itemModel.ToMutable();

		await PromptManager.Prompt(new TreasureItemRewardPrompt(character, itemModel, null, true), character);

		void OnScenarioEnd(ScenarioResult scenarioResult, SavedScenarioProgress savedScenarioProgress)
		{
			SavedItem savedItem = GameController.Instance.SavedCampaign.GetSavedItem(itemModel);
			savedItem.AddUnlocked(item.ShopCount);
			savedItem.AddStock(item.ShopCount);
		}

		GameController.Instance.EndEvent += OnScenarioEnd;
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
		List<ItemModel> availableItems = new List<ItemModel>();
		foreach(ItemModel itemModel in itemModels)
		{
			SavedItem savedItem = GameController.Instance.SavedCampaign.GetSavedItem(itemModel);
			int unlockedCount = savedItem.UnlockedCount;
			for(int i = 0; i < itemModel.ShopCount - unlockedCount; i++)
			{
				availableItems.Add(itemModel);
			}
		}

		return availableItems.Count == 0 ? null : availableItems.PickRandom(GameController.Instance.StateRNG);
	}

	public static async GDTask Lose()
	{
		await ScenarioEvents.ScenarioEndedEvent.CreatePrompt(new ScenarioEvents.ScenarioEnded.Parameters(false));

		GameController.Instance.MarkScenarioEnded();
		GameController.Instance.ScenarioLostView.Open();
		await GDTask.Never(GameController.CancellationToken);
	}

	public static async GDTask Win()
	{
		await ScenarioEvents.ScenarioEndedEvent.CreatePrompt(new ScenarioEvents.ScenarioEnded.Parameters(true));

		GameController.Instance.MarkScenarioEnded();
		GameController.Instance.ScenarioWonView.Open();
		await GDTask.Never(GameController.CancellationToken);
	}

	public static void SubscribeDuringCharacterTurn(IEventSubscriber eventSubscriber, EffectType effectType, Func<Character, bool> canApply,
		Func<Character, GDTask> apply,
		EffectButtonParameters effectButtonParameters, EffectInfoViewParameters effectInfoViewParameters,
		int order = 0, bool canApplyMultipleTimesDuringAbility = false)
	{
		ScenarioEvents.CardSideSelectionEvent.Subscribe(eventSubscriber,
			canApplyParameters => canApply == null || canApply(canApplyParameters.Character),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.Character);
				}
			},
			effectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: effectButtonParameters,
			effectInfoViewParameters: effectInfoViewParameters);

		ScenarioEvents.AfterCardsPlayedEvent.Subscribe(eventSubscriber,
			canApplyParameters => canApply == null || canApply(canApplyParameters.Character),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.Character);
				}
			},
			effectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: effectButtonParameters,
			effectInfoViewParameters: effectInfoViewParameters);

		ScenarioEvents.LongRestCardSelectionEvent.Subscribe(eventSubscriber,
			canApplyParameters => canApply == null || canApply(canApplyParameters.Character),
			async applyParameters =>
			{
				if(apply != null)
				{
					await apply(applyParameters.Character);
				}
			},
			effectType,
			order: order,
			canApplyMultipleTimesInEffectCollection: canApplyMultipleTimesDuringAbility,
			effectButtonParameters: effectButtonParameters,
			effectInfoViewParameters: effectInfoViewParameters);
	}

	public static void UnsubscribeDuringTurn(IEventSubscriber eventSubscriber)
	{
		ScenarioEvents.CardSideSelectionEvent.Unsubscribe(eventSubscriber);
		ScenarioEvents.AfterCardsPlayedEvent.Unsubscribe(eventSubscriber);
		ScenarioEvents.LongRestCardSelectionEvent.Unsubscribe(eventSubscriber);
	}

	public static async GDTask AddShield(Figure figure, object subscriber, int shieldValue, bool conditionalValue = false, bool pierceable = true,
		RangeType? requiredRangeType = null, Func<ScenarioEvents.SufferDamage.Parameters, bool> customCanApply = null,
		bool customCanApplyReplaceFully = false)
	{
		ScenarioCheckEvents.ShieldCheckEvent.Subscribe(figure, subscriber,
			parameters =>
				parameters.Figure == figure,
			parameters =>
			{
				if(conditionalValue)
				{
					parameters.SetExtraValue();
				}
				else
				{
					parameters.AdjustShield(shieldValue);
				}
			}
		);

		ScenarioEvents.SufferDamageEvent.Subscribe(figure, subscriber,
			parameters =>
			{
				bool canApply =
					parameters.Figure == figure && parameters.FromAttack &&
					(!requiredRangeType.HasValue ||
					 ((AttackAbility.State)parameters.PotentialAbilityState).SingleTargetRangeType == requiredRangeType);

				if(customCanApply != null)
				{
					if(customCanApplyReplaceFully)
					{
						return customCanApply(parameters);
					}

					canApply = canApply && customCanApply(parameters);
				}

				return canApply;
			},
			async parameters =>
			{
				if(pierceable)
				{
					parameters.AdjustShield(shieldValue);
				}
				else
				{
					parameters.AdjustUnpierceableShield(shieldValue);
				}

				await GDTask.CompletedTask;
			}
		);

		AppController.Instance.AudioController.PlayFastForwardable(SFX.Shield, delay: 0f);

		await GDTask.CompletedTask;
	}

	public static void RemoveShield(Figure figure, object subscriber)
	{
		ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(figure, subscriber);
		ScenarioEvents.SufferDamageEvent.Unsubscribe(figure, subscriber);
	}

	public static async GDTask AddRetaliate(Figure figure, object subscriber, int retaliateValue, int range,
		Func<ScenarioEvents.Retaliate.Parameters, bool> customCanApply = null, bool customCanApplyReplaceFully = false)
	{
		ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(figure, subscriber,
			canApplyParameters =>
				canApplyParameters.Figure == figure,
			applyParameters =>
			{
				applyParameters.AddRetaliate(retaliateValue, range);
			}
		);

		ScenarioEvents.RetaliateEvent.Subscribe(figure, subscriber,
			canApplyParameters =>
			{
				bool canApply =
					canApplyParameters.RetaliatingFigure == figure &&
					RangeHelper.Distance(canApplyParameters.AbilityState.Performer.Hex, figure.Hex) <= range;

				if(customCanApply != null)
				{
					if(customCanApplyReplaceFully)
					{
						return customCanApply(canApplyParameters);
					}

					canApply = canApply && customCanApply(canApplyParameters);
				}

				return canApply;
			},
			async applyParameters =>
			{
				applyParameters.AdjustRetaliate(retaliateValue);

				await GDTask.CompletedTask;
			}
		);

		await GDTask.CompletedTask;
	}

	public static void RemoveRetaliate(Figure figure, object subscriber)
	{
		ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(figure, subscriber);
		ScenarioEvents.RetaliateEvent.Unsubscribe(figure, subscriber);
	}

	public static MoveAbility.MoveBuilder SummonMovePlusX(int plusMove)
	{
		return MoveAbility.Builder()
			.WithDistance(plusMove)
			.WithOnAbilityStarted(async moveState =>
			{
				moveState.AdjustMoveValue(((Summon)moveState.Performer).Stats.Move ?? 0);

				await GDTask.CompletedTask;
			});
	}

	public static AttackAbility.AttackBuilder SummonAttackPlusX(int plusAttack)
	{
		return AttackAbility.Builder()
			.WithDamage(plusAttack)
			.WithOnAbilityStarted(async state =>
			{
				Summon summon = ((Summon)state.Performer);

				state.AbilityAdjustAttackValue(summon.Stats.Attack ?? 0);

				int range = summon.Stats.Range ?? 1;
				state.AbilityAdjustRange(range - 1);
				state.AbilitySetRangeType(range == 1 ? RangeType.Melee : RangeType.Range);

				await GDTask.CompletedTask;
			});
		// .WithDuringAttackSubscription(ScenarioEvents.DuringAttack.Subscription.New(
		// 	parameters => true,
		// 	async parameters =>
		// 	{
		// 		parameters.AbilityState.AbilityAdjustAttackValue(((Summon)parameters.Performer).Stats.Attack ?? 0);
		//
		// 		int range = ((Summon)parameters.Performer).Stats.Range ?? 1;
		// 		parameters.AbilityState.AbilityAdjustRange(range - 1);
		// 		parameters.AbilityState.AbilitySetRangeType(range == 1 ? RangeType.Melee : RangeType.Range);
		//
		// 		await GDTask.CompletedTask;
		// 	}
		// ));
	}

	public static async GDTask<bool> CurseMonsters()
	{
		await GDTask.CompletedTask;

		bool success = GameController.Instance.AMDManager.CurseMonsters();
		return success;
	}

	private static async GDTask<bool> TrySwap(AbilityState potentialAbilityState, Figure authority, Figure figureA, Figure figureB)
	{
		if(!CanSwap(figureA, figureB))
		{
			return false;
		}

		if(!GameController.FastForward)
		{
			await GameController.Instance.ScreenDistortion.Swap(figureA, figureB, 1.4f).PlayFastForwardableAsync();
		}

		Hex hexA = figureA.Hex;
		Hex hexB = figureB.Hex;
		await EnterHex(potentialAbilityState, figureB, authority, hexA, true, true);
		await EnterHex(potentialAbilityState, figureA, authority, hexB, true, true);
		potentialAbilityState?.SetPerformed();

		return true;
	}

	public static bool CanConsumeElement(Element element, Figure potentialConsumer)
	{
		if(GameController.Instance.ElementManager.GetState(element) == ElementState.Inert ||
		   !ScenarioCheckEvents.CanConsumeElementCheckEvent
			   .Fire(new ScenarioCheckEvents.CanConsumeElementCheck.Parameters(potentialConsumer, element)).CanConsume)
		{
			return false;
		}

		return true;
	}
}