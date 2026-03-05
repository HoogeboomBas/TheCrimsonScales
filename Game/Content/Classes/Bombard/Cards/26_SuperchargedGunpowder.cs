using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class SuperchargedGunpowder : BombardCardModel<SuperchargedGunpowder.CardTop, SuperchargedGunpowder.CardBottom>
{
	public override string Name => "Supercharged Gunpowder";
	public override int Level => 9;
	public override int Initiative => 90;
	protected override int AtlasIndex => 26;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ProjectileAbility.Builder()
				.WithGetAbilities(hex =>
				[
					AttackAbility.Builder()
						.WithDamage(3)
						.WithPierce(3)
						.WithAOEPattern(new AOEPattern(
							[
								new AOEHex(Vector2I.Zero, AOEHexType.Red),
								new AOEHex(Vector2I.Zero.Add(Direction.SouthWest), AOEHexType.Red, "Wound", Icons.GetCondition(Conditions.Wound1)),
								new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red, "Immobilize",
									Icons.GetCondition(Conditions.Immobilize))
							]
						))
						.WithAfterTargetConfirmedSubscriptions(
						[
							ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
								parameters => parameters.AbilityState.GetCustomMarkedHexes("Wound").Contains(parameters.AbilityState.Target.Hex),
								async parameters =>
								{
									parameters.AbilityState.SingleTargetAddCondition(Conditions.Wound1);

									await GDTask.CompletedTask;
								}
							),
							ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
								parameters => parameters.AbilityState.GetCustomMarkedHexes("Immobilize").Contains(parameters.AbilityState.Target.Hex),
								async parameters =>
								{
									parameters.AbilityState.SingleTargetAddCondition(Conditions.Immobilize);

									await GDTask.CompletedTask;
								}
							)
						])
						.WithTargetHex(hex)
						.Build()
				])
				.WithAbilityCardSide(this)
				.WithRange(4, new ProjectileRangeSquare(this, new Vector2(0.3408154f, 0.6195522f), EnhancementCostType.MultiTarget))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62138146f, 0.7084656f)))
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer,
						async parameters =>
						{
							parameters.AbilityState.AdjustTargets(1);
							ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);
							ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
							await GDTask.CompletedTask;
						});
					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async _ =>
						{
							ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);
							ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.Build())
		];
	}
}