using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class FarseersPilum : IncarnateCardModel<FarseersPilum.CardTop, FarseersPilum.CardBottom>
{
	public override string Name => "Farseer's Pilum";
	public override int Level => 1;
	public override int Initiative => 48;
	protected override int AtlasIndex => 1;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(5)
				.WithRange(5)
				.WithDuringAttackSubscription(
					InSpiritSubscription<ScenarioEvents.DuringAttack.Parameters>(IncarnateSpirit.Ritualist,
						async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Incarnate.Enfeeble);
							parameters.AbilityState.AbilityAddCondition(Incarnate.Enfeeble);

							await GDTask.CompletedTask;
						}))
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6218548f, 0.6349031f)))
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => parameters.AbilityState.Performer == state.Performer &&
						              parameters.AbilityState.IsSingleTarget &&
						              parameters.AbilityState.AbilityRangeType == RangeType.Melee,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustRange(2);
							parameters.AbilityState.AbilitySetRangeType(RangeType.Range);

							await GDTask.CompletedTask;
						});

					await AbilityCmd.InfuseElement(state, Element.Air);
					state.ActionState.SetOverrideRound();
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Ritualist))
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Conqueror, IncarnateSpirit.Reaver];
	}
}