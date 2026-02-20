using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class DevourWhole : RuinmawCardModel<DevourWhole.CardTop, DevourWhole.CardBottom>
{
	public override string Name => "Devour Whole";
	public override int Level => 8;
	public override int Initiative => 69;
	protected override int AtlasIndex => 26;

	public class CardTop : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => IsSated(parameters.Performer),
						async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AbilityAdjustAttackValue(2);
							await GDTask.CompletedTask;
						}
					)
				)
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(
						parameters => parameters.AbilityState.Target.Health <= 2,
						async parameters =>
						{
							await AbilityCmd.KillOrExhaust(parameters.AbilityState, parameters.AbilityState.Target);
						}
					)
				)
				.WithOnAbilityEndedPerformed(async state =>
					{
						if(IsSated(state.Performer))
						{
							await AbilityCmd.GainXP(state.Performer, 1);
						}
					}
				)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					AbilityCard selectedAbilityCard =
						await AbilityCmd.SelectAbilityCard((Character)state.Performer, list =>
						{
							foreach(AbilityCard card in ((Character)state.Performer).Cards)
							{
								if(card.CardState == CardState.Discarded && card.Model.Level == 1)
								{
									list.Add(card);
								}
							}
						}, CardState.Discarded, hintText: $"Select a level 1 discarded card to recover");

					if(selectedAbilityCard != null)
					{
						await AbilityCmd.ReturnToHand(selectedAbilityCard);
						state.SetPerformed();
					}
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return IsSated(state.Performer) && state.ActionState.GetAbilityState<AttackAbility.State>(0).KilledTargets.Count > 0;
				})
				.Build()),
		];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(6)
				.WithMoveType(MoveType.Jump)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(6)
				.WithCustomGetTargets((state, targets) =>
				{
					MoveAbility.State moveAbilityState = state.ActionState.GetAbilityState<MoveAbility.State>(0);
					targets.AddRange(moveAbilityState.Hexes
						.SelectMany(hex => hex.GetHexObjectsOfType<Figure>())
						.Where(f => state.Performer.EnemiesWith(f)));
				})
				.Build()),
		];

		protected override bool Sate => true;
		public override int XP => 2;
		public override bool Loss => true;
	}
}