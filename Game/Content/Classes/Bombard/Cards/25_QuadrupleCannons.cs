using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class QuadrupleCannons : BombardCardModel<QuadrupleCannons.CardTop, QuadrupleCannons.CardBottom>
{
	public override string Name => "Quadruple Cannons";
	public override int Level => 8;
	public override int Initiative => 86;
	protected override int AtlasIndex => 25;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithRange(4, new RangeSquare(this, new Vector2(0.6133333f, 0.2063492f)))
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East).Add(Direction.SouthEast), AOEHexType.Red)
					]
				))
				.Build())
		];
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.61787784f, 0.62962955f)))
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.ProjectileTokenCreatedEvent.Subscribe(state, this,
						parameters => parameters.TokenCreator == state.Performer,
						async parameters =>
						{
							await new ActionState(parameters.TokenCreator, [MoveAbility.Builder().WithDistance(2).Build()]).Perform();
							await state.AdvanceUseSlot();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.ProjectileTokenCreatedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.5f, 0.88888884f), GainXP))
				.Build())
		];

		public override bool Persistent => true;
	}
}