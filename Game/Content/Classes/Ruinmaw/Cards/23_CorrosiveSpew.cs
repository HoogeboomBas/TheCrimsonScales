using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class CorrosiveSpew : RuinmawCardModel<CorrosiveSpew.CardTop, CorrosiveSpew.CardBottom>
{
	public override string Name => "Corrosive Spew";
	public override int Level => 6;
	public override int Initiative => 50;
	protected override int AtlasIndex => 23;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3)
				.WithPierce(1)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red),
					]
				))
				.WithAbilityStartedSubscription(
					ScenarioEvents.AbilityStarted.Subscription.New(
						parameters => IsSated(parameters.Performer),
						async parameters =>
						{
							((AttackAbility.State)parameters.AbilityState).AbilityAdjustPierce(2);
							((AttackAbility.State)parameters.AbilityState).AbilityAddConditionPreAbility(Conditions.Poison1);
							await GDTask.CompletedTask;
						}
					)
				)
				.WithOnAbilityEnded(async state =>
					{
						if (state.Performed && IsSated(state.Performer))
						{
							await AbilityCmd.GainXP(state.Performer, 1);
						}
					}
				)
				.Build()),
		];
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Figure.EnemiesWith(state.Performer) && canApplyParameters.PotentialAbilityState?.Performer == state.Performer,
						async applyParameters =>
						{
							object obj = new object();

							ScenarioEvents.AbilityEndedEvent.Subscribe(state, obj, parameters => true,
								async parameters =>
								{
									ScenarioEvents.AbilityEndedEvent.Unsubscribe(state, obj);
									ActionState actionState = new ActionState(state.Performer,
									[
										HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).WithConditions(Conditions.EmpowerRuinmaw).Build(),
									]);
									await actionState.Perform();
								}
							);
							
							if(state.UseSlotIndex == 0)
							{
								await SateRuinmaw(state.Performer);
							}
							await state.AdvanceUseSlot();
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureKilledEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.16650043f, 0.3549993f)),
					new UseSlot(new Vector2(0.36999783f, 0.3549993f), GainXP),
					new UseSlot(new Vector2(0.57749975f, 0.3549993f)),
					new UseSlot(new Vector2(0.78700954f, 0.3549993f), GainXP)
				])
				//TODO: Fix use slot positioning
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.EmpowerAddedEvent.Subscribe(state, this,
						canApplyParameters =>
							canApplyParameters.EmpoweredFigure == state.Performer,
						async parameters =>
						{
							parameters.SetShuffleDrawPile(false);
							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.EmpowerAddedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];
		
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}
}