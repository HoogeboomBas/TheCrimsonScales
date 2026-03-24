using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class UnbreakablePosition : BombardCardModel<UnbreakablePosition.CardTop, UnbreakablePosition.CardBottom>
{
	public override string Name => "Unbreakable Position";
	public override int Level => 5;
	public override int Initiative => 15;
	protected override int AtlasIndex => 18;

	public class CardTop : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.Build()),
			//Retaliate 0 ability for other things that care about retaliate abilities being performed
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(0)
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && parameters.FromAttack,
						async parameters =>
						{
							ScenarioEvents.RetaliateEvent.Subscribe(state, this,
								retaliateParameters => retaliateParameters.RetaliatingFigure == state.Performer &&
								                       parameters.PotentialAbilityState == retaliateParameters.AbilityState &&
								                       RangeHelper.Distance(state.Performer.Hex, retaliateParameters.Performer.Hex) <= 1,
								async retaliateParameters =>
								{
									retaliateParameters.AdjustRetaliate(parameters.TotalShield);
									ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
									await GDTask.CompletedTask;
								});
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 1))
				.WithMandatory(true)
				.Build())
		];

		public override bool Round => true;
	}

	public class CardBottom : BombardCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer && parameters.FromAttack && parameters.WouldSufferDamage,
						async parameters =>
						{
							parameters.AdjustShield(2);
							ScenarioEvents.RetaliateEvent.Subscribe(state, this,
								retaliateParameters => retaliateParameters.RetaliatingFigure == state.Performer &&
								                       parameters.PotentialAbilityState == retaliateParameters.AbilityState,
								async retaliateParameters =>
								{
									if(RangeHelper.Distance(state.Performer.Hex, retaliateParameters.Performer.Hex) <= 3)
									{
										retaliateParameters.AdjustRetaliate(2);
									}

									ScenarioEvents.RetaliateEvent.Unsubscribe(state, this);
									await state.AdvanceUseSlot();
								});
							await GDTask.CompletedTask;
						});
					ScenarioCheckEvents.ShieldCheckEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						parameters =>
						{
							parameters.AdjustShield(2);
						});
					ScenarioCheckEvents.RetaliateCheckEvent.Subscribe(state, this,
						parameters => parameters.Figure == state.Performer,
						parameters =>
						{
							parameters.AddRetaliate(2, 3);
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.SufferDamageEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.ShieldCheckEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.RetaliateCheckEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.16404444f, 0.78412694f)),
					new UseSlot(new Vector2(0.37037036f, 0.78412694f), GainXP),
					new UseSlot(new Vector2(0.5792592f, 0.78412694f)),
					new UseSlot(new Vector2(0.78814816f, 0.78412694f), GainXP)
				])
				.Build())
		];

		public override bool Persistent => true;
		public override bool Loss => true;
	}
}