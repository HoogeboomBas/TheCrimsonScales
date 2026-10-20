using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class SpikedEmbrace : ThornreaperCardModel<SpikedEmbrace.CardTop, SpikedEmbrace.CardBottom>
{
	public override string Name => "Spiked Embrace";
	public override int Level => 1;
	public override int Initiative => 31;
	protected override int AtlasIndex => 13;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithRange(2, new RangeSquare(this, new Vector2(0.2f, 0.3f)))
				.WithPull(1)
				.WithPierce(2, new PierceSquare(this, new Vector2(0.2f, 0.3f)))
				.Build()),
		];

		public override int XP => 1;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Hex hex = state.Performer.Hex;

					if (hex.IsFeatureless())
					{
						List<Hex> selectedHexes =
							await AbilityCmd.SelectHexes(state, list => list.Add(hex), 0, 1, true, "Create hazardous terrain?");
						
						foreach (Hex selectedHex in selectedHexes)
						{
							await CreateHazardousTerrain(hex, state.Performer);
							state.SetPerformed();
						}
					}
				})
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(3, new HealSquare(this, new Vector2(0.2f, 0.3f)))
				.WithTarget(Target.Self)
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					if (GetLightElementState() is ElementState.Waning)
					{
						await AbilityCmd.InfuseElement(state, Element.Light);
					}
					state.SetPerformed();
				})
				.Build()),
		];
	}
}