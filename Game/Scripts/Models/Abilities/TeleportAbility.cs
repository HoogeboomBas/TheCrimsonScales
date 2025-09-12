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

		if(abilityState.Authority is Character)
		{
			// Character teleporting
			TeleportPrompt.Answer teleportAnswer =
				await PromptManager.Prompt(
					new TeleportPrompt(abilityState, performer, null,
						() => $"Select a destination for {Icons.HintText(Icons.Teleport)}{abilityState.Distance}"),
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

		ScreenDistortion screenDistortion = GameController.Instance.ScreenDistortion;
		screenDistortion.Open(GameController.Instance.CharacterManager.GetCharacter(0));

		const float animationSpeed = 1.4f;
		const float radius = 0.7f;

		screenDistortion.SetPower(1f);
		screenDistortion.SetRadius(0.4f * radius);

		AppController.Instance.AudioController.Play("res://Audio/SFX/WHOOSH_Steam_Fast_01_mono.wav", 0.9f, 1.1f, delay: 0.0f);

		if(!GameController.FastForward)
		{
			// Disappear
			await GTweenSequenceBuilder.New()
				.Append(screenDistortion.TweenPower(1.1f, 0.2f / animationSpeed).SetEasing(Easing.OutCubic))
				.Join(screenDistortion.TweenRadius(0.4f * radius, 0.2f / animationSpeed).SetEasing(Easing.OutCubic))
				.Append(screenDistortion.TweenPower(0.4f, 0.5f / animationSpeed).SetEasing(Easing.OutCubic))
				.Join(screenDistortion.TweenRadius(0.3f * radius, 0.5f / animationSpeed).SetEasing(Easing.OutCubic))
				.Join(performer.TweenScale(0f, 0.5f / animationSpeed).SetEasing(Easing.Linear))
				.Append(screenDistortion.TweenPower(1f, 0.5f / animationSpeed).SetEasing(Easing.OutBack))
				.Join(screenDistortion.TweenRadius(0.4f * radius, 0.2f / animationSpeed).SetEasing(Easing.OutCubic))
				.Build().PlayFastForwardableAsync();
		}

		performer.SetOriginHexAndRotation(destination);

		AppController.Instance.AudioController.Play("res://Audio/SFX/WHOOSH_Steam_Fast_01_mono.wav", 0.9f, 1.1f, delay: 0.0f);

		if(!GameController.FastForward)
		{
			// Appear
			await GTweenSequenceBuilder.New()
				.Append(screenDistortion.TweenPower(1.1f, 0.2f / animationSpeed).SetEasing(Easing.OutCubic))
				.Join(screenDistortion.TweenRadius(0.4f * radius, 0.2f / animationSpeed).SetEasing(Easing.OutCubic))
				.Append(screenDistortion.TweenPower(0.4f, 0.5f / animationSpeed).SetEasing(Easing.OutCubic))
				.Join(screenDistortion.TweenRadius(0.3f * radius, 0.5f / animationSpeed).SetEasing(Easing.OutCubic))
				.Append(screenDistortion.TweenPower(1f, 0.5f / animationSpeed).SetEasing(Easing.OutBack))
				.Join(screenDistortion.TweenRadius(0.4f * radius, 0.2f / animationSpeed).SetEasing(Easing.OutCubic))
				.Join(performer.TweenScale(1f, 0.2f / animationSpeed).SetEasing(Easing.OutCubic))
				.Build().PlayFastForwardableAsync();
		}

		screenDistortion.Close();

		await AbilityCmd.EnterHex(abilityState, performer, abilityState.Authority, destination, true);
	}
}