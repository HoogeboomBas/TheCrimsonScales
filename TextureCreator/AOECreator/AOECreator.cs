using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Fractural.Tasks;
using Godot;

public partial class AOECreator : Node2D
{
	private const float HexSize = 126f;
	private static readonly float Sqrt3 = Mathf.Sqrt(3);

	private static readonly List<AOEPattern> Patterns =
	[
		new AOEPattern(
			[
				new AOEHex(Vector2I.Zero, AOEHexType.Gray),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
			]
		),
		new AOEPattern(
			[
				new AOEHex(Vector2I.Zero, AOEHexType.Gray),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East).Add(Direction.East), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East).Add(Direction.East), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
			]
		),
		new AOEPattern(
			[
				new AOEHex(Vector2I.Zero, AOEHexType.Gray),
				new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
				new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.East), AOEHexType.Red),
			]
		),
	];

	[Export]
	private Camera2D _camera;

	[Export]
	private PackedScene _aoePatternViewScene;
	[Export]
	private Node2D _aoePatternViewParent;

	public override void _Ready()
	{
		base._Ready();

		CreateTextures();
	}

	private async void CreateTextures()
	{
		try
		{
			Window window = GetWindow();

			// Disable for this scene
			window.ContentScaleMode = Window.ContentScaleModeEnum.Disabled;
			window.ContentScaleAspect = Window.ContentScaleAspectEnum.Keep;

			await GDTask.Delay(1f);

			const string patternPath = "res://../Game/Art/AOEPatterns/";
			string globalPath = ProjectSettings.GlobalizePath(patternPath);
			if(!Directory.Exists(globalPath))
			{
				Directory.CreateDirectory(globalPath);
			}

			foreach(AOEPattern aoePattern in Patterns)
			{
				const float scale = 0.5f;
				AOEPatternView patternView = _aoePatternViewScene.Instantiate<AOEPatternView>();
				_aoePatternViewParent.AddChild(patternView);
				patternView.Init(aoePattern);

				// Center the pattern
				const float hexHalfWidth = 218.238f / 2;
				const float hexHalfHeight = HexSize;
				float minX = float.MaxValue;
				float maxX = float.MinValue;
				float minY = float.MaxValue;
				float maxY = float.MinValue;
				foreach(AOEHexView hexView in patternView.Hexes)
				{
					Vector2 position = hexView.GlobalPosition;
					minX = Mathf.Min(minX, position.X);
					maxX = Mathf.Max(maxX, position.X);
					minY = Mathf.Min(minY, position.Y);
					maxY = Mathf.Max(maxY, position.Y);
				}

				Rect2 containerRect = new Rect2(
					minX - hexHalfWidth,
					minY - hexHalfHeight,
					maxX - minX + hexHalfWidth * 2,
					maxY - minY + hexHalfHeight * 2);

				FitCameraAndWindow(_camera, containerRect, scale);

				await GDTask.Delay(0.1f);

				Viewport viewport = GetViewport();
				ViewportTexture viewportTexture = viewport.GetTexture();
				Image image = viewportTexture.GetImage();
				image.SavePng($"{patternPath}{PatternToString(aoePattern)}.png");

				patternView.QueueFree();
			}
		}
		catch(Exception e)
		{
			Log.Error(e);
		}
	}

	public static Vector2 CoordsToGlobalPosition(Vector2I coords)
	{
		return new Vector2(Sqrt3 * coords.X + Sqrt3 / 2 * coords.Y, 1.5f * coords.Y) * HexSize;
	}

	public static Vector2I GlobalPositionToCoords(Vector2 globalPosition)
	{
		// Algorithm works with specific size, so multiply global point
		globalPosition /= Sqrt3 * HexSize;
		Vector2 point = new Vector2(globalPosition.X, globalPosition.Y);

		int temp = Mathf.FloorToInt(point.X + Sqrt3 * point.Y + 1);
		int r = Mathf.FloorToInt((temp + Mathf.Floor(-point.X + Sqrt3 * point.Y + 1)) / 3);
		int q = Mathf.FloorToInt((Mathf.Floor(2 * point.X + 1) + temp) / 3f) - r;

		return new Vector2I(q, r);
	}

	private static void FitCameraAndWindow(Camera2D camera, Rect2 rect, float scale = 1f)
	{
		// 1. Window size = rect size scaled
		Vector2I windowSize = new Vector2I(
			Mathf.CeilToInt(rect.Size.X * scale),
			Mathf.CeilToInt(rect.Size.Y * scale)
		);

		DisplayServer.WindowSetSize(windowSize);

		// 2. Center camera
		camera.GlobalPosition = rect.Position + rect.Size / 2f;

		// 3. Zoom is inverse of scale
		float zoom = scale;
		camera.Zoom = new Vector2(zoom, zoom);
	}

	private static string PatternToString(AOEPattern aoePattern)
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<AOEHex> listCopy = aoePattern.LocalHexes.ToList();
		listCopy.Sort((a, b) => (a.Coords.X + a.Coords.Y * 100).CompareTo((b.Coords.X + b.Coords.Y * 100)));
		foreach(AOEHex aoeHex in listCopy)
		{
			stringBuilder.Append(aoeHex.Coords.X);
			stringBuilder.Append(aoeHex.Coords.Y);
			stringBuilder.Append(aoeHex.Type.ToString()[0]);
		}

		return stringBuilder.ToString();
	}
}