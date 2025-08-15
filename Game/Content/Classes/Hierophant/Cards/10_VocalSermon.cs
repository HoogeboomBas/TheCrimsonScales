using System.Collections.Generic;
using Fractural.Tasks;

public class VocalSermon : HierophantCardModel<VocalSermon.CardTop, VocalSermon.CardBottom>
{
	public override string Name => "Vocal Sermon";
	public override int Level => 1;
	public override int Initiative => 32;
	protected override int AtlasIndex => 29 - 10;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new OtherAbility(async state =>
				{
					int remainingRecoverCount = 6;

					foreach(Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 3))
					{
						if(remainingRecoverCount > 0 && figure is Character character && state.Performer.AlliedWith(figure, true))
						{
							IEnumerable<AbilityCard> selectedAbilityCards =
								await AbilityCmd.SelectAbilityCards(character, CardState.Discarded, 0, remainingRecoverCount,
									hintText: $"Select up to {remainingRecoverCount} cards to recover");

							foreach(AbilityCard selectedAbilityCard in selectedAbilityCards)
							{
								await AbilityCmd.ReturnToHand(selectedAbilityCard);
								remainingRecoverCount--;

								state.SetPerformed();
							}
						}
					}
				}
			))
		];

		protected override IEnumerable<Element> Elements => [Element.Earth, Element.Light];
		protected override int XP => 1;
		protected override bool Loss => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new MoveAbility(3, MoveType.Jump,
				duringMovementSubscriptions:
				[
					// ScenarioEvent<ScenarioEvents.DuringMovement.Parameters>.Subscription.ConsumeElement(Element.Light,
					// 	canApplyFunction: canApplyParameters =>
					// 	{
					// 		MoveAbility.State moveAbilityState = canApplyParameters.AbilityState;
					// 		foreach(Hex hex in moveAbilityState.Hexes)
					// 		{
					// 			foreach(Figure figure in hex.GetHexObjectsOfType<Figure>())
					// 			{
					// 				if(figure != parameters.Performer && parameters.Performer.AlliedWith(figure))
					// 				{
					// 					return true;
					// 				}
					// 			}
					// 		}
					//
					// 		return false;
					// 	},
					// 	applyFunction: async applyParameters =>
					// 	{
					// 		MoveAbilityState moveAbilityState = (MoveAbilityState)applyParameters.AbilityState;
					// 		await GivePrayerCard(applyParameters.AbilityState,
					// 			customGetTargets: list =>
					// 			{
					// 				foreach(Hex hex in moveAbilityState.Hexes)
					// 				{
					// 					foreach(Figure figure in hex.GetHexObjectsOfType<Figure>())
					// 					{
					// 						if(figure != parameters.Performer && parameters.Performer.AlliedWith(figure))
					// 						{
					// 							list.Add(figure);
					// 						}
					// 					}
					// 				}
					// 			});
					// 	})
				]
			)),

			new AbilityCardAbility(GivePrayerCardAbility(
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
				},
				customGetTargets: (state, list) =>
				{
					MoveAbility.State moveAbilityState = state.ActionState.GetAbilityState<MoveAbility.State>(0);

					foreach(Hex hex in moveAbilityState.Hexes)
					{
						foreach(Figure figure in hex.GetHexObjectsOfType<Figure>())
						{
							if(state.Performer.AlliedWith(figure))
							{
								list.Add(figure);
							}
						}
					}
				}
			))
		];
	}
}