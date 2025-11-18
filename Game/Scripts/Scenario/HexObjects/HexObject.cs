using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;

public partial class HexObject : Node2D, IReferenced
{
	[Export]
	public HexObjectShape HexObjectShape { get; private set; }

	[Export]
	public bool CannotBeDestroyed { get; private set; }

	public Hex Hex { get; private set; }
	public int RotationIndex { get; private set; }

	public Hex[] Hexes { get; private set; }

	public bool IsDestroyed { get; private set; }

	public int DefaultZIndex { get; private set; }

	public int ReferenceId { get; set; }

	public virtual SFX.StepType? OverrideStepType => null;

	private readonly Dictionary<Type, HexObjectViewComponent> _hexObjectComponentCache = new Dictionary<Type, HexObjectViewComponent>();

	public event Action<HexObject> DestroyEvent;
	public event Action<HexObject> HexesChangedEvent;

	public override void _Ready()
	{
		base._Ready();

		List<HexObjectViewComponent> components = this.GetChildrenOfType<HexObjectViewComponent>();
		foreach(HexObjectViewComponent component in components)
		{
			_hexObjectComponentCache.TryAdd(component.GetType(), component);
		}
	}

	public virtual async GDTask Init(Hex originHex, int rotationIndex = 0, bool hexCanBeNull = false)
	{
		this.InitReference();

		DefaultZIndex = ZIndex;

		List<HexObjectViewComponent> components = this.GetChildrenOfType<HexObjectViewComponent>();
		foreach(HexObjectViewComponent component in components)
		{
			_hexObjectComponentCache.TryAdd(component.GetType(), component);
		}

		Hexes = new Hex[(int)HexObjectShape + 1];

		if(originHex == null && hexCanBeNull)
		{
			RemoveFromMap();
		}
		else
		{
			SetOriginHexAndRotation(originHex, rotationIndex);
		}

		foreach(HexObjectViewComponent component in components)
		{
			component.Init(this);
		}

		await GDTask.CompletedTask;
	}

	public virtual async GDTask Destroy(bool immediately = false, bool forceDestroy = false)
	{
		if(CannotBeDestroyed && !forceDestroy)
		{
			Log.Error($"Trying to destroy {Name}, but it can not be destroyed!");
			return;
		}

		IsDestroyed = true;

		if(Hex != null)
		{
			foreach(Hex hex in Hexes)
			{
				hex.DeregisterHexObject(this);
			}
		}

		if(immediately)
		{
			Hide();
		}
		else
		{
			DestroyAnimation();
		}

		DestroyEvent?.Invoke(this);

		await GDTask.CompletedTask;
	}

	public void SetOriginHexAndRotation(Hex originHex, int rotationIndex = 0)
	{
		if(Hex != null)
		{
			foreach(Hex hex in Hexes)
			{
				hex.DeregisterHexObject(this);
			}
		}

		Hex = originHex;

		if(Hex == null)
		{
			Log.Error("Setting hex to null is probably wrong?");
		}
		else
		{
			RotationIndex = rotationIndex;
			GlobalRotationDegrees = rotationIndex * 60f;
			GlobalPosition = Hex.GlobalPosition;

			Hexes[0] = Hex;

			if((int)HexObjectShape > 0)
			{
				int rotation = (1 + RotationIndex) % 6;
				Vector2I coords = Map.GetNeighbourCoords(Hex.Coords, rotation);
				Hexes[1] = GameController.Instance.Map.GetHex(coords);
			}

			if(HexObjectShape == HexObjectShape.Triangle)
			{
				int rotation = (0 + RotationIndex) % 6;
				Vector2I coords = Map.GetNeighbourCoords(Hex.Coords, rotation);
				Hexes[2] = GameController.Instance.Map.GetHex(coords);
			}

			foreach(Hex hex in Hexes)
			{
				if(hex == null)
				{
					Log.Write("Trying to register a hex object with a non-existent hex.");
					continue;
				}

				hex.RegisterHexObject(this);
			}
		}

		HexesChangedEvent?.Invoke(this);
	}

	public void RemoveFromMap()
	{
		Hex = null;

		foreach(Hex hex in Hexes)
		{
			if(hex != null)
			{
				hex.DeregisterHexObject(this);
			}
		}

		for(int i = 0; i < Hexes.Length; i++)
		{
			Hexes[i] = null;
		}

		HexesChangedEvent?.Invoke(this);
	}

	public T GetViewComponent<T>()
		where T : HexObjectViewComponent
	{
		if(_hexObjectComponentCache.TryGetValue(typeof(T), out HexObjectViewComponent component))
		{
			return (T)component;
		}

		_hexObjectComponentCache.Add(typeof(T), default);

		return default;
	}

	public virtual void AddInfoItemParameters(List<InfoItemParameters> parametersList)
	{
	}

	protected virtual void DestroyAnimation()
	{
		this.TweenScale(0f, 0.3f).SetEasing(Easing.InBack).OnComplete(Hide).PlayFastForwardable();
	}

	public void SetCannotBeDestroyed(bool cannotBeDestroyed)
    {
		CannotBeDestroyed = cannotBeDestroyed;
    }
}