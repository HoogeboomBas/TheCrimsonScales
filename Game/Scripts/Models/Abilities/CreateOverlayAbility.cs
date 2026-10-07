using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

/// <summary>
/// An <see cref="Ability{T}"/> that allows a figure to create an Obstacle of a specific kind in an empty hex
/// </summary>
public class CreateOverlayAbility : Ability<CreateOverlayAbility.State>
{
	public class State : AbilityState
	{
		public List<OverlayTile> CreatedOverlays { get; set; } = [];
	}

	public int Range { get; private set; } = 1;
	public int OverlayCount { get; private set; } = 1;
	public string AssetPath = "res://Content/OverlayTiles/Obstacles/Boulder1H.tscn";
	public string OverlayName = "Obstacle";
    public Type OverlayType = typeof(Obstacle);

	public Action<State, List<Hex>> CustomSelectHexes { get; private set; } = null;
	public bool Mandatory = false;

	/// <summary>
	/// A builder extending <see cref="Ability{T}.AbstractBuilder{TBuilder, TAbility}"/> with setter methods
	/// for values defined in CreateOverlayAbility. Enables inheritors of CreateOverlayAbility to further extend the builder.
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam> Any builder extending this AbstractBuilder.
	/// <typeparam name="TAbility"></typeparam> Any ability extending CreateOverlayAbility.
	public new abstract class AbstractBuilder<TBuilder, TAbility> : Ability<State>.AbstractBuilder<TBuilder, TAbility>
		where TBuilder : AbstractBuilder<TBuilder, TAbility>
		where TAbility : CreateOverlayAbility, new()
	{
		public TBuilder WithRange(int range)
		{
			Obj.Range = range;
			return (TBuilder)this;
		}

        public TBuilder WithOverlayType(Type overlayType)
        {
            Obj.OverlayType = overlayType;
            return (TBuilder)this;
        }

		public TBuilder WithOverlayCount(int overlayCount)
		{
			Obj.OverlayCount = overlayCount;
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

		public TBuilder WithCustomName(string overlayName)
		{
			Obj.OverlayName = overlayName;
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
	public class CreateOverlayBuilder : AbstractBuilder<CreateOverlayBuilder, CreateOverlayAbility>
	{
		internal CreateOverlayBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of CreateTrapBuilder.
	/// </summary>
	/// <returns></returns>
	public static CreateOverlayBuilder Builder()
	{
		return new CreateOverlayBuilder();
	}

	protected override async GDTask Perform(State abilityState)
	{
        PackedScene packedScene = SceneLoader.LoadPackedScene(AssetPath);
		for(int i = 0; i < OverlayCount; i++)
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
				hintText: $"Select a hex to place the {OverlayName}");

			if(hex == null)
			{
				return;
			}

			abilityState.CreatedOverlays.Add(await AbilityCmd.CreateOverlayTile<OverlayTile>(hex, packedScene));

			abilityState.SetPerformed();
		}
	}
}