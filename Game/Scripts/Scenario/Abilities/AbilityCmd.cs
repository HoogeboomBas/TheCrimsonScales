using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweens.Builders;
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
		if(card.CardState is CardState.Round or CardState.Persistent or CardState.PersistentNoDeactivate)
		{
			await DiscardCard(card);
		}

		if(card.CardState is CardState.RoundLoss or CardState.PersistentLoss)
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
		return OtherActiveAbility.Builder()
			.WithOnActivate(async state =>
			{
				ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, state.Performer,
					parameters => parameters.AbilityState.Target == state.Performer,
					async parameters =>
					{
						parameters.AbilityState.SingleTargetSetHasDisadvantage();

						await GDTask.CompletedTask;
					}
				);

				ScenarioCheckEvents.DisadvantageCheckEvent.Subscribe(state, state.Performer,
					parameters => parameters.Target == state.Performer,
					parameters => parameters.SetDisadvantage(true)
				);

				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(state, state.Performer,
					parameters => state.Performer == parameters.Figure,
					parameters => parameters.Add(
						new InfoTextExtraEffect.Parameters(textParameters => "All attacks targeting this figure this round gain disadvantage."))
				);

				await GDTask.CompletedTask;
			})
			.WithOnDeactivate(async state =>
			{
				ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, state.Performer);
				ScenarioCheckEvents.DisadvantageCheckEvent.Unsubscribe(state, state.Performer);
				ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Unsubscribe(state, state.Performer);

				await GDTask.CompletedTask;
			})
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

		if(sufferDamageParameters.DamagePrevented || sufferDamageParameters.CalculatedCurrentDamage == 0)
		{
			return 0;
		}

		int damageDealt = sufferDamageParameters.CalculatedCurrentDamage;

		ScenarioEvents.JustBeforeSufferDamage.Parameters justBeforeSufferDamageParameters =
			await ScenarioEvents.JustBeforeSufferDamageEvent.CreatePrompt(
				new ScenarioEvents.JustBeforeSufferDamage.Parameters(target, damageDealt, potentialAbilityState, sufferDamageParameters), target);

		if(justBeforeSufferDamageParameters.Prevented)
		{
			return 0;
		}

		potentialAbilityState?.DamagedFigures.Add(target);

		int newHealth = Mathf.Max(target.Health - damageDealt, 0);

		int damageSuffered = target.Health - newHealth;

		GameController.Instance.CameraController.Shake(damageSuffered * 4f, fromAttack ? 0.15f / AppController.Instance.GameplayTimeScale : 0f);

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

		if(damageDealt > 0)
		{
			if(!GameController.FastForward)
			{
				//TODO: Implement damage sound?
				// AppController.Instance.AudioController.PlayFastForwardable(SFX.Heal, delay: 0.0f);

				//TODO: Implement blood effect?
				// HealEffect healEffect = SceneLoader.InstantiateScene<HealEffect>("res://Scenes/Scenario/Effects/HealEffect.tscn");
				// target.AddChild(healEffect);
				// healEffect.Init();

				Color damageColor = Color.Color8(200, 20, 10);

				target.Visual.SetSelfModulate(damageColor);
				GTweenSequenceBuilder.New()
					.AppendTime(0.1f)
					.Append(target.Visual.TweenInstanceShaderPropertyFloat("tintFactor", 0.6f, 0.2f))
					.AppendTime(0.1f)
					.Append(target.Visual.TweenInstanceShaderPropertyFloat("tintFactor", 0f, 0.15f))
					.AppendCallback(() =>
					{
						target.Visual.SetSelfModulate(Colors.White);
					})
					.Build().PlayFastForwardable();
			}

			await ScenarioEvents.AfterSufferDamageEvent.CreatePrompt(
				new ScenarioEvents.AfterSufferDamage.Parameters(target, damageDealt, damageSuffered, potentialAbilityState, sufferDamageParameters),
				target);
		}

		return damageDealt;
	}

	public static async GDTask<int> SufferDamage(Figure target, int damage, Figure potentialDamageDealer, bool fromAttack = false)
	{
		return await SufferDamage(null, target, damage, potentialDamageDealer, fromAttack);
	}

	public static async GDTask KillOrExhaust(AbilityState potentialAbilityState, Figure target, Figure potentialKiller)
	{
		potentialAbilityState?.SetPerformed();

		ScenarioEvents.BeforeFigureKilled.Parameters beforeFigureKilledParameters =
			await ScenarioEvents.BeforeFigureKilledEvent.CreatePrompt(
				new ScenarioEvents.BeforeFigureKilled.Parameters(potentialAbilityState, target), potentialKiller);

		if(!beforeFigureKilledParameters.Prevented)
		{
			await target.Destroy();

			await ScenarioEvents.FigureKilledEvent.CreatePrompt(
				new ScenarioEvents.FigureKilled.Parameters(potentialAbilityState, target, potentialKiller), target);
		}
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

	public static GDTask AddCondition(AbilityState potentialAbilityState, Figure target, ConditionModel conditionModel,
		Figure potentialConditionGiver = null)
	{
		return AddConditions(potentialAbilityState, target, [conditionModel], potentialConditionGiver);
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

					await ScenarioEvents.ConditionAddedEvent.CreatePrompt(
						new ScenarioEvents.ConditionAdded.Parameters(potentialAbilityState, target, potentialConditionGiver, conditionModel), target);
				}
			}
		}

		potentialAbilityState?.SetPerformed();
	}

	public static async GDTask RemoveCondition(Condition condition, AbilityState potentialAbilityState = null)
	{
		Figure target = condition.Owner;

		ScenarioEvents.RemoveCondition.Parameters removeConditionParameters =
			await ScenarioEvents.RemoveConditionEvent.CreatePrompt(
				new ScenarioEvents.RemoveCondition.Parameters(condition, potentialAbilityState), condition.Owner);


		if(!removeConditionParameters.Prevented)
		{
			await target.RemoveCondition(condition);
		}

		await ScenarioEvents.AfterRemoveConditionEvent.CreatePrompt(
			new ScenarioEvents.AfterRemoveCondition.Parameters(target, condition.ConditionModel, potentialAbilityState), target);
	}

	public static async GDTask<bool> RemoveCondition(Figure target, ConditionModel conditionModel, AbilityState potentialAbilityState = null)
	{
		Condition condition = target.GetCondition(conditionModel);
		if(condition != null)
		{
			await RemoveCondition(condition, potentialAbilityState);

			return true;
		}

		return false;
	}

	public static async GDTask RemoveOneNegativeCondition(AbilityState potentialAbilityState, Figure target)
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
						potentialAbilityState?.SetPerformed();
						await RemoveCondition(target, condition.ConditionModel, potentialAbilityState);
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

	public static async GDTask RemoveChillStack(Figure target)
	{
		if(target.TryGetCondition(Conditions.Chill, out Condition chill))
		{
			if(chill.StackCount > 1)
			{
				chill.AdjustStackCount(-1);
			}
			else
			{
				await RemoveCondition(chill);
			}
		}
	}

	public static async GDTask AddCharacterToken(AbilityState abilityState, Figure target, TextHelper.LabelTextDelegate getEffectText)
	{
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(abilityState, target,
			parameters => parameters.Figure == target,
			parameters => parameters.Add(new InfoTextExtraEffect.Parameters(getEffectText))
		);

		if(abilityState.Performer is Character character)
		{
			target.AddEffectView<CharacterTokenHexObjectEffectView>(new CharacterTokenHexObjectEffectView.Parameters(character, abilityState));
		}

		await GDTask.CompletedTask;
	}

	public static async GDTask AddCharacterToken(Character character, Figure target, TextHelper.LabelTextDelegate getEffectText)
	{
		ScenarioCheckEvents.FigureInfoItemExtraEffectsCheckEvent.Subscribe(character, target,
			parameters => parameters.Figure == target,
			parameters => parameters.Add(new InfoTextExtraEffect.Parameters(getEffectText))
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

	public static async GDTask GainXP(Figure figure, int xp, bool fromScenario = false)
	{
		if(figure is Character character)
		{
			await ScenarioEvents.GainedExperienceEvent.CreatePrompt(
				new ScenarioEvents.GainedExperience.Parameters(figure, xp, fromScenario));

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

	public static async GDTask<Obstacle> CreateObstacle(Hex hex, string assetPath)
	{
		PackedScene scene = SceneLoader.LoadPackedScene(assetPath);

		return await CreateOverlayTile<Obstacle>(hex, scene);
	}

	public static async GDTask<List<Coin>> SpawnCoin(Hex hex, Figure dropper = null)
	{
		ScenarioCheckEvents.SpawnCoinCheck.Parameters spawnCoinCheckEventParameters =
			ScenarioCheckEvents.SpawnCoinCheckEvent.Fire(new ScenarioCheckEvents.SpawnCoinCheck.Parameters(dropper));

		List<Coin> coins = new List<Coin>();
		for(int i = 0; i < spawnCoinCheckEventParameters.CoinsToSpawn; i++)
		{
			Coin coin = SceneLoader.InstantiateScene<Coin>("res://Scenes/Scenario/Coin.tscn");
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
		Alignment alignment = Alignment.Monsters)
	{
		return await GameController.Instance.Map.CreateMonster(monsterModel, monsterType, hex.Coords, true, monsterLevel, alignment);
	}

	public static async GDTask<Monster> SpawnMonster(MonsterModel monsterModel, MonsterType monsterType, Hex hex, int? monsterLevel = null,
		Alignment alignment = Alignment.Monsters)
	{
		return await GameController.Instance.Map.CreateMonster(monsterModel, monsterType, hex.Coords, false, monsterLevel, alignment);
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

	public static async GDTask<Hex> RelocateOverlayTile(AbilityState state, Action<List<OverlayTile>> selectOverlayTiles,
		Action<OverlayTile, List<Hex>> moveToHexes, string selectionHintText = "Select an overlay tile to relocate")
	{
		OverlayTile overlayTile = await SelectOverlayTile(state, selectOverlayTiles, hintText: selectionHintText);

		if(overlayTile == null)
		{
			return null;
		}

		Hex movedToHex = await SelectHex(state.Performer, list => moveToHexes(overlayTile, list), mandatory: true,
			hintText: "Select a hex to move the overlay tile to");

		if(movedToHex == null)
		{
			return null;
		}

		await overlayTile.TweenGlobalPosition(movedToHex.GlobalPosition, 0.3f).SetEasing(Easing.OutSine).PlayFastForwardableAsync();
		await GDTask.DelayFastForwardable(0.03f);
		overlayTile.SetOriginHexAndRotation(movedToHex);

		await ScenarioEvents.OverlayTileMovedEvent.CreatePrompt(
			new ScenarioEvents.OverlayTileMoved.Parameters(overlayTile));

		state.SetPerformed();

		return overlayTile.Hex;
	}

	public static async GDTask<List<Trap>> CreateTraps(int damage, Figure performer, Figure authority = null,
		Action<List<Hex>> customSelectHexes = null, int range = 1, int trapCount = 1, ConditionModel[] conditions = null,
		bool mandatory = false, string assetPath = null)
	{
		List<Hex> targetHexes = await SelectHexes(authority ?? performer, list =>
			{
				if(customSelectHexes != null)
				{
					customSelectHexes(list);
				}
				else
				{
					list.AddRange(RangeHelper.GetHexesInRange(performer.Hex, range)
						.Where(hex => hex.IsEmpty()));
				}
			},
			minSelectionCount: mandatory ? trapCount : 0,
			maxSelectionCount: trapCount,
			autoSelectIfMaxCountIsValidCount: false,
			hintText: (trapCount == 1) ? $"Select a hex to place the trap" : $"Select up to {trapCount} hexes to place the traps");

		List<Trap> createdTraps = [];

		if(targetHexes.Count > 0)
		{
			foreach(Hex hex in targetHexes)
			{
				createdTraps.Add(await PlaceTrap(hex, assetPath: assetPath, damage: damage, conditions: conditions));
			}
		}

		return createdTraps;
	}

	private static async GDTask<Trap> PlaceTrap(Hex hex, string assetPath = null,
		int damage = 0, ConditionModel[] conditions = null)
	{
		PackedScene scene = SceneLoader.LoadPackedScene(assetPath ?? "res://Content/OverlayTiles/Traps/BearTrap1H.tscn");

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
			new HexSelectionPrompt(getValidHexes, false, null, () => hintText, mandatory ? 1 : 0), authority);

		return answer.Skipped ? null : answer.CoordSets.Select(coords => GameController.Instance.Map.GetHex(coords)).FirstOrDefault();
	}

	public static GDTask<Figure> SelectFigure(AbilityState state, Action<List<Figure>> getValidTargets, bool mandatory = false,
		bool autoSelectIfOne = true, bool autoSkipIfNone = false, EffectCollection effectCollection = null, Func<string> hintText = null)
	{
		return SelectFigure(state.Authority, getValidTargets, mandatory, autoSelectIfOne, autoSkipIfNone, effectCollection, hintText);
	}

	public static async GDTask<Figure> SelectFigure(Figure authority, Action<List<Figure>> getValidTargets, bool mandatory = false,
		bool autoSelectIfOne = true, bool autoSkipIfNone = false, EffectCollection effectCollection = null, Func<string> hintText = null)
	{
		TargetSelectionPrompt.Answer targetAnswer = await PromptManager.Prompt(
			new TargetSelectionPrompt(getValidTargets, autoSelectIfOne, autoSkipIfNone, mandatory, effectCollection,
				hintText ?? (() => "Select a target")),
			authority);

		if(targetAnswer.Skipped)
		{
			return null;
		}

		return GameController.Instance.ReferenceManager.Get<Figure>(targetAnswer.FigureReferenceId);
	}

	public static GDTask<OverlayTile> SelectOverlayTile(AbilityState state, Action<List<OverlayTile>> getValidOverlayTiles, bool mandatory = false,
		string hintText = "Select a hex")
	{
		return SelectOverlayTile(state.Authority, getValidOverlayTiles, mandatory, hintText);
	}

	public static async GDTask<OverlayTile> SelectOverlayTile(Figure authority, Action<List<OverlayTile>> getValidOverlayTiles,
		bool mandatory = false,
		string hintText = "Select a hex")
	{
		OverlayTileSelectionPrompt.Answer answer = await PromptManager.Prompt(
			new OverlayTileSelectionPrompt(getValidOverlayTiles, false, null, () => hintText, mandatory ? 1 : 0), authority);

		return answer.Skipped
			? null
			: answer.OverlayTileReferenceIds.Select(referenceId => GameController.Instance.ReferenceManager.Get<OverlayTile>(referenceId))
				.FirstOrDefault();
	}

	public static GDTask<List<OverlayTile>> SelectOverlayTiles(AbilityState state, Action<List<OverlayTile>> getValidOverlayTiles,
		int minSelectionCount, int maxSelectionCount,
		bool autoSelectIfMaxCountIsValidCount, string hintText)
	{
		return SelectOverlayTiles(state.Authority, getValidOverlayTiles, minSelectionCount, maxSelectionCount, autoSelectIfMaxCountIsValidCount,
			hintText);
	}

	public static async GDTask<List<OverlayTile>> SelectOverlayTiles(Figure authority, Action<List<OverlayTile>> getValidOverlayTiles,
		int minSelectionCount, int maxSelectionCount,
		bool autoSelectIfMaxCountIsValidCount, string hintText)
	{
		OverlayTileSelectionPrompt.Answer answer = await PromptManager.Prompt(
			new OverlayTileSelectionPrompt(getValidOverlayTiles, autoSelectIfMaxCountIsValidCount, null, () => hintText, minSelectionCount,
				maxSelectionCount),
			authority);

		return answer.Skipped
			? []
			: answer.OverlayTileReferenceIds.Select(referenceId => GameController.Instance.ReferenceManager.Get<OverlayTile>(referenceId)).ToList();
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
		bool setPosition, bool forcedMovement = false)
	{
		figure.SetOriginHexAndRotation(hex, setPosition: setPosition);

		await ScenarioEvents.FigureEnteredHexEvent.CreatePrompt(
			new ScenarioEvents.FigureEnteredHex.Parameters(potentialAbilityState, figure, forcedMovement),
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
						new ScenarioEvents.HazardousTerrainTriggered.Parameters(potentialAbilityState, hex, figure, hazardousTerrain, true),
						authority);
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

	public static async GDTask FigureLostFlying(AbilityState potentialAbilityState, Figure figure, Figure authority, Hex hex)
	{
		Hex landingHex = hex;

		if(hex.HasHexObjectOfType<Obstacle>())
		{
			List<Hex> possibleHexes = [];

			// Find unoccupied hexes around the obstacle that are closest to it
			int closestHexRange = int.MaxValue;

			foreach(Hex neighbourHex in hex.Neighbours)
			{
				if(!CanForceMoveTo(potentialAbilityState, figure, hex))
				{
					continue;
				}

				int range = RangeHelper.Distance(neighbourHex, hex);

				if(range == closestHexRange)
				{
					possibleHexes.Add(neighbourHex);
				}
				else if(range < closestHexRange)
				{
					closestHexRange = range;
					possibleHexes.Clear();
					possibleHexes.Add(neighbourHex);
				}
			}

			landingHex = await SelectHex(potentialAbilityState, list =>
			{
				list.AddRange(possibleHexes);
			}, hintText: "Select a hex to force land into.");
		}

		if(hex != landingHex)
		{
			await figure.TweenGlobalPosition(landingHex.GlobalPosition, 0.3f).SetEasing(Easing.OutSine).PlayFastForwardableAsync();
		}

		await ExitHex(potentialAbilityState, figure, authority);
		await EnterHex(potentialAbilityState, figure, authority, landingHex, true, true, true);

		AppController.Instance.AudioController.PlayFastForwardable(SFX.GetLand(figure.Hex), delay: 0f);
	}

	public static async GDTask Teleport(AbilityState potentialAbilityState, Figure figure, Hex destination, bool forcedMovement = false)
	{
		potentialAbilityState?.SetPerformed();

		Figure authority = potentialAbilityState?.Authority ?? figure;

		await ExitHex(potentialAbilityState, figure, authority);

		const float animationSpeed = 1.4f;

		if(!GameController.FastForward)
		{
			// Disappear
			await GameController.Instance.ScreenDistortion.Disappear(figure, animationSpeed, true).PlayFastForwardableAsync();
		}

		figure.SetOriginHexAndRotation(destination);

		if(!GameController.FastForward)
		{
			// Appear
			await GameController.Instance.ScreenDistortion.Appear(figure, animationSpeed, true).PlayFastForwardableAsync();
		}

		await EnterHex(potentialAbilityState, figure, authority, destination,
			triggerHexEffects: true, setPosition: true, forcedMovement: forcedMovement);
	}

	public static GDTask<bool> TrySwap(Figure authority, HexObject hexObjectA, HexObject hexObjectB)
	{
		return TrySwap(null, authority, hexObjectA, hexObjectB);
	}

	public static GDTask<bool> TrySwap(AbilityState abilityState, HexObject hexObjectA, HexObject hexObjectB)
	{
		return TrySwap(abilityState, abilityState.Authority, hexObjectA, hexObjectB);
	}

	public static bool CanSwap(AbilityState potentialAbilityState, HexObject hexObjectA, HexObject hexObjectB)
	{
		if(hexObjectA == hexObjectB)
		{
			return false;
		}

		if(hexObjectA is Figure figureA && !CanForceMoveTo(potentialAbilityState, figureA, hexObjectB.Hex))
		{
			return false;
		}

		if(hexObjectB is Figure figureB && !CanForceMoveTo(potentialAbilityState, figureB, hexObjectA.Hex))
		{
			return false;
		}

		return true;

		// if(figureA.Hex.TryGetHexObjectOfType(out Obstacle obstacle) &&
		//    !ScenarioCheckEvents.FlyingCheckEvent.Fire(new ScenarioCheckEvents.FlyingCheck.Parameters(figureB)).HasFlying)
		// {
		// 	ScenarioCheckEvents.CanEnterObstacleCheck.Parameters canEnterObstacleParameters =
		// 		ScenarioCheckEvents.CanEnterObstacleCheckEvent.Fire(
		// 			new ScenarioCheckEvents.CanEnterObstacleCheck.Parameters(figureB, figureA.Hex, obstacle, true));
		//
		// 	if(!canEnterObstacleParameters.CanEnter)
		// 	{
		// 		return false;
		// 	}
		// }
		//
		// if(figureB.Hex.TryGetHexObjectOfType(out Obstacle obstacle2) &&
		//    !ScenarioCheckEvents.FlyingCheckEvent.Fire(new ScenarioCheckEvents.FlyingCheck.Parameters(figureA)).HasFlying)
		// {
		// 	ScenarioCheckEvents.CanEnterObstacleCheck.Parameters canEnterObstacleParameters =
		// 		ScenarioCheckEvents.CanEnterObstacleCheckEvent.Fire(
		// 			new ScenarioCheckEvents.CanEnterObstacleCheck.Parameters(figureA, figureB.Hex, obstacle2, true));
		//
		// 	if(!canEnterObstacleParameters.CanEnter)
		// 	{
		// 		return false;
		// 	}
		// }
		//
		// if(ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Fire(
		// 	   new ScenarioCheckEvents.ImmuneToForcedMovementCheck.Parameters(figureA)).ImmuneToForcedMovement)
		// {
		// 	return false;
		// }
		//
		// if(ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Fire(
		// 	   new ScenarioCheckEvents.ImmuneToForcedMovementCheck.Parameters(figureB)).ImmuneToForcedMovement)
		// {
		// 	return false;
		// }
		//
		// ScenarioCheckEvents.CanEnterCheck.Parameters canEnterA =
		// 	ScenarioCheckEvents.CanEnterCheckEvent.Fire(
		// 		new ScenarioCheckEvents.CanEnterCheck.Parameters(figureA, figureB.Hex));
		//
		// ScenarioCheckEvents.CanEnterCheck.Parameters canEnterB =
		// 	ScenarioCheckEvents.CanEnterCheckEvent.Fire(
		// 		new ScenarioCheckEvents.CanEnterCheck.Parameters(figureB, figureA.Hex));
		//
		// if(!canEnterA.CanEnter || !canEnterB.CanEnter)
		// {
		// 	return false;
		// }
		//
		// return true;
	}

	public static bool CanForceMoveTo(AbilityState potentialAbilityState, Figure figure, Hex destination)
	{
		if(ScenarioCheckEvents.ImmuneToForcedMovementCheckEvent.Fire(
			   new ScenarioCheckEvents.ImmuneToForcedMovementCheck.Parameters(figure)).ImmuneToForcedMovement)
		{
			return false;
		}

		return MoveHelper.CanStopAt(potentialAbilityState, figure, destination);
	}

	public static async GDTask<bool> HasPerformedAbility(AbilityState abilityState, int abilityIndex)
	{
		await GDTask.CompletedTask;

		AbilityState otherAbilityState = abilityState.ActionState.GetAbilityState<AbilityState>(abilityIndex);

		return otherAbilityState.Performed;
	}

	public static async GDTask GenericChoice(Figure authority, List<ScenarioEvents.GenericChoice.Subscription> subscriptions,
		bool canSelectMultiple = false, string hintText = "Make a selection")
	{
		EffectCollection effectCollection = GenericChoiceCollection(authority, subscriptions, canSelectMultiple: canSelectMultiple);
		await ScenarioEvents.GenericChoiceEvent.CreatePrompt(effectCollection, authority, hintText);
		ClearGenericChoiceCollection(effectCollection);
	}

	public static EffectCollection GenericChoiceCollection(Figure authority, List<ScenarioEvents.GenericChoice.Subscription> subscriptions,
		bool canSelectMultiple = false)
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

		return ScenarioEvents.GenericChoiceEvent.CreateEffectCollection(new ScenarioEvents.GenericChoice.Parameters(subscriber));
	}

	public static void ClearGenericChoiceCollection(EffectCollection effectCollection)
	{
		foreach(Effect effect in effectCollection.Effects)
		{
			ScenarioEvents.GenericChoiceEvent.Unsubscribe(effect.Subscription.Subscriber);
		}
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

	public static async GDTask<bool> AskConsumeElement(Figure authority, Element element, bool mandatory = false, string effectInfoText = null,
		string hintText = null)
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
			},
			mandatory ? EffectType.SelectableMandatory : EffectType.Selectable, 0, false, false,
			new ConsumeElementEffectButton.Parameters(element),
			new TextEffectInfoView.Parameters(effectInfoText ?? $"Consume {Icons.Inline(Icons.GetElement(element))}"));

		ScenarioEvents.ConsumeElement.Parameters consumeEventParameters =
			await ScenarioEvents.ConsumeElementEvent.CreatePrompt(
				new ScenarioEvents.ConsumeElement.Parameters([element]), authority,
				hintText ?? $"Consume {Icons.HintText(Icons.GetElement(element))}?");

		ScenarioEvents.ConsumeElementEvent.Unsubscribe(authority, subscriber);

		return consumeEventParameters.Consumed;
	}

	public static async GDTask<List<Element>> ConsumeElements(Figure authority, List<CardElementConsumption> consumptions)
	{
		List<List<Element>> possibilities = FindConsumptionPossibilities(consumptions, authority);
		if(possibilities.Count == 0)
		{
			throw new InvalidOperationException("Tried consuming elements, but no possible solutions were found");
		}

		if(possibilities.Count == 1)
		{
			foreach(Element element in possibilities[0])
			{
				await TryConsumeElement(element);
			}

			return possibilities[0];
		}
		else
		{
			List<ScenarioEvents.GenericChoice.Subscription> subscriptions = [];
			List<Element> chosenConsumption = null;

			foreach(List<Element> possibility in possibilities)
			{
				string text = "Consume";
				foreach(Element element in possibility)
				{
					text += $" {Icons.Inline(Icons.GetElement(element))}";
				}

				subscriptions.Add(ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
					applyFunction: async parameters =>
					{
						chosenConsumption = possibility;

						await GDTask.CompletedTask;
					},
					effectType: EffectType.SelectableMandatory,
					effectButtonParameters: new ConsumeElementEffectButton.Parameters(possibility),
					effectInfoViewParameters: new TextEffectInfoView.Parameters(text)
				));
			}

			await GenericChoice(authority ?? GameController.Instance.CharacterManager.FirstAlive(), subscriptions,
				hintText: consumptions.Count == 1 ? "Select an element to consume" : "Select a set of elements to consume");

			if(chosenConsumption == null)
			{
				return null;
			}

			foreach(Element element in chosenConsumption)
			{
				await TryConsumeElement(element);
			}

			return chosenConsumption;
		}
	}

	public static async GDTask<bool> TryConsumeElement(Element element)
	{
		if(GameController.Instance.ElementManager.GetState(element) == ElementState.Inert)
		{
			return false;
		}

		await GameController.Instance.ElementManager.Consume(element);

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

	public static async GDTask<ItemModel> SelectItem(Figure authority, List<ItemModel> items, bool mandatory = false,
		string hintText = "Select an item")
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
				effectType: mandatory ? EffectType.SelectableMandatory : EffectType.Selectable,
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

	public static async GDTask GainGold(Character character, int amount)
	{
		void OnScenarioEnd(ScenarioResult scenarioResult, SavedScenarioProgress savedScenarioProgress)
		{
			character.SavedCharacter.AddGold(amount);
		}

		GameController.Instance.EndEvent += OnScenarioEnd;

		await GDTask.CompletedTask;
	}

	// public static async GDTask GainXP(Character character, int amount)
	// {
	// 	void OnScenarioEnd(ScenarioResult scenarioResult, SavedScenarioProgress savedScenarioProgress)
	// 	{
	// 		character.SavedCharacter.AddXP(amount);
	// 	}
	//
	// 	GameController.Instance.EndEvent += OnScenarioEnd;
	//
	// 	await GDTask.CompletedTask;
	// }

	public static async GDTask GainCheckmark(Character character)
	{
		void OnScenarioEnd(ScenarioResult scenarioResult, SavedScenarioProgress savedScenarioProgress)
		{
			character.SavedCharacter.AddCheckmark();
		}

		GameController.Instance.EndEvent += OnScenarioEnd;

		await GDTask.CompletedTask;
	}

	public static ItemModel GetRandomAvailableOrb()
	{
		return AppController.GetRandomAvailableOrb(GameController.Instance.SavedCampaign, GameController.Instance.StateRNG);
	}

	public static ItemModel GetRandomAvailableStone()
	{
		return AppController.GetRandomAvailableStone(GameController.Instance.SavedCampaign, GameController.Instance.StateRNG);
	}

	public static async GDTask Lose()
	{
		await ScenarioEvents.ScenarioEndedEvent.CreatePrompt(new ScenarioEvents.ScenarioEnded.Parameters(false));

		GameController.Instance.MarkScenarioEnded();
		GameController.Instance.ScenarioLostView.Open();

		await GDTask.WaitWhile(() => GameController.Instance.ScenarioResult == ScenarioResult.None);

		await GameController.Instance.EndScenario();

		await GDTask.Never(GameController.CancellationToken);
	}

	public static async GDTask Win()
	{
		await ScenarioEvents.ScenarioEndedEvent.CreatePrompt(new ScenarioEvents.ScenarioEnded.Parameters(true));

		await GameController.Instance.OpenStoryViewConclusion();

		GameController.Instance.MarkScenarioEnded();
		GameController.Instance.ScenarioWonView.Open();

		await GDTask.WaitWhile(() => GameController.Instance.ScenarioResult == ScenarioResult.None);

		await GameController.Instance.EndScenario();

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

		ScenarioEvents.DuringMovementEvent.Subscribe(eventSubscriber,
			canApplyParameters => canApplyParameters.Performer is Character character && canApply(character),
			async applyParameters =>
			{
				await apply(applyParameters.Performer as Character);
			},
			EffectType.Selectable,
			order: 0,
			canApplyMultipleTimesInEffectCollection: true,
			effectButtonParameters: effectButtonParameters,
			effectInfoViewParameters: effectInfoViewParameters);
	}

	public static void UnsubscribeDuringCharacterTurn(IEventSubscriber eventSubscriber)
	{
		ScenarioEvents.CardSideSelectionEvent.Unsubscribe(eventSubscriber);
		ScenarioEvents.AfterCardsPlayedEvent.Unsubscribe(eventSubscriber);
		ScenarioEvents.LongRestCardSelectionEvent.Unsubscribe(eventSubscriber);
		ScenarioEvents.DuringMovementEvent.Unsubscribe(eventSubscriber);
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

	private static async GDTask<bool> TrySwap(AbilityState potentialAbilityState, Figure authority, HexObject hexObjectA, HexObject hexObjectB)
	{
		Figure figureA = hexObjectA as Figure;
		Figure figureB = hexObjectB as Figure;

		object subscriber = new object();

		if(figureA != null && figureB != null)
		{
			// Ignore figure to swap with
			ScenarioCheckEvents.CanStopMoveAtHexWithFigureCheckEvent.Subscribe(authority, subscriber,
				parameters =>
					(parameters.Figure == figureA &&
					 parameters.OtherFigure == figureB) ||
					(parameters.OtherFigure == figureA &&
					 parameters.Figure == figureB),
				parameters =>
				{
					parameters.SetCanStopAt();
				}
			);
		}

		if(!CanSwap(potentialAbilityState, hexObjectA, hexObjectB))
		{
			return false;
		}

		ScenarioCheckEvents.CanStopMoveAtHexWithFigureCheckEvent.Unsubscribe(authority, subscriber);

		if(!GameController.FastForward)
		{
			await GameController.Instance.ScreenDistortion.Swap(hexObjectA, hexObjectB, 1.4f).PlayFastForwardableAsync();
		}

		Hex hexA = hexObjectA.Hex;
		Hex hexB = hexObjectB.Hex;

		if(figureA != null)
		{
			await ExitHex(potentialAbilityState, figureA, authority);
		}

		if(figureB != null)
		{
			await ExitHex(potentialAbilityState, figureB, authority);
			await EnterHex(potentialAbilityState, figureB, authority, hexA,
				triggerHexEffects: true, setPosition: true, forcedMovement: figureB.EnemiesWith(authority));
		}
		else
		{
			hexObjectB.SetOriginHexAndRotation(hexA, setPosition: true);
		}

		if(figureA != null)
		{
			await EnterHex(potentialAbilityState, figureA, authority, hexB,
				triggerHexEffects: true, setPosition: true, forcedMovement: figureA.EnemiesWith(authority));
			potentialAbilityState?.SetPerformed();
		}
		else
		{
			hexObjectA.SetOriginHexAndRotation(hexB, setPosition: true);
		}

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

	public static bool CanConsumeElements(List<CardElementConsumption> consumptions, Figure potentialConsumer)
	{
		List<Element> remainingElements = GetAvailableElementsToConsume(consumptions, potentialConsumer);
		foreach(CardElementConsumption elementConsumption in consumptions)
		{
			foreach(Element element in elementConsumption.ConsumableElements.Except(remainingElements))
			{
				if(CanConsumeElement(element, potentialConsumer))
				{
					remainingElements.Add(element);
				}
			}
		}

		List<CardElementConsumption> orderedConsumptions = consumptions.OrderBy(consumption => consumption.ConsumableElements.Count).ToList();

		return TryMatchConsumptions(orderedConsumptions, remainingElements, 0);
	}

	private static bool TryMatchConsumptions(List<CardElementConsumption> consumptions, List<Element> remainingElements, int index)
	{
		if(index >= consumptions.Count)
		{
			return true;
		}

		CardElementConsumption consumption = consumptions[index];

		foreach(Element element in consumption.ConsumableElements)
		{
			int elementIndex = remainingElements.IndexOf(element);
			if(elementIndex >= 0)
			{
				remainingElements.RemoveAt(elementIndex);
				if(TryMatchConsumptions(consumptions, remainingElements, index + 1))
				{
					return true;
				}

				remainingElements.Insert(elementIndex, element);
			}
		}

		return false;
	}

	private static List<List<Element>> FindConsumptionPossibilities(List<CardElementConsumption> consumptions, Figure potentialConsumer)
	{
		List<Element> remainingElements = GetAvailableElementsToConsume(consumptions, potentialConsumer);

		List<List<Element>> possibilities = [];

		List<CardElementConsumption> orderedConsumptions = consumptions.OrderBy(consumption => consumption.ConsumableElements.Count).ToList();

		TryMatchConsumptionPossibilities(orderedConsumptions, remainingElements, 0, [], possibilities);
		return possibilities;
	}

	private static void TryMatchConsumptionPossibilities(List<CardElementConsumption> consumptions, List<Element> remainingElements, int index,
		List<Element> current, List<List<Element>> possibilities)
	{
		if(index >= consumptions.Count)
		{
			List<Element> sorted = current.OrderBy(e => e).ToList();

			if(!possibilities.Any(possibility => possibility.SequenceEqual(sorted)))
			{
				possibilities.Add(sorted);
			}

			return;
		}

		foreach(Element element in consumptions[index].ConsumableElements)
		{
			int elementIndex = remainingElements.IndexOf(element);
			if(elementIndex >= 0)
			{
				remainingElements.RemoveAt(elementIndex);
				current.Add(element);
				TryMatchConsumptionPossibilities(consumptions, remainingElements, index + 1, current, possibilities);
				current.RemoveAt(current.Count - 1);
				remainingElements.Insert(elementIndex, element);
			}
		}
	}

	private static List<Element> GetAvailableElementsToConsume(List<CardElementConsumption> consumptions, Figure potentialConsumer)
	{
		List<Element> remainingElements = [];
		foreach(CardElementConsumption elementConsumption in consumptions)
		{
			foreach(Element element in elementConsumption.ConsumableElements.Except(remainingElements))
			{
				if(CanConsumeElement(element, potentialConsumer))
				{
					remainingElements.Add(element);
				}
			}
		}

		return remainingElements;
	}

	public static void LinkHexes(Hex hex1, Hex hex2)
	{
		hex1.AddNeighbour(hex2);
		hex2.AddNeighbour(hex1);
	}
}