using System.Collections.Generic;
using Godot;

public class LashingVines : MirefootCardModel<LashingVines.CardTop, LashingVines.CardBottom>
{
	public override string Name => "Lashing Vines";
	public override int Level => 1;
	public override int Initiative => 13;
	protected override int AtlasIndex => 4;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(0, new AttackDiamond(this, new Vector2(0.61555624f, 0.1930237f)))
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(0)
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(0, new AttackDiamond(this, new Vector2(0.61555624f, 0.38992193f)))
				.Build())
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SummonAbility.Builder()
				.WithName("Flailing Ivies")
				.WithTexturePath("res://Content/Classes/Mirefoot/FlailingIvies.png")
				.WithHealth(1)
				.WithAttack(1)
				.WithTraits(new TargetsTrait(3))
				.WithGetValidHexes((abilityState, list) =>
					{
						RangeHelper.FindHexesInRange(abilityState.Performer.Hex, 3, true, list);

						for(int i = list.Count - 1; i >= 0; i--)
						{
							Hex hex = list[i];

							if(!hex.HasHexObjectOfType<DifficultTerrain>() || hex.HasHexObjectOfType<Figure>())
							{
								list.RemoveAt(i);
							}
						}
					}
				)
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
	}
}