using System;
using System.Collections.Generic;
using Fractural.Tasks;

public abstract class ThornreaperLevelUpCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : ThornreaperCardSide
	where TBottom : ThornreaperCardSide
{
	protected override string TexturePath => "res://Content/Classes/Thornreaper/LevelUpCards.jpg";
	protected override int ColumnCount => 5;
	protected override int RowCount => 4;
}

public abstract class ThornreaperCardModel<TTop, TBottom> : AbilityCardModel<TTop, TBottom>
	where TTop : ThornreaperCardSide
	where TBottom : ThornreaperCardSide
{
	protected override string TexturePath => "res://Content/Classes/Thornreaper/Cards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 4;
}

public abstract class ThornreaperCardSide : AbilityCardSideModel<Thornreaper>
{
	protected GiveAbilityCardAbility GivePrayerCardAbility(int targets = 1, int range = 1,
		Action<GiveAbilityCardAbility.State, List<Figure>> customGetTargets = null,
		GiveAbilityCardAbility.ConditionalAbilityCheckDelegate conditionalAbilityCheck = null)
	{
		return GiveAbilityCardAbility.Builder()
			.WithGetAbilityCards((state, list) =>
			{
				Thornreaper Thornreaper = GetOriginalOwner(state);
				list.AddRange(Thornreaper.PrayerCards);
			})
			.WithOnCardGiven(OnCardGiven)
			.WithOnCardDiscarded(OnCardDiscarded)
			.WithOnCardLost(OnCardLost)
			.WithTargets(targets)
			.WithRange(range)
			.WithCustomGetTargets(customGetTargets)
			.WithConditionalAbilityCheck(conditionalAbilityCheck)
			.Build();
	}

	protected async GDTask GivePrayerCard(AbilityState abilityState, Figure target)
	{
		await GiveAbilityCardAbility.GiveAbilityCard(abilityState, target,
			(state, list) =>
			{
				Thornreaper Thornreaper = GetOriginalOwner(state);
				list.AddRange(Thornreaper.PrayerCards);
			},
			OnCardGiven, OnCardDiscarded, OnCardLost
		);
	}

	public static async GDTask GivePrayerCard(AbilityState abilityState, Thornreaper Thornreaper, Figure target)
	{
		await GiveAbilityCardAbility.GiveAbilityCard(abilityState, target,
			(state, list) =>
			{
				list.AddRange(Thornreaper.PrayerCards);
			},
			OnCardGiven, OnCardDiscarded, OnCardLost
		);
	}

	private static async GDTask OnCardGiven(AbilityState abilityState, AbilityCard abilityCard)
	{
		Thornreaper Thornreaper = (Thornreaper)abilityCard.OriginalOwner;
		Thornreaper.PrayerCards.Remove(abilityCard);

		await GDTask.CompletedTask;
	}

	private static async GDTask OnCardDiscarded(AbilityCard abilityCard)
	{
		abilityCard.Owner.RemoveCard(abilityCard);

		Thornreaper Thornreaper = (Thornreaper)abilityCard.OriginalOwner;
		Thornreaper.PrayerCards.Add(abilityCard);
		abilityCard.SetOwner(Thornreaper);

		await AbilityCmd.ReturnToHand(abilityCard);
	}

	private static async GDTask OnCardLost(AbilityCard abilityCard)
	{
		abilityCard.Owner.RemoveCard(abilityCard);

		Thornreaper Thornreaper = (Thornreaper)abilityCard.OriginalOwner;
		Thornreaper.PrayerCards.Add(abilityCard);
		abilityCard.SetOwner(Thornreaper);

		await GDTask.CompletedTask;
	}
}