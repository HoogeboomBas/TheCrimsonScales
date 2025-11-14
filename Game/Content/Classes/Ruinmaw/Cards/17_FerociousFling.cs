using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class FerociousFling : RuinmawCardModel<FerociousFling.CardTop, FerociousFling.CardBottom>
{
	public override string Name => "Ferocious Fling";
	public override int Level => 3;
	public override int Initiative => 86;
	protected override int AtlasIndex => 17;

	public class CardTop : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(4)
				.WithPush(2)
				.Build()),
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(1)
				.WithCustomGetTargets(async (state, targets) =>
				{
					AttackAbility.State attackAbilityState = state.ActionState.GetAbilityState<AttackAbility.State>(0);
					Figure figure = await AbilityCmd.SelectFigure(state, list => list.AddRange(attackAbilityState.UniqueTargetedFigures)
						, true, hintText: "Select a target of the attack ability");
					if (figure != null)
                    {
						targets.AddRange(RangeHelper.GetFiguresInRange(figure.Hex, 1, false));
                    }
				})
				.WithTarget(Target.Enemies | Target.TargetAll)
				.Build()),
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					IEnumerable<Figure> figures = state.ActionState.AbilityStates
						.OfType<TargetedAbilityState>()
						.SelectMany(abilityState => abilityState.UniqueTargetedFigures.Where(figure => figure.EnemiesWith(state.Performer)));
					foreach(Figure figure in figures)
                    {
						await AbilityCmd.SufferDamage(state, figure, 2);    
                    }
				})
				.Build())
		];

		protected override bool Sate => true;
		protected override IEnumerable<Element> Elements => [Element.Air];
		protected override int XP => 2;
		protected override bool Loss => true;
	}

	public class CardBottom : RuinmawCardSide
	{
		protected override IEnumerable<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.Build()),
			new AbilityCardAbility(ConditionAbility.Builder()
				.WithConditions(Conditions.Rupture)
				.WithAOEPattern(new AOEPattern(
					[
						new AOEHex(Vector2I.Zero, AOEHexType.Gray),
						new AOEHex(Vector2I.Zero.Add(Direction.East), AOEHexType.Red),
						new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					]
				))
				.Build()),
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Subscribe(state, this,
						parameters =>
							state.Performer == parameters.Performer,
						async parameters =>
						{
							parameters.AbilityState.SingleTargetAdjustPush(1);

							await GDTask.CompletedTask;
						}
					);

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.DuringAttackEvent.Unsubscribe(state, this);

					await GDTask.CompletedTask;
				})
				.WithConditionalAbilityCheck(async state =>
				{
					await GDTask.CompletedTask;
					return IsSated(state.Performer);
				})
				.WithOnAbilityEndedPerformed(async state =>
					{
						state.ActionState.SetOverrideRound();

						await GDTask.CompletedTask;
					}
				)
				.Build())
		];
	}
}