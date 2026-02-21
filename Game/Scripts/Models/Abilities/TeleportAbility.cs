using System;
using System.Collections.Generic;
using Fractural.Tasks;
using GTweens.Builders;
using GTweens.Easings;

/// <summary>
/// An <see cref="Ability{T}"/> that allows figures to teleport to a target hex.
/// </summary>
public class TeleportAbility : Ability<TeleportAbility.State>
{
	public class State : AbilityState
	{
		public Hex Origin { get; set; }

		public int Distance { get; set; }

		public void AdjustDistance(int amount)
		{
			Distance += amount;
		}
	}

	public int Distance { get; private set; }
	public Action<State, List<Hex>> CustomGetHexes { get; set; }

	/// <summary>
	/// A builder extending <see cref="Ability{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in TeleportAbility. Enables inheritors of TeleportAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending TeleportAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : Ability<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IDistanceStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : TeleportAbility, new()
	{
		public interface IDistanceStep
		{
			TBuilder WithDistance(int distance);
		}

		public TBuilder WithDistance(int distance)
		{
			Obj.Distance = distance;
			return (TBuilder)this;
		}
		
		public TBuilder WithCustomGetHexes(Action<State, List<Hex>> getHexes)
		{
			Obj.CustomGetHexes = getHexes;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class TeleportBuilder : AbstractBuilder<TeleportBuilder, TeleportAbility>
	{
		internal TeleportBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of TeleportBuilder.
	/// </summary>
	/// <returns></returns>
	public static TeleportBuilder.IDistanceStep Builder()
	{
		return new TeleportBuilder();
	}

	public TeleportAbility() { }

	protected override void InitializeState(State abilityState)
	{
		base.InitializeState(abilityState);

		Figure performer = abilityState.Performer;

		abilityState.Origin = performer.Hex;
		abilityState.Distance = Distance;
	}

	protected override async GDTask Perform(State abilityState)
	{
		Figure performer = abilityState.Performer;

		Hex destination = null;

		List<Hex> customHexes = CustomGetHexes == null ? null : [];
		CustomGetHexes?.Invoke(abilityState, customHexes);

		if(abilityState.Authority is Character)
		{
			// Character teleporting
			TeleportPrompt.Answer teleportAnswer =
				await PromptManager.Prompt(
					new TeleportPrompt(abilityState, performer, null, customHexes: customHexes,
						getHintText: () => $"Select a destination for {Icons.HintText(Icons.Teleport)}{abilityState.Distance}"),
					abilityState.Authority);

			if(!teleportAnswer.Skipped)
			{
				destination = GameController.Instance.Map.GetHex(teleportAnswer.DestinationCoords);
			}
		}
		else
		{
			// Monster teleporting is not implemented (yet)
		}

		if(destination == null)
		{
			return;
		}

		abilityState.SetPerformed();

		await AbilityCmd.ExitHex(abilityState, performer, abilityState.Authority);

		const float animationSpeed = 1.4f;

		if(!GameController.FastForward)
		{
			// Disappear
			await GameController.Instance.ScreenDistortion.Disappear(performer, animationSpeed, true).PlayFastForwardableAsync();
		}

		performer.SetOriginHexAndRotation(destination);

		if(!GameController.FastForward)
		{
			// Appear
			await GameController.Instance.ScreenDistortion.Appear(performer, animationSpeed, true).PlayFastForwardableAsync();
		}

		await AbilityCmd.EnterHex(abilityState, performer, abilityState.Authority, destination, true, true);
	}
}