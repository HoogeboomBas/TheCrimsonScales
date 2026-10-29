using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class AncientBanner : IncarnateCardModel<AncientBanner.CardTop, AncientBanner.CardBottom>
{
	public override string Name => "Ancient Banner";
	public override int Level => 1;
	public override int Initiative => 20;
	protected override int AtlasIndex => 0;

	public class CardTop : IncarnateCardSide
	{
		private MoveCircle _moveEnhancementMark;
		private AttackDiamond _attackEnhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_moveEnhancementMark = new MoveCircle(this, new Vector2(0.44623938f, 0.27162224f), EnhancementCostType.MultiTarget);
			_attackEnhancementMark = new AttackDiamond(this, new Vector2(0.44371122f, 0.3821715f), EnhancementCostType.MultiTarget);
		}

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(
					MoveAbility.Builder()
						.WithDistance(3, _moveEnhancementMark)
						.Build(),
					AttackAbility.Builder()
						.WithDamage(3, _attackEnhancementMark)
						.Build())
				.WithTargets(2)
				.WithTarget(Target.SelfOrAllies | Target.SelfCountsForTargets)
				.WithRange(2)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.Build()),
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, _moveEnhancementMark)
				.WithDuringMovementSubscription(
					InSpiritSubscription<ScenarioEvents.DuringMovement.Parameters>(IncarnateSpirit.Ritualist,
						async parameters =>
						{
							parameters.AbilityState.AdjustMoveValue(2);

							await GDTask.CompletedTask;
						}))
				.WithConditionalAbilityCheck(async state => !await InSpirit(state, IncarnateSpirit.Conqueror))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, _attackEnhancementMark)
				.WithDuringAttackSubscription(
					InSpiritSubscription<ScenarioEvents.DuringAttack.Parameters>(IncarnateSpirit.Reaver,
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(2);

							await GDTask.CompletedTask;
						}))
				.WithConditionalAbilityCheck(async state => !await InSpirit(state, IncarnateSpirit.Conqueror))
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(ShieldAbility.Builder().WithShieldValue(1).Build())
				.WithRange(2)
				.Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(RetaliateAbility.Builder().WithRetaliateValue(1).Build())
				.WithRange(2)
				.Build()),
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices =>
			[IncarnateSpirit.Ritualist, IncarnateSpirit.Conqueror, IncarnateSpirit.Reaver];

		public override bool Round => true;
	}
}