using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class VadisLastStand : IncarnateCardModel<VadisLastStand.CardTop, VadisLastStand.CardBottom>
{
	public override string Name => "Vadi's Last Stand";
	public override int Level => 9;
	public override int Initiative => 21;
	protected override int AtlasIndex => 28;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.61735046f, 0.1700831f)))
				.WithDuringAttackSubscriptions(
				[
					InSpiritSubscription<ScenarioEvents.DuringAttack.Parameters>(IncarnateSpirit.Ritualist,
						async parameters =>
						{
							parameters.AbilityState.AdjustTargets(2);

							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Air);
						}),
					InSpiritSubscription<ScenarioEvents.DuringAttack.Parameters>(IncarnateSpirit.Reaver,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(4);

							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Fire);
						})
				])
				.Build()),
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(5, new HealDiamondPlus(this, new Vector2(0.4880471f, 0.39158976f)))
				.WithTarget(Target.Self)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.InfuseElement(state, Element.Earth);
				})
				.Build())
		];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(6, new MoveCircle(this, new Vector2(0.63036764f, 0.6379732f)))
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Ritualist))
				.Build()),
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(3, new ShieldCircle(this, new Vector2(0.56575394f, 0.7268698f)))
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.WithOnAbilityEndedPerformed(async state =>
				{
					state.ActionState.SetOverrideRound();

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(3, new RetaliateCircle(this, new Vector2(0.5649779f, 0.8149585f)))
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Reaver))
				.WithOnAbilityEndedPerformed(async state =>
				{
					state.ActionState.SetOverrideRound();

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices =>
			[IncarnateSpirit.Ritualist, IncarnateSpirit.Conqueror, IncarnateSpirit.Reaver];
	}
}