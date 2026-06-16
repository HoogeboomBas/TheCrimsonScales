using System;
using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class TricklingSting : LuminaryCardModel<TricklingSting.CardTop, TricklingSting.CardBottom>
{
	public override string Name => "Trickling Sting";
	public override int Level => 1;
	public override int Initiative => 43;
	protected override int AtlasIndex => 13;

	public class CardTop : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2, new AttackDiamond(this, new Vector2(0.6654667f, 0.22265568f)))
				.WithOnAbilityStarted(async state =>
				{
					int count = 0;
					foreach(Element element in global::Elements.All)
					{
						if(GameController.Instance.ElementManager.GetState(element) == ElementState.Waning ||
						   GameController.Instance.ElementManager.GetState(element) == ElementState.Strong)
						{
							count = Math.Min(count + 1, 4);
						}
					}

					state.AbilityAdjustAttackValue(count);

					await GDTask.CompletedTask;
				})
				.Build())
		];
	}

	public class CardBottom : LuminaryCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.6210601f, 0.6248515f)))
				.Build()),

			new AbilityCardAbility(CreateTrapAbility.Builder()
				.WithDamage(4)
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					CreateTrapAbility.State createTrapState = state.ActionState.GetAbilityState<CreateTrapAbility.State>(1);

					ScenarioEvents.TrapTriggeredEvent.Subscribe(state, this,
						canApply: canApplyParameters => createTrapState.CreatedTraps.Contains(canApplyParameters.Trap),
						async applyParameters =>
						{
							if(applyParameters.Figure.EnemiesWith(state.Performer))
							{
								await AbilityCmd.InfuseWildElement(state);
								await AbilityCmd.GainXP(state.Performer, 1);
							}

							await state.ActionState.RequestDiscardOrLose();
						}
					);
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.TrapTriggeredEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(state => AbilityCmd.HasPerformedAbility(state, 1))
				.Build())
		];

		public override int XP => 1;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}