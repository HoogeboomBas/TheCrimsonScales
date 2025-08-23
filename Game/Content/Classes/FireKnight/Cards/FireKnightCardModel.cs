using System;
using System.Collections.Generic;
using Fractural.Tasks;

public abstract class FireKnightLevelUpCardModel<TTop, TBottom> : AtlasAbilityCardModel<TTop, TBottom>
	where TTop : FireKnightCardSide, new()
	where TBottom : FireKnightCardSide, new()
{
	protected override string TexturePath => "res://Content/Classes/FireKnight/LevelUpCards.jpg";
	protected override int ColumnCount => 4;
	protected override int RowCount => 4;
}

public abstract class FireKnightCardModel<TTop, TBottom> : AtlasAbilityCardModel<TTop, TBottom>
	where TTop : FireKnightCardSide, new()
	where TBottom : FireKnightCardSide, new()
{
	protected override string TexturePath => "res://Content/Classes/FireKnight/Cards.jpg";
	protected override int ColumnCount => 5;
	protected override int RowCount => 3;
}

public abstract class FireKnightCardSide : AbilityCardSide
{
	protected const string LadderIconPath = "res://Content/Classes/FireKnight/LadderIcon.svg";

	protected GiveItemAbility GiveFireKnightItemAbility(IList<ItemModel> possibleItemModels,
		int targets = 1, int range = 1, Target target = Target.Allies,
		Action<GiveItemAbility.State, List<Figure>> customGetTargets = null,
		Func<AbilityState, ItemModel, GDTask> onItemGiven = null,
		GiveItemAbility.ConditionalAbilityCheckDelegate conditionalAbilityCheck = null)
	{
		return GiveItemAbility.Builder()
			.WithGetItems((state, list) =>
			{
				FireKnight fireKnight = (FireKnight)AbilityCard.OriginalOwner;
				foreach(ItemModel item in fireKnight.FireKnightItems)
				{
					if(possibleItemModels.Contains(item.ImmutableInstance))
					{
						list.Add(item);
					}
				}
			})
			.WithOnItemGiven((state, item) => OnItemGiven(state, item, onItemGiven))
			.WithOnItemConsumed(OnItemConsumed)
			.WithTargets(targets)
			.WithRange(range)
			.WithTarget(target)
			.WithCustomGetTargets(customGetTargets)
			.WithConditionalAbilityCheck(conditionalAbilityCheck)
			.Build();
	}

	protected async GDTask GiveFireKnightItem(AbilityState abilityState, IList<ItemModel> possibleItemModels, Character target,
		Func<AbilityState, ItemModel, GDTask> onItemGiven = null, bool selectAutomatically = false)
	{
		await GiveItemAbility.GiveItem(abilityState, target,
			(state, list) =>
			{
				FireKnight fireKnight = (FireKnight)AbilityCard.OriginalOwner;
				foreach(ItemModel item in fireKnight.FireKnightItems)
				{
					if(possibleItemModels.Contains(item.ImmutableInstance))
					{
						list.Add(item);
					}
				}
			},
			(state, item) => OnItemGiven(state, item, onItemGiven), OnItemConsumed, selectAutomatically
		);
	}

	private async GDTask OnItemGiven(AbilityState abilityState, ItemModel item, Func<AbilityState, ItemModel, GDTask> onItemGiven)
	{
		FireKnight fireKnight = (FireKnight)item.OriginalOwner;
		fireKnight.FireKnightItems.Remove(item);

		if(onItemGiven != null)
		{
			await onItemGiven(abilityState, item);
		}
	}

	private async GDTask OnItemConsumed(ItemModel item)
	{
		item.Owner.RemoveItem(item);

		FireKnight fireKnight = (FireKnight)item.OriginalOwner;
		fireKnight.FireKnightItems.Add(item);
		item.SetOwner(null);

		await item.Refresh();
	}
}