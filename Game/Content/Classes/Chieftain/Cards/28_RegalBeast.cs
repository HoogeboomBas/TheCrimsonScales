using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class RegalBeast : ChieftainCardModel<RegalBeast.CardTop, RegalBeast.CardBottom>
{
	public override string Name => "Regal Beast";
	public override int Level => 9;
	public override int Initiative => 81;
	protected override int AtlasIndex => 28;

	public class CardTop : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Sabretooth Tiger")
				.WithTexturePath("res://Content/Classes/Chieftain/Summons/sabretooth_tiger.png")
				.WithHealth(8, new SummonHealthSquare(this, new Vector2(0.4331527f, 0.18997385f)))
				.WithMove(3, new SummonMoveSquare(this, new Vector2(0.6306499f, 0.18997385f)))
				.WithAttack(3)
				.WithTraits(
					new AllAttacksGainAdvantageTrait(),
					new MountTrait(
						async (owner, mount) =>
						{
							ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(owner, this,
								parameters => parameters.AbilityState.Performer == owner,
								async parameters =>
								{
									parameters.SetCannotGainDisadvantage();
									await GDTask.CompletedTask;
								}
							);
							ScenarioCheckEvents.DisadvantageCheckEvent.Subscribe(owner, this,
								parameters => parameters.Attacker == owner,
								parameters => parameters.SetDisadvantage(false),
								order: 100
							);
							await GDTask.CompletedTask;
						},
						async (owner, mount) =>
						{
							ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(owner, this);
							ScenarioCheckEvents.DisadvantageCheckEvent.Unsubscribe(owner, this);
							await GDTask.CompletedTask;
						}
					)
				)
				.Build()
			),
		];

		public override int XP => 6;
		public override bool Persistent => true;
		public override bool Loss => true;
		public override bool Unrecoverable => true;
	}

	public class CardBottom : ChieftainCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					IEnumerable<AbilityCard> selectedAbilityCards =
						await AbilityCmd.SelectAbilityCards((Character)state.Performer, CardState.Lost, 0, 3,
							canSelectFunc: abilityCard => abilityCard.Top.Model.Abilities
								.Concat(abilityCard.Bottom.Model.Abilities)
								.Any(cardAbility => cardAbility.Ability is SummonAbility),
							hintText: $"Select up to 3 lost cards with summon abilities to recover");

					foreach(AbilityCard abilityCard in selectedAbilityCards)
					{
						await AbilityCmd.ReturnToHand(abilityCard);
					}

					IEnumerable<AbilityCardSide> abilitySides = selectedAbilityCards
						.SelectMany<AbilityCard, AbilityCardSide>(abilityCard => [abilityCard.Top, abilityCard.Bottom])
						.Where(abilityCardSide => abilityCardSide.Model.Abilities
							.Any(cardAbility => cardAbility.Ability is SummonAbility));

					ScenarioEvents.AbilityStartedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer && parameters.AbilityState is not SummonAbility.State,
						async parameters =>
						{
							// Allow only summon abilities if there are others on the chosen card side
							parameters.SetIsBlocked(true);

							await GDTask.CompletedTask;
						}
					);

					foreach(AbilityCardSide abilitySide in abilitySides)
					{
						await abilitySide.Perform(state.Performer);
						if(!abilitySide.AbilityCard.CardState.IsPersistent())
						{
							await AbilityCmd.LoseCard(abilitySide.AbilityCard);
							//await abilitySide.AbilityCard.SetCardState(CardState.Lost);
						}

						state.SetPerformed();
					}

					ScenarioEvents.AbilityStartedEvent.Unsubscribe(state, this);
				})
				.Build())
		];

		public override bool Loss => true;
		public override bool Unrecoverable => true;
	}
}