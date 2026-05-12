using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;

/// <summary>
/// An <see cref="ActiveAbility{T}"/> that creates a spirit ally.
/// </summary>
public class SpawnAbility : ActiveAbility<SpawnAbility.State>
{
	public class State : ActiveAbilityState
	{
		public string Name { get; set; }
		public int Health { get; set; }
		public int? Move { get; set; }
		public int? Attack { get; set; }
		public int? Range { get; set; }
		public List<FigureTrait> Traits { get; set; }

		public Spirit Spirit { get; private set; }

		public void SetSpirit(Spirit spirit)
		{
			Spirit = spirit;
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
	private bool _requestDiscardOrLoseAfterSpiritKilled = true;

	public string Name { get; private set; }
	public int Health { get; private set; }
	public int? Move { get; private set; }
	public int? Attack { get; private set; }
	public int? Range { get; private set; }
	public FigureTrait[] Traits { get; private set; }

	/// <summary>
	/// A builder extending <see cref="ActiveAbility{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in SpawnAbility. Enables inheritors of SpawnAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending SpawnAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : ActiveAbility<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.INameStep,
		AbstractBuilder<TBuilder, TAbility>.ITexturePathStep,
		AbstractBuilder<TBuilder, TAbility>.IHealthStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : SpawnAbility, new()
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
			TBuilder WithHealth(int health);
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

		public TBuilder WithHealth(int health)
		{
			Obj.Health = health;
			return (TBuilder)this;
		}

		public TBuilder WithMove(int move)
		{
			Obj.Move = move;
			return (TBuilder)this;
		}

		public TBuilder WithAttack(int attack)
		{
			Obj.Attack = attack;
			return (TBuilder)this;
		}

		public TBuilder WithRange(int range)
		{
			Obj.Range = range;
			return (TBuilder)this;
		}

		public TBuilder WithTraits(params FigureTrait[] traits)
		{
			Obj.Traits = traits;
			return (TBuilder)this;
		}

		public TBuilder WithGetValidHexes(Action<State, List<Hex>> getValidHexes)
		{
			Obj._getValidHexes = getValidHexes;
			return (TBuilder)this;
		}

		public TBuilder WithSetDontRequestDiscardOrLoseAfterSpiritKilled()
		{
			Obj._requestDiscardOrLoseAfterSpiritKilled = false;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class SpawnBuilder : AbstractBuilder<SpawnBuilder, SpawnAbility>
	{
		internal SpawnBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of SpawnBuilder.
	/// </summary>
	/// <returns></returns>
	public static SpawnBuilder.INameStep Builder()
	{
		return new SpawnBuilder();
	}

	public SpawnAbility() { }

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

						if(!hex.IsUnoccupied())
						{
							list.RemoveAt(i);
						}
					}
				}
				else
				{
					_getValidHexes(abilityState, list);
				}
			}, hintText: $"Select a hex to spawn {Name}");

		if(targetedHex != null)
		{
			PackedScene spiritScene = ResourceLoader.Load<PackedScene>("res://Content/Classes/SpiritCaller/Spirit.tscn");
			Spirit spirit = spiritScene.Instantiate<Spirit>();
			GameController.Instance.Map.AddChild(spirit);
			await spirit.Init(targetedHex);
			await spirit.Spawn(abilityState.Health, abilityState.Move, abilityState.Attack, abilityState.Range, abilityState.Traits.ToArray(),
				(Character)abilityState.Performer, abilityState.Name, _texturePath, _mapIconTexturePath);
			abilityState.SetSpirit(spirit);

			spirit.Scale = Vector2.Zero;
			await spirit.TweenScale(1f, 0.3f).SetEasing(Easing.OutBack).PlayFastForwardableAsync();

			if(_requestDiscardOrLoseAfterSpiritKilled)
			{
				ScenarioEvents.FigureKilledEvent.Subscribe(abilityState, this,
					parameters => parameters.Figure == spirit,
					async parameters =>
					{
						await abilityState.ActionState.RequestDiscardOrLose();
					}
				);
			}

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

		if(abilityState.Spirit != null && !abilityState.Spirit.IsDead)
		{
			await abilityState.Spirit.Destroy();
		}
	}
}