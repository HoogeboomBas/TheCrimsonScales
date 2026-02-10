using System;
using System.Collections.Generic;
using Godot;
using System.Linq;
using Fractural.Tasks;
using GTweens.Easings;
using GTweensGodot.Extensions;

public abstract class AmberAegisCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : AmberAegisCardSide
	where TBottom : AmberAegisCardSide
{
	protected override string TexturePath => "res://Content/Classes/AmberAegis/Cards.jpg";
	protected override int ColumnCount => 8;
	protected override int RowCount => 4;
}

public abstract class AmberAegisCardSide : AbilityCardSideModel<Character>
{
	protected OtherAbility PlaceColonyTokenAbility<T>(List<Element> elements = null)
		where T : ColonyToken, IColonyToken
	{
		return OtherAbility.Builder()
			.WithPerformAbility(async state =>
			{
				await PlaceColonyToken<T>(state, list => list.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 2)
					.Where(hex => hex.IsEmpty() && !hex.HasHexObjectOfType<ColonyToken>())));
			})
			.WithAbilityPerformedSubscription(
				ScenarioEvents.AbilityPerformed.Subscription.ConsumeWildElement(
					parameters => elements != null,
					async parameters =>
					{
						if(elements?.Count == 1)
						{
							await AbilityCmd.InfuseElement(parameters.AbilityState, elements[0]);
						}
						else
						{
							await AbilityCmd.InfuseElement(parameters.AbilityState, elements);
						}
					}, effectInfoViewParameters: new TextEffectInfoView.Parameters(elements == null
						? ""
						: $"{Icons.Inline(Icons.GetElement(elements[0]))}" +
						  (elements.Count > 1 ? $" or {Icons.Inline(Icons.GetElement(elements[1]))}" : ""))))
			.Build();
	}

	private async GDTask<ColonyToken> PlaceColonyToken<T>(AbilityState state, Action<List<Hex>> getValidHexes)
		where T : ColonyToken, IColonyToken
	{
		await AtColonyTokenLimit<T>(state);
		Hex hex = await AbilityCmd.SelectHex(state, getValidHexes,
			hintText: $"Select a hex to place {Icons.HintText(T.IconPath)}");
		if(hex == null)
		{
			return null;
		}

		PackedScene scene = ResourceLoader.Load<PackedScene>(T.ScenePath);
		T colonyToken = scene.Instantiate<T>();
		GameController.Instance.Map.AddChild(colonyToken);
		await colonyToken.Init(hex);

		colonyToken.Scale = Vector2.Zero;
		colonyToken.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardable();
		colonyToken.DestroyEvent += async hexObject => await AbilityCmd.SufferDamage(state.Performer, 1, state.Performer);
		state.SetPerformed();
		return colonyToken;
	}


	protected async GDTask<ColonyToken> PlaceAnyColonyToken(AbilityState state, Action<List<Hex>> getValidHexes)
	{
		ColonyToken colonyToken = null;
		List<ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription> subscriptions = [];
		subscriptions.Add(ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
			parameters => true,
			async parameters =>
			{
				colonyToken = await PlaceColonyToken<RockspineTermiteColony>(state, getValidHexes);
			},
			effectType: EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(RockspineTermiteColony.IconPath),
			effectInfoViewParameters: new TextEffectInfoView.Parameters(
				$"Place a {Icons.Inline(RockspineTermiteColony.IconPath)}")
		));

		subscriptions.Add(ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
			parameters => true,
			async parameters =>
			{
				colonyToken = await PlaceColonyToken<GhostshimmerBeeColony>(state, getValidHexes);
			},
			effectType: EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(GhostshimmerBeeColony.IconPath),
			effectInfoViewParameters: new TextEffectInfoView.Parameters(
				$"Place a {Icons.Inline(GhostshimmerBeeColony.IconPath)}")
		));

		subscriptions.Add(ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
			parameters => true,
			async parameters =>
			{
				colonyToken = await PlaceColonyToken<FirespitterAntColony>(state, getValidHexes);
			},
			effectType: EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(FirespitterAntColony.IconPath),
			effectInfoViewParameters: new TextEffectInfoView.Parameters(
				$"Place a {Icons.Inline(FirespitterAntColony.IconPath)}")
		));

		subscriptions.Add(ScenarioEvent<ScenarioEvents.GenericChoice.Parameters>.Subscription.New(
			parameters => true,
			async parameters =>
			{
				colonyToken = await PlaceColonyToken<DeathshroudSpiderColony>(state, getValidHexes);
			},
			effectType: EffectType.Selectable,
			effectButtonParameters: new IconEffectButton.Parameters(DeathshroudSpiderColony.IconPath),
			effectInfoViewParameters: new TextEffectInfoView.Parameters(
				$"Place a {Icons.Inline(DeathshroudSpiderColony.IconPath)}")
		));

		await AbilityCmd.GenericChoice(state.Performer, subscriptions, hintText: $"Choose a {Icons.HintText(ColonyToken.AnyColony)} to place");

		return colonyToken;
	}

	private async GDTask AtColonyTokenLimit<T>(AbilityState state)
		where T : ColonyToken, IColonyToken
	{
		if(GameController.Instance.Map.Hexes.Values.Count(hex => hex.HasHexObjectOfType<T>()) < T.MaxCount)
		{
			return;
		}

		Hex hex = await AbilityCmd.SelectHex(state,
			list => list.AddRange(GameController.Instance.Map.Hexes.Values.Where(hex => hex.HasHexObjectOfType<T>())), true,
			$"Select a {Icons.HintText(T.IconPath)} to remove");
		//TODO: Change to select overlaytile
		hex.GetHexObjectOfType<T>().RemoveFromMap();
	}

	protected bool IsAdjacentToColonyToken<T>(Figure figure)
		where T : ColonyToken
	{
		return IsAdjacentToColonyToken<T>(figure.Hex);
	}

	protected bool IsAdjacentToColonyToken<T>(Hex hex)
		where T : ColonyToken
	{
		return GameController.Instance.Map.Hexes.Values
			.Where(mapHex => mapHex.HasHexObjectOfType<T>())
			.Any(tokenHex => RangeHelper.Distance(hex, tokenHex) <= 1);
	}

	protected async GDTask MoveColonyToken(AbilityState state, int hexes, Func<Hex, ActionState, GDTask> onColonyMoved = null)
	{
		Hex hex = await AbilityCmd.SelectHex(state,
			list => list.AddRange(GameController.Instance.Map.Hexes.Values.Where(hex => hex.HasHexObjectOfType<ColonyToken>())),
			hintText: $"Select a {Icons.HintText(ColonyToken.AnyColony)} to move");
		//TODO: Change to selecting the colony token
		if(hex == null)
		{
			return;
		}

		ColonyToken colonyToken = hex.GetHexObjectOfType<ColonyToken>();

		for(int i = 0; i < hexes; i++)
		{
			Hex moveToHex = await AbilityCmd.SelectHex(state,
				list => list.AddRange(RangeHelper.GetHexesInRange(colonyToken.Hex, 1)
					.Where(movetoHex => movetoHex.IsEmpty() && !movetoHex.HasHexObjectOfType<ColonyToken>())),
				hintText: $"Select a hex to move {Icons.HintText(ColonyToken.AnyColony)} to");
			if(moveToHex == null)
			{
				break;
			}

			await colonyToken.TweenGlobalPosition(moveToHex.GlobalPosition, 0.3f).SetEasing(Easing.OutSine)
				.PlayFastForwardableAsync();
			await GDTask.DelayFastForwardable(0.03f);
			colonyToken.SetOriginHexAndRotation(moveToHex);
			state.SetPerformed();
			if(onColonyMoved != null)
			{
				await onColonyMoved.Invoke(moveToHex, state.ActionState);
			}
		}
	}
}