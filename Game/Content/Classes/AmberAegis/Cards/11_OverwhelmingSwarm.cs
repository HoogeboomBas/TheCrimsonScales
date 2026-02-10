using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class OverwhelmingSwarm : AmberAegisCardModel<OverwhelmingSwarm.CardTop, OverwhelmingSwarm.CardBottom>
{
	public override string Name => "Overwhelming Swarm";
	public override int Level => 1;
	public override int Initiative => 44;
	protected override int AtlasIndex => 11;

	public class CardTop : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4, new AttackDiamond(this, new Vector2(0.5085482f, 0.2262132f)))
				.WithConditions(Conditions.Immobilize)
				.Build())
			//TODO: Add Perform Hex (requires scenarios)
		];

		public override int XP => 1;
	}

	public class CardBottom : AmberAegisCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(2)
				.WithTarget(Target.SelfOrAllies | Target.TargetAll)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(GameController.Instance.Map.Figures.Where(figure =>
						figure == state.Performer || (figure.AlliedWith(state.Performer) && IsAdjacentToColonyToken<ColonyToken>(figure))));
				})
				.Build())
		];
	}
}