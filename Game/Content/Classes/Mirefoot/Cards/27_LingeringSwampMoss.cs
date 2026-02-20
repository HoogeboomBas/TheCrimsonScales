using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class LingeringSwampMoss : MirefootCardModel<LingeringSwampMoss.CardTop, LingeringSwampMoss.CardBottom>
{
	public override string Name => "Lingering Swamp Moss";
	public override int Level => 9;
	public override int Initiative => 94;
	protected override int AtlasIndex => 27;

	public class CardTop : MirefootCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.4952556f, 0.2913344f)))
				.WithConditions(Conditions.Poison4)
				.Build()),
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

								await AbilityCmd.AddCondition(state, figure, Conditions.Immobilize);
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
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						parameters => parameters.Figure.EnemiesWith(state.Performer) && parameters.Figure.Hex.HasHexObjectOfType<DifficultTerrain>(),
						async parameters =>
						{
							await AbilityCmd.RemoveCondition(parameters.Figure, Conditions.Immobilize);
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithMandatory(true)
				.Build()),
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}