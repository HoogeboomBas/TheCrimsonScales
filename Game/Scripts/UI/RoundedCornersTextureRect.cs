using Godot;

public partial class RoundedCornersTextureRect : TextureRect
{
	private const string AtlasRegionName = "atlas_region";
	private const string AtlasSizeName = "atlas_size";

	public new void SetTexture(Texture2D texture)
	{
		base.SetTexture(texture);

		Vector2 size = texture.GetSize();
		Vector2 atlasSize = size;
		Vector4 atlasRegion = new Vector4(0f, 0f, size.X, size.Y);

		if(texture is AtlasTexture atlasTexture)
		{
			Rect2 region = atlasTexture.Region;
			atlasSize = atlasTexture.Atlas.GetSize();
			atlasRegion = new Vector4(region.Position.X, region.Position.Y, region.Size.X, region.Size.Y);
		}

		SetInstanceShaderParameter(AtlasRegionName, atlasRegion);
		SetInstanceShaderParameter(AtlasSizeName, atlasSize);
	}
}