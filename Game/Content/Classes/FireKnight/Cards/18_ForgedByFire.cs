using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class ForgedByFire : FireKnightLevelUpCardModel<ForgedByFire.CardTop, ForgedByFire.CardBottom>
{
	public override string Name => "Forged By Fire";
	public override int Level => 4;
	public override int Initiative => 39;
	protected override int AtlasIndex => 10;

	public class CardTop : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3)
				.WithTargets(2)
				.WithRange(2)
				.WithDuringHealSubscription(
					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Fire,
						parameters => true,
						async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Bless);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.GetCondition(Conditions.Bless))}")
					))
				.WithAfterHealPerformedSubscription(
					ScenarioEvents.AfterHealPerformed.Subscription.New(
						parameters => parameters.AbilityState.SingleTargetState.RemovedConditions.Count > 0,
						async parameters =>
						{
							await AbilityCmd.AddCondition(parameters.AbilityState, parameters.AbilityState.SingleTargetState.Target,
								Conditions.Strengthen);
						}
					)
				)
				.Build())
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder().WithDistance(5).Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					int itemCount = 2;

					FireKnight fireKnight = (FireKnight)AbilityCard.OriginalOwner;
					FireKnightModel fireKnightModel = (FireKnightModel)fireKnight.ClassModel;
					List<ItemModel> remainingItemModels = fireKnightModel.AllItems.ToList();

					for(int i = 0; i < itemCount; i++)
					{
						ItemModel itemModel = await AbilityCmd.SelectItem(state.Performer, remainingItemModels, "Select an item to give");

						if(itemModel == null)
						{
							break;
						}

						Figure figure = await AbilityCmd.SelectFigure(state,
							list =>
							{
								MoveAbility.State moveAbilityState = state.ActionState.GetAbilityState<MoveAbility.State>(0);
								foreach(Hex hex in moveAbilityState.Hexes)
								{
									foreach(Character character in hex.GetHexObjectsOfType<Character>())
									{
										if(state.Performer.AlliedWith(character))
										{
											list.Add(character);
										}
									}
								}

								list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1));
							}, hintText: $"Select an ally to give {itemModel.Name} to"
						);

						if(figure == null)
						{
							break;
						}

						await GiveFireKnightItem(state, [itemModel], (Character)figure,
							async (abilityState, item) =>
							{
								remainingItemModels.Remove(item.ImmutableInstance);

								await GDTask.CompletedTask;
							}, true
						);
					}
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					MoveAbility.State moveAbilityState = state.ActionState.GetAbilityState<MoveAbility.State>(0);

					if(moveAbilityState.Performed)
					{
						foreach(Hex hex in moveAbilityState.Hexes)
						{
							foreach(Figure figure in hex.GetHexObjectsOfType<Figure>())
							{
								if(state.Performer.AlliedWith(figure))
								{
									return true;
								}
							}
						}
					}

					return false;
				})
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, false)
							.Any(figure => state.Performer.AlliedWith(figure)),
						async parameters =>
						{
							FireKnight fireKnight = (FireKnight)AbilityCard.OriginalOwner;

							if(await AbilityCmd.AskConsumeElement(state.Performer, Element.Fire,
								   $"Consume {Icons.Inline(Icons.GetElement(Element.Fire))} to give an adjacent ally a {Icons.Inline(fireKnight.ClassModel.IconPath)} item."))
							{
								FireKnightModel fireKnightModel = (FireKnightModel)fireKnight.ClassModel;
								List<ItemModel> remainingItemModels = fireKnightModel.AllItems.ToList();
								ItemModel itemModel = await AbilityCmd.SelectItem(state.Performer, remainingItemModels, "Select an item to give");

								Figure figure = await AbilityCmd.SelectFigure(state,
									list =>
									{
										list.AddRange(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1));

										for(int itemIndex = list.Count - 1; itemIndex >= 0; itemIndex--)
										{
											Figure potentialTarget = list[itemIndex];
											if(!state.Performer.AlliedWith(potentialTarget) && potentialTarget is Character)
											{
												list.RemoveAt(itemIndex);
											}
										}
									}, hintText: $"Select an ally to give {itemModel.Name} to"
								);

								if(figure == null)
								{
									return;
								}

								await GiveFireKnightItem(state, [itemModel], (Character)figure, null, true);
							}
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}