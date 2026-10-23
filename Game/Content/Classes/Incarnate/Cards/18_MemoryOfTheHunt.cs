using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class MemoryOfTheHunt : IncarnateCardModel<MemoryOfTheHunt.CardTop, MemoryOfTheHunt.CardBottom>
{
	public override string Name => "Memory of the Hunt";
	public override int Level => 4;
	public override int Initiative => 12;
	protected override int AtlasIndex => 18;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.6170745f, 0.17728531f)))
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2, new HealDiamondPlus(this, new Vector2(0.2738522f, 0.29307482f)))
				.WithTarget(Target.Self)
				.WithDuringHealSubscription(
					InSpiritSubscription<ScenarioEvents.DuringHeal.Parameters>(IncarnateSpirit.Conqueror,
						async parameters =>
						{
							parameters.AbilityState.SetTarget(Target.SelfOrAllies);
							parameters.AbilityState.AdjustTargets(1);
							parameters.AbilityState.AbilityAdjustRange(1);

							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Earth);
						}))
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Ritualist];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.6186266f, 0.701739f)))
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustPierce(3);

							await state.ActionState.RequestDiscardOrLose();
						});

					state.ActionState.SetOverrideRound();

					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Reaver))
				.Build())
		];
	}
}