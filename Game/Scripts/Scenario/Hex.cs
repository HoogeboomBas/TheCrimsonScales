using System;
using System.Collections.Generic;
using Godot;

public partial class Hex : Node2D
{
	public Vector2I Coords { get; private set; }
	public MapTile MapTile { get; private set; }

	public bool Revealed { get; private set; }

	public List<Hex> Neighbours { get; } = new List<Hex>();
	public List<HexObject> HexObjects { get; } = new List<HexObject>();

	public event Action<Hex> HexObjectsChangedEvent;

	public void InitCoords()
	{
		Coords = Map.GlobalPositionToCoords(GlobalPosition);
		MapTile = GetParent().GetParentOrNull<MapTile>();
	}

	public void Reveal()
	{
		if(Revealed)
		{
			return;
		}

		Revealed = true;
	}

	public void AddNeighbour(Hex hex)
	{
		Neighbours.AddIfNew(hex);
	}

	public void RegisterHexObject(HexObject hexObject)
	{
		HexObjects.Add(hexObject);

		SortHexObjects();

		HexObjectsChangedEvent?.Invoke(this);
	}

	public void DeregisterHexObject(HexObject hexObject)
	{
		HexObjects.Remove(hexObject);

		SortHexObjects();

		HexObjectsChangedEvent?.Invoke(this);
	}

	public bool TryGetHexObjectOfType<T>(out T hexObject)
	{
		hexObject = GetHexObjectOfType<T>();
		return hexObject != null;
	}

	public T GetHexObjectOfType<T>()
	{
		foreach(HexObject hexObject in HexObjects)
		{
			if(hexObject is T castObject)
			{
				return castObject;
			}
		}

		return default;
	}

	public bool HasHexObjectOfType<T>()
	{
		return GetHexObjectOfType<T>() != null;
	}

	public IEnumerable<T> GetHexObjectsOfType<T>()
	{
		for(int i = HexObjects.Count - 1; i >= 0; i--)
		{
			HexObject hexObject = HexObjects[i];
			if(hexObject is T castObject)
			{
				yield return castObject;
			}
		}
	}

	public bool IsEmpty()
	{
		foreach(HexObject hexObject in HexObjects)
		{
			switch(hexObject)
			{
				case Figure:
				case Obstacle:
				case DifficultTerrain:
				case HazardousTerrain:
				case Trap:
				case Door:
				case PressurePlate:
					return false;
			}
		}

		return true;
	}

	public bool IsFeatureless()
	{
		foreach(HexObject hexObject in HexObjects)
		{
			switch(hexObject)
			{
				case Objective:
				case Obstacle:
				case DifficultTerrain:
				case HazardousTerrain:
				case Trap:
				case Door:
				case PressurePlate:
					return false;
			}
		}

		return true;
	}

	public bool IsUnoccupied()
	{
		foreach(HexObject hexObject in HexObjects)
		{
			switch(hexObject)
			{
				case Figure:
					return false;
			}
		}

		return true;
	}

	private void SortHexObjects()
	{
		HexObjects.Sort((a, b) => b.DefaultZIndex.CompareTo(a.DefaultZIndex));
	}
}