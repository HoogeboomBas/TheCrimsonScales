using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;

public class CompoundToxin : MirefootCardModel<CompoundToxin.CardTop, CompoundToxin.CardBottom>
{
	public override string Name => "Compound Toxin";
	public override int Level => 5;
	public override int Initiative => 61;
	protected override int AtlasIndex => 20;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithConditions(Conditions.Poison1)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.HasPoison(),
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAddCondition(Conditions.Poison3);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						}))
				.Build())
		];
	}

	public class CardBottom : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DifficultTerrainTriggeredEvent.Subscribe(state, this,
						parameters => parameters.Figure.EnemiesWith(state.Performer),
						async parameters =>
						{
							await AbilityCmd.AddCondition(state, parameters.Figure, Conditions.Poison2);
						});
					ScenarioEvents.OverlayTileCreatedEvent.Subscribe(state, this,
						parameters => parameters.OverlayTile is DifficultTerrain,
						async parameters =>
						{
							foreach(Figure figure in parameters.OverlayTile.Hexes.SelectMany(hex => hex.GetHexObjectsOfType<Figure>()))
							{
								ScenarioCheckEvents.FlyingCheck.Parameters flyingCheckParameters =
									ScenarioCheckEvents.FlyingCheckEvent.Fire(new ScenarioCheckEvents.FlyingCheck.Parameters(figure));

								if(flyingCheckParameters.HasFlying)
								{
									return;
								}

								await AbilityCmd.AddCondition(state, figure, Conditions.Poison2);
							}
						});
					//TODO: Make it so you can choose the path of the enemy when it matters
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DifficultTerrainTriggeredEvent.Unsubscribe(state, this);
					ScenarioEvents.OverlayTileCreatedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}