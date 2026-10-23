using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class CinnabarSeeding : ShardrenderCardModel<CinnabarSeeding.CardTop, CinnabarSeeding.CardBottom>
{
	public override string Name => "Cinnabar Seeding";
	public override int Level => 5;
	public override int Initiative => 38;
	protected override int AtlasIndex => 21;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithPierce(1)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.SouthEast), AOEHexType.Red)
					]
				))
				.WithDuringAttackSubscription(
					AdvanceCrystallizeSubscription<ScenarioEvents.DuringAttack.Parameters>(async parameters =>
					{
						parameters.AbilityState.AbilityAddCondition(Conditions.Poison1);

						await GDTask.CompletedTask;
					}, new TextEffectInfoView.Parameters($"{Icons.Inline(Icons.GetCondition(Conditions.Poison1))}")))
				.Build())
		];
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6214549f, 0.6466453f)))
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters =>
							parameters.FromAttack && parameters.PotentialAbilityState.Performer == state.Performer &&
							parameters.TotalShield < Mathf.Max(((AttackAbility.State)parameters.PotentialAbilityState).SingleTargetPierce, 0),
						async parameters =>
						{
							parameters.AdjustAttackValue(1);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}