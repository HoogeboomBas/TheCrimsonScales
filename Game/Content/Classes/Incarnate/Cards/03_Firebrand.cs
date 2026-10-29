using System.Collections.Generic;
using Godot;

public class Firebrand : IncarnateCardModel<Firebrand.CardTop, Firebrand.CardBottom>
{
	public override string Name => "Firebrand";
	public override int Level => 1;
	public override int Initiative => 30;
	protected override int AtlasIndex => 3;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.61930263f, 0.13739613f)))
				.Build()),
			new AbilityCardAbility(SufferDamageAbility.Builder()
				.WithDamage(1)
				.WithTarget(Target.TargetAll | Target.Enemies)
				.WithRange(1)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Reaver))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.InfuseElement(state, Element.Fire);
				})
				.Build()),
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(1, new RetaliateDiamondPlus(this, new Vector2(0.61930263f, 0.3556787f)))
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Conqueror];
		public override bool Round => true;
	}

	public class CardBottom : IncarnateCardSide
	{
		private AttackDiamond _attackEnhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_attackEnhancementMark = new AttackDiamond(this, new Vector2(0.66363853f, 0.7911358f));
		}

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(AttackAbility.Builder().WithDamage(2, _attackEnhancementMark).Build())
				.WithRange(1)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.Build()),
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Reaver];
		public override int XP => 1;
	}
}