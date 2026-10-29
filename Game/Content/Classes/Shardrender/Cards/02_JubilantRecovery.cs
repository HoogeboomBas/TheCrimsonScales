using System.Collections.Generic;
using Godot;

public class JubilantRecovery : ShardrenderCardModel<JubilantRecovery.CardTop, JubilantRecovery.CardBottom>
{
	public override string Name => "Jubilant Recovery";
	public override int Level => 1;
	public override int Initiative => 44;
	protected override int AtlasIndex => 2;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.50832486f, 0.23268698f)))
				.WithConditions(Conditions.Muddle)
				.Build()),
			new AbilityCardAbility(MoveCharacterTokenBackAbility(1).Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(CrystallizeAbility.Builder()
				.WithUseSlots(
					[
						new UseSlot(new Vector2(0.39657098f, 0.8199446f)),
						new UseSlot(new Vector2(0.60455734f, 0.8199446f))
					]
				)
				.Build())
		];

		public override bool Persistent => true;
	}
}