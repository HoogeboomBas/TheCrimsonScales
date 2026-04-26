using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class FindAnOpening : HollowpactCardModel<FindAnOpening.CardTop, FindAnOpening.CardBottom>
{
	public override string Name => "Find an Opening";
	public override int Level => 1;
	public override int Initiative => 88;
	protected override int AtlasIndex => 8;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(VoidsightAbility.Builder().Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithDuringAttackSubscription(LoseVoidEnergySubscription<ScenarioEvents.DuringAttack.Parameters>(1,
					async parameters =>
					{
						parameters.AbilityState.AbilityAdjustAttackValue(1);
						parameters.AbilityState.AbilityAdjustPierce(1);
						await AbilityCmd.GainXP(parameters.AbilityState.Performer, 1);
					},
					new TextEffectInfoView.Parameters($"+1{Icons.Inline(Icons.Damage)}, {Icons.Inline(Icons.Pierce)} 1")))
				.WithOnAbilityStarted(async state =>
				{
					ScenarioEvents.RetaliateEvent.Subscribe(state, this,
						parameters => parameters.RetaliatingFigure == state.Target,
						async parameters =>
						{
							parameters.SetRetaliateBlocked();
							
							await GDTask.CompletedTask;
						}
					);
					
					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async state =>
				{
					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
					
					await GDTask.CompletedTask;
				})
				.Build())
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreateObstacleAbility.Builder()
				.WithRange(3)
				.WithObstacleCount(2)
				.WithCustomAsset("res://Content/Classes/Hollowpact/VoidPit.tscn")
				.WithOnAbilityEndedPerformed(GainVoidEnergy)
				.Build()),
			
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithCustomGetTargets((state, list) =>
				{
					list.AddRange(GameController.Instance.Map.Figures
							.Where(figure => RangeHelper.GetHexesInRange(figure.Hex, 1, includeOrigin: true, requiresLineOfSight: false)
								.Any(hex => hex.GetHexObjectsOfType<Obstacle>().Any())));
				})
				.Build()),
		];
		
		public override int XP => 1;
		public override bool Loss => true;
	}
}