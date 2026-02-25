using System;
using System.Collections.Generic;
using System.Linq;
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
		public string Name { get; set; }
		public int Health { get; set; }
		public int? Move { get; set; }
		public int? Attack { get; set; }
		public int? Range { get; set; }
		public List<FigureTrait> Traits { get; set; }

		public Summon Summon { get; private set; }

		public void SetSummon(Summon summon)
		{
			Summon = summon;
		}

		public void AdjustHealth(int amount)
		{
			Health += amount;
		}

		public void AdjustMove(int amount)
		{
			Move += amount;
		}

		public void AdjustAttack(int amount)
		{
			Attack += amount;
		}

		public void AdjustRange(int amount)
		{
			Range += amount;
		}
	}

	private string _texturePath;
	private string _mapIconTexturePath;
	private Action<State, List<Hex>> _getValidHexes;

	public string Name { get; private set; }
	public int Health { get; private set; }
	public int? Move { get; private set; }
	public int? Attack { get; private set; }
	public int? Range { get; private set; }
	public FigureTrait[] Traits { get; private set; } = [];

	/// <summary>
	/// A builder extending <see cref="ActiveAbility{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in SummonAbility. Enables inheritors of SummonAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending SummonAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : ActiveAbility<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.INameStep,
		AbstractBuilder<TBuilder, TAbility>.ITexturePathStep,
		AbstractBuilder<TBuilder, TAbility>.IHealthStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : SummonAbility, new()
	{
		public interface INameStep
		{
			ITexturePathStep WithName(string name);
		}

		public interface ITexturePathStep
		{
			IHealthStep WithTexturePath(string texturePath);
		}

		public interface IHealthStep
		{
			TBuilder WithHealth(int health, params SummonHealthSquare[] enhancementMarks);
		}

		public ITexturePathStep WithName(string name)
		{
			Obj.Name = name;
			return (TBuilder)this;
		}

		public IHealthStep WithTexturePath(string texturePath)
		{
			Obj._texturePath = texturePath;
			Obj._mapIconTexturePath = $"{texturePath.GetBaseName()}MapIcon.tres";
			return (TBuilder)this;
		}

		public TBuilder WithHealth(int health, params SummonHealthSquare[] enhancementMarks)
		{
			Obj.Health = health;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithMove(int move, params SummonMoveSquare[] enhancementMarks)
		{
			Obj.Move = move;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithAttack(int attack, params SummonAttackSquare[] enhancementMarks)
		{
			Obj.Attack = attack;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithRange(int range, params SummonRangeSquare[] enhancementMarks)
		{
			Obj.Range = range;
			AddEnhancements(enhancementMarks);
			return (TBuilder)this;
		}

		public TBuilder WithTraits(params FigureTrait[] traits)
		{
			Obj.Traits = traits;
			return (TBuilder)this;
		}

		public TBuilder WithGetValidHexes(
			Action<State, List<Hex>> getValidHexes)
		{
			Obj._getValidHexes = getValidHexes;
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
	public static SummonBuilder.INameStep Builder()
	{
		return new SummonBuilder();
	}

	public SummonAbility() { }

	protected override void InitializeState(State abilityState)
	{
		base.InitializeState(abilityState);

		abilityState.Name = Name;
		abilityState.Health = Health;
		abilityState.Move = Move;
		abilityState.Attack = Attack;
		abilityState.Range = Range;
		abilityState.Traits = Traits?.ToList() ?? [];
	}

	protected override async GDTask Perform(State abilityState)
	{
		// Target a hex within range
		Hex targetedHex = await AbilityCmd.SelectHex(abilityState, list =>
			{
				if(_getValidHexes == null)
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
					_getValidHexes(abilityState, list);
				}
			}, hintText: $"Select a hex to summon {Name}");

		if(targetedHex != null)
		{
			SummonStats summonStats = new SummonStats
			{
				Health = abilityState.Health,
				Move = abilityState.Move,
				Attack = abilityState.Attack,
				Range = abilityState.Range,
				Traits = abilityState.Traits.ToArray()
			};

			PackedScene summonScene = ResourceLoader.Load<PackedScene>("res://Scenes/Scenario/Summon.tscn");
			Summon summon = summonScene.Instantiate<Summon>();
			GameController.Instance.Map.AddChild(summon);
			await summon.Init(targetedHex);
			await summon.Spawn(summonStats, (Character)abilityState.Performer, abilityState.Name, _texturePath, _mapIconTexturePath);
			abilityState.SetSummon(summon);

			summon.Scale = Vector2.Zero;
			await summon.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardableAsync();

			ScenarioEvents.FigureKilledEvent.Subscribe(abilityState, this,
				parameters => parameters.Figure == summon,
				async parameters =>
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