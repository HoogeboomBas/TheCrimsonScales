using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

[Serializable, JsonObject(MemberSerialization.OptIn)]
public abstract class ClassModel : AbstractModel<ClassModel>
{
	public abstract string Name { get; }
	public abstract MaxHealthValues MaxHealthValues { get; }
	public abstract int HandSize { get; }

	public abstract string AssetPath { get; }
	public abstract Color PrimaryColor { get; }
	public abstract Color SecondaryColor { get; }

	public abstract IList<AbilityCardModel> AbilityCards { get; }

	public virtual XPLevelValues XPLevelValues => XPLevelValues.Default;
	public abstract PackedScene Scene { get; } // => ResourceLoader.Load<PackedScene>($"{AssetPath}/{ClassName}.tscn");
	public virtual Texture2D PortraitTexture => ResourceLoader.Load<Texture2D>($"{AssetPath}/Portrait.tres");
	public virtual Texture2D CharacterTokenTexture => ResourceLoader.Load<Texture2D>($"{AssetPath}/CharacterToken.png");
	public virtual Texture2D MatFrontTexture => ResourceLoader.Load<Texture2D>($"{AssetPath}/MatFront.png");
	public virtual string IconPath => $"{AssetPath}/Icon.svg";
	public virtual Texture2D IconTexture => ResourceLoader.Load<Texture2D>(IconPath);
	public virtual bool HasAnimatedSprite => true;
}