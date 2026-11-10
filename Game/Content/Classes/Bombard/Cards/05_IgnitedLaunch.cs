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
		private AttackEnhancementMark _enhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_enhancementMark = new AttackDiamond(this, new Vector2(0.619184f, 0.29891837f));
		}

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ProjectileAbility.Builder()
				.WithGetAbilities(hex =>
				[
					AttackAbility.Builder()
						.WithDamage(5, _enhancementMark)
						.WithRangeType(RangeType.Range)
						.WithTargetHex(hex)
						.Build()
				])
				.WithAbilityCardSide(this)
				.WithRange(4, new ProjectileRangeSquare(this, new Vector2(0.33784395f, 0.16527776f)))
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
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnEndedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							if(parameters.Figure.TurnMovedHexes.Count <= 1)
							{
								await AbilityCmd.AddRetaliate(state.Performer, this, 2, 1);

								ScenarioEvents.RoundEndedEvent.Subscribe(state, this,
									parameters => true,
									async parameters =>
									{
										AbilityCmd.RemoveRetaliate(state.Performer, this);

										ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);

										await GDTask.CompletedTask;
									}
								);
							}

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
					{
						ScenarioEvents.FigureTurnEndedEvent.Unsubscribe(state, this);

						AbilityCmd.RemoveRetaliate(state.Performer, this);

						ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}