using System;

public enum PartyAchievement
{
	OozeDestroyed = 0,
	FollowTheMoney = 1,
	FallenLava = 2,
	FrozenWarrior = 3,
	APotionLost = 4,
	TakeTheMoney = 5,
	OaksAlliance = 6,
	AccomplishedMercenaries = 7,
	InoxAlliance = 8,
}

public static class PartyAchievementExtensions
{
	public static string ToPrettyString(this PartyAchievement achievement)
	{
		switch(achievement)
		{
			case PartyAchievement.OozeDestroyed:
				return "Ooze Destroyed";
			case PartyAchievement.FollowTheMoney:
				return "Follow the Money";
			case PartyAchievement.FallenLava:
				return "Fallen Lava";
			case PartyAchievement.FrozenWarrior:
				return "Frozen Warrior";
			case PartyAchievement.APotionLost:
				return "A Potion Lost";
			case PartyAchievement.TakeTheMoney:
				return "Take the Money";
			case PartyAchievement.OaksAlliance:
				return "Oak's Alliance";
			case PartyAchievement.AccomplishedMercenaries:
				return "Accomplished Mercenaries";
			case PartyAchievement.InoxAlliance:
				return "Inox Alliance";
			default:
				throw new ArgumentOutOfRangeException(nameof(achievement), achievement, null);
		}
	}
}