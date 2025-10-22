using System;
using System.Collections.Generic;
using Fractural.Tasks;

public abstract class HierophantLevelUpCardModel<TTop, TBottom> : AtlasAbilityCardModel<TTop, TBottom>
	where TTop : HierophantCardSide, new()
	where TBottom : HierophantCardSide, new()
{
	protected override string TexturePath => "res://Content/Classes/Hierophant/LevelUpCards.jpg";
	protected override int ColumnCount => 5;
	protected override int RowCount => 4;
}

public abstract class HierophantCardModel<TTop, TBottom> : AtlasAbilityCardModel<TTop, TBottom>
	where TTop : HierophantCardSide, new()
	where TBottom : HierophantCardSide, new()
{
	protected override string TexturePath => "res://Content/Classes/Hierophant/Cards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 4;
}

public abstract class HierophantCardSide : AbilityCardSide
{
	protected GiveAbilityCardAbility GivePrayerCardAbility(int targets = 1, int range = 1,
		Action<GiveAbilityCardAbility.State, List<Figure>> customGetTargets = null,
		GiveAbilityCardAbility.ConditionalAbilityCheckDelegate conditionalAbilityCheck = null)
	{
		return GiveAbilityCardAbility.Builder()
			.WithGetAbilityCards((state, list) =>
			{
				Hierophant hierophant = (Hierophant)AbilityCard.OriginalOwner;
				list.AddRange(hierophant.PrayerCards);
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
				Hierophant hierophant = (Hierophant)AbilityCard.OriginalOwner;
				list.AddRange(hierophant.PrayerCards);
			},
			OnCardGiven, OnCardDiscarded, OnCardLost
		);
	}

	private async GDTask OnCardGiven(AbilityState abilityState, AbilityCard abilityCard)
	{
		Hierophant hierophant = (Hierophant)abilityCard.OriginalOwner;
		hierophant.PrayerCards.Remove(abilityCard);

		await GDTask.CompletedTask;
	}

	private async GDTask OnCardDiscarded(AbilityCard abilityCard)
	{
		abilityCard.Owner.RemoveCard(abilityCard);

		Hierophant hierophant = (Hierophant)abilityCard.OriginalOwner;
		hierophant.PrayerCards.Add(abilityCard);
		abilityCard.SetOwner(hierophant);

		await AbilityCmd.ReturnToHand(abilityCard);
	}

	private async GDTask OnCardLost(AbilityCard abilityCard)
	{
		abilityCard.Owner.RemoveCard(abilityCard);

		Hierophant hierophant = (Hierophant)abilityCard.OriginalOwner;
		hierophant.PrayerCards.Add(abilityCard);
		abilityCard.SetOwner(hierophant);

		await GDTask.CompletedTask;
	}
}