using System;
using System.Collections.Generic;
using System.IO;
using Fractural.Tasks;
using Godot;

public partial class ScreenshotHelper : Node
{
	private static readonly Dictionary<string, Vector2I> Resolutions = new Dictionary<string, Vector2I>()
	{
		["Android"] = new Vector2I(1920, 1080),
		// ["iPhone6.5inch"] = new Vector2I(2778, 1284),
		// ["iPhone6.9inch"] = new Vector2I(2796, 1290),
		// ["iPad13inch"] = new Vector2I(2732, 2048),
	};

	public override void _Input(InputEvent @event)
	{
		if(OS.IsDebugBuild())
		{
			if(@event is InputEventKey inputEventKey && inputEventKey.Keycode == Key.F12 && inputEventKey.Pressed)
			{
				Capture();
			}
		}
	}

	private async void Capture()
	{
		try
		{
			const string screenshotPath = "res://../Screenshots/";
			string globalPath = ProjectSettings.GlobalizePath(screenshotPath);
			if(!Directory.Exists(globalPath))
			{
				Directory.CreateDirectory(globalPath);
			}

			string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");

			foreach((string resolutionName, Vector2I dimensions) in Resolutions)
			{
				GetWindow().SetSize(dimensions);
				await GDTask.Delay(0.5f);

				Viewport viewport = GetViewport();
				ViewportTexture viewportTexture = viewport.GetTexture();

				Image image = viewportTexture.GetImage(); //.get_rect(fullscreen)
				image.SavePng($"{screenshotPath}{timeStamp}-{resolutionName}.png");
			}
		}
		catch(Exception e)
		{
			Log.Error(e);
		}
	}
}