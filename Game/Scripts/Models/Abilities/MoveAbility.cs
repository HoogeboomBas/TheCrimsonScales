using System.Collections.Generic;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

/// <summary>
/// An <see cref="Ability{T}"/> that allows figures to move.
/// </summary>
public class MoveAbility : Ability<MoveAbility.State>
{
	public class State : AbilityState
	{
		public Hex Origin { get; set; }
		public List<Hex> Hexes { get; } = new List<Hex>();

		public int MoveValue { get; set; }
		public MoveType MoveType { get; set; }

		public void AdjustMoveValue(int amount)
		{
			MoveValue += amount;
		}

		public void AdjustMoveType(MoveType moveType)
		{
			MoveType = moveType;
		}

		public void AddJump()
		{
			if(MoveType == MoveType.Regular)
			{
				MoveType = MoveType.Jump;
			}
		}
	}

	public int Distance { get; private set; }
	public MoveType MoveType { get; private set; }
	public List<ScenarioEvents.DuringMovement.Subscription> DuringMovementSubscriptions { get; private set; } = [];
	//public List<ScenarioEvent<ScenarioEvents.FigureEnteredHex.Parameters>.Subscription> FigureEnteredHexSubscriptions { get; }

	/// <summary>
	/// A builder extending <see cref="Ability{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in MoveAbility. Enables inheritors of MoveAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending MoveAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : Ability<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IDistanceStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : MoveAbility, new()
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

		public TBuilder WithMoveType(MoveType moveType)
		{
			Obj.MoveType = moveType;
			return (TBuilder)this;
		}

		public TBuilder WithDuringMovementSubscription(ScenarioEvents.DuringMovement.Subscription movementSubscription)
		{
			Obj.DuringMovementSubscriptions.Add(movementSubscription);
			return (TBuilder)this;
		}

