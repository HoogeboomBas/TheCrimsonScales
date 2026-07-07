using System.Collections.Generic;
using System.Linq;
using Godot;
using SharpVoronoiLib;

public partial class FracturedHexObjectEffect : Node2D
{
	[Export]
	private int _shardCount;
	[Export]
	private Texture2D _texture;
	[Export]
	private Node2D _shardsParent;
	[Export]
	private PackedScene _shardScene;

	private static readonly Vector2 TextureSize = new Vector2(256f, 256f);

	private readonly List<FracturedHexObjectEffectShard> _shards = new List<FracturedHexObjectEffectShard>();

	public override void _Ready()
	{
		base._Ready();

		//Generate();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if(@event is InputEventKey inputEventKey && inputEventKey.Pressed)
		{
			switch(inputEventKey.Keycode)
			{
				case Key.Space:
					Generate();
					break;
			}
		}
	}

	public void Generate()
	{
		VoronoiPlane plane = new VoronoiPlane(0, 0, TextureSize.X, TextureSize.Y);

		// Create sites
		List<VoronoiSite> sites = new List<VoronoiSite>();

		for(int i = 0; i < _shardCount; i++)
		{
			sites.Add(new VoronoiSite(GD.Randf() * TextureSize.X, GD.Randf() * TextureSize.Y));
		}

		plane.SetSites(sites);
		plane.Tessellate();

		List<VoronoiEdge> edges = plane.Relax();

		// Build polygons
		foreach(VoronoiSite site in sites)
		{
			List<Vector2> points = GetPolygonForSite(site, edges);

			if(points.Count < 3)
			{
				continue;
			}

			CreateShard(points);
		}
	}

	private List<Vector2> GetPolygonForSite(VoronoiSite site, List<VoronoiEdge> edges)
	{
		List<Vector2> points = new List<Vector2>();

		foreach(var edge in edges)
		{
			if(edge.Left == site || edge.Right == site)
			{
				points.Add(
					new Vector2(
						(float)edge.Start.X,
						(float)edge.Start.Y
					)
				);

				points.Add(
					new Vector2(
						(float)edge.End.X,
						(float)edge.End.Y
					)
				);
			}
		}

		// Remove duplicates
		points = points.Distinct().ToList();

		// Sort around center
		Vector2 center = Vector2.Zero;

		foreach(Vector2 p in points)
		{
			center += p;
		}

		center /= points.Count;

		points.Sort(
			(a, b) =>
			{
				float aa =
					Mathf.Atan2(
						a.Y - center.Y,
						a.X - center.X
					);

				float bb =
					Mathf.Atan2(
						b.Y - center.Y,
						b.X - center.X
					);

				return aa.CompareTo(bb);
			}
		);

		return points;
	}

	private void CreateShard(List<Vector2> polygon)
	{
		FracturedHexObjectEffectShard shard = _shardScene.Instantiate<FracturedHexObjectEffectShard>();
		_shardsParent.AddChild(shard);
		shard.Init(polygon.ToArray(), _texture, TextureSize);
		_shards.Add(shard);
	}
}