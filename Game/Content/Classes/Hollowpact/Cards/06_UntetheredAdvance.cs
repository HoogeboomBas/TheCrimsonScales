using System.Collections.Generic;
using System.Linq;
using Godot;

public class UntetheredAdvance : HollowpactCardModel<UntetheredAdvance.CardTop, UntetheredAdvance.CardBottom>
{
	public override string Name => "Untethered Advance";
	public override int Level => 1;
	public override int Initiative => 46;
	protected override int AtlasIndex => 6;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.48416662f, 0.17638887f)))
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Red)
				]))
				.WithDuringAttackSubscription(LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(1,
					async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);
						await AbilityCmd.GainXP(parameters.AbilityState.Performer, 1);
					},
					new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Damage)}")))
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Dark))
				.Build())
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62167794f, 0.70833325f)))
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					OverlayTile overlayTile = await AbilityCmd.SelectOverlayTile(state,
						overlayTiles =>
						{
							overlayTiles.AddRange(RangeHelper.GetOverlayTilesInRange<Obstacle>(state.Performer, 1)
								.Where(obstacle => !obstacle.CannotBeDestroyed));
							overlayTiles.AddRange(RangeHelper.GetOverlayTilesInRange<Trap>(state.Performer, 1)
								.Where(trap => !trap.CannotBeDestroyed));
						},
						hintText: "Select an obstacle or trap to destroy");

					if(overlayTile != null)
					{
						await overlayTile.Destroy();
						await GainVoidEnergy(state);

						state.SetPerformed();
					}
				})
				.Build()),
		];
	}
}