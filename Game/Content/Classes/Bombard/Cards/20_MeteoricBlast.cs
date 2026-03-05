using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class MeteoricBlast : BombardCardModel<MeteoricBlast.CardTop, MeteoricBlast.CardBottom>
{
	public override string Name => "Meteoric Blast";
	public override int Level => 6;
	public override int Initiative => 26;
	protected override int AtlasIndex => 20;

	public class CardTop : BombardCardSide
	{
		private AttackEnhancementMark _enhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_enhancementMark = new AttackDiamond(this, new Vector2(0.61895925f, 0.25301f));
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
						.WithAfterAttackPerformedSubscription(
							ScenarioEvents.AfterAttackPerformed.Subscription.New(
								applyFunction: async parameters =>
								{
									foreach(Figure figure in RangeHelper.GetFiguresInRange(parameters.AbilityState.Target, 1).Where(figure =>
										        figure != parameters.AbilityState.Target && figure.EnemiesWith(parameters.Performer)))
									{
										await AbilityCmd.SufferDamage(parameters.AbilityState, figure, 2);
									}
								}))
						.Build(),
				])
				.WithAbilityCardSide(this)
				.WithRange(5, new ProjectileRangeSquare(this, new Vector2(0.33703706f, 0.16507936f)))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Earth)];
		public override int XP => 1;
		public override bool Persistent => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62124085f, 0.7279423f)))
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && parameters.WouldSufferDamage,
						async parameters =>
						{
							parameters.SetDamagePrevented();
							await state.ActionState.RequestDiscardOrLose();
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override bool Round => true;
	}
}