using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class DevoutAssistance : HierophantLevelUpCardModel<DevoutAssistance.CardTop, DevoutAssistance.CardBottom>
{
	public override string Name => "Devout Assistance";
	public override int Level => 5;
	public override int Initiative => 37;
	protected override int AtlasIndex => 15 - 6;

	public class CardTop : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.Figure == state.Performer,
						async parameters =>
						{
							ActionState actionState = new ActionState(state.Performer,
							[
								GrantAbility.Builder()
									.WithGetAbilities(grantAbilityState =>
									[
										ShieldAbility.Builder()
											.WithShieldValue(1)
											.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth))
											.WithOnAbilityEndedPerformed(async shieldAbilityState =>
											{
												ScenarioEvents.RoundEndedEvent.Subscribe(shieldAbilityState, this,
													parameters => true,
													async parameters =>
													{
														await shieldAbilityState.RemoveFromActive();
													});
												await GDTask.CompletedTask;
											})
											.Build(),

										HealAbility.Builder()
											.WithHealValue(3)
											.WithTarget(Target.Self)
											.WithConditionalAbilityCheck(async healAbilityState =>
											{
												await GDTask.CompletedTask;
												return !healAbilityState.ActionState.GetAbilityState<ShieldAbility.State>(0).Performed &&
												       await AbilityCmd.AskConsumeElement(state.Performer, Element.Light);
											})
											.Build()
									])
									.Build()
							]);
							await actionState.Perform();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}

	public class CardBottom : HierophantCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4, new MoveCircle(this, new Vector2(0.6207892f, 0.70303124f)))
				.WithOnAbilityEndedPerformed(async state =>
				{
					if(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, false).Any(figure => figure.EnemiesWith(state.Performer)))
					{
						await AbilityCmd.InfuseElement(state, Element.Earth);
					}

					if(RangeHelper.GetFiguresInRange(state.Performer.Hex, 1, false).Any(figure => figure.AlliedWith(state.Performer)))
					{
						await AbilityCmd.InfuseElement(state, Element.Light);
					}
				})
				.Build()),
		];
	}
}