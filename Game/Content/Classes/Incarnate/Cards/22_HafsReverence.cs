using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class HafsReverence : IncarnateCardModel<HafsReverence.CardTop, HafsReverence.CardBottom>
{
	public override string Name => "Haf's Reverence";
	public override int Level => 6;
	public override int Initiative => 34;
	protected override int AtlasIndex => 22;

	public class CardTop : IncarnateCardSide
	{
		private AttackDiamond _attackEnhancementMark;

		protected override void InitExtraEnhancements()
		{
			base.InitExtraEnhancements();

			_attackEnhancementMark = new AttackDiamond(this, new Vector2(0.66121036f, 0.42483076f));
		}

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions([Incarnate.Empower, Incarnate.Empower])
				.WithTargets(2)
				.WithTarget(Target.SelfOrAllies | Target.SelfCountsForTargets)
				.WithRange(2)
				.Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(HealAbility.Builder().WithHealValue(2).WithTarget(Target.Self).Build())
				.WithTargets(2)
				.WithTarget(Target.SelfOrAllies | Target.SelfCountsForTargets)
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.ActionState.GetAbilityState<ConditionAbility.State>(0).UniqueTargetedFigures);
				})
				.WithConditionalAbilityCheck(async state =>
					await AbilityCmd.HasPerformedAbility(state, 0) && InSpirit(state.Performer, IncarnateSpirit.Ritualist))
				.Build()),
			new AbilityCardAbility(GrantAbility.Builder()
				.WithAbilities(AttackAbility.Builder().WithDamage(4, _attackEnhancementMark).Build())
				.WithCustomGetTargets((state, figures) =>
				{
					figures.AddRange(state.ActionState.GetAbilityState<ConditionAbility.State>(0).UniqueTargetedFigures
						.Where(figure => figure != state.Performer));
				})
				.WithConditionalAbilityCheck(async state =>
					await AbilityCmd.HasPerformedAbility(state, 0) && InSpirit(state.Performer, IncarnateSpirit.Conqueror))
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : IncarnateCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Subscribe(state, this,
						parameters => parameters.AMDCard.Model is EmpowerAMDCard && state.Performer.AlliedWith(parameters.Performer, true) &&
						              RangeHelper.Distance(state.Performer.Hex, parameters.Performer.Hex) <= 2,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustAttackValue(1);

							await GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.AMDCardDrawnEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];

		protected override IEnumerable<IncarnateSpirit> SwitchSpiritChoices => [IncarnateSpirit.Reaver];
		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Fire)];
		public override bool Persistent => true;
	}
}