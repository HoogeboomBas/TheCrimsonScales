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
			new AbilityCardAbility(new HealAbility(3, targets: 2, range: 2,
				duringHealSubscriptions:
				[
					ScenarioEvents.DuringHeal.Subscription.ConsumeElement(Element.Fire,
						parameters => true,
						async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Bless);

							await AbilityCmd.GainXP(parameters.Performer, 1);
						},
						effectInfoViewParameters: new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.GetCondition(Conditions.Bless))}")
					)
				],
				afterHealPerformedSubscriptions:
				[
					ScenarioEvents.AfterHealPerformed.Subscription.New(
						parameters => parameters.AbilityState.SingleTargetState.RemovedConditions.Count > 0,
						async parameters =>
						{
							await AbilityCmd.AddCondition(parameters.AbilityState, parameters.AbilityState.SingleTargetState.Target, Conditions.Strengthen);
						}
					)
				]
			))
		];
	}

	public class CardBottom : FireKnightCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new MoveAbility(5)),

			new AbilityCardAbility(new OtherAbility(
				async state =>
				{
					int itemCount = 2;

					FireKnight fireKnight = (FireKnight)AbilityCard.OriginalOwner;
					FireKnightModel fireKnightModel = (FireKnightModel)fireKnight.ClassModel;
					List<ItemModel> remainingItemModels = fireKnightModel.AllItems.ToList(); // fireKnight.ClassModel.Select(item => item.ImmutableInstance).ToList();
					//remainingItemModels.Shuffle(GameController.Instance.StateRNG);
					//remainingItemModels = remainingItemModels.Take(Mathf.Min(fireKnight.FireKnightItems.Count, itemCount)).ToList();

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

								await GDTask.CompletedTask;
							}, true
						);
					}
				},
				conditionalAbilityCheck: async state =>
				{
					MoveAbility.State moveAbilityState = state.ActionState.GetAbilityState<MoveAbility.State>(0);

					if(moveAbilityState.Performed)
					{
						foreach(Hex hex in moveAbilityState.Hexes)
						{
							foreach(Figure figure in hex.GetHexObjectsOfType<Figure>())
							{
								if(state.Performer.AlliedWith(figure))
								{
									return await AbilityCmd.AskConsumeElement(state.Performer, Element.Light);
								}
							}
						}
					}

					return false;
				}
			)),

			new AbilityCardAbility(new OtherActiveAbility(
				async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, false).Any(figure => state.Performer.AlliedWith(figure)),
						async parameters =>
						{
							FireKnight fireKnight = (FireKnight)AbilityCard.OriginalOwner;
							if(await AbilityCmd.AskConsumeElement(state.Performer, Element.Fire, $"Consume {Icons.Inline(Icons.GetElement(Element.Fire))} to give an adjacent ally a {Icons.Inline(fireKnight.ClassModel.IconPath)} item."))
							{
							}
						}
					);

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.FigureTurnEndingEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}
			))
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}