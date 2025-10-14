using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class SacredDeath : HierophantCardModel<SacredDeath.CardTop, SacredDeath.CardBottom>
{
	public override string Name => "Sacred Death";
	public override int Level => 1;
	public override int Initiative => 81;
	protected override int AtlasIndex => 13 - 9;

	public class CardTop : HierophantCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder().WithDamage(3).WithRange(3).Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Bless)
				.WithRange(2)
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.WithConditionalAbilityCheck(async state =>
					{
						await GDTask.CompletedTask;

						AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);

						return attackAbilityState.Performed && attackAbilityState.KilledTargets.Count > 0;
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
						AbilityCard abilityCard = await AbilityCmd.SelectAbilityCard(state.Performer, list =>
						{
							if(state.Performer is Character character)
							{
								foreach(AbilityCard roundCard in character.RoundCards)
								{
									if(roundCard.CardState == CardState.Lost)
									{
										list.Add(roundCard);
									}
								}
							}
						}, CardState.Lost, hintText: "Select a lost card to recover.");

						if(abilityCard != null)
						{
							await AbilityCmd.ReturnToHand(abilityCard);

							state.SetPerformed();
						}

						List<AbilityCard> selectedAbilityCards =
							await AbilityCmd.SelectAbilityCards(state.Performer as Character, CardState.Discarded, 0, 2,
								hintText: $"Select up to two discarded cards to recover");

						foreach(AbilityCard selectedAbilityCard in selectedAbilityCards)
						{
							await AbilityCmd.ReturnToHand(selectedAbilityCard);

							state.SetPerformed();
						}

						state.SetCustomValue(this, "RecoveredCards", selectedAbilityCards.Select(card => card.ReferenceId).ToList());
					}
				)
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					List<int> recoveredCardIds = state.ActionState.GetAbilityState<OtherAbility.State>(0)
						.GetCustomValue<List<int>>(this, "RecoveredCards");

					AbilityCard selectedAbilityCard =
						await AbilityCmd.SelectAbilityCard(state.Performer, list =>
						{
							foreach(int recoveredCardId in recoveredCardIds)
							{
								AbilityCard abilityCard = GameController.Instance.ReferenceManager.Get<AbilityCard>(recoveredCardId);
								list.Add(abilityCard);
							}
						}, CardState.Hand, hintText: "Select a recovered card to play.");

					if(selectedAbilityCard != null)
					{
						await selectedAbilityCard.Bottom.Perform(state.Performer);
					}
				})
				.WithConditionalAbilityCheck(async state =>
					{
						if(!await AbilityCmd.HasPerformedAbility(state, 0))
						{
							return false;
						}

						List<int> recoveredCardIds = state.ActionState.GetAbilityState<OtherAbility.State>(0)
							.GetCustomValue<List<int>>(this, "RecoveredCards");

						if(recoveredCardIds == null || recoveredCardIds.Count == 0)
						{
							return false;
						}

						return true;
					}
				)
				.Build())
		];

		protected override bool Loss => true;
		protected override bool Unrecoverable => true;
	}
}