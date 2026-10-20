using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class LashingThorns : ThornreaperCardModel<LashingThorns.CardTop, LashingThorns.CardBottom>
{
	public override string Name => "Lashing Thorns";
	public override int Level => 1;
	public override int Initiative => 39;
	protected override int AtlasIndex => 4;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
					{
						if (GetLightElementState() is ElementState.Waning or ElementState.Strong)
						{
							await AbilityCmd.InfuseElement(state, Element.Earth);
						}
					})
				.Build()),

			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithPierce(1, new PierceSquare(this, new Vector2(0.2f, 0.3f)))
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthWest), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
				]),
				new AOEHexMark(Vector2I.Zero.Add(Direction.NorthWest).Add(Direction.West), this, new Vector2(0.5f, 0.4f)),
				new AOEHexMark(Vector2I.Zero.Add(Direction.East).Add(Direction.East), this, new Vector2(0.5f, 0.4f)))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.Performer.Hex.HasHexObjectOfType<HazardousTerrain>(),
						async parameters =>
						{
							await AbilityCmd.GainXP(parameters.Performer, 1);
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light);
							parameters.AbilityState.SetCustomValue(this, "TargetOnHazardous", true);
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);
						}
					))
				.WithGetTargetingHintText(state =>
					{
						if (state.GetCustomValue<bool>(this, "TargetOnHazardous"))
						{
							return $"Select a target for {Icons.HintText(Icons.Attack)}3";
						}
						else
						{
							return $"Select a target for {Icons.HintText(Icons.Attack)}2";
						}
					})
				.Build())
		];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.2f, 0.3f)))
				.Build()),

			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(2)
				.WithRange(3, new RangeSquare(this, new Vector2(0.2f, 0.3f)))
				.WithTargets(2, new TargetsSquare(this, new Vector2(0.2f, 0.3f)))
				.Build())
		];

		public override int XP => 1;
		public override bool Loss => true;
	}
}