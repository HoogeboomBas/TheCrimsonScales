using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

/// <summary>
/// An <see cref="Ability{T}"/> that allows a figure to create an Obstacle of a specific kind in an empty hex
/// </summary>
public class CreateObstacleAbility : Ability<CreateObstacleAbility.State>
{
	public class State : AbilityState
	{
		public List<Obstacle> CreatedObstacles { get; set; } = [];
	}

	public int Range { get; private set; } = 1;
	public int ObstacleCount { get; private set; } = 1;
	public string AssetPath = "res://Content/OverlayTiles/Obstacles/Boulder1H.tscn";
	public string ObstacleName = "Obstacle";

	public Action<State, List<Hex>> CustomSelectHexes { get; private set; } = null;
	public bool Mandatory = false;

	/// <summary>
	/// A builder extending <see cref="Ability{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in CreateObstacleAbility. Enables inheritors of CreateObstacleAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending CreateObstacleAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : Ability<State>.AbstractBuilder<TBuilder, TAbility>
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : CreateObstacleAbility, new()
	{
		public TBuilder WithRange(int range)
		{
			Obj.Range = range;
			return (TBuilder)this;
		}

		public TBuilder WithObstacleCount(int obstacleCount)
		{
			Obj.ObstacleCount = obstacleCount;
			return (TBuilder)this;
		}

		public TBuilder WithCustomSelectHexes(Action<State, List<Hex>> selectHexes)
		{
			Obj.CustomSelectHexes = selectHexes;
			return (TBuilder)this;
		}

		public TBuilder WithCustomAsset(string assetPath)
		{
			Obj.AssetPath = assetPath;
			return (TBuilder)this;
		}

		public TBuilder WithCustomName(string obstacleName)
		{
			Obj.ObstacleName = obstacleName;
			return (TBuilder)this;
		}

		public TBuilder WithMandatory(bool mandatory)
		{
			Obj.Mandatory = mandatory;
			return (TBuilder)this;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class CreateObstacleBuilder : AbstractBuilder<CreateObstacleBuilder, CreateObstacleAbility>
	{
		internal CreateObstacleBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of CreateTrapBuilder.
	/// </summary>
	/// <returns></returns>
	public static CreateObstacleBuilder Builder()
	{
		return new CreateObstacleBuilder();
	}

	protected override async GDTask Perform(State abilityState)
	{
		for(int i = 0; i < ObstacleCount; i++)
		{
			Hex hex = await AbilityCmd.SelectHex(abilityState, list =>
				{
					if(CustomSelectHexes != null)
					{
						CustomSelectHexes(abilityState, list);
					}
					else
					{
						list.AddRange(RangeHelper.GetHexesInRange(abilityState.Performer.Hex, Range).Where(hex => hex.IsEmpty()));
					}

					for(int j = list.Count - 1; j >= 0; j--)
					{
						Hex hex = list[j];

						if(!RangeHelper.CheckCanPlaceObstacle(hex))
						{
							list.RemoveAt(j);
						}
					}
				},
				mandatory: Mandatory,
				hintText: $"Select a hex to place the {ObstacleName}");

			if(hex == null)
			{
				return;
			}

			abilityState.CreatedObstacles.Add(await AbilityCmd.CreateObstacle(hex, AssetPath));

			abilityState.SetPerformed();
		}
	}
}