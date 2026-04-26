using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class TouchOfTheVoid : HollowpactCardModel<TouchOfTheVoid.CardTop, TouchOfTheVoid.CardBottom>
{
	public override string Name => "Touch of TheVoid";
	public override int Level => 1;
	public override int Initiative => 29;
	protected override int AtlasIndex => 7;

	public class CardTop : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(VoidsightAbility.Builder().Build()),
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(1)
				.WithConditions(Conditions.Stun)
				.Build())
			//TODO: Consume +1dmg, generate dark, +1xp
		];
	}

	public class CardBottom : HollowpactCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Subscribe(state, this,
						canApplyParameters => canApplyParameters.Target == state.Performer && 
						                      canApplyParameters.ConditionModel == Conditions.Muddle,
						async applyParameters =>
						{
							if(!applyParameters.Prevented)
							{
								applyParameters.SetPrevented(true);
							}

							await GDTask.CompletedTask;
						});
					
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.InflictConditionEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
			
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					//TODO: Produce 1
					await state.AdvanceUseSlot();
					
					ScenarioEvents.FigureTurnStartedEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
							async parameters =>
							{
								//TODO: Produce 1
								await state.AdvanceUseSlot();
							});
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);
					
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.16650043f, 0.3549993f)),
					new UseSlot(new Vector2(0.36999783f, 0.3549993f)),
					new UseSlot(new Vector2(0.57749975f, 0.3549993f), GainXP),
					new UseSlot(new Vector2(0.78700954f, 0.3549993f))
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}