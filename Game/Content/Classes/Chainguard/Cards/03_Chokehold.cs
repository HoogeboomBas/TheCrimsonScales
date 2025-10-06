using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class Chokehold : ChainguardCardModel<Chokehold.CardTop, Chokehold.CardBottom>
{
	public override string Name => "Chokehold";
	public override int Level => 1;
	public override int Initiative => 22;
	protected override int AtlasIndex => 12 - 3;

	public class CardTop : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Chainguard.Shackle)
				.WithRange(1)
				.Build()
			),

			new AbilityCardAbility(UseSlotAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this,
						parameters =>
							parameters.Performer == state.Performer && parameters.AbilityState.Target.HasCondition(Chainguard.Shackle),
						async parameters =>
						{
							switch(state.UseSlotIndex)
							{
								case 0:
									parameters.AbilityState.SingleTargetAdjustAttackValue(2);
									break;
								case 1:
									parameters.AbilityState.SingleTargetAdjustAttackValue(2);
									break;
								case 2:
									parameters.AbilityState.SingleTargetAdjustAttackValue(3);
									break;
							}

							await state.AdvanceUseSlot();
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithUseSlots(
				[
					new UseSlot(new Vector2(0.2889934f, 0.38399956f)),
					new UseSlot(new Vector2(0.5f, 0.38399956f)),
					new UseSlot(new Vector2(0.7025001f, 0.38399956f))
				])
				.Build())
		];

		protected override int XP => 2;
		protected override bool Persistent => true;
		protected override bool Loss => true;
	}

	public class CardBottom : ChainguardCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Figure figure = await AbilityCmd.SelectFigure(state, list =>
					{
						foreach(Figure figure in RangeHelper.GetFiguresInRange(state.Performer.Hex, 1))
						{
							if(state.Authority.EnemiesWith(figure) && figure.HasCondition(Chainguard.Shackle))
							{
								list.Add(figure);
							}
						}
					});

					if(figure == null)
					{
						return;
					}

					await AbilityCmd.SufferDamage(null, figure, 1);
				}
			)
			.Build())
		];
	}
}