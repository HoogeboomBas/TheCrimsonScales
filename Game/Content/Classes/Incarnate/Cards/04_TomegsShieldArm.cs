using System.Collections.Generic;
using Godot;

public class TomegsShieldArm : IncarnateCardModel<TomegsShieldArm.CardTop, TomegsShieldArm.CardBottom>
{
	public override string Name => "Tomeg's Shield Arm";
	public override int Level => 1;
	public override int Initiative => 18;
	protected override int AtlasIndex => 4;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.49715996f, 0.18161745f)))
				.WithPush(1)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Incarnate.Empower)
				.WithRange(1)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.InfuseElement(state, Element.Earth);
				})
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Ritualist];
	}

	public class CardBottom : IncarnateCardSide
	{
		private ShieldCircle _shieldEnhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_shieldEnhancementMark = new ShieldCircle(this, new Vector2(0.62007874f, 0.67922443f), EnhancementCostType.MultiTarget);
		}

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(ShieldAbility.Builder().WithShieldValue(1, _shieldEnhancementMark).Build())
				.WithRange(1)
				.WithTarget(Target.TargetAll | Target.SelfOrAllies)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Incarnate.Enfeeble)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithRange(1)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Ritualist))
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Conqueror];
		public override int XP => 1;
		public override bool Round => true;
	}
}