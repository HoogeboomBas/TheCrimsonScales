using Godot;

public static class Icons
{
	public const string LoseCard = "res://Art/Icons/Abilities/LoseCard.svg";
	public const string LoseDiscardedCards = "res://Art/Icons/Abilities/LoseDiscardedCards.svg";
	public const string RecoverCard = "res://Art/Icons/Abilities/LoseCard.svg";
	public const string Attack = "res://Art/Icons/Abilities/Attack.svg";
	public const string Move = "res://Art/Icons/Abilities/Move.svg";
	public const string Heal = "res://Art/Icons/Abilities/Heal.svg";
	public const string Range = "res://Art/Icons/Abilities/Range.svg";
	public const string Damage = "res://Art/Icons/Abilities/Damage.svg";
	public const string Push = "res://Art/Icons/ConditionsAndEffects/Push.svg";
	public const string Pull = "res://Art/Icons/ConditionsAndEffects/Pull.svg";
	public const string Swing = "res://Art/Icons/ConditionsAndEffects/Swing.svg";
	public const string Shield = "res://Art/Icons/Abilities/Shield.svg";
	public const string Retaliate = "res://Art/Icons/Abilities/Retaliate.svg";
	public const string Pierce = "res://Art/Icons/ConditionsAndEffects/Pierce.svg";
	public const string Targets = "res://Art/Icons/Abilities/Targets.svg";
	public const string Jump = "res://Art/Icons/Abilities/Jump.svg";
	public const string Teleport = "res://Art/Icons/Abilities/Teleport.svg";
	public const string Loot = "res://Art/Icons/Abilities/Loot.svg";
	public const string Cards = "res://Art/Icons/Other/Cards.svg";
	public const string DiscardedCards = "res://Art/Icons/Other/DiscardedCards.svg";
	public const string UnlockableCards = "res://Art/Icons/Other/LevelCrown.svg";
	public const string UnavailableCards = "res://Art/Icons/Other/CloseIcon.svg";
	public const string PlayingCards = "res://Art/Icons/Other/Card.svg";
	public const string Active = "res://Art/Icons/Other/Active.svg";
	public const string Coins = "res://Art/Icons/Other/Coins.svg";
	public const string XP = "res://Art/Icons/Abilities/XP.svg";
	public const string Checkmark = "res://Art/Icons/Other/CheckMark.svg";
	public const string Obstacle = "res://Art/Icons/Other/Cross.svg";
	public const string StartHexMove = "res://Art/Icons/Other/StartHexMoveIcon.svg";

	public static string GetElement(Element element)
	{
		return $"res://Art/Icons/Elements/{element.ToString()}.svg";
	}

	public static string GetItem(ItemType itemType)
	{
		return $"res://Art/Icons/Items/{itemType.ToString()}.svg";
	}

	public static string GetCondition(ConditionModel conditionModel)
	{
		return conditionModel.IconPath;
	}

	public static string InlineMarker(Marker.Type markerType, int size = 30)
	{
		return Inline(GetMarker(markerType), size);
	}

	public static string GetMarker(Marker.Type markerType)
	{
		return $"res://Art/Markers/{markerType.ToString().Replace("_", string.Empty)}.png";
	}

	public static string Inline(string iconPath, int size = 30, Color? color = null)
	{
		Color finalColor = color ?? Colors.White;
		return $"[img width={size} color=#{finalColor.ToHtml()}]{iconPath}[/img]";
	}

	public static string HintText(string iconPath)
	{
		return $"[img={{{50}}}]{iconPath}[/img]";
	}
}