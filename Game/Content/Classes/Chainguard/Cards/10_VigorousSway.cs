using System.Collections.Generic;
using System.Linq;
using Godot;

public class VigorousSway : ChainguardCardModel<VigorousSway.CardTop, VigorousSway.CardBottom>
{
	public override string Name => "Vigorous Sway";
	public override int Level => 1;
	public override int Initiative => 52;
	protected override int AtlasIndex => 12 - 10;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(SwingAbility.Builder()
				.WithSwing(3)
				.WithRange(2)
				.WithConditions(Chainguard.Shackle)
				.WithOnAbilityEndedPerformed(async state => 
				{
					if(state.SingleTargetState.ForcedMovementHexes.Count < state.SingleTargetSwing) 
					{
						int abilityRange = RangeHelper.Distance(state.Target.Hex, state.Performer.Hex);
						// Find hexes that are both adjacent to the target and on a circle around the performer with radius of distance to the target
						// That is 0-2 hexes, depending on walls
						List<Hex> list = RangeHelper.GetHexesInRange(state.Target.Hex, 1, includeOrigin: false).Where(hex => RangeHelper.Distance(hex, state.Performer.Hex) == abilityRange).ToList();

						// 0 or 1 hex means 1 or 2 hexes are walls, otherwise check if one of the hexes has an obstacle
						if(list.Count < 2 || list.Any(hex => hex.HasHexObjectOfType<Obstacle>()))
						{
							await AbilityCmd.SufferDamage(null, state.Target, 2);
							await AbilityCmd.GainXP(state.Performer, 1);
						}
					}
				})
				.Build())
		];
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),

			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Chainguard.Shackle)
				.WithRange(1)
				.Build()),

			new AbilityCardAbility(CreateTrapAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Stun)
				// .WithCustomAsset("cs-rope-trap.png")
				.Build())
		];

		protected override int XP => 2;
		protected override bool Loss => true;
	}
}