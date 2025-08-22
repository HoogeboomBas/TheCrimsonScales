using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;

/// <summary>
/// An <see cref="ActiveAbility{T}"/> that creates a summon ally.
/// </summary>
public class SummonAbility : ActiveAbility<SummonAbility.State>
{
	public class State : ActiveAbilityState
	{
		public Summon Summon { get; private set; }

		public void SetSummon(Summon summon)
		{
			Summon = summon;
		}
	}

	protected SummonStats SummonStats { get; set; }
	protected string Name { get; set; }
	protected string TexturePath { get; set; }
	protected Action<State, List<Hex>> GetValidHexes { get; set; }

	/// <summary>
	/// A builder extending <see cref="ActiveAbility{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in SummonAbility. Enables inheritors of SummonAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending SummonAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : ActiveAbility<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.ISummonStatsStep,
		AbstractBuilder<TBuilder, TAbility>.INameStep,
		AbstractBuilder<TBuilder, TAbility>.ITexturePathStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : SummonAbility, new()
	{

		public interface ISummonStatsStep
		{
			INameStep WithSummonStats(SummonStats summonStats);
		}

		public interface INameStep
		{
			ITexturePathStep WithName(string name);
		}

		public interface ITexturePathStep
		{
			TBuilder WithTexturePath(string texturePath);
		}

		public INameStep WithSummonStats(SummonStats summonStats)
		{
			Obj.SummonStats = summonStats;
			return (TBuilder)this;
		}

		public ITexturePathStep WithName(string name)
		{
			Obj.Name = name;
			return (TBuilder)this;
		}

		public TBuilder WithTexturePath(string texturePath)
		{
			Obj.TexturePath = texturePath;
			return (TBuilder)this;
		}

		public TBuilder WithGetValidHexes(
			Action<State, List<Hex>> getValidHexes)
		{
			Obj.GetValidHexes = getValidHexes;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class SummonBuilder : AbstractBuilder<SummonBuilder, SummonAbility>
	{
		internal SummonBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of SummonBuilder.
	/// </summary>
	/// <returns></returns>
	public static AbstractBuilder<SummonBuilder, SummonAbility>.ISummonStatsStep Builder()
	{
		return new SummonBuilder();
	}

	public SummonAbility() { }

	public SummonAbility(SummonStats summonStats, string name, string texturePath, Action<State, List<Hex>> getValidHexes = null,
		Func<State, GDTask> onAbilityStarted = null, Func<State, GDTask> onAbilityEnded = null, Func<State, GDTask> onAbilityEndedPerformed = null,
		ConditionalAbilityCheckDelegate conditionalAbilityCheck = null,
		Func<State, string> getHintText = null,
		List<ScenarioEvent<ScenarioEvents.AbilityStarted.Parameters>.Subscription> abilityStartedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityEnded.Parameters>.Subscription> abilityEndedSubscriptions = null,
		List<ScenarioEvent<ScenarioEvents.AbilityPerformed.Parameters>.Subscription> abilityPerformedSubscriptions = null)
		: base(onAbilityStarted, onAbilityEnded, onAbilityEndedPerformed, conditionalAbilityCheck, getHintText, abilityStartedSubscriptions,
			abilityEndedSubscriptions, abilityPerformedSubscriptions)
	{
		SummonStats = summonStats;
		Name = name;
		TexturePath = texturePath;
		GetValidHexes = getValidHexes;
	}

	protected override async GDTask Perform(State abilityState)
	{
		// Target a hex within range
		Hex targetedHex = await AbilityCmd.SelectHex(abilityState, list =>
		{
			if(GetValidHexes == null)
			{
				RangeHelper.FindHexesInRange(abilityState.Performer.Hex, 1, true, list);

				for(int i = list.Count - 1; i >= 0; i--)
				{
					Hex hex = list[i];

					if(!hex.IsEmpty())
					{
						list.RemoveAt(i);
					}
				}
			}
			else
			{
				GetValidHexes(abilityState, list);
			}
		});

		if(targetedHex != null)
		{
			PackedScene summonScene = ResourceLoader.Load<PackedScene>("res://Scenes/Scenario/Summon.tscn");
			Summon summon = summonScene.Instantiate<Summon>();
			GameController.Instance.Map.AddChild(summon);
			await summon.Init(targetedHex);
			summon.Spawn(SummonStats, (Character)abilityState.Performer, Name, TexturePath);
			abilityState.SetSummon(summon);

			summon.Scale = Vector2.Zero;
			await summon.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardableAsync();


			ScenarioEvents.FigureKilledEvent.Subscribe(abilityState, this,
				canApplyParameters => canApplyParameters.Figure == summon,
				async applyParameters =>
				{
					await abilityState.ActionState.RequestDiscardOrLose();
				});

			await Activate(abilityState);
		}
	}

	protected override async GDTask Activate(State abilityState)
	{
		await base.Activate(abilityState);
	}

	protected override async GDTask Deactivate(State abilityState)
	{
		await base.Deactivate(abilityState);

		ScenarioEvents.FigureKilledEvent.Unsubscribe(abilityState, this);

		if(abilityState.Summon != null && !abilityState.Summon.IsDead)
		{
			await abilityState.Summon.Destroy();
		}
	}
}