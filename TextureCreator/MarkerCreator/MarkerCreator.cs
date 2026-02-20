using System;
using System.IO;
using Fractural.Tasks;
using Godot;
using Godot.Collections;

// ReSharper disable InconsistentNaming

public partial class MarkerCreator : Node2D
{
	public enum MarkerType
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

	[Export]
	private Label _label;
	[Export]
	private int _spriteSize;

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

			const string markerPath = "res://../Game/Art/Markers/";
			string globalPath = ProjectSettings.GlobalizePath(markerPath);
			if(!Directory.Exists(globalPath))
			{
				Directory.CreateDirectory(globalPath);
			}

			LabelSettings labelSettings = _label.LabelSettings;
			Font font = labelSettings.Font;
			int fontSize = labelSettings.FontSize;
			Array<Rid> rids = font.GetRids();
			Rid fontRid = rids[0];
			TextServer ts = TextServerManager.GetPrimaryInterface();

			double ascent = ts.FontGetAscent(fontRid, fontSize);
			double descent = ts.FontGetDescent(fontRid, fontSize);
			double baselineCenter = (ascent - descent) * 0.5f;

			GetWindow().SetSize(_spriteSize * Vector2I.One);

			foreach(MarkerType type in Enum.GetValues<MarkerType>())
			{
				string markerText = type.ToString().Replace("_", string.Empty);
				_label.Text = markerText;

				int codepoint = char.ConvertToUtf32(markerText, 0);
				long glyphIndex = ts.FontGetGlyphIndex(fontRid, fontSize, codepoint, 0);
				Vector2 glyphSize = ts.FontGetGlyphSize(fontRid, new Vector2I(fontSize, fontSize), glyphIndex);
				Vector2 glyphOffset = ts.FontGetGlyphOffset(fontRid, new Vector2I(fontSize, fontSize), glyphIndex);

				float visualCenter = glyphOffset.Y + glyphSize.Y * 0.5f;

				float offset = (float)baselineCenter - visualCenter;
				_label.SetPosition(new Vector2(_label.Position.X, -_label.Size.Y + offset - 70));

				await GDTask.Delay(0.1f);

				Viewport viewport = GetViewport();
				ViewportTexture viewportTexture = viewport.GetTexture();
				Image image = viewportTexture.GetImage();
				image.SavePng($"{markerPath}{markerText}.png");
			}
		}
		catch(Exception e)
		{
			Log.Error(e);
		}
	}
}