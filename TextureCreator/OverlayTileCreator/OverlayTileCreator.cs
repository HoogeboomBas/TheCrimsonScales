using System;
using System.IO;
using Fractural.Tasks;
using Godot;

public partial class OverlayTileCreator : Node2D
{
	[Export]
	private Camera2D _camera;

	[Export]
	private Node2D _1HContainer;
	[Export]
	private Texture2D[] _1HTextures;
	[Export]
	private Sprite2D _1HSprite;

	[Export]
	private Node2D _2HContainer;
	[Export]
	private Texture2D[] _2HTextures;
	[Export]
	private Sprite2D _2HSprite;

	[Export]
	private Node2D _3HContainer;
	[Export]
	private Texture2D[] _3HTextures;
	[Export]
	private Sprite2D _3HSprite;

	public override void _Ready()
	{
		base._Ready();

		CreateTextures();
	}

	private async void CreateTextures()
	{
		try
		{
			await GDTask.Delay(1f);

			_1HContainer.SetVisible(false);
			_2HContainer.SetVisible(false);
			_3HContainer.SetVisible(false);

			await CreateTextures(_1HContainer, _1HTextures, _1HSprite, 1920f / 256, 256, 256);
			await CreateTextures(_2HContainer, _2HTextures, _2HSprite, 1080f / 256, 512, 256);
			await CreateTextures(_3HContainer, _3HTextures, _2HSprite, 1920f / 512, 512, 512);
		}
		catch(Exception e)
		{
			Log.Error(e);
		}
	}

	private async GDTask CreateTextures(Node2D container, Texture2D[] textures, Sprite2D sprite, float zoom, int width, int height)
	{
		container.SetVisible(true);

		GetWindow().SetSize(new Vector2I(width, height));
		_camera.SetZoom(zoom * Vector2.One);
		const string texturesPath = "res://../Game/Art/OverlayTiles/";
		string globalPath = ProjectSettings.GlobalizePath(texturesPath);
		if(!Directory.Exists(globalPath))
		{
			Directory.CreateDirectory(globalPath);
		}

		foreach(Texture2D texture in textures)
		{
			sprite.Texture = texture;

			await GDTask.Delay(0.1f);

			string fileName = Path.GetFileName(texture.ResourcePath);

			Viewport viewport = GetViewport();
			ViewportTexture viewportTexture = viewport.GetTexture();
			Image image = viewportTexture.GetImage();
			image.SavePng($"{texturesPath}{fileName}");
		}

		await GDTask.Delay(0.1f);

		container.SetVisible(false);
	}
}