		public TBuilder WithDuringMovementSubscriptions(
			List<ScenarioEvents.DuringMovement.Subscription> movementSubscriptions)
		{
			Obj.DuringMovementSubscriptions = movementSubscriptions;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class MoveBuilder : AbstractBuilder<MoveBuilder, MoveAbility>
	{
		internal MoveBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of MoveBuilder.
	/// </summary>
	/// <returns></returns>
	public static MoveBuilder.IDistanceStep Builder()
	{
		return new MoveBuilder();
	}

	public MoveAbility() { }

	protected override void InitializeState(State abilityState)
	{
		base.InitializeState(abilityState);

		MoveType moveType = MoveType;
		Figure performer = abilityState.Performer;

		ScenarioCheckEvents.FlyingCheck.Parameters flyingCheckParameters =
			ScenarioCheckEvents.FlyingCheckEvent.Fire(new ScenarioCheckEvents.FlyingCheck.Parameters(performer));

		if(flyingCheckParameters.HasFlying)
		{
			moveType = MoveType.Flying;
		}

		abilityState.Origin = performer.Hex;
		abilityState.MoveValue = Distance;
		abilityState.MoveType = moveType;
	}

	protected override async GDTask Perform(State abilityState)
	{
		Figure performer = abilityState.Performer;

		async GDTask MovePath(List<Vector2I> path)
		{
			if(abilityState.MoveType == MoveType.Jump)
			{
				AppController.Instance.AudioController.PlayFastForwardable(SFX.GetJump(performer.Hex), delay: 0.0f);
			}

			bool playedLandSound = false;

			for(int i = 0; i < path.Count && !performer.IsDestroyed; i++)
			{
				ScenarioEvents.CanMoveFurtherCheck.Parameters canMoveFurtherParameters =
					await ScenarioEvents.CanMoveFurtherCheckEvent.CreatePrompt(new ScenarioEvents.CanMoveFurtherCheck.Parameters(performer));

				if(!canMoveFurtherParameters.CanMoveFurther)
				{
					abilityState.MoveValue = 0;
					break;
				}

				Vector2I coords = path[i];
				Hex hex = GameController.Instance.Map.GetHex(coords);

				if(abilityState.MoveType == MoveType.Regular)
				{
					AppController.Instance.AudioController.PlayFastForwardable(SFX.GetStep(hex), delay: 0.1f);
				}

				if(abilityState.MoveType == MoveType.Flying)
				{
					AppController.Instance.AudioController.PlayFastForwardable(SFX.MoveFlying, minPitch: 2.5f,
						maxPitch: 3.4f, delay: 0.1f);
				}

				if(abilityState.MoveType == MoveType.Jump && i == path.Count - 1)
				{
					playedLandSound = true;
					AppController.Instance.AudioController.PlayFastForwardable(SFX.GetLand(performer.Hex),
						delay: 0.25f);
				}

				abilityState.Hexes.Add(hex);

				ScenarioEvents.MoveTogetherCheck.Parameters moveTogetherCheckParameters =
					await ScenarioEvents.MoveTogetherCheckEvent.CreatePrompt(new ScenarioEvents.MoveTogetherCheck.Parameters(performer));

				if(moveTogetherCheckParameters.OtherFigure != null)
				{
					moveTogetherCheckParameters.OtherFigure.TweenGlobalPosition(hex.GlobalPosition, 0.3f).SetEasing(Easing.OutSine).PlayFastForwardable();
				}

				await performer.TweenGlobalPosition(hex.GlobalPosition, 0.3f).SetEasing(Easing.OutSine)
					.PlayFastForwardableAsync();
				
				await GDTask.DelayFastForwardable(0.03f);
				bool triggerHexEffects = abilityState.MoveType == MoveType.Regular || (abilityState.MoveType == MoveType.Jump && i == path.Count - 1);
				await AbilityCmd.EnterHex(abilityState, performer, abilityState.Authority, hex, triggerHexEffects);

				if(moveTogetherCheckParameters.OtherFigure != null)
				{
					await AbilityCmd.EnterHex(abilityState, moveTogetherCheckParameters.OtherFigure, abilityState.Authority, hex, triggerHexEffects);
				}
			}

			if(abilityState.MoveType == MoveType.Jump && !playedLandSound)
			{
				AppController.Instance.AudioController.PlayFastForwardable(SFX.GetLand(performer.Hex), delay: 0f);
			}
		}

		ScenarioEvents.DuringMovementEvent.Subscribe(abilityState, this, DuringMovementSubscriptions);

		if(abilityState.Authority is Character)
		{
			// Character moving
			ScenarioEvents.DuringMovement.Parameters duringMovementAbilityStateParameters =
				new ScenarioEvents.DuringMovement.Parameters(abilityState);

			performer.SetZIndex(100);

			while(abilityState.MoveValue > 0)
			{
				if(performer.IsDestroyed)
				{
					break;
				}

				EffectCollection effectCollection =
					ScenarioEvents.DuringMovementEvent.CreateEffectCollection(duringMovementAbilityStateParameters);

				MovePrompt.Answer moveAnswer =
					await PromptManager.Prompt(
						new MovePrompt(abilityState, performer, effectCollection,
							() => $"Select a path for {Icons.HintText(Icons.Move)}{abilityState.MoveValue}"),
						abilityState.Authority);

				if(moveAnswer.Skipped)
				{
					break;
				}

				await MovePath(moveAnswer.Path);

				abilityState.AdjustMoveValue(-moveAnswer.MoveSpent);

				abilityState.SetPerformed();
			}

			performer.SetZIndex(performer.DefaultZIndex);
		}
		else
		{
			// Monster moving
			MonsterMovePrompt.Answer monsterMoveAnswer = await PromptManager.Prompt(
				new MonsterMovePrompt(abilityState, performer, abilityState.ActionState.GetAIMoveParameters(),
					await abilityState.ActionState.GetFocus(),
					null, () => "Select a path"), abilityState.Authority);

			performer.SetZIndex(100);

			if(!monsterMoveAnswer.Skipped)
			{
				await MovePath(monsterMoveAnswer.Path);
				abilityState.SetPerformed();
			}

			performer.SetZIndex(performer.DefaultZIndex);
		}

		ScenarioEvents.DuringMovementEvent.Unsubscribe(DuringMovementSubscriptions);
	}
}