using Godot;

public static class AtlasTextureHelper
{
	public static AtlasTexture CreateAtlasTexture(int index, int columnCount, int rowCount, Texture2D atlas)
	{
		int x = index % columnCount;
		int y = index / columnCount;

		float itemWidth = (float)atlas.GetWidth() / columnCount;
		float itemHeight = (float)atlas.GetHeight() / rowCount;

		AtlasTexture texture = new AtlasTexture
		{
			Atlas = atlas,
			Region = new Rect2(itemWidth * x, itemHeight * y, itemWidth, itemHeight)
		};

		return texture;
	}
}