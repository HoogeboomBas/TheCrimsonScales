using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class RadiantCrust : ShardrenderCardModel<RadiantCrust.CardTop, RadiantCrust.CardBottom>
{
	public override string Name => "Radiant Crust";
	public override int Level => 1;
	public override int Initiative => 17;
	protected override int AtlasIndex => 6;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1, new ShieldDiamondPlus(this, new Vector2(0.6198027f, 0.23364903f)))
				.Build()),
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(1, new RetaliateDiamondPlus(this, new Vector2(0.61957866f, 0.333518f)))
				.Build())
		];

		public override int XP => 1;
		public override bool Round => true;
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.5239462f, 0.66814405f)))
				.WithMoveType(MoveType.Jump)
				.Build()),
			new AbilityCardAbility(MoveCharacterTokenBackAbility(1)
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;

					return state.ActionState.GetAbilityState<MoveAbility.State>(0).Hexes
						.Any(hex => hex.GetFigures().Any(figure => state.Performer.EnemiesWith(figure)));
				})
				.Build())
		];
	}
}