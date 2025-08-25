using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

/// <summary>
/// A signature <see cref="ActiveAbility{T}"/> of the <see cref="BombardModel"/> class.
/// </summary>
public class ProjectileAbility : ActiveAbility<ProjectileAbility.State>
{
	public class State : ActiveAbilityState
	{
		public List<BombardProjectileToken> Tokens { get; } = new List<BombardProjectileToken>();

		public void AddToken(BombardProjectileToken token)
		{
			Tokens.Add(token);
		}
	}

	protected Func<Hex, List<Ability>> _getAbilities { get; set; }
	public AbilityCardSide AbilityCardSide { get; protected set; }

	public int Range { get; protected set; }
	public int Targets { get; protected set; } = 1;

	/// <summary>
	/// A builder extending <see cref="Ability{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in ProjectileAbility. Enables inheritors of ProjectileAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending ProjectileAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : ActiveAbility<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IGetAbilitiesStep,
		AbstractBuilder<TBuilder, TAbility>.IAbilityCardSideStep,
		AbstractBuilder<TBuilder, TAbility>.IRangeStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : ProjectileAbility, new()
	{
		
		public interface IGetAbilitiesStep
		{
			IAbilityCardSideStep WithGetAbilities(Func<Hex, List<Ability>> getAbilities);
		}
		
		public interface IAbilityCardSideStep
		{
			IRangeStep WithAbilityCardSide(AbilityCardSide abilityCardSide);
		}
		
		public interface IRangeStep
		{
			TBuilder WithRange(int range);
		}
		
		public IAbilityCardSideStep WithGetAbilities(Func<Hex, List<Ability>> getAbilities)
		{
			Obj._getAbilities = getAbilities;
			return (TBuilder)this;
		}

		public IRangeStep WithAbilityCardSide(AbilityCardSide abilityCardSide)
		{
			Obj.AbilityCardSide = abilityCardSide;
			return (TBuilder)this;
		}

		public TBuilder WithRange(int range)
		{
			Obj.Range = range;
			return (TBuilder)this;
		}

		public TBuilder WithTargets(int targets)
		{
			Obj.Targets = targets;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class ProjectileBuilder : AbstractBuilder<ProjectileBuilder, ProjectileAbility>
	{
		internal ProjectileBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of ProjectileBuilder.
	/// </summary>
	/// <returns></returns>
	public static ProjectileBuilder.IGetAbilitiesStep Builder()
	{
		return new ProjectileBuilder();
	}

	public ProjectileAbility() { }

	public ProjectileAbility(int range, Func<Hex, List<Ability>> getAbilities, AbilityCardSide abilityCardSide, int targets = 1,
		Func<State, GDTask> onAbilityStarted = null, Func<State, GDTask> onAbilityEnded = null, Func<State, GDTask> onAbilityEndedPerformed = null,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		Func<State, string> getHintText = null,
		List<ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription> abilityEndedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions = null)
		: base(onAbilityStarted, onAbilityEnded, onAbilityEndedPerformed, conditionalAbilityCheck, getHintText, abilityStartedSubscriptions,
			abilityEndedSubscriptions, abilityPerformedSubscriptions)
	{
		_getAbilities = getAbilities;
		Range = range;
		AbilityCardSide = abilityCardSide;
		Targets = targets;
	}

	protected override async GDTask Perform(State abilityState)
	{
		for(int i = 0; i < Targets; i++)
		{
			Hex targetedHex = await AbilityCmd.SelectHex(abilityState, list =>
			{
				RangeHelper.FindHexesInRange(abilityState.Performer.Hex, Range, true, list);
			}, hintText: "Select a hex to target with the Projectile ability");

			if(targetedHex != null)
			{
				BombardProjectileToken token = ResourceLoader.Load<PackedScene>("res://Content/Classes/Bombard/BombardProjectile.tscn")
					.Instantiate<BombardProjectileToken>();
				GameController.Instance.Map.AddChild(token);
				token.SetCardSide(AbilityCardSide);

				await token.Init(targetedHex);

				abilityState.AddToken(token);
			}
		}

		if(abilityState.Tokens.Count > 0)
		{
			await Activate(abilityState);

			ScenarioEvents.FigureTurnStartedEvent.Subscribe(abilityState, this,
				canApplyParameters => canApplyParameters.Figure == abilityState.Performer,
				async applyParameters =>
				{
					foreach(BombardProjectileToken token in abilityState.Tokens)
					{
						bool targetFound = false;

						foreach(Figure figure in token.Hex.GetHexObjectsOfType<Figure>())
						{
							if(abilityState.Authority.EnemiesWith(figure))
							{
								targetFound = true;
							}
						}

						if(targetFound)
						{
							// Perform the actual abilities
							ActionState actionState = new ActionState(abilityState.Performer, _getAbilities(token.Hex), abilityState.ActionState);
							await actionState.Perform();
						}
					}

					await abilityState.ActionState.RequestDiscardOrLose();
				}
			);
		}
	}

	protected override async GDTask Activate(State abilityState)
	{
		await base.Activate(abilityState);
	}

	protected override async GDTask Deactivate(State abilityState)
	{
		await base.Deactivate(abilityState);

		ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(abilityState, this);

		foreach(BombardProjectileToken token in abilityState.Tokens)
		{
			await token.Destroy();
		}
	}
}