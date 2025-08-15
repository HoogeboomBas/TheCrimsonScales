using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class IgnitedLaunch : BombardCardModel<IgnitedLaunch.CardTop, IgnitedLaunch.CardBottom>
{
	public override string Name => "Ignited Launch";
	public override int Level => 1;
	public override int Initiative => 81;
	protected override int AtlasIndex => 5;

	public class CardTop : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new ProjectileAbility(4,
				hex =>
				[
					new AttackAbility(5, rangeType: RangeType.Range, targetHex: hex)
				], this
			))
		];

		protected override IEnumerable<Element> Elements => [Element.Fire];

		protected override int XP => 1;
		protected override bool Persistent => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(new OtherActiveAbility(
				async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							if(parameters.Figure.TurnMovedHexCount <= 1)
							{
								// Gain 2 retaliate for the round
								ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(state, this,
									canApplyParameters =>
										canApplyParameters.Figure == state.Performer,
									applyParameters =>
									{
										applyParameters.AddRetaliate(2, 1);
									}
								);

								//state.Performer.UpdateRetaliate();

								ScenarioEvents.RetaliateEvent.Subscribe(state, this,
									canApplyParameters =>
									{
										return
											canApplyParameters.RetaliatingFigure == state.Performer &&
											RangeHelper.Distance(canApplyParameters.AbilityState.Performer.Hex, state.Performer.Hex) <= 1;
									},
									async applyParameters =>
									{
										applyParameters.AdjustRetaliate(2);

										await GDTask.CompletedTask;
									}
								);

								ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
									parameters => true,
									async parameters =>
									{
										ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(state, this);

										//state.Performer.UpdateRetaliate();

										ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);

										ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);

										await GDTask.CompletedTask;
									}
								);
							}

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				},
				async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);

					ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(state, this);

					//state.Performer.UpdateRetaliate();

					ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);

					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				}
			))
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}