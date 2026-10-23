using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class AncestralBlade : IncarnateCardModel<AncestralBlade.CardTop, AncestralBlade.CardBottom>
{
	public override string Name => "Ancestral Blade";
	public override int Level => 1;
	public override int Initiative => 65;
	protected override int AtlasIndex => 11;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.61930263f, 0.18060942f)))
				.Build()),
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(1, new PushCircle(this, new Vector2(0.46098468f, 0.29418284f)))
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(1)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Reaver))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.GainXP(state.Performer, 1);
				})
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Ritualist, IncarnateSpirit.Conqueror];
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(2)
				.WithRangeType(RangeType.Melee)
				.WithDuringAttackSubscription(
					InSpiritSubscription<ScenarioEvents.DuringAttack.Parameters>(IncarnateSpirit.Reaver,
						async parameters =>
						{
							parameters.AbilityState.AbilityAddCondition(Conditions.Rupture);

							await GDTask.CompletedTask;
						}))
				.Build())
		];

		public override int XP => 2;
		public override bool Loss => true;
	}
}