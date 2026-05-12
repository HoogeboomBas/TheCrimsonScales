using System.Collections.Generic;
using System.Linq;
using Godot;

public class CircleOfLifeless : SpiritCallerCardModel<CircleOfLifeless.CardTop, CircleOfLifeless.CardBottom>
{
	public override string Name => "Circle of Lifeless";
	public override int Level => 8;
	public override int Initiative => 90;
	protected override int AtlasIndex => 28 - 26;

	public class CardTop : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					IEnumerable<AbilityCard> selectedAbilityCards =
						await AbilityCmd.SelectAbilityCards((Character)state.Performer, CardState.Lost, 0, 2,
							canSelectFunc: abilityCard => abilityCard.Top.Model.Abilities
								.Concat(abilityCard.Bottom.Model.Abilities)
								.Any(cardAbility => cardAbility.Ability is SpawnAbility),
							hintText: $"Select up to 2 lost cards with spawn abilities to recover");

					foreach(AbilityCard abilityCard in selectedAbilityCards)
					{
						await AbilityCmd.ReturnToHand(abilityCard);
					}

					IEnumerable<AbilityCardSide> abilitySides = selectedAbilityCards
						.SelectMany<AbilityCard,
							AbilityCardSide>(abilityCard => [abilityCard.Top]) //, abilityCard.Bottom]) //TODO: Allow bottoms to be played
						.Where(abilityCardSide => abilityCardSide.Model.Abilities
							.Any(cardAbility => cardAbility.Ability is SpawnAbility));

					// ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
					// 	parameters => parameters.Performer == state.Performer && parameters.AbilityState is not SpawnAbility.State,
					// 	async parameters =>
					// 	{
					// 		// Allow only summon abilities if there are others on the chosen card side
					// 		parameters.SetIsBlocked(true);
					//
					// 		await GDTask.CompletedTask;
					// 	}
					// );

					foreach(AbilityCardSide abilitySide in abilitySides)
					{
						await abilitySide.Perform(state.Performer);

						if(!abilitySide.AbilityCard.CardState.IsPersistent())
						{
							await AbilityCmd.LoseCard(abilitySide.AbilityCard);
							// await abilitySide.AbilityCard.SetCardState(CardState.Lost);
						}

						state.SetPerformed();
					}

					//ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);
				})
				.Build())
		];

		public override bool Unrecoverable => true;
		public override bool Loss => true;
	}

	public class CardBottom : SpiritCallerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.52304226f, 0.6980486f)))
				.WithMoveType(MoveType.Jump)
				.WithOnAbilityEndedPerformed(async state =>
				{
					Figure spirit = await AbilityCmd.SelectFigure(state, list =>
					{
						foreach(Hex hex in state.Hexes)
						{
							list.AddRange(hex.GetFigures(true).Where(figure => Spirit.CountsAsSpirit(figure)));
						}
					});

					if(spirit != null)
					{
						await AbilityCmd.AddCondition(state, state.Performer, Conditions.Curse);

						await Spirit.RemoveDamageCounters(spirit, 1);
					}
				})
				.Build()),
		];
	}
}