using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class PressureSpike : RimehearthCardModel<PressureSpike.CardTop, PressureSpike.CardBottom>
{
	public override string Name => "Pressure Spike";
	public override int Level => 5;
	public override int Initiative => 54;
	protected override int AtlasIndex => 20;

	public class CardTop : RimehearthCardSide
	{
		private AttackEnhancementMark _enhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_enhancementMark = new AttackDiamond(this, new Vector2(0.6169745f, 0.28934258f));
		}

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.WithConditions(Conditions.Brittle)
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						async parameters =>
						{
							ActionState actionState = new ActionState(parameters.Figure,
							[
								AttackAbility.Builder().WithDamage(4, _enhancementMark).Build(),
							]);
							await actionState.Perform();

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.50064015f, 0.39501387f), GainXP))
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : RimehearthCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Conditions.Wound1, Conditions.Chill])
				.WithTarget(Target.Self)
				.WithMandatory(true)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Brittle)
				.WithRange(1)
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse([Element.Fire, Element.Ice])];
	}
}