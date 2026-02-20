using System;
using System.Collections.Generic;
using Godot;

public partial class PerksPopupPerkBoxes : Control
{
	[Export]
	private BetterButton _button;

	[Export]
	private PackedScene _perkBoxScene;
	[Export]
	private Control _perkBoxParent;

	private readonly List<PerksPopupPerkBox> _boxes = new List<PerksPopupPerkBox>();

	public int PerkIndex { get; private set; }
	public int BoxCount { get; private set; }

	public event Action<PerksPopupPerkBoxes> PressedEvent;

	public override void _Ready()
	{
		base._Ready();

		_button.Pressed += OnPressed;
	}

	public void Init(int perkIndex, int boxCount)
	{
		PerkIndex = perkIndex;
		BoxCount = boxCount;

		for(int i = 0; i < BoxCount; i++)
		{
			PerksPopupPerkBox box = _perkBoxScene.Instantiate<PerksPopupPerkBox>();
			_perkBoxParent.AddChild(box);
			_boxes.Add(box);
		}

		SetCustomMinimumSize(new Vector2(Size.X, Size.Y * BoxCount));
	}

	public void SetAcquired(bool acquired)
	{
		foreach(PerksPopupPerkBox box in _boxes)
		{
			box.SetAcquired(acquired);
		}
	}

	public void SetCanPress(bool canPress)
	{
		_button.SetEnabled(canPress);
	}

	private void OnPressed()
	{
		PressedEvent?.Invoke(this);
	}
}