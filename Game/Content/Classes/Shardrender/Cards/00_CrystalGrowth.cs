using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class CrystalGrowth : ShardrenderCardModel<CrystalGrowth.CardTop, CrystalGrowth.CardBottom>
{
	public override string Name => "Crystal Growth";
	public override int Level => 1;
	public override int Initiative => 68;
	protected override int AtlasIndex => 0;

	public class CardTop : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Ward)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AbilityPerformedEvent.Subscribe(state, this,
						parameters => parameters.Performer == state.Performer && parameters.AbilityState is CrystallizeAbility.State,
						async parameters =>
						{
							object subscriber = new object();
							await AbilityCmd.AddShield(parameters.Performer, ScenarioEvents.GetSubscriberPair(state, subscriber), 1);

							ScenarioEvents.RoundEndedEvent.Subscribe(state, subscriber,
								_ => true,
								async _ =>
								{
									AbilityCmd.RemoveShield(parameters.Performer, ScenarioEvents.GetSubscriberPair(state, subscriber));
									ScenarioEvents.RoundEndedEvent.Unsubscribe(state, subscriber);

									await GDTask.CompletedTask;
								});

							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AbilityPerformedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : ShardrenderCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6214308f, 0.71468145f)))
				.Build()),
			new AbilityCardAbility(MoveCharacterTokenBackAbility(2, false).Build())
		];
	}
}