using System.Collections.Generic;
using Fractural.Tasks;

public class VitalBond : HierophantLevelUpCardModel<VitalBond.CardTop, VitalBond.CardBottom>
{
	public override string Name => "Vital Bond";
	public override int Level => 3;
	public override int Initiative => 48;
	protected override int AtlasIndex => 15 - 3;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithTargets(2)
				.WithRange(4)
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

					ConfirmPrompt.Answer confirmAnswer =
						await PromptManager.Prompt(new ConfirmPrompt(null, () => "Swap the positions of the targets?"), state.Performer);
					if(confirmAnswer.Confirmed)
					{
						state.SetPerformed();

						Figure figureA = attackAbilityState.UniqueTargetedFigures[0];
						Figure figureB = attackAbilityState.UniqueTargetedFigures[0];

						Hex oldFigureAHex = figureA.Hex;

						// Swap the positions, currently not applying any effects in the hexes
						figureA.SetOriginHexAndRotation(figureB.Hex);
						figureB.SetOriginHexAndRotation(oldFigureAHex);

						await AbilityCmd.GainXP(state.Performer, 1);

						foreach(Figure figure in attackAbilityState.UniqueTargetedFigures)
						{
							await AbilityCmd.AddCondition(state, figure, Conditions.Muddle);
						}
					}

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(async state =>
					{
						await GDTask.CompletedTask;

						AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
						return attackAbilityState.Performed && attackAbilityState.UniqueTargetedFigures.Count == 2 &&
						       attackAbilityState.UniqueTargetedFigures.TrueForAll(figure => !figure.IsDead);
					}
				)
				.Build())
		];
	}

	public class CardBottom : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
					{
						Figure figure = await AbilityCmd.SelectFigure(state,
							list =>
							{
								foreach(Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 1))
								{
									if(
										state.Performer.AlliedWith(figure) &&
										figure is Character character)
									{
										list.Add(figure);
									}
								}
							}
						);

						if(figure == null)
						{
							return;
						}

						state.SetPerformed();

						Character character = (Character)figure;
						AbilityCard abilityCard =
							await AbilityCmd.SelectAbilityCard(character, CardState.Lost, hintText: "Select a lost card to recover");
						if(abilityCard != null)
						{
							await AbilityCmd.ReturnToHand(abilityCard);
						}

						IEnumerable<AbilityCard> abilityCards = await AbilityCmd.SelectAbilityCards(character, CardState.Discarded, 0, 2,
							hintText: "Select up to two discarded cards to recover");

						foreach(AbilityCard card in abilityCards)
						{
							await AbilityCmd.ReturnToHand(card);
						}
					}
				)
				.Build())
		];

		protected override IEnumerable<Element> Elements => [Element.Earth];
		protected override int XP => 1;
		protected override bool Unrecoverable => true;
		protected override bool Loss => true;
	}
}