using System.Collections.Generic;
using Godot;

public class WildStings : MirefootCardModel<WildStings.CardTop, WildStings.CardBottom>
{
	public override string Name => "Wild Stings";
	public override int Level => 7;
	public override int Initiative => 71;
	protected override int AtlasIndex => 23;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1, new AttackDiamond(this, new Vector2(0.4637037f, 0.24021162f)))
				.WithTargets(2)
				.WithConditions(Conditions.Poison1)
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.50982964f, 0.33842435f)))
				.WithTargets(2)
				.Build())
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Crypt Nettle")
				.WithTexturePath("res://Content/Classes/Mirefoot/CryptNettle.png")
				.WithHealth(3)
				.WithAttack(1)
				.WithTraits(new ApplyConditionTrait(Conditions.Poison1),
					new PerformOnDeathTrait(ConditionAbility.Builder().WithConditions(Conditions.Poison2)
						.WithTarget(Target.TargetAll | Target.Enemies).WithRange(1).Build()))
				.WithGetValidHexes((abilityState, list) =>
					{
						RangeHelper.FindHexesInRange(abilityState.Performer.Hex, 3, true, list);

						list.RemoveAll(hex => !hex.HasHexObjectOfType<DifficultTerrain>() || !hex.IsUnoccupied());
					}
				)
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}
}