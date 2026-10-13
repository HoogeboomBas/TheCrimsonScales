using System.Collections.Generic;
using Godot;

public partial class TooltipView : Control
{
	[Export]
	private PackedScene _itemScene;
	[Export]
	private Control _itemParent;

	private object _requester;

	private readonly List<TooltipItem> _items = new List<TooltipItem>();

	public void Open(object requester, List<Tooltip> tooltips)
	{
		Close();

		_requester = requester;

		foreach(Tooltip tooltip in tooltips)
		{
			TooltipItem item = _itemScene.Instantiate<TooltipItem>();
			_itemParent.AddChild(item);
			item.Init(tooltip);
			_items.Add(item);
		}
	}

	public void Close(object requester)
	{
		if(_requester == requester)
		{
			Close();
		}
	}

	private void Close()
	{
		foreach(TooltipItem item in _items)
		{
			item.QueueFree();
		}

		_items.Clear();

		_requester = null;
	}
}