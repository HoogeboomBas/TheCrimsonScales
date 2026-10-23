using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class WarDrums : IncarnateCardModel<WarDrums.CardTop, WarDrums.CardBottom>
{
	public override string Name => "War Drums";
	public override int Level => 1;
	public override int Initiative => 27;
	protected override int AtlasIndex => 12;

	public class CardTop : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters => (state.Performer.AlliedWith(parameters.Performer) || state.Performer.EnemiesWith(parameters.Performer)) &&
						              RangeHelper.Distance(parameters.Performer.Hex, state.Performer.Hex) <= 2,
						async parameters =>
						{
							if(state.Performer.AlliedWith(parameters.Performer))
							{
								parameters.AbilityState.SingleTargetAdjustAttackValue(1);
							}
							else
							{
								parameters.AbilityState.SingleTargetAdjustAttackValue(-1);
							}

							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Incarnate.Empower)
				.WithRange(2)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Incarnate.Empower)
				.WithRange(2)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Incarnate.Empower)
				.WithRange(2)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Incarnate.Empower)
				.WithRange(2)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.Build()),
		];

		public override int XP => 2;
		public override bool Round => true;
		public override bool Loss => true;
	}

	public class CardBottom : IncarnateCardSide
	{
		private MoveCircle _moveEnhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_moveEnhancementMark = new MoveCircle(this, new Vector2(0.6648147f, 0.7911358f));
		}

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveCircle(this, new Vector2(0.62007874f, 0.6360111f)))
				.Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(MoveAbility.Builder().WithDistance(2, _moveEnhancementMark).Build())
				.WithRange(1)
				.WithConditionalAbilityCheck(state => InSpirit(state, IncarnateSpirit.Conqueror))
				.WithOnAbilityEndedPerformed(async state =>
				{
					await AbilityCmd.InfuseElement(state, Element.Earth);
				})
				.Build()),
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Ritualist, IncarnateSpirit.Reaver];
	}
}