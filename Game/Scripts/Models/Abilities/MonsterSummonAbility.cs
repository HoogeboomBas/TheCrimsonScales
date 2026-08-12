using System;
using System.Collections.Generic;
using Fractural.Tasks;

/// <summary>
/// An <see cref="Ability{T}"/> that creates a summon ally.
/// </summary>
public class MonsterSummonAbility : Ability<MonsterSummonAbility.State>
{
	public class State : AbilityState
	{
		public MonsterModel MonsterModel { get; private set; }
		public MonsterType MonsterType { get; private set; }
		public Monster SummonedMonster { get; private set; }
		public int? ForcedHitPoints { get; private set; }

		public void SetMonsterModel(MonsterModel monsterModel)
		{
			MonsterModel = monsterModel;
		}

		public void SetMonsterType(MonsterType monsterType)
		{
			MonsterType = monsterType;
		}

		public void SetSummonedMonster(Monster monster)
		{
			SummonedMonster = monster;
		}

		public void SetForcedHitPoints(int hitPoints)
		{
			ForcedHitPoints = hitPoints;
		}
	}

	private MonsterModel _monsterModel;
	private MonsterType _monsterType;
	private Action<State, List<Hex>> _getValidHexes;

	/// <summary>
	/// A builder extending <see cref="ActiveAbility{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in MonsterSummonAbility. Enables inheritors of MonsterSummonAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending MonsterSummonAbility.
	public new class AbstractBuilder<TBuilder, TAbility> : Ability<State>.AbstractBuilder<TBuilder, TAbility>,
		AbstractBuilder<TBuilder, TAbility>.IMonsterModelStep,
		AbstractBuilder<TBuilder, TAbility>.IMonsterTypeStep
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : MonsterSummonAbility, new()
	{
		public interface IMonsterModelStep
		{
			IMonsterTypeStep WithMonsterModel(MonsterModel monsterModel);
		}

		public interface IMonsterTypeStep
		{
			TBuilder WithMonsterType(MonsterType monsterType);
		}

		public IMonsterTypeStep WithMonsterModel(MonsterModel monsterModel)
		{
			Obj._monsterModel = monsterModel;
			return (TBuilder)this;
		}

		public TBuilder WithMonsterType(MonsterType monsterType)
		{
			Obj._monsterType = monsterType;
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
	public class MonsterSummonBuilder : AbstractBuilder<MonsterSummonBuilder, MonsterSummonAbility>
	{
		internal MonsterSummonBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of SummonBuilder.
	/// </summary>
	/// <returns></returns>
	public static MonsterSummonBuilder.IMonsterModelStep Builder()
	{
		return new MonsterSummonBuilder();
	}

	public MonsterSummonAbility() { }

	protected override void InitializeState(State abilityState)
	{
		base.InitializeState(abilityState);

		abilityState.SetMonsterModel(_monsterModel);
		abilityState.SetMonsterType(_monsterType);
	}

	protected override async GDTask Perform(State abilityState)
	{
		// Target a hex within range closest to an enemy
		Hex targetedHex = await AbilityCmd.SelectHex(abilityState, list =>
			{
				List<Hex> possibleHexes = new List<Hex>();

				if(_getValidHexes == null)
				{
					RangeHelper.FindHexesInRange(abilityState.Performer.Hex, 1, true, possibleHexes);

					for(int i = possibleHexes.Count - 1; i >= 0; i--)
					{
						Hex hex = possibleHexes[i];

						if(!hex.IsEmpty())
						{
							possibleHexes.RemoveAt(i);
						}
					}
				}
				else
				{
					_getValidHexes(abilityState, possibleHexes);
				}

				int closestRange = int.MaxValue;

				foreach(Hex hex in possibleHexes)
				{
					foreach(Figure figure in GameController.Instance.Map.Figures)
					{
						if(abilityState.Performer.EnemiesWith(figure))
						{
							int range = RangeHelper.Distance(hex, figure.Hex);
							if(range == closestRange)
							{
								list.Add(hex);
							}
							else if(range < closestRange)
							{
								closestRange = range;
								list.Clear();
								list.Add(hex);
							}
						}
					}
				}
			},
			hintText: $"Select a hex to summon {(abilityState.MonsterType == MonsterType.Normal ?
				"a normal" : "an elite")} {abilityState.MonsterModel.Name} in",
			mandatory: abilityState.Authority is not Character
		);

		if(targetedHex != null)
		{
			Monster monster = await AbilityCmd.SummonMonster(abilityState.MonsterModel, abilityState.MonsterType, targetedHex);
			abilityState.SetSummonedMonster(monster);

			if(abilityState.ForcedHitPoints.HasValue && monster != null)
			{
				monster.SetHealth(abilityState.ForcedHitPoints.Value);
			}

			abilityState.SetPerformed();
		}
	}
}