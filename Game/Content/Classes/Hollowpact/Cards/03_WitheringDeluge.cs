using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class WitheringDeluge : HollowpactCardModel<WitheringDeluge.CardTop, WitheringDeluge.CardBottom>
{
	public override string Name => "Withering Deluge";
	public override int Level => 1;
	public override int Initiative => 47;
	protected override int AtlasIndex => 3;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithConditions(Conditions.Wound1)
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero, AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red)
				]))
				.WithDuringAttackSubscription(LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(2,
					async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(2);
						await AbilityCmd.GainXP(parameters.AbilityState.Performer, 1);
					},
					new TextEffectInfoView.Parameters($"+2{Icons.Inline(Icons.Damage)}")))
				.Build())
		];
		
		public override int XP => 1;
		public override bool Loss => true;
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3)
				.Build()),
			
			new AbilityCardAbility(CreateObstacleAbility.Builder()
				.WithCustomAsset("res://Content/Classes/Hollowpact/VoidPit.tscn")
				.WithRange(2)
				.WithOnAbilityEndedPerformed(GainVoidEnergy)
				.Build()),
		];
	}
}