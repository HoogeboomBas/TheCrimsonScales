using System.Collections.Generic;
using System.Linq;
using Godot;

public class Implosion : HollowpactCardModel<Implosion.CardTop, Implosion.CardBottom>
{
	public override string Name => "Implosion";
	public override int Level => 6;
	public override int Initiative => 49;
	protected override int AtlasIndex => 0;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateVoidPitObstacleAbilityBuilder()
				.WithRange(3)
				.WithConditionalAbilityCheck(async state =>
				{
					return await LoseVoidEnergyConditionalAbilityCheck(state.Performer, 2,
						new TextEffectInfoView.Parameters($"Create a Void Pit obstacle in an empty hex within {Icons.Inline(Icons.Range)}3."));
				})
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Muddle)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithConditionalAbilityCheck(async state =>
				{
					Hex hex = await AbilityCmd.SelectHex(state, hexes =>
					{
						hexes.AddRange(GameController.Instance.Map.GetChildrenOfType<Obstacle>()
											.Where(obstacle => obstacle is VoidPit)
											.Select(obstacle => obstacle.Hex));
					}, hintText: $"Select a hex with a Void Pit.");

					if(hex == null)
					{
						return false;
					}

					state.SetCustomValue(this, "Hex", hex);
					return true;
				})
				.WithCustomGetPerformHex(state => state.GetCustomValue<Hex>(this, "Hex"))
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.GetCustomValue<Hex>(this, "Hex").Neighbours.SelectMany(hex => hex.GetFigures()));
				})
				.Build()),
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.41209206f, 0.1612586f)))
				.Build()),

			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(1)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithRange(2)
				.Build()),

			new AbilityCardAbility(GainVoidEnergyAbilityBuilder()
				.Build()),
		];
	}
}