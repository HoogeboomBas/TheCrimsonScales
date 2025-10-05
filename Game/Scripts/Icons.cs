public static class Icons
{
	public const string LoseCard = "res://Art/Icons/Abilities/LoseCard.svg";
	public const string LoseDiscardedCards = "res://Art/Icons/Abilities/LoseDiscardedCards.svg";
	public const string RecoverCard = "res://Art/Icons/Abilities/LoseCard.svg";
	public const string Attack = "res://Art/Icons/Abilities/Attack.svg";
	public const string Move = "res://Art/Icons/Abilities/Move.svg";
	public const string Heal = "res://Art/Icons/Abilities/Health.svg";
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

	public static string Marker(Marker.Type markerType)
	{
		return $"[color=red]([/color]{markerType.ToString().Replace("_", string.Empty)}[color=red])[/color]";
	}

	public static string Inline(string iconPath, int size = 30)
	{
		return $"[img={{{size}}}]{iconPath}[/img]";
	}

	public static string HintText(string iconPath)
	{
		return $"[img={{{50}}}]{iconPath}[/img]";
	}
}