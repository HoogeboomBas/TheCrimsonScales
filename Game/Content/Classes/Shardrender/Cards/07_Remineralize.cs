using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class Remineralize : ShardrenderCardModel<Remineralize.CardTop, Remineralize.CardBottom>
{
	public override string Name => "Remineralize";
	public override int Level => 1;
	public override int Initiative => 82;
	protected override int AtlasIndex => 7;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.6192026f, 0.18791164f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(parameters =>
					{
						foreach(AbilityCard card in ((Character)parameters.Performer).Cards)
						{
							if(card.CardState is CardState.Persistent)
							{
								foreach(ActionState activeActionState in card.ActiveActionStates)
								{
									if(activeActionState.AbilityStates.FirstOrDefault(abilityState => abilityState is CrystallizeAbility.State) !=
									   null)
									{
										return true;
									}
								}
							}
						}

						return false;
					}, async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);

						await GDTask.CompletedTask;
					}, canApplyMultipleTimesDuringSubscription: false))
				.Build()),
			new AbilityCardAbility(MoveCharacterTokenBackAbility(1)
				.WithConditionalAbilityCheck(async state =>
				{
					if(state.ActionState.GetAbilityState<AttackAbility.State>(0).KilledTargets.Count == 0)
					{
						return false;
					}

					await AbilityCmd.GainXP(state.Performer, 1);
					return true;
				})
				.Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(LootAbility.Builder()
				.WithRange(2)
				.Build()),
			new AbilityCardAbility(
				MoveCharacterTokenBackAbility(new DynamicInt<OtherAbility.State>(state =>
						state.ActionState.GetAbilityState<LootAbility.State>(0).LootedCoinCount))
					.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 0))
					.Build())
		];

		public override int XP => 1;
		public override bool Loss => true;
	}
}