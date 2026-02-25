using System.Collections.Generic;
using System.Linq;
using Godot;

public class EnergyTransmission : ArtificerCardModel<EnergyTransmission.CardTop, EnergyTransmission.CardBottom>
{
	public override string Name => "Energy Transmission";
	public override int Level => 4;
	public override int Initiative => 44;
	protected override int AtlasIndex => 17;

	public class CardTop : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(4)
				.WithRange(4)
				.WithConditions(Conditions.Strengthen)
				.Build()),
			MoveCharacterTokenBackwardAbility()
		];
	}

	public class CardBottom : ArtificerCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6211814f, 0.71048194f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					//TODO: Change to directly selecting the overlay tile
					Hex hex = await AbilityCmd.SelectHex(state,
						hexes => hexes.AddRange(RangeHelper.GetHexesInRange(state.Performer.Hex, 1).Where(hex => hex.HasHexObjectOfType<Trap>())),
						hintText: $"Select a trap to increase its {Icons.HintText(Icons.Damage)} value by 2");
					if(hex == null)
					{
						return;
					}

					Trap trap = hex.GetHexObjectOfType<Trap>();
					trap.SetTrapDamage(trap.Damage + 2);
					state.SetPerformed();
				})
				.Build())
		];
	}
}