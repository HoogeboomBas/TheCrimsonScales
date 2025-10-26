using System;
using Godot;

public static class UIHelper
{
	public static readonly Color SpentColor = Color.FromHtml("888888");
	public static readonly Color LostColor = Color.FromHtml("f8625a");

	public static void SetCardMaterial(TextureRect textureRect, CardSelectionListCategoryType cardSelectionListCategoryType)
	{
		float grayscaleFactor = 0f;
		Color modulateColor = Colors.White;

		switch(cardSelectionListCategoryType)
		{
			case CardSelectionListCategoryType.None:
				grayscaleFactor = 0f;
				break;

			case CardSelectionListCategoryType.Active:
				grayscaleFactor = -1.5f;
				break;
			case CardSelectionListCategoryType.Playing:
				grayscaleFactor = -0.5f;
				break;
			case CardSelectionListCategoryType.Hand:
				grayscaleFactor = 0f;
				break;
			case CardSelectionListCategoryType.Discarded:
				grayscaleFactor = 1f;
				break;
			case CardSelectionListCategoryType.Lost:
				grayscaleFactor = 1f;
				modulateColor = LostColor;
				break;
			case CardSelectionListCategoryType.UnrecoverablyLost:
				grayscaleFactor = 1f;
				modulateColor = LostColor;
				break;

			case CardSelectionListCategoryType.Unlockable:
				grayscaleFactor = 0f;
				break;
			case CardSelectionListCategoryType.Unavailable:
				grayscaleFactor = 1f;
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(cardSelectionListCategoryType), cardSelectionListCategoryType, null);
		}

		textureRect.SetInstanceShaderParameter("grayscaleFactor", grayscaleFactor);
		textureRect.SetSelfModulate(modulateColor);
	}

	public static void SetItemMaterial(TextureRect textureRect, ItemState itemState)
	{
		float grayscaleFactor = 0f;
		Color modulateColor = Colors.White;

		switch(itemState)
		{
			case ItemState.Available:
				grayscaleFactor = 0f;
				break;
			case ItemState.Spent:
				grayscaleFactor = 1f;
				break;
			case ItemState.Consumed:
				grayscaleFactor = 1f;
				modulateColor = LostColor;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(itemState), itemState, null);
		}

		textureRect.SetInstanceShaderParameter("grayscaleFactor", grayscaleFactor);
		textureRect.SetSelfModulate(modulateColor);
	}
}