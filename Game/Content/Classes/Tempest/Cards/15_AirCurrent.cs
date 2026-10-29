using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class AirCurrent : TempestCardModel<AirCurrent.CardTop, AirCurrent.CardBottom>
{
	public override string Name => "Air Current";
	public override int Level => 3;
	public override int Initiative => 14;
	protected override int AtlasIndex => 15;

	public class CardTop : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.62115484f, 0.13351801f)))
				.Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackDiamond(this, new Vector2(0.44506335f, 0.21495846f)))
				.WithPush(2, new PushSquare(this, new Vector2(0.6658668f, 0.21440443f)))
				.WithAfterAttackPerformedSubscription(
					ScenarioEvents.AfterAttackPerformed.Subscription.New(parameters => parameters.AbilityState.SingleTargetState.PushHexes.Count > 0,
						async parameters =>
						{
							Figure figure = await AbilityCmd.SelectFigure(parameters.AbilityState,
								figures => figures.AddRange(RangeHelper.GetFiguresInRange(parameters.AbilityState.Target, 1).Where(figure =>
									parameters.Performer.EnemiesWith(figure) && figure != parameters.AbilityState.Target)));

							if(figure != null)
							{
								await AbilityCmd.SufferDamage(parameters.AbilityState, figure, 1);
							}
						}))
				.Build()),
			new AbilityCardAbility(GainSparkAbilityBuilder()
				.WithConditionalAbilityCheck(async state =>
				{
					if(state.Performer.HasCondition(Conditions.Dodge))
					{
						return false;
					}

					await AbilityCmd.GainXP(state.Performer, 1);
					return true;
				})
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Air)];
	}

	public class CardBottom : TempestCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveCircle(this, new Vector2(0.6210789f, 0.6170745f)))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Dodge)
				.WithTarget(Target.Self)
				.Build()),
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DodgeUsedEvent.Subscribe(state, this,
						parameters => parameters.DodgeUser == state.Performer,
						async parameters =>
						{
							await AbilityCmd.InfuseElement(state, Element.Light);

							await new ActionState(parameters.DodgeUser, [MoveAbility.Builder().WithDistance(2).Build()]).Perform();

							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DodgeUsedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlot(new UseSlot(new Vector2(0.4997881f, 0.896953f), GainXP))
				.Build())
		];

		public override bool Persistent => true;
	}
}