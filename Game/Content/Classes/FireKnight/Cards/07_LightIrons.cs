using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class LightIrons : FireKnightCardModel<LightIrons.CardTop, LightIrons.CardBottom>
{
	public override string Name => "Light Irons";
	public override int Level => 1;
	public override int Initiative => 33;
	protected override int AtlasIndex => 12 - 7;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
					{
						int itemCount = 4;

						if(await AbilityCmd.AskConsumeElement(state.Performer, Element.Fire))
						{
							itemCount++;
							await AbilityCmd.GainXP(state.Performer, 1);
						}

						FireKnight fireKnight = (FireKnight)AbilityCard.OriginalOwner;
						List<ItemModel> remainingItemModels = fireKnight.FireKnightItems.Select(item => item.ImmutableInstance).ToList();
						remainingItemModels.Shuffle(GameController.Instance.StateRNG);
						remainingItemModels = remainingItemModels.Take(Mathf.Min(fireKnight.FireKnightItems.Count, itemCount)).ToList();

						int itemsGivenToSelfCount = 0;

						while(remainingItemModels.Count > 0)
						{
							ItemModel itemModel = await AbilityCmd.SelectItem(state.Performer, remainingItemModels, "Select an item to give");

							if(itemModel == null)
							{
								break;
							}

							Figure figure = await AbilityCmd.SelectFigure(state,
								list =>
								{
									list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1));

									for(int itemIndex = list.Count - 1; itemIndex >= 0; itemIndex--)
									{
										Figure potentialTarget = list[itemIndex];
										if(!state.Performer.AlliedWith(potentialTarget, itemsGivenToSelfCount < 2) && potentialTarget is Character)
										{
											list.RemoveAt(itemIndex);
										}
									}
								}, hintText: $"Select a figure to give {itemModel.Name} to"
							);

							if(figure == null)
							{
								break;
							}

							await GiveFireKnightItem(state, [itemModel], (Character)figure,
								async (abilityState, item) =>
								{
									remainingItemModels.Remove(item.ImmutableInstance);

									if(figure == state.Performer)
									{
										itemsGivenToSelfCount++;
									}

									await GDTask.CompletedTask;
								}, true
							);
						}
					})
				.Build())
		];

		protected override int XP => 1;
		protected override bool Loss => true;
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(2).Build()),

			new AbilityCardAbility(GiveFireKnightItemAbility([ModelDB.Item<PikeHook>(), ModelDB.Item<FireproofHelm>()]))
		];
	}
}