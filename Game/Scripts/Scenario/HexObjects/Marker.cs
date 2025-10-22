using Godot;

// ReSharper disable InconsistentNaming

[Tool]
public partial class Marker : Node2D
{
	public enum Type
	{
		a = 0,
		b,
		c,
		d,
		e,
		f,
		g,
		h,
		i,
		j,
		k,
		l,
		m,
		n,
		o,
		p,
		q,
		r,
		s,
		t,
		u,
		v,
		x,
		y,
		z,
		_1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7,
		_8,
		_9,
		_10
	}

	private Type _type;

	[Export]
	public Type MarkerType
	{
		get => _type;
		set
		{
			_type = value;
			MarkDirty();
		}
	}

	[Export]
	private bool _hideDuringPlay;

	public Hex Hex => GetHexObject<HexObject>()?.Hex ?? GameController.Instance.Map.GetHex(Map.GlobalPositionToCoords(GlobalPosition));

	public override void _Ready()
	{
		base._Ready();

		if(!Engine.IsEditorHint() && _hideDuringPlay)
		{
			Hide();
		}
	}

	public T GetHexObject<T>()
		where T : HexObject
	{
		return GetParent() as T;
	}

	private void MarkDirty()
	{
		UpdateVisuals();
		NotifyPropertyListChanged();
	}

	private void UpdateVisuals()
	{
		GetNode<Label>("Visual/Label").Text = MarkerType.ToString().Replace("_", string.Empty);
	}
}