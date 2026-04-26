using System.Collections.Generic;
using Godot;

public partial class AOEPatternView : Node2D
{
	[Export]
	private PackedScene _redHexScene;
	[Export]
	private PackedScene _yellowHexScene;
	[Export]
	private PackedScene _grayHexScene;
	[Export]
	private PackedScene _emptyHexScene;

	[Export]
	private Node2D _hexParent;

	public List<AOEHexView> Hexes { get; } = new List<AOEHexView>();

	public void Init(AOEPattern pattern)
	{
		foreach(AOEHex aoeHex in pattern.LocalHexes)
		{
			PackedScene hexScene = null;
			if(aoeHex.Type.HasFlag(AOEHexType.Red))
			{
				hexScene = _redHexScene;
			}
			else if(aoeHex.Type.HasFlag(AOEHexType.Gray))
			{
				hexScene = _grayHexScene;
			}
			else if(aoeHex.Type.HasFlag(AOEHexType.Yellow))
			{
				hexScene = _yellowHexScene;
			}
			else //if(aoeHex.Type.HasFlag(AOEHexType.Empty))
			{
				hexScene = _emptyHexScene;
			}

			AOEHexView hexView = hexScene.Instantiate<AOEHexView>();
			_hexParent.AddChild(hexView);
			hexView.Init(aoeHex);

			Hexes.Add(hexView);
		}
	}
}