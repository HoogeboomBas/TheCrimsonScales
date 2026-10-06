using System.Collections.Generic;
using Godot;
using Fractural.Tasks;

public class ProvokingRoar : BruiserCardModel<ProvokingRoar.CardTop, ProvokingRoar.CardBottom>
{
	public override string Name => "Provoking Roar";
	public override int Level => 1;
	public override int Initiative => 18;
	protected override int AtlasIndex => 11;

	public class CardTop : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(2, new PullSquare(this, new Vector2(0.2f, 0.3f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.5f, 0.3f)))
				.WithConditions(Conditions.Muddle)
				.Build()),

			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(2)
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.RetaliateEvent.Subscribe(state, this,
						parameters => parameters.RetaliatingFigure == state.Performer,
						async parameters =>
						{
							await AbilityCmd.GainXP(state.Performer, 1);
							ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;

	}

	public class CardBottom : BruiserCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.5f, 0.6f)))
				.Build()),

			new AbilityCardAbility(PullAbility.Builder()
				.WithPull(2, new PullSquare(this, new Vector2(0.3f, 0.6f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.4f, 0.8f)))
				.Build())
		];
	}
}